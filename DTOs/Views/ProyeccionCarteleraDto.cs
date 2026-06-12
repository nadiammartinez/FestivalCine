namespace FestivalCine.DTOs.Views;

public sealed class ProyeccionCarteleraDto
{
    public required string IdProyeccion { get; init; }
    public DateTime Fecha { get; init; }
    public TimeSpan Hora { get; init; }
    public required string IdPelicula { get; init; }
    public required string Titulo { get; init; }
    public int Duracion { get; init; }
    public required string ClasificacionEdad { get; init; }
    public required string Formato { get; init; }
    public required string IdSala { get; init; }
    public required string Sala { get; init; }
    public int Capacidad { get; init; }
    public required string IdSede { get; init; }
    public required string Sede { get; init; }
    public required string IdEdicion { get; init; }
    public required string Edicion { get; init; }
    public int Anio { get; init; }
    public int EntradasVendidas { get; init; }
    public int IngresosPorAbono { get; init; }
    public int TotalOcupado { get; init; }
    public int AforoDisponible { get; init; }
}
