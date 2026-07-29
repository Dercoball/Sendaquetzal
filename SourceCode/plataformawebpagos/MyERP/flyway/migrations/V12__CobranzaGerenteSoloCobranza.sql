/* =====================================================================
   V12 - El Gerente de Cobranza (10) SÓLO ve el módulo de Cobranza
   Corrige V11, que además le había dado Clientes / Corriente / Vencida /
   Reportes. Según el área: "el Gerente de Cobranza solamente va a haber
   lo de Cobranza y ya".
   ===================================================================== */

DELETE FROM permisos_tipo_usuario
WHERE id_tipo_usuario = 10
  AND id_permiso NOT IN (90, 92, 93);

/* Asegura que conserve los tres del módulo de Cobranza */
INSERT INTO permisos_tipo_usuario (id_tipo_usuario, id_permiso)
SELECT 10, p.id_permiso
FROM (VALUES (90), (92), (93)) AS p(id_permiso)
WHERE NOT EXISTS (
    SELECT 1 FROM permisos_tipo_usuario ptu
    WHERE ptu.id_tipo_usuario = 10 AND ptu.id_permiso = p.id_permiso
);
