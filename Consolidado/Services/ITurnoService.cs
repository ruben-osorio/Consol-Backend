using System.Collections.Generic;
using System.Threading.Tasks;
using Consolidado.Models;

namespace Consolidado.Services
{
    public interface ITurnoService
    {
        Task<IEnumerable<TurnoModel>> GetTurnos();
        Task<TurnoModel?> GetTurnoById(int id);
        Task<bool> SincronizarTurnos();
        Task<bool> ActualizarDefinicionTurno(int idTurno, string definicion);
    }
}