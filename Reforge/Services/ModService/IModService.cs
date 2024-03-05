using Microsoft.AspNetCore.Mvc;

namespace Reforge.Services.ModService
{
    public interface IModService
    {
        Task<ServiceResponse<GetModDto>> AddMod(AddModDto newMod);
        Task<ServiceResponse<List<GetModDto>>> GetMods();
    }
}
