namespace FestivalCine.DTOs.Views;

public sealed class PeliculaCatalogoDto
{
    public required string IdPelicula { get; init; }
    public required string Titulo { get; init; }
    public int AnoProduccion { get; init; }
    public int Duracion { get; init; }
    public required string PaisOrigen { get; init; }
    public string? Sinopsis { get; init; }
    public required string ClasificacionEdad { get; init; }
    public required string Formato { get; init; }
    public required string Estado { get; init; }
    public string? Generos { get; init; }
}
