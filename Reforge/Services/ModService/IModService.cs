using Microsoft.AspNetCore.Mvc;

namespace Reforge.Services.ModService
{
    public interface IModService
    {
        Task<ServiceResponse<GetModDto>> AddMod(AddModDto newMod);
        Task<ServiceResponse<List<GetModDto>>> GetMods(QueryObject query);
        Task<ServiceResponse<GetModDto>> GetMod(int id);
        Task<ServiceResponse<string>> DeleteMod(int id);
        Task<ServiceResponse<GetModDto>> UpdateMod(UpdateModDto updatedMod);
        Task<ServiceResponse<GetModDto>> LikeMod(int id);
        Task<ServiceResponse<GetModDto>> DislikeMod(int id);
    }
}
