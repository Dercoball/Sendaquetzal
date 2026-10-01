/* =====================================================================
   V14 - La semana extra TAMBIÉN genera multa
   Confirmado por el área: se cobran $50 por cada semana vencida sin pagar,
   incluida la semana extra.
   ===================================================================== */

UPDATE cobranza_parametro
SET descripcion = 'CONFIRMADO: $50 por CADA semana vencida sin pagar (3 fallas = $150). Incluye la semana extra, que además se cobra por el mismo monto del abono semanal.'
WHERE nombre = 'monto_multa';
