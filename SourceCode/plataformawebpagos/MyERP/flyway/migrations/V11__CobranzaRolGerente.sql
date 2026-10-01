/* =====================================================================
   V11 - Rol "Gerente de Cobranza" y reasignación de permisos
   - Nuevo puesto/rol: posicion 10 = Gerente de Cobranza
     (usuario.id_tipo_usuario se alimenta de empleado.id_posicion)
   - El menú Cobranza queda SÓLO para Gerente de Cobranza (10) y Director (1)
   - Al Gestor (7) se le quitan los permisos de colocación de crédito
   ===================================================================== */

/* ------------------------------------------------------------------
   1. Nuevo puesto: Gerente de Cobranza
   ------------------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM posicion WHERE id_posicion = 10)
BEGIN
    DECLARE @isIdentityPos BIT = (SELECT is_identity FROM sys.columns WHERE object_id = OBJECT_ID('posicion') AND name = 'id_posicion');
    IF @isIdentityPos = 1 SET IDENTITY_INSERT posicion ON;
    INSERT INTO posicion (id_posicion, nombre, activo, eliminado, pagina_entrada)
    VALUES (10, 'Gerente de Cobranza', 1, 0, 'Cobranza/Cartera.aspx');
    IF @isIdentityPos = 1 SET IDENTITY_INSERT posicion OFF;
END;

/* ------------------------------------------------------------------
   2. El menú Cobranza sólo para Gerente de Cobranza (10) y Director (1)
   ------------------------------------------------------------------ */
DELETE FROM permisos_tipo_usuario
WHERE id_permiso IN (90, 92, 93)
  AND id_tipo_usuario NOT IN (1, 10);

INSERT INTO permisos_tipo_usuario (id_tipo_usuario, id_permiso)
SELECT v.id_tipo_usuario, p.id_permiso
FROM (VALUES (1), (10)) AS v(id_tipo_usuario)
CROSS JOIN (VALUES (90), (92), (93)) AS p(id_permiso)
WHERE NOT EXISTS (
    SELECT 1 FROM permisos_tipo_usuario ptu
    WHERE ptu.id_tipo_usuario = v.id_tipo_usuario AND ptu.id_permiso = p.id_permiso
);

/* ------------------------------------------------------------------
   3. Menús operativos para el Gerente de Cobranza
      21 Clientes | 15 Corriente | 61 Vencida | 19 Reportes
   ------------------------------------------------------------------ */
INSERT INTO permisos_tipo_usuario (id_tipo_usuario, id_permiso)
SELECT 10, p.id_permiso
FROM (VALUES (21), (15), (61), (19)) AS p(id_permiso)
WHERE NOT EXISTS (
    SELECT 1 FROM permisos_tipo_usuario ptu
    WHERE ptu.id_tipo_usuario = 10 AND ptu.id_permiso = p.id_permiso
);

/* ------------------------------------------------------------------
   4. El Gestor (7) ya no coloca crédito: se le quitan
      12 Nuevo préstamo y 13 Préstamos.
      Conserva 21 Clientes, 15 Corriente y 61 Vencida, que ahora se
      limitan por código a su cartera asignada.
   ------------------------------------------------------------------ */
DELETE FROM permisos_tipo_usuario
WHERE id_tipo_usuario = 7 AND id_permiso IN (12, 13);

INSERT INTO permisos_tipo_usuario (id_tipo_usuario, id_permiso)
SELECT 7, p.id_permiso
FROM (VALUES (21), (15), (61)) AS p(id_permiso)
WHERE NOT EXISTS (
    SELECT 1 FROM permisos_tipo_usuario ptu
    WHERE ptu.id_tipo_usuario = 7 AND ptu.id_permiso = p.id_permiso
);

/* ------------------------------------------------------------------
   5. Página de entrada del Gestor: la lista de cuentas vencidas
   ------------------------------------------------------------------ */
UPDATE posicion SET pagina_entrada = 'Loans/PaymentsOverdue.aspx'
WHERE id_posicion = 7 AND ISNULL(pagina_entrada, '') = 'Loans/LoanRequest.aspx';
