namespace FestivalCine.DTOs.Views;

public sealed class EdicionFestivalDto
{
    public required string IdEdicion { get; init; }
    public required string Nombre { get; init; }
    public int Anio { get; init; }
    public DateTime FechaInicio { get; init; }
    public DateTime FechaFin { get; init; }
}
