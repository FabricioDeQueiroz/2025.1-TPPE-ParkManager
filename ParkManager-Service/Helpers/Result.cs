using ParkManager_Service.Models;
using ParkManager_Service.Views;

namespace ParkManager_Service.Helpers
{
    public class Resultado<T>
    {
        public bool Success { get; init; }
        public string? Error { get; init; }
        public T? Data { get; init; }

        public static Resultado<T> Ok(T data) => new() { Success = true, Data = data };
        public static Resultado<T> Falha(string error) => new() { Success = false, Error = error };
    }
}
