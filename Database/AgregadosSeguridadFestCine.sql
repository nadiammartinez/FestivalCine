USE FestCineSantaCruzDB;
GO

/* ============================================================
   SEGURIDAD DE APLICACION

   Estas tablas son para usuarios del sistema, no para roles
   cinematograficos. La tabla Rol existente se mantiene para
   Director, Guionista, Productor, etc.
   ============================================================ */

CREATE TABLE RolSistema (
    IdRolSistema CHAR(5) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL UNIQUE,
    Descripcion VARCHAR(200) NULL,
    Estado VARCHAR(30) NOT NULL DEFAULT 'Activo',
    CHECK (Estado IN ('Activo', 'Inactivo'))
);
GO

CREATE TABLE UsuarioSistema (
    IdUsuario CHAR(5) PRIMARY KEY,
    NombreUsuario VARCHAR(80) NOT NULL UNIQUE,
    NombreCompleto VARCHAR(120) NOT NULL,
    Email VARCHAR(120) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Estado VARCHAR(30) NOT NULL DEFAULT 'Activo',
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    UltimoAcceso DATETIME NULL,
    CHECK (Estado IN ('Activo', 'Inactivo', 'Bloqueado'))
);
GO

CREATE TABLE UsuarioRolSistema (
    IdUsuario CHAR(5) NOT NULL,
    IdRolSistema CHAR(5) NOT NULL,
    FechaAsignacion DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (IdUsuario, IdRolSistema),
    FOREIGN KEY (IdUsuario) REFERENCES UsuarioSistema(IdUsuario),
    FOREIGN KEY (IdRolSistema) REFERENCES RolSistema(IdRolSistema)
);
GO

INSERT INTO RolSistema (
    IdRolSistema,
    Nombre,
    Descripcion,
    Estado
)
VALUES
('RS001', 'Admin', 'Administra agenda, catalogos, reportes y usuarios del sistema.', 'Activo'),
('RS002', 'Taquilla', 'Puede vender entradas, vender abonos y consultar cartelera.', 'Activo'),
('RS003', 'Consulta', 'Puede consultar cartelera, peliculas, eventos y datos publicos.', 'Activo');
GO

/* PasswordHash debe guardar un hash generado por el backend.
   Este usuario inicial sirve solo como semilla academica.
   Cambia el hash por uno real antes de publicar. */
INSERT INTO UsuarioSistema (
    IdUsuario,
    NombreUsuario,
    NombreCompleto,
    Email,
    PasswordHash,
    Estado
)
VALUES (
    'US001',
    'admin',
    'Administrador FestCine',
    'admin@festcine.com',
    'CAMBIAR_POR_HASH_REAL',
    'Activo'
);
GO

INSERT INTO UsuarioRolSistema (
    IdUsuario,
    IdRolSistema
)
VALUES ('US001', 'RS001');
GO

/* ============================================================
   VISTAS PARA EL BACKEND
   ============================================================ */

CREATE OR ALTER VIEW dbo.vw_RolesSistema
AS
SELECT
    IdRolSistema,
    Nombre,
    Descripcion,
    Estado
FROM RolSistema;
GO

CREATE OR ALTER VIEW dbo.vw_UsuariosSistema
AS
SELECT
    u.IdUsuario,
    u.NombreUsuario,
    u.NombreCompleto,
    u.Email,
    u.Estado,
    u.FechaCreacion,
    u.UltimoAcceso,
    STRING_AGG(r.Nombre, ', ') AS Roles
FROM UsuarioSistema u
LEFT JOIN UsuarioRolSistema ur ON u.IdUsuario = ur.IdUsuario
LEFT JOIN RolSistema r ON ur.IdRolSistema = r.IdRolSistema
GROUP BY
    u.IdUsuario,
    u.NombreUsuario,
    u.NombreCompleto,
    u.Email,
    u.Estado,
    u.FechaCreacion,
    u.UltimoAcceso;
GO

CREATE OR ALTER VIEW dbo.vw_UsuariosSistemaAutenticacion
AS
SELECT
    u.IdUsuario,
    u.NombreUsuario,
    u.NombreCompleto,
    u.Email,
    u.PasswordHash,
    u.Estado,
    r.Nombre AS Rol
FROM UsuarioSistema u
INNER JOIN UsuarioRolSistema ur ON u.IdUsuario = ur.IdUsuario
INNER JOIN RolSistema r ON ur.IdRolSistema = r.IdRolSistema
WHERE u.Estado = 'Activo'
  AND r.Estado = 'Activo';
GO

/* ============================================================
   PROCEDIMIENTOS PARA OPERACIONES DE SEGURIDAD
   ============================================================ */

