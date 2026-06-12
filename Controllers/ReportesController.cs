using FestivalCine.Common;
using FestivalCine.DTOs.Views;
using FestivalCine.Services;
using Microsoft.AspNetCore.Mvc;

namespace FestivalCine.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ReportesController : ControllerBase
{
    private readonly IReportesService _reportesService;

    public ReportesController(IReportesService reportesService)
    {
        _reportesService = reportesService;
    }

    [HttpGet("ocupacion-peliculas")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<OcupacionPeliculaDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<OcupacionPeliculaDto>>>> ObtenerOcupacionPeliculas()
    {
        var ocupacion = await _reportesService.ObtenerOcupacionPeliculasAsync();
        return Ok(ApiResponse<IReadOnlyList<OcupacionPeliculaDto>>.Success(ocupacion));
    }

    [HttpGet("premios")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PremioResultadoDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PremioResultadoDto>>>> ObtenerPremios()
    {
        var premios = await _reportesService.ObtenerPremiosAsync();
        return Ok(ApiResponse<IReadOnlyList<PremioResultadoDto>>.Success(premios));
    }

    [HttpGet("recaudacion")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RecaudacionVentaDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RecaudacionVentaDto>>>> ObtenerRecaudacion()
    {
        var recaudacion = await _reportesService.ObtenerRecaudacionAsync();
        return Ok(ApiResponse<IReadOnlyList<RecaudacionVentaDto>>.Success(recaudacion));
    }
}
