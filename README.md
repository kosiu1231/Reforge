# Reforge Mods API

An application designed to manage game mods.

## Demo

https://reforgemods.azurewebsites.net/swagger/index.html

Consider that some endpoints require you to log in(lock icon) or use admin account(POST /api/game).

## How to log in

* Use /auth/login endpoint to recive JWT token
* Copy the token from response body
* Click the "Authorize" button
* Enter the token in the format "bearer TOKEN"

## Accounts ready to use

### Admin
Email: user2@example<span>.</span>com

Password: string

### User
Email: user@example<span>.</span>com

Password: string

## If you want to create new account

You can use any "10min mail" site instead of real e-mail address, e.g. https://temp-mail.org

It has to be real address. When creating an account verification code is sent and is needed to verify.

## Tech Stack

* .NET 8
* SQL Server
* Entity Framework
* Linq
* AutoMapper
* Azure
* SendGrid
