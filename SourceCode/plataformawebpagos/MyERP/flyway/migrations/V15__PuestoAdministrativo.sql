/* =====================================================================
   V15 - Puesto "Administrativo" (usuario administrativo)
   Administración de personal y accesos: Colaboradores (8), Usuarios y
   Puestos / tipos de usuario (10). Como los demás puestos administrativos
   (Director 1, Capturista 9, Gerente de Cobranza 10, Recursos Humanos 11)
   no requiere plaza, supervisor ni ejecutivo, y ve todas las plazas.
   ===================================================================== */

IF EXISTS (SELECT 1 FROM posicion WHERE id_posicion = 12 AND nombre <> 'Administrativo')
BEGIN
    RAISERROR('V15: la posición 12 ya existe con otro nombre; revisar antes de continuar.', 16, 1);
    RETURN;
END;

IF NOT EXISTS (SELECT 1 FROM posicion WHERE id_posicion = 12)
BEGIN
    DECLARE @isIdentityPos BIT = (SELECT is_identity FROM sys.columns WHERE object_id = OBJECT_ID('posicion') AND name = 'id_posicion');
    IF @isIdentityPos = 1 SET IDENTITY_INSERT posicion ON;
    INSERT INTO posicion (id_posicion, nombre, activo, eliminado, pagina_entrada)
    VALUES (12, 'Administrativo', 1, 0, 'Config/Employees.aspx');
    IF @isIdentityPos = 1 SET IDENTITY_INSERT posicion OFF;
END;

/* Permisos: 8 Colaboradores, 10 Puestos / tipos de usuario y la pantalla
   de Usuarios (se busca por su ruta porque su id no está fijo en el código). */
INSERT INTO permisos_tipo_usuario (id_tipo_usuario, id_permiso)
SELECT 12, p.id_permiso
FROM permisos p
WHERE (p.id_permiso IN (8, 10) OR p.nombre_interno LIKE '%Usuarios.aspx')
  AND NOT EXISTS (
    SELECT 1 FROM permisos_tipo_usuario ptu
    WHERE ptu.id_tipo_usuario = 12 AND ptu.id_permiso = p.id_permiso
);
