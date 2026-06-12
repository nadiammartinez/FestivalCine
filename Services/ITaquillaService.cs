using FestivalCine.DTOs.Requests;
using FestivalCine.DTOs.Responses;
using FestivalCine.DTOs.Views;

namespace FestivalCine.Services;

public interface ITaquillaService
{
    Task<IReadOnlyList<PeliculaCatalogoDto>> ListarPeliculasAsync();
    Task<IReadOnlyList<ProyeccionCarteleraDto>> ListarCarteleraAsync();
    Task<IReadOnlyList<ProyeccionCarteleraDto>> ListarProyeccionesDisponiblesAsync(string idPelicula);
    Task<IReadOnlyList<EventoParaleloDto>> ListarEventosAsync();
    Task<IReadOnlyList<RecaudacionVentaDto>> ObtenerRecaudacionAsync();
    Task<CompraEntradaResponse> ComprarEntradaAsync(ComprarEntradaRequest request);
    Task<CompraEntradaResponse> ComprarEntradaEventoAsync(ComprarEntradaEventoRequest request);
    Task<VenderAbonoResponse> VenderAbonoAsync(VenderAbonoRequest request);
}
