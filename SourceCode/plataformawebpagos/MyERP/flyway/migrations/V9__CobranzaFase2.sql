/* =====================================================================
   V9 - Módulo de Cobranza (Fases 2 y 3)
   Bloque B: captura de cobros, folios y reporte semanal de comisiones.
   Bloque C: resumen económico (dentro del reporte semanal).
   Las tablas de datos (cobranza_cobro, cobranza_folio, cobranza_canal_pago)
   ya se crearon en V8. Aquí sólo se registran los permisos/menú.
   ===================================================================== */

DECLARE @isIdentityPerm BIT = (SELECT is_identity FROM sys.columns WHERE object_id = OBJECT_ID('permisos') AND name = 'id_permiso');

/* Folios de cobranza (asignación de blocks) -> id_permiso 92 */
IF NOT EXISTS (SELECT 1 FROM permisos WHERE id_permiso = 92)
BEGIN
    IF @isIdentityPerm = 1 SET IDENTITY_INSERT permisos ON;
    INSERT INTO permisos (id_permiso, nombre, tipo_permiso, activo, nombre_interno, nombre_recurso, id_tipo_usuario)
    VALUES (92, 'Cobranza/Folios', 30, 1, 'Cobranza/Folios.aspx', 'Folios', 1);
    IF @isIdentityPerm = 1 SET IDENTITY_INSERT permisos OFF;
END;

/* Reporte semanal de cobranza + resumen económico -> id_permiso 93 */
IF NOT EXISTS (SELECT 1 FROM permisos WHERE id_permiso = 93)
BEGIN
    IF @isIdentityPerm = 1 SET IDENTITY_INSERT permisos ON;
    INSERT INTO permisos (id_permiso, nombre, tipo_permiso, activo, nombre_interno, nombre_recurso, id_tipo_usuario)
    VALUES (93, 'Cobranza/Reporte', 30, 1, 'Cobranza/Reporte.aspx', 'Reporte semanal', 1);
    IF @isIdentityPerm = 1 SET IDENTITY_INSERT permisos OFF;
END;

/* Folios: sólo gerencia (Director=1, Coordinador=2, Superadmin=6) */
INSERT INTO permisos_tipo_usuario (id_tipo_usuario, id_permiso)
SELECT v.id_tipo_usuario, 92
FROM (VALUES (1),(2),(6)) AS v(id_tipo_usuario)
WHERE NOT EXISTS (
    SELECT 1 FROM permisos_tipo_usuario ptu
    WHERE ptu.id_tipo_usuario = v.id_tipo_usuario AND ptu.id_permiso = 92
);

/* Reporte: gerencia + gestor (7) para ver sus comisiones */
INSERT INTO permisos_tipo_usuario (id_tipo_usuario, id_permiso)
SELECT v.id_tipo_usuario, 93
FROM (VALUES (1),(2),(6),(7)) AS v(id_tipo_usuario)
WHERE NOT EXISTS (
    SELECT 1 FROM permisos_tipo_usuario ptu
    WHERE ptu.id_tipo_usuario = v.id_tipo_usuario AND ptu.id_permiso = 93
);

/* Marca de canales "no tangibles" para el resumen económico (Bloque C, punto 2):
   fallos, depósitos y pagos en oficina. Se agrega columna si no existe. */
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('cobranza_canal_pago') AND name = 'no_tangible')
BEGIN
    ALTER TABLE cobranza_canal_pago ADD no_tangible INT NULL DEFAULT (0);
END;
GO

UPDATE cobranza_canal_pago SET no_tangible = 1
WHERE nombre IN ('Formato de fallo', 'Depósito a cuenta oficial', 'Pago en oficina');

UPDATE cobranza_canal_pago SET no_tangible = 0
WHERE nombre IN ('Cobro en campo', 'Formato de recuperado', 'Folio de cobranza');
