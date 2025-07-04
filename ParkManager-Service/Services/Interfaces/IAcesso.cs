using ParkManager_Service.Helpers;
using ParkManager_Service.Models;
using ParkManager_Service.Views;

namespace ParkManager_Service.Services.Interfaces
{
    public interface IAcesso
    {
        Task<IEnumerable<AcessoGetDto>> GetAllAcessosAsync();
        Task<AcessoGetDto?> GetAcessoByIdAsync(int id);
        Task<Resultado<AcessoGetDto>> AddAcessoAsync(AcessoCreateDto acesso);
        Task<bool> UpdateAcessoAsync(AcessoUpdateDto acesso);
        Task<bool> DeleteAcessoAsync(int id);
        Task<Resultado<AcessoGetDto>> IdentifyTypeAndValueOfAccessoAsync(int id, bool encerrar = false);
    }
}
