namespace FestivalCine.DTOs.Views;

public sealed class PremioResultadoDto
{
    public required string IdCategoria { get; init; }
    public required string Categoria { get; init; }
    public required string IdPelicula { get; init; }
    public required string PeliculaGanadora { get; init; }
    public required string IdPremio { get; init; }
    public required string Premio { get; init; }
    public DateTime? FechaEntrega { get; init; }
    public decimal PromedioVotacion { get; init; }
}
