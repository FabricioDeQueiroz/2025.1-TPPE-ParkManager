using ParkManager_Service.Views;

namespace ParkManager_Service.Services.Interfaces
{
    public interface IEstacionamento
    {
        Task<IEnumerable<EstacionamentoGetDto>> GetAllEstacionamentosAsync();
        Task<EstacionamentoGetDto?> GetEstacionamentoByIdAsync(int id);
        Task<EstacionamentoGetDto> AddEstacionamentoAsync(Models.Estacionamento estacionamento);
        Task<bool> UpdateEstacionamentoAsync(Models.Estacionamento estacionamento);
        Task<bool> DeleteEstacionamentoAsync(int id);
    }
}
