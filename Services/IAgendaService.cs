using FestivalCine.DTOs.Requests;
using FestivalCine.DTOs.Responses;
using FestivalCine.DTOs.Views;

namespace FestivalCine.Services;

public interface IAgendaService
{
    Task<IReadOnlyList<PeliculaCatalogoDto>> ListarPeliculasAsync();
    Task<IReadOnlyList<SalaCatalogoDto>> ListarSalasAsync();
    Task<IReadOnlyList<EdicionFestivalDto>> ListarEdicionesAsync();
    Task<IReadOnlyList<ProyeccionCarteleraDto>> ListarProyeccionesAsync();
    Task<ProgramarProyeccionResponse> ProgramarProyeccionAsync(ProgramarProyeccionRequest request);
}
