using FestivalCine.Common;
using FestivalCine.DTOs.Requests;
using FestivalCine.DTOs.Responses;
using FestivalCine.DTOs.Views;
using FestivalCine.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FestivalCine.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public sealed class AgendaController : ControllerBase
{
    private readonly IAgendaService _agendaService;

    public AgendaController(IAgendaService agendaService)
    {
        _agendaService = agendaService;
    }

    [HttpGet("peliculas")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PeliculaCatalogoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PeliculaCatalogoDto>>>> ListarPeliculas()
    {
        var peliculas = await _agendaService.ListarPeliculasAsync();
        return Ok(ApiResponse<IReadOnlyList<PeliculaCatalogoDto>>.Success(peliculas));
    }

    [HttpGet("salas")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SalaCatalogoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SalaCatalogoDto>>>> ListarSalas()
    {
        var salas = await _agendaService.ListarSalasAsync();
        return Ok(ApiResponse<IReadOnlyList<SalaCatalogoDto>>.Success(salas));
    }

    [HttpGet("ediciones")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EdicionFestivalDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<EdicionFestivalDto>>>> ListarEdiciones()
    {
        var ediciones = await _agendaService.ListarEdicionesAsync();
        return Ok(ApiResponse<IReadOnlyList<EdicionFestivalDto>>.Success(ediciones));
    }

    [HttpGet("proyecciones")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProyeccionCarteleraDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProyeccionCarteleraDto>>>> ListarProyecciones()
    {
        var proyecciones = await _agendaService.ListarProyeccionesAsync();
        return Ok(ApiResponse<IReadOnlyList<ProyeccionCarteleraDto>>.Success(proyecciones));
    }

    [HttpPost("proyecciones")]
    [ProducesResponseType(typeof(ApiResponse<ProgramarProyeccionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ProgramarProyeccionResponse>>> ProgramarProyeccion(ProgramarProyeccionRequest request)
    {
        var proyeccion = await _agendaService.ProgramarProyeccionAsync(request);
        return Ok(ApiResponse<ProgramarProyeccionResponse>.Success(proyeccion, proyeccion.Mensaje));
    }
}