CREATE OR ALTER PROCEDURE dbo.RegistrarUsuarioSistema
    @NombreUsuario VARCHAR(80),
    @NombreCompleto VARCHAR(120),
    @Email VARCHAR(120),
    @PasswordHash VARCHAR(255),
    @Rol VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NuevoIdUsuario CHAR(5);
    DECLARE @IdRolSistema CHAR(5);
    DECLARE @Numero INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM UsuarioSistema WHERE NombreUsuario = @NombreUsuario)
            RAISERROR('El nombre de usuario ya existe.', 16, 1);

        IF EXISTS (SELECT 1 FROM UsuarioSistema WHERE Email = @Email)
            RAISERROR('El correo electronico ya esta registrado.', 16, 1);

        SELECT @IdRolSistema = IdRolSistema
        FROM RolSistema
        WHERE Nombre = @Rol
          AND Estado = 'Activo';

        IF @IdRolSistema IS NULL
            RAISERROR('El rol indicado no existe o esta inactivo.', 16, 1);

        SELECT @Numero = ISNULL(MAX(CAST(SUBSTRING(IdUsuario, 3, 3) AS INT)), 0) + 1
        FROM UsuarioSistema WITH (UPDLOCK, HOLDLOCK);

        SET @NuevoIdUsuario = 'US' + RIGHT('000' + CAST(@Numero AS VARCHAR(3)), 3);

        INSERT INTO UsuarioSistema (
            IdUsuario,
            NombreUsuario,
            NombreCompleto,
            Email,
            PasswordHash,
            Estado
        )
        VALUES (
            @NuevoIdUsuario,
            @NombreUsuario,
            @NombreCompleto,
            @Email,
            @PasswordHash,
            'Activo'
        );

        INSERT INTO UsuarioRolSistema (
            IdUsuario,
            IdRolSistema
        )
        VALUES (
            @NuevoIdUsuario,
            @IdRolSistema
        );

        COMMIT;

        SELECT
            'Usuario registrado correctamente' AS Mensaje,
            @NuevoIdUsuario AS IdUsuarioGenerado;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.ObtenerUsuarioParaLogin
    @NombreUsuario VARCHAR(80)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdUsuario,
        NombreUsuario,
        NombreCompleto,
        Email,
        PasswordHash,
        Estado,
        Rol
    FROM dbo.vw_UsuariosSistemaAutenticacion
    WHERE NombreUsuario = @NombreUsuario
       OR Email = @NombreUsuario;
END;
GO

CREATE OR ALTER PROCEDURE dbo.RegistrarAccesoUsuarioSistema
    @IdUsuario CHAR(5)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE UsuarioSistema
    SET UltimoAcceso = GETDATE()
    WHERE IdUsuario = @IdUsuario
      AND Estado = 'Activo';

    SELECT
        'Acceso registrado correctamente' AS Mensaje,
        @IdUsuario AS IdUsuario;
END;
GO

CREATE OR ALTER PROCEDURE dbo.CambiarEstadoUsuarioSistema
    @IdUsuario CHAR(5),
    @Estado VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Estado NOT IN ('Activo', 'Inactivo', 'Bloqueado')
        RAISERROR('Estado de usuario no valido.', 16, 1);

    IF NOT EXISTS (SELECT 1 FROM UsuarioSistema WHERE IdUsuario = @IdUsuario)
        RAISERROR('El usuario no existe.', 16, 1);

    UPDATE UsuarioSistema
    SET Estado = @Estado
    WHERE IdUsuario = @IdUsuario;

    SELECT
        'Estado de usuario actualizado correctamente' AS Mensaje,
        @IdUsuario AS IdUsuario,
        @Estado AS Estado;
END;
GO

CREATE OR ALTER PROCEDURE dbo.AsignarRolUsuarioSistema
    @IdUsuario CHAR(5),
    @Rol VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdRolSistema CHAR(5);

    IF NOT EXISTS (SELECT 1 FROM UsuarioSistema WHERE IdUsuario = @IdUsuario)
        RAISERROR('El usuario no existe.', 16, 1);

    SELECT @IdRolSistema = IdRolSistema
    FROM RolSistema
    WHERE Nombre = @Rol
      AND Estado = 'Activo';

    IF @IdRolSistema IS NULL
        RAISERROR('El rol indicado no existe o esta inactivo.', 16, 1);

    IF EXISTS (
        SELECT 1
        FROM UsuarioRolSistema
        WHERE IdUsuario = @IdUsuario
          AND IdRolSistema = @IdRolSistema
    )
        RAISERROR('El usuario ya tiene asignado ese rol.', 16, 1);

    INSERT INTO UsuarioRolSistema (
        IdUsuario,
        IdRolSistema
    )
    VALUES (
        @IdUsuario,
        @IdRolSistema
    );

    SELECT
        'Rol asignado correctamente' AS Mensaje,
        @IdUsuario AS IdUsuario,
        @Rol AS Rol;
END;
GO

CREATE OR ALTER PROCEDURE dbo.CambiarPasswordUsuarioSistema
    @IdUsuario CHAR(5),
    @PasswordHash VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM UsuarioSistema WHERE IdUsuario = @IdUsuario)
        RAISERROR('El usuario no existe.', 16, 1);

    UPDATE UsuarioSistema
    SET PasswordHash = @PasswordHash
    WHERE IdUsuario = @IdUsuario;

    SELECT
        'Password actualizado correctamente' AS Mensaje,
        @IdUsuario AS IdUsuario;
END;
GO
