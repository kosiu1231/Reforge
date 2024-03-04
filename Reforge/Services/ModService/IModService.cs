namespace Reforge.Services.ModService
{
    public interface IModService
    {
        Task<ServiceResponse<GetModDto>> AddMod(AddModDto newMod);
    }
}
