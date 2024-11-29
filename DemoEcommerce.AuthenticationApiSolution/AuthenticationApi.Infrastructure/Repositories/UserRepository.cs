using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.Repositories
{
    internal class UserRepository
        (AuthenticationDbContext context,IUser userInterface,IConfiguration configuration) : IUser
    {
        private async Task<AppUser> GetUserByEmail(string email) 
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user is null ? null! : user!;
        }
        public async Task<GetUserDTO> GetUser(int userId)
        {
            var user = await context.Users.FindAsync(userId);
            return user is not null ? new GetUserDTO
                (user.Id, user.Name!, user.TelephoneNumber!, user.Address!, user.Email!, user.Role!) : null! ;

        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var existingUser = await GetUserByEmail(loginDTO.Email);
            if (existingUser is  null)
                return new Response(false, "Invalid Credentials");
            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, existingUser.Password);
            if(!verifyPassword)
                return new Response(false, "Invalid Credentials");

            string token = GenerateToken(existingUser);
            return new Response(true, token);

        }

        private  string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(configuration.GetSection("Authentication:key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name,user.Name!),
                new(ClaimTypes.Email,user.Email!)
            };
            if (!string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role))
                claims.Add(new(ClaimTypes.Role, user.Role!));

            var token = new JwtSecurityToken
                (
                    issuer: configuration["Authentication:Issuer"],
                    audience: configuration["Authentication:Audience"],
                    claims : claims,
                    expires :null,
                    signingCredentials : credentials

                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var existingUser = await GetUserByEmail(appUserDTO.Email);
            if (existingUser is not null)
                return new Response(false, "This email is already in use.");
            var result = context.Users.Add(new AppUser()
            {
                Name = appUserDTO.Name,
                Email = appUserDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
                TelephoneNumber = appUserDTO.TelephoneNumber,
                Address = appUserDTO.Address,
                Role = appUserDTO.Role
            });
            await context.SaveChangesAsync();
            return result.Entity.Id > 0 ? new Response(true, "User Registered Scuccessfully")
                : new Response(false, "Invalid data");

        }
    }
}
