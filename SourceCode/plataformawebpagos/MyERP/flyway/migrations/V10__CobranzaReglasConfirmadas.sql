/* =====================================================================
   V10 - Reglas de negocio de Cobranza CONFIRMADAS por el área (jul-2026)
   Los valores no cambian; se actualizan las descripciones para dejar
   constancia de que ya fueron validadas y cómo aplican.
   ===================================================================== */

UPDATE cobranza_parametro
SET valor = 17,
    descripcion = 'CONFIRMADO: los créditos son a 13 semanas y caen a gestoría/cobranza en la semana 17 (contada desde el inicio del crédito). Aplica igual para todos los productos.'
WHERE nombre = 'semana_caida_cobranza';

UPDATE cobranza_parametro
SET valor = 50,
    descripcion = 'CONFIRMADO: $50 por CADA semana fallada (3 fallas = $150). No se aplica multa sobre el renglón de semana extra, que ya es un cargo en sí (una semana adicional por el mismo monto del abono semanal).'
WHERE nombre = 'monto_multa';

UPDATE cobranza_parametro
SET valor = 0.10,
    descripcion = 'CONFIRMADO: 10% de gasto de cobranza sobre (adeudo + multas).'
WHERE nombre = 'porcentaje_gasto_cobranza';

/* Buckets de comisión confirmados: 17-19 = 15%, 20-23 = 20%, 24+ = 25% */
UPDATE cobranza_regla_comision SET porcentaje = 0.15 WHERE bucket = 1;
UPDATE cobranza_regla_comision SET porcentaje = 0.20 WHERE bucket = 2;
UPDATE cobranza_regla_comision SET porcentaje = 0.25 WHERE bucket = 3;
