using ParkManager_Service.Views;

namespace ParkManager_Service.Services.Interfaces
{
    public interface IUsuario
    {
        Task<UsuarioGetDto?> GetUsuarioByIdAsync(string id);
        Task<UsuarioLoginResponseDto?> LoginAsync(UsuarioLoginDto user);
        Task<string?> RegisterAsync(UsuarioRegisterDto user);
    }
}
