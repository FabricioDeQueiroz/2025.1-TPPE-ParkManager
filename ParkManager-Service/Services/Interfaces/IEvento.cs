using ParkManager_Service.Models;
using ParkManager_Service.Views;

namespace ParkManager_Service.Services.Interfaces
{
    public interface IEvento
    {
        Task<IEnumerable<EventoGetDto>> GetAllEventosAsync();
        Task<EventoGetDto?> GetEventoByIdAsync(int id);
        Task<EventoGetDto> AddEventoAsync(EventoCreateDto evento);
        Task<bool> UpdateEventoAsync(EventoUpdateDto evento);
        Task<bool> DeleteEventoAsync(int id);
    }
}
