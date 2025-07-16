using ParkManager_Service.Helpers;
using ParkManager_Service.Views;

namespace ParkManager_Service.Services.Interfaces
{
    public interface IUsuario
    {
        Task<UsuarioGetDto?> GetUsuarioByIdAsync(string id);
        Task<Resultado<UsuarioLoginResponseDto>> LoginAsync(UsuarioLoginDto user);
        Task<string?> RegisterAsync(UsuarioRegisterDto user);
        Task<bool> DeleteUsuarioByEmailAsync(string email);
    }
}
