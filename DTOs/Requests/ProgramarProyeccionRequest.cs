namespace FestivalCine.DTOs.Requests;

public sealed class ProgramarProyeccionRequest
{
    public required string IdPelicula { get; init; }
    public required string IdSala { get; init; }
    public required string IdEdicion { get; init; }
    public DateTime Fecha { get; init; }
    public TimeSpan Hora { get; init; }
}
