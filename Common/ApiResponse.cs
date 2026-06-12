namespace FestivalCine.Common;

public sealed record ApiResponse<T>(
    bool Ok,
    string Mensaje,
    T? Data = default)
{
    public static ApiResponse<T> Success(T? data, string mensaje = "Operacion realizada correctamente")
        => new(true, mensaje, data);

    public static ApiResponse<T> Fail(string mensaje)
        => new(false, mensaje);
}
