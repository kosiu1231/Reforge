﻿using Reforge.Models;
using System.Security.Claims;

namespace Reforge.Services.ModService
{
    public class ModService : IModService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ModService> _logger;

        public ModService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor, ILogger<ModService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _logger = logger;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);

        private string GetUserRole() => _httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.Role)!;

        public async Task<ServiceResponse<GetModDto>> AddMod(AddModDto newMod)
        {
            var response = new ServiceResponse<GetModDto>();

            try
            {
                var mod = _mapper.Map<Mod>(newMod);
                mod.Creator = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
                mod.Game = await _context.Games.FirstOrDefaultAsync(g => g.Id == newMod.GameId);
               
                if(mod.Game is null)
                {
                    response.Success = false;
                    response.Message = "Game not found";
                    return response;
                }

                _context.Mods.Add(mod);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetModDto>(mod);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            _logger.LogInformation($"AddMod({newMod.Name}) invoked by {GetUserId()}.");
            return response;
        }

        public async Task<ServiceResponse<List<GetModDto>>> GetMods(QueryObject query)
        {
            var response = new ServiceResponse<List<GetModDto>>();
            string logMessage = "";

            try
            {
                var mods = _context.Mods
                    .Include(g => g.Game)
                    .Include(c => c.Creator)
                    .Include(c => c.Comments).AsQueryable();

                if(!string.IsNullOrWhiteSpace(query.Name))
                {
                    mods = mods.Where(m => m.Name.Contains(query.Name));
                    logMessage = $"Query.Name used: {query.Name}";
                }

                if (!string.IsNullOrWhiteSpace(query.SortBy))
                {
                    if(query.SortBy.Equals("LikeAmount", StringComparison.OrdinalIgnoreCase))
                    {
                        mods = query.IsDescending ? mods.OrderByDescending(m => m.LikeAmount) : mods.OrderBy(m => m.LikeAmount);
                    }
                    else if (query.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                    {
                        mods = query.IsDescending ? mods.OrderByDescending(m => m.CreatedAt) : mods.OrderBy(m => m.CreatedAt);
                    }
                    else if (query.SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    {
                        mods = query.IsDescending ? mods.OrderByDescending(m => m.Name) : mods.OrderBy(m => m.Name);
                    }
                }
                int skipNumber = (query.PageNumber - 1) * query.PageSize;

                mods = mods.Skip(skipNumber).Take(query.PageSize);

                if (mods.Count() == 0)
                {
                    response.Success = false;
                    response.Message = "No mods found";
                    return response;
                }

                response.Data = await mods.Select(m => _mapper.Map<GetModDto>(m)).ToListAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            _logger.LogInformation("GetMods invoked. " + logMessage);
            return response;
        }

        public async Task<ServiceResponse<GetModDto>> GetMod(int id)
        {
            var response = new ServiceResponse<GetModDto>();

            try
            {
                var mod = await _context.Mods
                    .Include(g => g.Game)
                    .Include(c => c.Creator)
                    .Include(c => c.Comments)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (mod is null)
                {
                    response.Success = false;
                    response.Message = "Mod not found";
                    return response;
                }

                response.Data = _mapper.Map<GetModDto>(mod);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            _logger.LogInformation($"GetMod({id}) invoked. ");
            return response;
        }

        public async Task<ServiceResponse<GetModDto>> LikeMod(int id)
        {
            var response = new ServiceResponse<GetModDto>();
            string logMessage = "";

            try
            {
                var mod = await _context.Mods
                    .Include(g => g.Game)
                    .Include(c => c.Creator)
                    .Include(c => c.Comments)
                    .FirstOrDefaultAsync(m => m.Id == id);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

                if (mod is null)
                {
                    response.Success = false;
                    response.Message = "Mod not found";
                    return response;
                } else if(user is null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return response;
                }

                if (await _context.Likes.AnyAsync(l => l.User!.Id == user!.Id && l.Mod!.Id == mod.Id))
                {
                    response.Success = false;
                    response.Message = "Mod already liked by this user";
                    logMessage = $"{user.Id} tried to like mod {mod.Name} second time";
                    return response;
                }

                var like = new Like
                {
                    User = user,
                    Mod = mod,
                };

                user!.Likes!.Add(like);
                mod.Likes!.Add(like);
                mod.LikeAmount += 1;

                _context.Likes.Add(like);
                await _context.SaveChangesAsync();

                logMessage = $"{user.Id} likes mod {mod.Name}";
                response.Data = _mapper.Map<GetModDto>(mod);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            _logger.LogInformation(logMessage);
            return response;
        }

        public async Task<ServiceResponse<GetModDto>> DislikeMod(int id)
        {
            var response = new ServiceResponse<GetModDto>();
            string logMessage = "";

            try
            {
                var mod = await _context.Mods
                    .Include(g => g.Game)
                    .Include(c => c.Creator)
                    .Include(c => c.Comments)
                    .FirstOrDefaultAsync(m => m.Id == id);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

                if (mod is null)
                {
                    response.Success = false;
                    response.Message = "Mod not found";
                    return response;
                }

                var like = await _context.Likes.FirstOrDefaultAsync(l => l.User!.Id == user!.Id && l.Mod!.Id == mod.Id);

                if (like is null)
                {
                    response.Success = false;
                    response.Message = "Mod not liked by this user";
                    return response;
                }

                user!.Likes!.Remove(like);
                mod.Likes!.Remove(like);
                mod.LikeAmount -= 1;

                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();

                logMessage = $"{user.Id} dislikes mod {mod.Name}";
                response.Data = _mapper.Map<GetModDto>(mod);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            _logger.LogInformation(logMessage);
            return response;
        }

        public async Task<ServiceResponse<GetModDto>> UpdateMod(UpdateModDto updatedMod)
        {
            var response = new ServiceResponse<GetModDto>();

            try
            {
                //User who created mod or admin
                var mod = await _context.Mods.FirstOrDefaultAsync(m => m.Id == updatedMod.Id
                && (m.Creator!.Id == GetUserId() || GetUserRole() == "Admin"));

                if (mod is null)
                {
                    response.Success = false;
                    response.Message = "Mod not found";
                    return response;
                }

                mod.Name = updatedMod.Name;
                mod.Description = updatedMod.Description;
                mod.ImageUrl = updatedMod.ImageUrl;
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetModDto>(mod);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            _logger.LogInformation($"UpdateMod(ID={updatedMod.Id}) invoked by {GetUserId()}.");
            return response;
        }

        public async Task<ServiceResponse<string>> DeleteMod(int id)
        {
            var response = new ServiceResponse<string>();
            string logMessage = "";

            try
            {
                //User who created comment or admin
                var mod = await _context.Mods
                    .Include(m => m.Likes)
                    .FirstOrDefaultAsync(m => m.Id == id
                && (m.Creator!.Id == GetUserId() || GetUserRole() == "Admin"));

                if (mod is null)
                {
                    response.Success = false;
                    response.Message = "Mod not found";
                    return response;
                }

                logMessage = mod.Name;

                _context.Likes.RemoveRange(mod.Likes!);
                _context.Mods.Remove(mod);
                await _context.SaveChangesAsync();

                response.Data = "Mod deleted";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            _logger.LogInformation($"DeleteMod({logMessage}) invoked by {GetUserId()}.");
            return response;
        }
    }
}
