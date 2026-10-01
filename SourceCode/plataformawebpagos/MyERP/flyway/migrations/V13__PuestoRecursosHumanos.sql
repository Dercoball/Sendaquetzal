/* =====================================================================
   V13 - Puesto "Recursos Humanos" (administrativo)
   Los puestos administrativos (Director 1, Capturista 9,
   Gerente de Cobranza 10, Recursos Humanos 11) no requieren plaza,
   supervisor ni ejecutivo, y ven la información de todas las plazas.
   ===================================================================== */

IF NOT EXISTS (SELECT 1 FROM posicion WHERE id_posicion = 11)
BEGIN
    DECLARE @isIdentityPos BIT = (SELECT is_identity FROM sys.columns WHERE object_id = OBJECT_ID('posicion') AND name = 'id_posicion');
    IF @isIdentityPos = 1 SET IDENTITY_INSERT posicion ON;
    INSERT INTO posicion (id_posicion, nombre, activo, eliminado, pagina_entrada)
    VALUES (11, 'Recursos Humanos', 1, 0, 'Config/Employees.aspx');
    IF @isIdentityPos = 1 SET IDENTITY_INSERT posicion OFF;
END;

/* Recursos Humanos administra colaboradores: permiso 8 (Config/Colaboradores)
   y 10 (Config/Tipos usuario - posiciones). */
INSERT INTO permisos_tipo_usuario (id_tipo_usuario, id_permiso)
SELECT 11, p.id_permiso
FROM (VALUES (8), (10)) AS p(id_permiso)
WHERE NOT EXISTS (
    SELECT 1 FROM permisos_tipo_usuario ptu
    WHERE ptu.id_tipo_usuario = 11 AND ptu.id_permiso = p.id_permiso
);
