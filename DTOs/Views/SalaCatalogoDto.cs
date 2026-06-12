namespace FestivalCine.DTOs.Views;

public sealed class SalaCatalogoDto
{
    public required string IdSala { get; init; }
    public required string Sala { get; init; }
    public int Capacidad { get; init; }
    public required string IdSede { get; init; }
    public required string Sede { get; init; }
}
