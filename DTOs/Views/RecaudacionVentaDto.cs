namespace FestivalCine.DTOs.Views;

public sealed class RecaudacionVentaDto
{
    public required string TipoVenta { get; init; }
    public required string TipoTarifa { get; init; }
    public int CantidadVentas { get; init; }
    public decimal TotalRecaudado { get; init; }
}
