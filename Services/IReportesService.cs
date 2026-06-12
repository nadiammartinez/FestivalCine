using FestivalCine.DTOs.Views;

namespace FestivalCine.Services;

public interface IReportesService
{
    Task<IReadOnlyList<OcupacionPeliculaDto>> ObtenerOcupacionPeliculasAsync();
    Task<IReadOnlyList<PremioResultadoDto>> ObtenerPremiosAsync();
    Task<IReadOnlyList<RecaudacionVentaDto>> ObtenerRecaudacionAsync();
}
