namespace FestivalCine.DTOs.Requests;

public sealed class VenderAbonoRequest
{
    public required string IdAsistente { get; init; }
    public required string TipoAbono { get; init; }
    public required string IdEdicion { get; init; }
    public bool PagoAprobado { get; init; }
}
