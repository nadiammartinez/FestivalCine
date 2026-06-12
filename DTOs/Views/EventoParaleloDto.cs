namespace FestivalCine.DTOs.Views;

public sealed class EventoParaleloDto
{
    public required string IdEvento { get; init; }
    public required string Nombre { get; init; }
    public required string Tipo { get; init; }
    public DateTime Fecha { get; init; }
    public TimeSpan Hora { get; init; }
    public int Aforo { get; init; }
    public decimal Costo { get; init; }
    public required string IdSala { get; init; }
    public required string Sala { get; init; }
    public required string IdSede { get; init; }
    public required string Sede { get; init; }
    public required string IdEdicion { get; init; }
    public required string Edicion { get; init; }
    public int Anio { get; init; }
    public int EntradasVendidas { get; init; }
    public int AforoDisponible { get; init; }
}
