namespace FestivalCine.DTOs.Responses;

public sealed class VenderAbonoResponse
{
    public required string Mensaje { get; init; }
    public required string IdAbono { get; init; }
    public required string IdPago { get; init; }
    public required string IdFactura { get; init; }
    public decimal MontoPagado { get; init; }
}
