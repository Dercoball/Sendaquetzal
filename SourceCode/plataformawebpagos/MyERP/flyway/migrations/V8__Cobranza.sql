/* =====================================================================
   V8 - Módulo de Cobranza (Fase 1)
   Basado en "DIAGRAMA COBRANZA.xlsx".
   Los valores de negocio marcados como (POR CONFIRMAR) quedan
   parametrizados en cobranza_parametro para poder ajustarlos sin
   recompilar cuando el área de cobranza confirme las reglas.
   ===================================================================== */

/* ------------------------------------------------------------------
   1. Parámetros de negocio (editables)
   ------------------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'cobranza_parametro')
BEGIN
    CREATE TABLE cobranza_parametro (
        nombre       VARCHAR(60)  NOT NULL PRIMARY KEY,
        valor        FLOAT        NULL,
        descripcion  VARCHAR(300) NULL
    );

    INSERT INTO cobranza_parametro (nombre, valor, descripcion) VALUES
        ('semana_caida_cobranza', 17, 'Semana (desde el inicio del crédito) a partir de la cual una cuenta no finiquitada cae a cobranza. (POR CONFIRMAR: absoluto vs plazo+N)'),
        ('monto_multa',           50, 'Multa en pesos por cada pago semanal fallado (completo o parcial). Se exenta el último pago del plazo. (POR CONFIRMAR)'),
        ('porcentaje_gasto_cobranza', 0.10, 'Porcentaje de gasto de cobranza aplicado sobre (adeudo + multas). (POR CONFIRMAR)');
END;

/* ------------------------------------------------------------------
   2. Reglas de comisión por BUCKET (semanas de vencimiento)
   ------------------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'cobranza_regla_comision')
BEGIN
    CREATE TABLE cobranza_regla_comision (
        id_regla     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        bucket       INT   NOT NULL,
        semana_desde INT   NOT NULL,
        semana_hasta INT   NOT NULL,   -- 9999 = en adelante
        porcentaje   FLOAT NOT NULL,   -- fracción: 0.15 = 15%
        activo       INT   NULL DEFAULT (1),
        eliminado    INT   NULL DEFAULT (0)
    );

    INSERT INTO cobranza_regla_comision (bucket, semana_desde, semana_hasta, porcentaje, activo, eliminado) VALUES
        (1, 17, 19,   0.15, 1, 0),
        (2, 20, 23,   0.20, 1, 0),
        (3, 24, 9999, 0.25, 1, 0);
END;

/* ------------------------------------------------------------------
   3. Catálogo de canales de pago de cobranza
   ------------------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'cobranza_canal_pago')
BEGIN
    CREATE TABLE cobranza_canal_pago (
        id_canal_pago INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        nombre        VARCHAR(100) NOT NULL,
        activo        INT NULL DEFAULT (1)
    );

    INSERT INTO cobranza_canal_pago (nombre, activo) VALUES
        ('Cobro en campo', 1),
        ('Formato de fallo', 1),
        ('Formato de recuperado', 1),
        ('Depósito a cuenta oficial', 1),
        ('Folio de cobranza', 1),
        ('Pago en oficina', 1);
END;

/* ------------------------------------------------------------------
   4. Asignación de cartera (gestor <-> crédito)  [solo gerencia asigna]
   ------------------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'cobranza_asignacion')
BEGIN
    CREATE TABLE cobranza_asignacion (
        id_cobranza_asignacion INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        id_prestamo       INT NOT NULL,
        id_gestor         INT NOT NULL,          -- empleado (posición Gestor = 7)
        id_usuario_asigna INT NULL,              -- quién asignó (gerente de cobranza)
        fecha_asignacion  DATETIME NULL DEFAULT (GETDATE()),
        observaciones     VARCHAR(1500) NULL,    -- editable solo por gerente/asistente
        activo            INT NULL DEFAULT (1),
        eliminado         INT NULL DEFAULT (0)
    );
    CREATE INDEX IX_cobranza_asignacion_prestamo ON cobranza_asignacion (id_prestamo);
    CREATE INDEX IX_cobranza_asignacion_gestor   ON cobranza_asignacion (id_gestor);
END;

/* ------------------------------------------------------------------
   5. Folios de cobranza (blocks asignables)  [Fase 2]
   ------------------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'cobranza_folio')
BEGIN
    CREATE TABLE cobranza_folio (
        id_folio          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        folio             VARCHAR(50) NOT NULL,
        id_gestor         INT NULL,
        id_usuario_asigna INT NULL,
        id_cobranza_cobro INT NULL,             -- se llena cuando el folio se usa
        estatus           INT NULL DEFAULT (1), -- 1 disponible, 2 usado, 3 cancelado
        fecha_asignacion  DATETIME NULL,
        fecha_uso         DATETIME NULL,
        eliminado         INT NULL DEFAULT (0)
    );
    CREATE INDEX IX_cobranza_folio_gestor ON cobranza_folio (id_gestor);
END;

/* ------------------------------------------------------------------
   6. Cobros de cobranza (recuperación en campo)  [Fase 2]
   ------------------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'cobranza_cobro')
BEGIN
    CREATE TABLE cobranza_cobro (
        id_cobranza_cobro   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        id_prestamo         INT NOT NULL,
        id_gestor           INT NULL,
        id_usuario          INT NULL,           -- quién capturó
        fecha_cobro         DATE NULL,          -- fecha en que se cobró
        monto_recuperado    FLOAT NULL,
        id_canal_pago       INT NULL,
        id_folio            INT NULL,
        semanas_vencimiento INT NULL,           -- snapshot al momento del cobro
        bucket              INT NULL,
        porcentaje_comision FLOAT NULL,
        comision            FLOAT NULL,
        observaciones       VARCHAR(1000) NULL,
        fecha_registro      DATETIME NULL DEFAULT (GETDATE()),
        eliminado           INT NULL DEFAULT (0)
    );
    CREATE INDEX IX_cobranza_cobro_prestamo ON cobranza_cobro (id_prestamo);
    CREATE INDEX IX_cobranza_cobro_gestor   ON cobranza_cobro (id_gestor);
END;

/* ------------------------------------------------------------------
   7. Tipo de documento "INE" (liga de imagen del INE del cliente)
   ------------------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM tipo_documento WHERE nombre = 'INE')
    INSERT INTO tipo_documento (nombre) VALUES ('INE');

/* ------------------------------------------------------------------
   8. Permisos / menú del módulo de Cobranza
      tipo_permiso = 30  ->  grupo de menú "Cobranza"
      id_permiso 90..95 reservados para el módulo
   ------------------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM permisos WHERE id_permiso = 90)
BEGIN
    DECLARE @isIdentity90 BIT = (SELECT is_identity FROM sys.columns WHERE object_id = OBJECT_ID('permisos') AND name = 'id_permiso');
    IF @isIdentity90 = 1 SET IDENTITY_INSERT permisos ON;
    INSERT INTO permisos (id_permiso, nombre, tipo_permiso, activo, nombre_interno, nombre_recurso, id_tipo_usuario)
    VALUES (90, 'Cobranza/Cartera', 30, 1, 'Cobranza/Cartera.aspx', 'Cartera', 1);
    IF @isIdentity90 = 1 SET IDENTITY_INSERT permisos OFF;
END;

/* Roles que ven la Cartera de cobranza:
   1 Director, 2 Coordinador, 6 Superadmin, 7 Gestor
   (POR CONFIRMAR: rol dedicado "Gerente de cobranza") */
INSERT INTO permisos_tipo_usuario (id_tipo_usuario, id_permiso)
SELECT v.id_tipo_usuario, 90
FROM (VALUES (1),(2),(6),(7)) AS v(id_tipo_usuario)
WHERE NOT EXISTS (
    SELECT 1 FROM permisos_tipo_usuario ptu
    WHERE ptu.id_tipo_usuario = v.id_tipo_usuario AND ptu.id_permiso = 90
);
