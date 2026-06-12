namespace FestivalCine.DTOs.Requests;

public sealed class ComprarEntradaRequest
{
    public required string IdAsistente { get; init; }
    public required string IdProyeccion { get; init; }
    public required string TipoEntrada { get; init; }
}
