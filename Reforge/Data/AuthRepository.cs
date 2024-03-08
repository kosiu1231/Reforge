﻿using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Reforge.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> Login(string email, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }
            else if(user.VerifiedAt is null)
            {
                response.Success = false;
                response.Message = "Email not verified";
            }
            else
            {
                response.Data = CreateToken(user);
            }

            return response;
        }

        public async Task<ServiceResponse<string>> Register(User user, string password, string confirmPassword)
        {
            var response = new ServiceResponse<string>();
            if (await EmailExists(user.Email) || await UsernameExists(user.Username))
            { 
                response.Success = false;
                response.Message = "User already exists";
                return response;
            }else if(!password.Equals(confirmPassword))
            {
                response.Success = false;
                response.Message = "Passwords do not match";
                return response;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.VerificationToken = await CreateRandomToken();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            response.Data = user.Email;

            SendVerificationEmail(user.Email, user.Username, user.VerificationToken);

            return response;
        }

        public async Task<ServiceResponse<string>> ForgotPassword(string email)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user is null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            user.PasswordResetToken = await CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddMinutes(15);
            await _context.SaveChangesAsync();

            response.Message = "Token will be valid for 15min";

            SendResetPasswordEmail(user.Email, user.Username, user.PasswordResetToken);

            return response;
        }

        public async Task<ServiceResponse<string>> ResetPassword(ResetPasswordRequest request)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);

            if (user is null || user.ResetTokenExpires < DateTime.Now)
            {
                response.Success = false;
                response.Message = "Invalid token";
                return response;
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            response.Data = "Password changed";

            return response;
        }

        public async Task<ServiceResponse<string>> Verify(string token)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

            if (user is null)
            {
                response.Success = false;
                response.Message = "Invalid token";
                return response;
            }

            response.Data = user.Email;

            user.VerifiedAt = DateTime.Now;
            user.Role = UserRole.User;
            await _context.SaveChangesAsync();

            return response;
        }

        public async Task<bool> UsernameExists(string username)
        {
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            { return true; }
            return false;
        }

        public async Task<bool> EmailExists(string email)
        {
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
            { return true; }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()!)
            };

            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if (appSettingsToken is null)
            {
                throw new Exception("AppSettings Token is null.");
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(appSettingsToken));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<string> CreateRandomToken()
        {
            string token;
            do
            {
                token = Convert.ToHexString(RandomNumberGenerator.GetBytes(24));
            } while ((await _context.Users.AnyAsync(t => t.PasswordResetToken == token)));
            return token;
        }

        private async void SendVerificationEmail(string email, string username, string verificationToken)
        {
            var client = new SendGridClient(_configuration.GetSection("SendGrid:ApiKey").Value);
            var to = new EmailAddress(email, username);
            var from = new EmailAddress("reforgemods@proton.me", "Reforge Mods");
            var message = MailHelper.CreateSingleTemplateEmail(from, to, "d-0626ad7711a647a08d3c9d218424783c", new
            {
                emailUsername = username,
                emailVerifyCode = verificationToken
            });
            var emailResponse = await client.SendEmailAsync(message);
            var emailResponseBody = await emailResponse.Body.ReadAsStringAsync();
            //log responses
        }

        private async void SendResetPasswordEmail(string email, string username, string resetToken)
        {
            var client = new SendGridClient(_configuration.GetSection("SendGrid:ApiKey").Value);
            var to = new EmailAddress(email, username);
            var from = new EmailAddress("reforgemods@proton.me", "Reforge Mods");
            var message = MailHelper.CreateSingleTemplateEmail(from, to, "d-8ddfcaac969042ae930c617a1d4709eb", new
            {
                emailUsername = username,
                emailResetToken = resetToken
            });
            var emailResponse = await client.SendEmailAsync(message);
            var emailResponseBody = await emailResponse.Body.ReadAsStringAsync();
            //log responses
        }
    }
}
