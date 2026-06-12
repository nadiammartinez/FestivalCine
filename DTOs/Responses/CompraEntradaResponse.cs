namespace FestivalCine.DTOs.Responses;

public sealed class CompraEntradaResponse
{
    public required string Mensaje { get; init; }
    public required string IdEntradaGenerado { get; init; }
    public decimal TarifaAplicada { get; init; }
}
