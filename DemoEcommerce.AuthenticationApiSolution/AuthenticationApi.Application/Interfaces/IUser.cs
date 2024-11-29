using AuthenticationApi.Application.DTOs;
using eCommerce.SharedLibrary.Responses;

namespace AuthenticationApi.Application.Interfaces;

public interface IUser
{
    Task<Response> Register(AppUserDTO appUserDTO); 
    Task<Response> Login(LoginDTO loginDTOS);
    Task<AppUserDTO> GetUser(int userId);
}
