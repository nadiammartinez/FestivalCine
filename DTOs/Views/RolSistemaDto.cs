namespace FestivalCine.DTOs.Views;

public sealed class RolSistemaDto
{
    public required string IdRolSistema { get; init; }
    public required string Nombre { get; init; }
    public string? Descripcion { get; init; }
    public required string Estado { get; init; }
}
