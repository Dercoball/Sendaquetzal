# Domain Map

## Problema que resuelve
ERP web para negocio de microprestamos con capital de inversionistas: gestiona clientes y avales, solicitudes y aprobacion de credito, cobranza semanal y mora, reglas de comision para fuerza comercial, control basico de activos/materiales y contenidos del sitio publico.

## Casos de uso principales
- Autenticacion y permisos por tipo de usuario; registro de `bitacora_login`.
- Administrar catalogos base (usuarios, plazas, puestos, calendarios/days off, mensajes, categorias, comisiones).
- Registrar clientes y avales con documentos/fotos; mantener historico.
- Capturar solicitudes de prestamo, revisar, aprobar/rechazar, asignar ejecutivo/supervisor, calcular cronograma y comisiones.
- Registrar pagos, abonos, semanas extra y gestion de mora; generar recibos PDF/Docx y evidencias.
- Gestionar solicitudes de aumento de credito.
- Administrar inversionistas, altas de inversiones, retiros y utilidades; cargar comprobantes y estados.
- Enviar notificaciones SMS/WhatsApp o correo para eventos configurados.
- Generar reportes consolidados de cartera/comisiones.
- Gestionar assets y entregas de materiales a plazas.
- Publicar contenido web (FAQs, terminos, avisos, tutoriales).

## Entidades centrales
- Usuario / Posicion / PermisoUsuario: control de acceso, pagina inicial.
- Cliente + Direccion + Documento + Garantia + Aval (en Cliente): solicitantes y respaldos; estados y colores.
- Prestamo + Pago + RelPrestamoAprobacion + SolicitudAumentoCredito + StatusPrestamo: ciclo de credito y cobranza.
- Empleado + Plaza + Puesto + Periodo + Calendario + DiaDeParo: organizacion, asignaciones y calendarios laborales.
- Comision + EvaluacionModulo + ValorReglaEvaluacionModulo + Modulo: reglas y calculo de comisiones/promotores.
- Inversionista + Inversion + InversionMovimiento + InversionRetiro + StatusInversion: fondeo y rendimiento.
- MaterialEntrega + Categoria/CategoriaMateriales + Unidad: inventario/logistica.
- Configuracion + Mensaje + Plantilla + PreguntaFrecuente + Tutorial: parametrizacion y contenido estatico.
