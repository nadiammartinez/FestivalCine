using FestivalCine.Common;
using FestivalCine.DTOs.Requests;
using FestivalCine.DTOs.Responses;
using FestivalCine.DTOs.Views;
using FestivalCine.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FestivalCine.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TaquillaController : ControllerBase
{
    private readonly ITaquillaService _taquillaService;

    public TaquillaController(ITaquillaService taquillaService)
    {
        _taquillaService = taquillaService;
    }

    [HttpGet("peliculas")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PeliculaCatalogoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PeliculaCatalogoDto>>>> ListarPeliculas()
    {
        var peliculas = await _taquillaService.ListarPeliculasAsync();
        return Ok(ApiResponse<IReadOnlyList<PeliculaCatalogoDto>>.Success(peliculas));
    }

    [HttpGet("cartelera")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProyeccionCarteleraDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProyeccionCarteleraDto>>>> ListarCartelera()
    {
        var cartelera = await _taquillaService.ListarCarteleraAsync();
        return Ok(ApiResponse<IReadOnlyList<ProyeccionCarteleraDto>>.Success(cartelera));
    }

    [HttpGet("peliculas/{idPelicula}/proyecciones-disponibles")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProyeccionCarteleraDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProyeccionCarteleraDto>>>> ListarProyeccionesDisponibles(string idPelicula)
    {
        var proyecciones = await _taquillaService.ListarProyeccionesDisponiblesAsync(idPelicula);
        return Ok(ApiResponse<IReadOnlyList<ProyeccionCarteleraDto>>.Success(proyecciones));
    }

    [HttpGet("eventos")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<EventoParaleloDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<EventoParaleloDto>>>> ListarEventos()
    {
        var eventos = await _taquillaService.ListarEventosAsync();
        return Ok(ApiResponse<IReadOnlyList<EventoParaleloDto>>.Success(eventos));
    }

    [HttpGet("recaudacion")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RecaudacionVentaDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RecaudacionVentaDto>>>> ObtenerRecaudacion()
    {
        var recaudacion = await _taquillaService.ObtenerRecaudacionAsync();
        return Ok(ApiResponse<IReadOnlyList<RecaudacionVentaDto>>.Success(recaudacion));
    }

    [HttpPost("entradas")]
    [Authorize(Roles = "Admin,Taquilla")]
    [ProducesResponseType(typeof(ApiResponse<CompraEntradaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CompraEntradaResponse>>> ComprarEntrada(ComprarEntradaRequest request)
    {
        var compra = await _taquillaService.ComprarEntradaAsync(request);
        return Ok(ApiResponse<CompraEntradaResponse>.Success(compra, compra.Mensaje));
    }

    [HttpPost("entradas-evento")]
    [Authorize(Roles = "Admin,Taquilla")]
    [ProducesResponseType(typeof(ApiResponse<CompraEntradaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CompraEntradaResponse>>> ComprarEntradaEvento(ComprarEntradaEventoRequest request)
    {
        var compra = await _taquillaService.ComprarEntradaEventoAsync(request);
        return Ok(ApiResponse<CompraEntradaResponse>.Success(compra, compra.Mensaje));
    }

    [HttpPost("abonos")]
    [Authorize(Roles = "Admin,Taquilla")]
    [ProducesResponseType(typeof(ApiResponse<VenderAbonoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<VenderAbonoResponse>>> VenderAbono(VenderAbonoRequest request)
    {
        var venta = await _taquillaService.VenderAbonoAsync(request);
        return Ok(ApiResponse<VenderAbonoResponse>.Success(venta, venta.Mensaje));
    }
}
