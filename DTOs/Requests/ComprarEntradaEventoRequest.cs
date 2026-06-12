namespace FestivalCine.DTOs.Requests;

public sealed class ComprarEntradaEventoRequest
{
    public required string IdAsistente { get; init; }
    public required string IdEvento { get; init; }
    public required string TipoEntrada { get; init; }
}
