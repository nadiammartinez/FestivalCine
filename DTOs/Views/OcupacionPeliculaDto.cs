namespace FestivalCine.DTOs.Views;

public sealed class OcupacionPeliculaDto
{
    public required string IdEdicion { get; init; }
    public required string Edicion { get; init; }
    public int Anio { get; init; }
    public required string IdPelicula { get; init; }
    public required string Titulo { get; init; }
    public int EntradasVendidas { get; init; }
    public int IngresosPorAbono { get; init; }
    public int TotalAsistentes { get; init; }
    public int CapacidadTotal { get; init; }
    public decimal PorcentajeOcupacion { get; init; }
}
