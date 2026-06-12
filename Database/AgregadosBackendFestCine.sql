USE FestCineSantaCruzDB;
GO

/* ============================================================
   FUNCIONES DE REGLAS DE NEGOCIO
   ============================================================ */

CREATE OR ALTER FUNCTION dbo.fn_TarifaEntrada
(
    @TipoEntrada VARCHAR(60)
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Tarifa DECIMAL(10,2);

    SET @Tarifa =
        CASE @TipoEntrada
            WHEN 'General' THEN 25.00
            WHEN 'Estudiante' THEN 15.00
            WHEN 'Jubilado' THEN 12.00
            WHEN 'Acreditado' THEN 0.00
            ELSE NULL
        END;

    RETURN @Tarifa;
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_TarifaAbono
(
    @TipoAbono VARCHAR(60)
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Precio DECIMAL(10,2);

    SET @Precio =
        CASE @TipoAbono
            WHEN 'Fin de Semana' THEN 80.00
            WHEN 'Total' THEN 150.00
            WHEN 'VIP' THEN 0.00
            WHEN 'Estudiante' THEN 60.00
            ELSE NULL
        END;

    RETURN @Precio;
END;
GO

CREATE OR ALTER FUNCTION dbo.fn_AforoDisponibleProyeccion
(
    @IdProyeccion CHAR(5)
)
RETURNS INT
AS
BEGIN
    DECLARE @Capacidad INT;
    DECLARE @EntradasVendidas INT;
    DECLARE @IngresosPorAbono INT;
    DECLARE @Disponible INT;

    SELECT @Capacidad = s.Capacidad
    FROM Proyeccion pr
    INNER JOIN Sala s ON pr.IdSala = s.IdSala
    WHERE pr.IdProyeccion = @IdProyeccion;

    SELECT @EntradasVendidas = COUNT(*)
    FROM Entrada
    WHERE IdProyeccion = @IdProyeccion;

    SELECT @IngresosPorAbono = COUNT(*)
    FROM Abono_Proyeccion
    WHERE IdProyeccion = @IdProyeccion;

    SET @Disponible = ISNULL(@Capacidad, 0)
        - ISNULL(@EntradasVendidas, 0)
        - ISNULL(@IngresosPorAbono, 0);

    RETURN @Disponible;
END;
GO

/* ============================================================
   VISTAS PARA LECTURA DESDE EL BACKEND
   ============================================================ */

CREATE OR ALTER VIEW dbo.vw_PeliculasCatalogo
AS
SELECT
    p.IdPelicula,
    p.Titulo,
    p.AnoProduccion,
    p.Duracion,
    p.PaisOrigen,
    p.Sinopsis,
    p.ClasificacionEdad,
    p.Formato,
    p.Estado,
    STRING_AGG(g.Genero, ', ') AS Generos
FROM Pelicula p
LEFT JOIN Pelicula_Genero pg ON p.IdPelicula = pg.IdPelicula
LEFT JOIN Genero g ON pg.IdGenero = g.IdGenero
GROUP BY
    p.IdPelicula,
    p.Titulo,
    p.AnoProduccion,
    p.Duracion,
    p.PaisOrigen,
    p.Sinopsis,
    p.ClasificacionEdad,
    p.Formato,
    p.Estado;
GO

CREATE OR ALTER VIEW dbo.vw_SalasCatalogo
AS
SELECT
    s.IdSala,
    s.Nombre AS Sala,
    s.Capacidad,
    se.IdSede,
    se.Nombre AS Sede
FROM Sala s
INNER JOIN Sede se ON s.IdSede = se.IdSede;
GO

CREATE OR ALTER VIEW dbo.vw_EdicionesFestival
AS
SELECT
    IdEdicion,
    Nombre,
    Anio,
    FechaInicio,
    FechaFin
FROM EdicionFestival;
GO

CREATE OR ALTER VIEW dbo.vw_AsistentesCatalogo
AS
SELECT
    IdAsistente,
    Nombre
FROM Asistentes;
GO

CREATE OR ALTER VIEW dbo.vw_CarteleraProyecciones
AS
SELECT
    pr.IdProyeccion,
    pr.Fecha,
    pr.Hora,
    pr.IdPelicula,
    p.Titulo,
    p.Duracion,
    p.ClasificacionEdad,
    p.Formato,
    pr.IdSala,
    s.Nombre AS Sala,
    s.Capacidad,
    se.IdSede,
    se.Nombre AS Sede,
    pr.IdEdicion,
    ed.Nombre AS Edicion,
    ed.Anio,
    COUNT(DISTINCT e.IdEntrada) AS EntradasVendidas,
    COUNT(DISTINCT ap.IdAbono) AS IngresosPorAbono,
    COUNT(DISTINCT e.IdEntrada) + COUNT(DISTINCT ap.IdAbono) AS TotalOcupado,
    s.Capacidad - COUNT(DISTINCT e.IdEntrada) - COUNT(DISTINCT ap.IdAbono) AS AforoDisponible
FROM Proyeccion pr
INNER JOIN Pelicula p ON pr.IdPelicula = p.IdPelicula
INNER JOIN Sala s ON pr.IdSala = s.IdSala
INNER JOIN Sede se ON s.IdSede = se.IdSede
INNER JOIN EdicionFestival ed ON pr.IdEdicion = ed.IdEdicion
LEFT JOIN Entrada e ON pr.IdProyeccion = e.IdProyeccion
LEFT JOIN Abono_Proyeccion ap ON pr.IdProyeccion = ap.IdProyeccion
GROUP BY
    pr.IdProyeccion,
    pr.Fecha,
    pr.Hora,
    pr.IdPelicula,
    p.Titulo,
    p.Duracion,
    p.ClasificacionEdad,
    p.Formato,
    pr.IdSala,
    s.Nombre,
    s.Capacidad,
    se.IdSede,
    se.Nombre,
    pr.IdEdicion,
    ed.Nombre,
    ed.Anio;
GO

CREATE OR ALTER VIEW dbo.vw_EventosParalelosCartelera
AS
SELECT
    ev.IdEvento,
    ev.Nombre,
    ev.Tipo,
    ev.Fecha,
    ev.Hora,
    ev.Aforo,
    ev.Costo,
    ev.IdSala,
    s.Nombre AS Sala,
    se.IdSede,
    se.Nombre AS Sede,
    ev.IdEdicion,
    ed.Nombre AS Edicion,
    ed.Anio,
    COUNT(e.IdEntrada) AS EntradasVendidas,
    ev.Aforo - COUNT(e.IdEntrada) AS AforoDisponible
FROM EventoParalelo ev
INNER JOIN Sala s ON ev.IdSala = s.IdSala
INNER JOIN Sede se ON s.IdSede = se.IdSede
INNER JOIN EdicionFestival ed ON ev.IdEdicion = ed.IdEdicion
LEFT JOIN Entrada e ON ev.IdEvento = e.IdEvento
GROUP BY
    ev.IdEvento,
    ev.Nombre,
    ev.Tipo,
    ev.Fecha,
    ev.Hora,
    ev.Aforo,
    ev.Costo,
    ev.IdSala,
    s.Nombre,
    se.IdSede,
    se.Nombre,
    ev.IdEdicion,
    ed.Nombre,
    ed.Anio;
GO

CREATE OR ALTER VIEW dbo.vw_OcupacionPeliculas
AS
WITH OcupacionPorProyeccion AS
(
    SELECT
        pr.IdPelicula,
        pr.IdEdicion,
        pr.IdProyeccion,
        s.Capacidad,
        COUNT(DISTINCT e.IdEntrada) AS EntradasVendidas,
        COUNT(DISTINCT ap.IdAbono) AS IngresosPorAbono
    FROM Proyeccion pr
    INNER JOIN Sala s ON pr.IdSala = s.IdSala
    LEFT JOIN Entrada e ON pr.IdProyeccion = e.IdProyeccion
    LEFT JOIN Abono_Proyeccion ap ON pr.IdProyeccion = ap.IdProyeccion
    GROUP BY
        pr.IdPelicula,
        pr.IdEdicion,
        pr.IdProyeccion,
        s.Capacidad
)
SELECT
    ed.IdEdicion,
    ed.Nombre AS Edicion,
    ed.Anio,
    p.IdPelicula,
    p.Titulo,
    SUM(op.EntradasVendidas) AS EntradasVendidas,
    SUM(op.IngresosPorAbono) AS IngresosPorAbono,
    SUM(op.EntradasVendidas + op.IngresosPorAbono) AS TotalAsistentes,
    SUM(op.Capacidad) AS CapacidadTotal,
    CAST(
        SUM(op.EntradasVendidas + op.IngresosPorAbono) * 100.0
        / NULLIF(SUM(op.Capacidad), 0)
        AS DECIMAL(10,2)
    ) AS PorcentajeOcupacion
FROM OcupacionPorProyeccion op
INNER JOIN Pelicula p ON op.IdPelicula = p.IdPelicula
INNER JOIN EdicionFestival ed ON op.IdEdicion = ed.IdEdicion
GROUP BY
    ed.IdEdicion,
    ed.Nombre,
    ed.Anio,
    p.IdPelicula,
    p.Titulo;
GO

CREATE OR ALTER VIEW dbo.vw_PremiosResultados
AS
SELECT
    c.IdCategoria,
    c.Nombre AS Categoria,
    p.IdPelicula,
    p.Titulo AS PeliculaGanadora,
    pr.IdPremio,
    pr.Nombre AS Premio,
    pr.FechaEntrega,
    CAST(AVG(e.Puntaje * 1.0) AS DECIMAL(10,2)) AS PromedioVotacion
FROM Premio pr
INNER JOIN Categoria c ON pr.IdCategoria = c.IdCategoria
INNER JOIN Pelicula p ON pr.IdPelicula = p.IdPelicula
INNER JOIN Evaluacion e
    ON p.IdPelicula = e.IdPelicula
   AND c.IdCategoria = e.IdCategoria
GROUP BY
    c.IdCategoria,
    c.Nombre,
    p.IdPelicula,
    p.Titulo,
    pr.IdPremio,
    pr.Nombre,
    pr.FechaEntrega;
GO

CREATE OR ALTER VIEW dbo.vw_RecaudacionVentas
AS
SELECT
    'Entrada Individual' AS TipoVenta,
    TipoEntrada AS TipoTarifa,
    COUNT(*) AS CantidadVentas,
    SUM(Tarifa) AS TotalRecaudado
FROM Entrada
GROUP BY TipoEntrada

UNION ALL

SELECT
    'Abono' AS TipoVenta,
    Tipo AS TipoTarifa,
    COUNT(*) AS CantidadVentas,
    SUM(Precio) AS TotalRecaudado
FROM Abono
GROUP BY Tipo;
GO

CREATE OR ALTER VIEW dbo.vw_CodigosAccesoAbono
AS
SELECT
    ca.IdCodigo,
    ca.CodigoAcceso,
    ca.IdAbono,
    a.IdAsistente,
    asi.Nombre AS Asistente,
    a.Tipo AS TipoAbono,
    ca.IdProyeccion,
    p.Titulo,
    pr.Fecha,
    pr.Hora
FROM CodigoAccesoAbono ca
INNER JOIN Abono a ON ca.IdAbono = a.IdAbono
INNER JOIN Asistentes asi ON a.IdAsistente = asi.IdAsistente
INNER JOIN Proyeccion pr ON ca.IdProyeccion = pr.IdProyeccion
INNER JOIN Pelicula p ON pr.IdPelicula = p.IdPelicula;
GO

/* ============================================================
   PROCEDIMIENTOS PARA OPERACIONES DESDE EL BACKEND
   ============================================================ */

CREATE OR ALTER PROCEDURE dbo.ComprarEntrada
    @IdAsistente CHAR(5),
    @IdProyeccion CHAR(5),
    @TipoEntrada VARCHAR(60)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Capacidad INT;
    DECLARE @EntradasVendidas INT;
    DECLARE @IngresosPorAbono INT;
    DECLARE @TotalOcupado INT;
    DECLARE @Tarifa DECIMAL(10,2);
    DECLARE @NuevoIdEntrada CHAR(5);
    DECLARE @Numero INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM Asistentes WHERE IdAsistente = @IdAsistente)
            RAISERROR('El asistente no existe.', 16, 1);

        IF NOT EXISTS (SELECT 1 FROM Proyeccion WHERE IdProyeccion = @IdProyeccion)
            RAISERROR('La proyeccion no existe.', 16, 1);

        SET @Tarifa = dbo.fn_TarifaEntrada(@TipoEntrada);

        IF @Tarifa IS NULL
            RAISERROR('Tipo de tarifa no valido.', 16, 1);

        SELECT @Capacidad = s.Capacidad
        FROM Proyeccion pr
        INNER JOIN Sala s ON pr.IdSala = s.IdSala
        WHERE pr.IdProyeccion = @IdProyeccion;

        SELECT @EntradasVendidas = COUNT(*)
        FROM Entrada WITH (UPDLOCK, HOLDLOCK)
        WHERE IdProyeccion = @IdProyeccion;

        SELECT @IngresosPorAbono = COUNT(*)
        FROM Abono_Proyeccion
        WHERE IdProyeccion = @IdProyeccion;

        SET @TotalOcupado = @EntradasVendidas + @IngresosPorAbono;

        IF @TotalOcupado >= @Capacidad
            RAISERROR('No hay aforo disponible para esta proyeccion.', 16, 1);

        SELECT @Numero = ISNULL(MAX(CAST(SUBSTRING(IdEntrada, 3, 3) AS INT)), 0) + 1
        FROM Entrada WITH (UPDLOCK, HOLDLOCK);

        SET @NuevoIdEntrada = 'EN' + RIGHT('000' + CAST(@Numero AS VARCHAR(3)), 3);

        INSERT INTO Entrada (
            IdEntrada,
            IdAsistente,
            IdProyeccion,
            IdEvento,
            TipoEntrada,
            Tarifa
        )
        VALUES (
            @NuevoIdEntrada,
            @IdAsistente,
            @IdProyeccion,
            NULL,
            @TipoEntrada,
            @Tarifa
        );

        COMMIT;

        SELECT
            'Compra registrada correctamente' AS Mensaje,
            @NuevoIdEntrada AS IdEntradaGenerado,
            @Tarifa AS TarifaAplicada;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ComprarEntradaEvento
    @IdAsistente CHAR(5),
    @IdEvento CHAR(5),
    @TipoEntrada VARCHAR(60)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Aforo INT;
    DECLARE @EntradasVendidas INT;
    DECLARE @Tarifa DECIMAL(10,2);
    DECLARE @NuevoIdEntrada CHAR(5);
    DECLARE @Numero INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM Asistentes WHERE IdAsistente = @IdAsistente)
            RAISERROR('El asistente no existe.', 16, 1);

        IF NOT EXISTS (SELECT 1 FROM EventoParalelo WHERE IdEvento = @IdEvento)
            RAISERROR('El evento paralelo no existe.', 16, 1);

        SELECT @Aforo = Aforo, @Tarifa = Costo
        FROM EventoParalelo
        WHERE IdEvento = @IdEvento;

        IF @TipoEntrada <> 'General'
            RAISERROR('Para eventos paralelos solo se permite tarifa General.', 16, 1);

        SELECT @EntradasVendidas = COUNT(*)
        FROM Entrada WITH (UPDLOCK, HOLDLOCK)
        WHERE IdEvento = @IdEvento;

        IF @EntradasVendidas >= @Aforo
            RAISERROR('No hay aforo disponible para este evento paralelo.', 16, 1);

        SELECT @Numero = ISNULL(MAX(CAST(SUBSTRING(IdEntrada, 3, 3) AS INT)), 0) + 1
        FROM Entrada WITH (UPDLOCK, HOLDLOCK);

        SET @NuevoIdEntrada = 'EN' + RIGHT('000' + CAST(@Numero AS VARCHAR(3)), 3);

        INSERT INTO Entrada (
            IdEntrada,
            IdAsistente,
            IdProyeccion,
            IdEvento,
            TipoEntrada,
            Tarifa
        )
        VALUES (
            @NuevoIdEntrada,
            @IdAsistente,
            NULL,
            @IdEvento,
            @TipoEntrada,
            @Tarifa
        );

        COMMIT;

        SELECT
            'Compra para evento registrada correctamente' AS Mensaje,
            @NuevoIdEntrada AS IdEntradaGenerado,
            @Tarifa AS TarifaAplicada;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.VenderAbono
    @IdAsistente CHAR(5),
    @TipoAbono VARCHAR(60),
    @IdEdicion CHAR(5),
    @PagoAprobado BIT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Precio DECIMAL(10,2);
    DECLARE @NuevoIdAbono CHAR(5);
    DECLARE @NuevoIdPago CHAR(5);
    DECLARE @NuevoIdFactura CHAR(5);
    DECLARE @Numero INT;
    DECLARE @UltimoCodigo INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM Asistentes WHERE IdAsistente = @IdAsistente)
            RAISERROR('El asistente no existe.', 16, 1);

        IF NOT EXISTS (SELECT 1 FROM EdicionFestival WHERE IdEdicion = @IdEdicion)
            RAISERROR('La edicion del festival no existe.', 16, 1);

        SET @Precio = dbo.fn_TarifaAbono(@TipoAbono);

        IF @Precio IS NULL
            RAISERROR('Tipo de abono no valido.', 16, 1);

        IF @PagoAprobado = 0
            RAISERROR('El pago fue rechazado por la pasarela.', 16, 1);

        SELECT @Numero = ISNULL(MAX(CAST(SUBSTRING(IdAbono, 3, 3) AS INT)), 0) + 1
        FROM Abono WITH (UPDLOCK, HOLDLOCK);

        SET @NuevoIdAbono = 'AB' + RIGHT('000' + CAST(@Numero AS VARCHAR(3)), 3);

        INSERT INTO Abono (
            IdAbono,
            IdAsistente,
            Nombre,
            Tipo,
            Precio
        )
        VALUES (
            @NuevoIdAbono,
            @IdAsistente,
            'Abono ' + @TipoAbono,
            @TipoAbono,
            @Precio
        );

        SELECT @Numero = ISNULL(MAX(CAST(SUBSTRING(IdPago, 3, 3) AS INT)), 0) + 1
        FROM PagoAbono WITH (UPDLOCK, HOLDLOCK);

        SET @NuevoIdPago = 'PG' + RIGHT('000' + CAST(@Numero AS VARCHAR(3)), 3);

        INSERT INTO PagoAbono (
            IdPago,
            IdAbono,
            FechaPago,
            Monto,
            EstadoPago
        )
        VALUES (
            @NuevoIdPago,
            @NuevoIdAbono,
            GETDATE(),
            @Precio,
            'Pagado'
        );

        IF @TipoAbono = 'Fin de Semana'
        BEGIN
            SET DATEFIRST 1;

            INSERT INTO Abono_Proyeccion (
                IdAbono,
                IdProyeccion
            )
            SELECT @NuevoIdAbono, IdProyeccion
            FROM Proyeccion
            WHERE IdEdicion = @IdEdicion
              AND DATEPART(WEEKDAY, Fecha) IN (6, 7);
        END
        ELSE
        BEGIN
            INSERT INTO Abono_Proyeccion (
                IdAbono,
                IdProyeccion
            )
            SELECT @NuevoIdAbono, IdProyeccion
            FROM Proyeccion
            WHERE IdEdicion = @IdEdicion;
        END;

        IF NOT EXISTS (
            SELECT 1
            FROM Abono_Proyeccion
            WHERE IdAbono = @NuevoIdAbono
        )
            RAISERROR('No existen proyecciones disponibles para generar accesos.', 16, 1);

        SELECT @UltimoCodigo = ISNULL(MAX(CAST(SUBSTRING(IdCodigo, 3, 3) AS INT)), 0)
        FROM CodigoAccesoAbono WITH (UPDLOCK, HOLDLOCK);

        INSERT INTO CodigoAccesoAbono (
            IdCodigo,
            IdAbono,
            IdProyeccion,
            CodigoAcceso
        )
        SELECT
            'CA' + RIGHT('000' + CAST(@UltimoCodigo + ROW_NUMBER() OVER (ORDER BY IdProyeccion) AS VARCHAR(3)), 3),
            IdAbono,
            IdProyeccion,
            IdAbono + '-' + IdProyeccion
        FROM Abono_Proyeccion
        WHERE IdAbono = @NuevoIdAbono;

        SELECT @Numero = ISNULL(MAX(CAST(SUBSTRING(IdFactura, 3, 3) AS INT)), 0) + 1
        FROM FacturaAbono WITH (UPDLOCK, HOLDLOCK);

        SET @NuevoIdFactura = 'FA' + RIGHT('000' + CAST(@Numero AS VARCHAR(3)), 3);

        INSERT INTO FacturaAbono (
            IdFactura,
            IdAbono,
            FechaEmision,
            MontoTotal
        )
        VALUES (
            @NuevoIdFactura,
            @NuevoIdAbono,
            GETDATE(),
            @Precio
        );

        COMMIT;

        SELECT
            'Venta de abono registrada correctamente' AS Mensaje,
            @NuevoIdAbono AS IdAbono,
            @NuevoIdPago AS IdPago,
            @NuevoIdFactura AS IdFactura,
            @Precio AS MontoPagado;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ProgramarProyeccion
    @IdPelicula CHAR(5),
    @IdSala CHAR(5),
    @IdEdicion CHAR(5),
    @Fecha DATE,
    @Hora TIME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NuevoIdProyeccion CHAR(5);
    DECLARE @Numero INT;
    DECLARE @FechaInicio DATE;
    DECLARE @FechaFin DATE;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM Pelicula WHERE IdPelicula = @IdPelicula)
            RAISERROR('La pelicula no existe.', 16, 1);

        IF NOT EXISTS (SELECT 1 FROM Sala WHERE IdSala = @IdSala)
            RAISERROR('La sala no existe.', 16, 1);

        SELECT
            @FechaInicio = FechaInicio,
            @FechaFin = FechaFin
        FROM EdicionFestival
        WHERE IdEdicion = @IdEdicion;

        IF @FechaInicio IS NULL
            RAISERROR('La edicion del festival no existe.', 16, 1);

        IF @Fecha < @FechaInicio OR @Fecha > @FechaFin
            RAISERROR('La fecha de proyeccion esta fuera del rango de la edicion del festival.', 16, 1);

        SELECT @Numero = ISNULL(MAX(CAST(SUBSTRING(IdProyeccion, 3, 3) AS INT)), 0) + 1
        FROM Proyeccion WITH (UPDLOCK, HOLDLOCK);

        SET @NuevoIdProyeccion = 'PR' + RIGHT('000' + CAST(@Numero AS VARCHAR(3)), 3);

        INSERT INTO Proyeccion (
            IdProyeccion,
            IdPelicula,
            IdSala,
            IdEdicion,
            Fecha,
            Hora
        )
        VALUES (
            @NuevoIdProyeccion,
            @IdPelicula,
            @IdSala,
            @IdEdicion,
            @Fecha,
            @Hora
        );

        COMMIT;

        SELECT
            'Proyeccion programada correctamente' AS Mensaje,
            @NuevoIdProyeccion AS IdProyeccionGenerado;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH;
END;
GO
