from datetime import date


BRAND = "Finaer"
DOC_TITLE = "Manual Operativo del Sistema"
DOC_SUBTITLE = (
    "Guia de operacion para administracion de usuarios, perfiles, creditos, "
    "cobranza, inversionistas, reportes y configuracion diaria."
)
VERSION = "Operacion v1.0"
DOC_DATE = date(2026, 9, 16).strftime("%d/%m/%Y")


def p(text):
    return {"type": "paragraph", "text": text}


def bullets(items):
    return {"type": "bullets", "items": items}


def steps(items):
    return {"type": "steps", "items": items}


def matrix(headers, rows, widths=None):
    return {"type": "table", "headers": headers, "rows": rows, "widths": widths}


TOC = [
    "Proposito y criterios de operacion",
    "Mapa operativo del sistema",
    "Perfiles, puestos y alcance",
    "Alta y mantenimiento de colaboradores",
    "Usuarios, contrasenas y permisos",
    "Configuracion base de la operacion",
    "Flujo de credito",
    "Pagos y cartera vencida",
    "Cobranza especializada",
    "Reportes y conciliacion",
    "Inversionistas",
    "Gastos, activos, materiales y contenido web",
    "Checklist operativo",
    "Respuestas rapidas y prompt sugerido",
]


SECTIONS = [
    {
        "title": "1. Proposito y criterios de operacion",
        "blocks": [
            p(
                "Este manual esta dirigido al equipo operativo y administrativo de Finaer. "
                "Describe como usar el sistema por modulos visibles en el menu, que perfil "
                "debe intervenir en cada actividad y que controles deben revisarse antes de "
                "dar por terminado un proceso."
            ),
            p(
                "La regla principal es simple: cada usuario solo debe ver y ejecutar los "
                "modulos que corresponden a su puesto, plaza y responsabilidad. Cuando una "
                "persona no ve una opcion, primero se revisa su colaborador, despues su tipo "
                "de usuario y finalmente los permisos del perfil."
            ),
            bullets(
                [
                    "Use Colaboradores para altas completas de personal, expediente, puesto y cuenta de acceso.",
                    "Use Usuarios para ajustar accesos existentes, cambiar contrasenas o corregir el tipo de usuario.",
                    "Use Tipos usuario para definir que modulos puede ver cada perfil.",
                    "Use Plazas, calendarios, mensajes, categorias y reglas antes de iniciar la operacion diaria.",
                    "Valide los cambios cerrando y entrando nuevamente con el usuario afectado.",
                ]
            ),
            p(
                "Este documento no esta orientado a desarrollo. La informacion se presenta "
                "como instructivo de trabajo: ruta del menu, responsable sugerido, pasos y "
                "resultado esperado."
            ),
        ],
    },
    {
        "title": "2. Mapa operativo del sistema",
        "blocks": [
            p(
                "El sistema se organiza alrededor de los procesos diarios de la financiera. "
                "Los nombres siguientes corresponden a las rutas funcionales que debe buscar "
                "el usuario en el menu."
            ),
            matrix(
                ["Proceso", "Ruta operativa", "Resultado esperado"],
                [
                    (
                        "Equipo y accesos",
                        "Configuracion > Colaboradores, Usuarios, Tipos usuario",
                        "Personal registrado, perfil correcto y permisos activos.",
                    ),
                    (
                        "Base de operacion",
                        "Configuracion > Plazas, calendarios, dias de paro, mensajes, categorias",
                        "Catalogos listos para capturar creditos, pagos y reportes.",
                    ),
                    (
                        "Credito",
                        "Prestamos > Nuevo prestamo y Prestamos > Solicitudes",
                        "Solicitud capturada, revisada y aprobada por el responsable aplicable.",
                    ),
                    (
                        "Pagos",
                        "Pagos corrientes y pagos vencidos",
                        "Cobros registrados con evidencia, notas y comprobante cuando aplique.",
                    ),
                    (
                        "Cobranza especializada",
                        "Cobranza > Cartera, Folios, Reporte semanal",
                        "Cuentas asignadas a gestores, folios controlados y corte semanal conciliado.",
                    ),
                    (
                        "Inversionistas",
                        "Inversionistas > Inversionistas, Inversiones, Dashboard",
                        "Inversionista dado de alta, inversion registrada y retiro controlado.",
                    ),
                    (
                        "Control gerencial",
                        "Reporte",
                        "Indicadores por fecha, plaza y estructura comercial para supervision.",
                    ),
                    (
                        "Otros controles",
                        "Otros > Gastos, Activos, Entrega de materiales",
                        "Gastos, activos y materiales registrados para seguimiento administrativo.",
                    ),
                    (
                        "Contenido publico",
                        "Web > FAQ, tutoriales, terminos, privacidad",
                        "Informacion publica actualizada y activa cuando corresponda.",
                    ),
                ],
            ),
            p(
                "El menu visible depende del perfil. Si una ruta no aparece, no se debe "
                "buscar otra forma de operar; se revisa el permiso del perfil o el alcance "
                "del colaborador."
            ),
        ],
    },
    {
        "title": "3. Perfiles, puestos y alcance",
        "blocks": [
            p(
                "El puesto del colaborador y el tipo de usuario de la cuenta determinan el "
                "alcance operativo. En altas completas, el puesto seleccionado para el "
                "colaborador debe coincidir con el perfil que se espera para su cuenta."
            ),
            matrix(
                ["Perfil", "Uso operativo", "Alcance esperado"],
                [
                    (
                        "Director",
                        "Direccion y supervision general de la operacion.",
                        "Puede tener alcance general o quedar limitado por plaza si se le asigna una.",
                    ),
                    (
                        "Superusuario",
                        "Administracion amplia del sistema.",
                        "Debe reservarse para personal autorizado por direccion.",
                    ),
                    (
                        "Recursos Humanos",
                        "Alta, edicion y seguimiento de colaboradores y perfiles.",
                        "Ideal para altas operativas de personal y mantenimiento de puestos.",
                    ),
                    (
                        "Capturista",
                        "Captura administrativa y apoyo operativo.",
                        "Perfil administrativo; normalmente no requiere plaza, supervisor ni ejecutivo en el alta.",
                    ),
                    (
                        "Coordinador / Ejecutivo",
                        "Seguimiento de estructura comercial y revision de solicitudes.",
                        "Debe operar dentro de su estructura asignada y revisar cartera de su equipo.",
                    ),
                    (
                        "Supervisor",
                        "Control del promotor y revision inicial del credito.",
                        "Opera con promotores asignados y con la plaza correspondiente.",
                    ),
                    (
                        "Promotor",
                        "Captacion, captura de prospectos y seguimiento de clientes.",
                        "Opera la informacion de sus clientes y solicitudes dentro de su estructura.",
                    ),
                    (
                        "Gerente de Cobranza",
                        "Control de cartera especializada, gestores, folios y corte semanal.",
                        "Debe ver Cobranza > Cartera, Folios y Reporte semanal.",
                    ),
                    (
                        "Gestor de Cobranza",
                        "Recuperacion de cartera asignada.",
                        "Solo debe operar cuentas asignadas en cartera de cobranza.",
                    ),
                    (
                        "Gerencia / Administracion",
                        "Uso administrativo cuando exista como perfil interno.",
                        "Se controla desde Colaboradores, Usuarios y Tipos usuario segun el permiso otorgado.",
                    ),
                ],
            ),
            p(
                "Director, Capturista, Gerente de Cobranza y Recursos Humanos se tratan "
                "como perfiles administrativos. Para perfiles comerciales, capture plaza, "
                "supervisor y ejecutivo cuando aplique; esa informacion alimenta filtros, "
                "reportes y visibilidad."
            ),
        ],
    },
    {
        "title": "4. Alta y mantenimiento de colaboradores",
        "blocks": [
            p(
                "Ruta recomendada: Configuracion > Colaboradores. Esta es la ruta principal "
                "para dar de alta a una persona que formara parte de la operacion, porque "
                "permite registrar el expediente, el puesto, la relacion jerarquica y la "
                "cuenta de acceso."
            ),
            steps(
                [
                    "Entrar a Configuracion > Colaboradores y seleccionar Nuevo colaborador.",
                    "Capturar fecha de ingreso y puesto. El puesto debe reflejar la funcion real de la persona.",
                    "Si el perfil es comercial, seleccionar plaza, supervisor y ejecutivo segun la estructura.",
                    "Si el perfil es administrativo, validar si requiere plaza. Director, Capturista, Gerente de Cobranza y Recursos Humanos pueden operar sin jerarquia comercial.",
                    "Capturar datos personales del colaborador y la informacion del aval cuando aplique.",
                    "Capturar usuario y contrasena inicial si la persona entrara al sistema.",
                    "Guardar y confirmar que el colaborador aparezca en el listado con el estatus correcto.",
                    "Entrar con la cuenta o pedir al usuario que entre para confirmar que solo vea los modulos autorizados.",
                ]
            ),
            matrix(
                ["Cambio requerido", "Ruta", "Control"],
                [
                    (
                        "Cambio de puesto o rol operativo",
                        "Configuracion > Colaboradores",
                        "Editar el colaborador, cambiar Puesto y guardar. Despues validar el tipo de usuario.",
                    ),
                    (
                        "Baja de colaborador",
                        "Configuracion > Colaboradores",
                        "Registrar fecha de baja o desactivar segun el proceso interno. Revisar que no conserve accesos innecesarios.",
                    ),
                    (
                        "Cambio de plaza o jerarquia",
                        "Configuracion > Colaboradores",
                        "Ajustar plaza, supervisor o ejecutivo y revisar reportes relacionados.",
                    ),
                    (
                        "Cambio de contrasena desde expediente",
                        "Configuracion > Colaboradores",
                        "Asignar nueva contrasena y solicitar validacion de acceso.",
                    ),
                ],
            ),
            p(
                "Cuando se trate de un usuario ya existente y solo se requiera corregir el "
                "acceso, puede usarse Configuracion > Usuarios. Para una alta nueva y "
                "ordenada, la ruta recomendada sigue siendo Colaboradores."
            ),
        ],
    },
    {
        "title": "5. Usuarios, contrasenas y permisos",
        "blocks": [
            p(
                "La administracion operativa de accesos se divide en tres rutas: "
                "Colaboradores para el expediente, Usuarios para la cuenta de entrada y "
                "Tipos usuario para los permisos del perfil."
            ),
            matrix(
                ["Necesidad", "Ruta correcta", "Quien debe operarlo"],
                [
                    (
                        "Alta completa de usuario y persona",
                        "Configuracion > Colaboradores > Nuevo colaborador",
                        "Recursos Humanos, Direccion o personal autorizado.",
                    ),
                    (
                        "Alta o ajuste rapido de una cuenta existente",
                        "Configuracion > Usuarios",
                        "Administracion o responsable con permiso de Usuarios.",
                    ),
                    (
                        "Cambio de contrasena",
                        "Configuracion > Usuarios o Colaboradores",
                        "Responsable autorizado; el usuario debe confirmar ingreso.",
                    ),
                    (
                        "Cambio de rol o tipo de usuario",
                        "Configuracion > Colaboradores y, si aplica, Configuracion > Usuarios",
                        "Responsable de administracion de personal y accesos.",
                    ),
                    (
                        "Agregar o quitar modulos a un perfil",
                        "Configuracion > Tipos usuario > Permisos",
                        "Direccion, Superusuario o responsable de seguridad operativa.",
                    ),
                ],
            ),
            p(
                "Para permisos por perfil, entre a Configuracion > Tipos usuario, ubique "
                "el perfil y seleccione Permisos. Pase a la lista del perfil los modulos "
                "que debe ver, retire los que no correspondan y guarde. El cambio aplica "
                "al perfil, no solo a una persona."
            ),
            bullets(
                [
                    "Si el usuario no ve un modulo, revise que su cuenta este ligada al colaborador correcto.",
                    "Si el colaborador tiene el puesto correcto pero no ve opciones, revise el tipo de usuario de la cuenta.",
                    "Si el tipo de usuario es correcto pero faltan opciones, revise los permisos del perfil.",
                    "Si se cambia un permiso, pida al usuario cerrar y entrar de nuevo para validar.",
                    "No otorgue permisos amplios por urgencia sin dejar responsable y motivo operativo.",
                ]
            ),
        ],
    },
    {
        "title": "6. Configuracion base de la operacion",
        "blocks": [
            p(
                "Antes de operar creditos, pagos o reportes, el responsable administrativo "
                "debe mantener actualizados los catalogos de trabajo. Esto evita errores "
                "de captura y diferencias de conciliacion."
            ),
            matrix(
                ["Modulo", "Para que se usa", "Revision sugerida"],
                [
                    (
                        "Plazas",
                        "Definir sucursales o zonas donde opera la financiera.",
                        "Antes de asignar colaboradores, clientes o reportes por plaza.",
                    ),
                    (
                        "Calendarios, periodos y dias de paro",
                        "Controlar fechas operativas y dias sin operacion.",
                        "Al inicio de cada periodo y antes de cierres semanales.",
                    ),
                    (
                        "Mensajes",
                        "Mantener plantillas de comunicacion para clientes.",
                        "Cuando cambien textos autorizados o campanas de cobranza.",
                    ),
                    (
                        "Comisiones y reglas",
                        "Controlar calculos operativos para incentivos o cobranza.",
                        "Solo por personal autorizado; documentar cada cambio.",
                    ),
                    (
                        "Categorias y tipos de cliente",
                        "Estandarizar capturas, clasificaciones y filtros.",
                        "Antes de nuevas campanas o cambios comerciales.",
                    ),
                    (
                        "Tipos usuario",
                        "Administrar perfiles y permisos por rol.",
                        "Cuando entra un nuevo rol o cambia una responsabilidad.",
                    ),
                ],
            ),
            p(
                "Todo cambio de configuracion debe validarse con un usuario de prueba o "
                "con el responsable del area, revisando que el menu y el resultado operativo "
                "sean los esperados."
            ),
        ],
    },
    {
        "title": "7. Flujo de credito",
        "blocks": [
            p(
                "El flujo de credito inicia con la captura de la solicitud y termina con "
                "la autorizacion o rechazo. Cada solicitud debe conservar cliente, avales, "
                "monto solicitado, promotor y evidencias necesarias."
            ),
            steps(
                [
                    "Entrar a Prestamos > Nuevo prestamo.",
                    "Capturar datos de la solicitud: fecha, tipo de cliente, monto solicitado y promotor.",
                    "Registrar informacion del cliente, aval principal y segundo aval cuando aplique.",
                    "Adjuntar o validar documentos operativos requeridos por politica interna.",
                    "Guardar la solicitud y revisar que quede con el siguiente responsable de revision.",
                    "El responsable revisa historial, alertas, capacidad y evidencias.",
                    "Aprobar, rechazar o devolver segun la revision operativa.",
                    "Una solicitud aprobada pasa a seguimiento de pagos y reportes.",
                ]
            ),
            matrix(
                ["Momento", "Responsable habitual", "Control esperado"],
                [
                    (
                        "Captura",
                        "Promotor o Capturista",
                        "Datos completos, cliente y avales sin omisiones.",
                    ),
                    (
                        "Revision inicial",
                        "Supervisor o Capturista autorizado",
                        "Validacion de informacion y coherencia de solicitud.",
                    ),
                    (
                        "Revision ejecutiva",
                        "Ejecutivo, Coordinador o Direccion segun flujo",
                        "Autorizacion final o rechazo documentado.",
                    ),
                    (
                        "Aumento de credito",
                        "Responsable autorizado",
                        "Comparar limite actual, credito requerido y comportamiento del cliente.",
                    ),
                ],
            ),
            p(
                "Si el sistema advierte que el cliente tiene un credito en proceso, fallas "
                "o antecedentes relevantes, no se debe forzar la captura. El responsable "
                "debe revisar el caso y dejar evidencia de la decision."
            ),
        ],
    },
    {
        "title": "8. Pagos y cartera vencida",
        "blocks": [
            p(
                "El seguimiento de pagos se separa entre cartera corriente y cartera vencida. "
                "La captura correcta de pagos es esencial para reportes, recibos, cortes y "
                "decision de cobranza."
            ),
            matrix(
                ["Situacion", "Ruta operativa", "Resultado"],
                [
                    (
                        "Pago esperado de credito activo",
                        "Pagos corrientes",
                        "Registrar monto recibido, validar estatus y generar comprobante si aplica.",
                    ),
                    (
                        "Pago parcial",
                        "Pagos corrientes o vencidos",
                        "Registrar abono y nota para seguimiento.",
                    ),
                    (
                        "Cliente con atraso",
                        "Pagos vencidos",
                        "Registrar pago, falla, observaciones de cliente o aval y acciones de seguimiento.",
                    ),
                    (
                        "Semana extra o diferencia",
                        "Pagos vencidos",
                        "Revisar saldo, monto pendiente y notas antes de cerrar la captura.",
                    ),
                ],
            ),
            steps(
                [
                    "Ubicar el credito por cliente, promotor, plaza o filtro disponible.",
                    "Verificar datos del cliente y saldo antes de capturar.",
                    "Capturar monto recibido y observacion clara.",
                    "Cuando aplique, registrar nota del cliente o aval.",
                    "Guardar el movimiento.",
                    "Generar o revisar comprobante.",
                    "Confirmar que el estatus quede como pendiente, falla, abonado o pagado segun el caso.",
                ]
            ),
            p(
                "Si el pago fue registrado con error, el usuario operativo no debe intentar "
                "compensarlo con capturas improvisadas. Debe escalarlo al responsable de "
                "administracion o cobranza para que se documente la correccion."
            ),
        ],
    },
    {
        "title": "9. Cobranza especializada",
        "blocks": [
            p(
                "Cobranza especializada atiende cuentas que ya salieron del seguimiento "
                "ordinario. En el flujo actual, una cuenta con saldo pendiente cae a cobranza "
                "a partir de la semana 17 desde el inicio del credito."
            ),
            bullets(
                [
                    "La multa operativa es de $50 por cada semana vencida, incluyendo semana extra cuando aplique.",
                    "El gasto de cobranza se calcula como 10% sobre adeudo mas multas.",
                    "Los grupos de comision se ordenan por antiguedad: semanas 17 a 19, 20 a 23 y 24 en adelante.",
                    "El Gerente de Cobranza asigna cartera y folios; el Gestor registra recuperaciones de sus cuentas asignadas.",
                ]
            ),
            matrix(
                ["Actividad", "Ruta", "Responsable"],
                [
                    (
                        "Revisar cuentas en cobranza",
                        "Cobranza > Cartera",
                        "Gerente de Cobranza o Direccion.",
                    ),
                    (
                        "Asignar gestor",
                        "Cobranza > Cartera",
                        "Gerente de Cobranza o responsable autorizado.",
                    ),
                    (
                        "Asignar block de folios",
                        "Cobranza > Folios",
                        "Gerente de Cobranza.",
                    ),
                    (
                        "Registrar recuperacion",
                        "Cobranza > Cartera",
                        "Gestor asignado o responsable autorizado.",
                    ),
                    (
                        "Conciliar semana",
                        "Cobranza > Reporte semanal",
                        "Gerente de Cobranza y Administracion.",
                    ),
                ],
            ),
            steps(
                [
                    "Entrar a Cobranza > Cartera y filtrar por plaza cuando aplique.",
                    "Abrir el detalle de la cuenta para revisar cliente, avales, adeudo, multas, gasto de cobranza y total actualizado.",
                    "Asignar gestor si la cuenta no tiene responsable.",
                    "Registrar observaciones relevantes para seguimiento.",
                    "Asignar o revisar folios disponibles para el gestor en Cobranza > Folios.",
                    "Cuando exista recuperacion, registrar monto, canal de pago, folio, fecha y observacion.",
                    "Revisar Cobranza > Reporte semanal para validar cobrado, no tangible, comisiones y diferencias.",
                ]
            ),
            p(
                "Un gestor solo debe ver y operar cartera asignada. Si una cuenta no aparece "
                "para el gestor, primero revise que la asignacion este hecha y activa."
            ),
        ],
    },
    {
        "title": "10. Reportes y conciliacion",
        "blocks": [
            p(
                "El modulo Reporte concentra la supervision diaria y semanal de la operacion. "
                "Debe usarse para revisar venta, efectivo esperado, recuperado, fallas y "
                "comportamiento por estructura."
            ),
            matrix(
                ["Revision", "Filtros habituales", "Uso operativo"],
                [
                    (
                        "Corte diario",
                        "Fecha, plaza, ejecutivo, supervisor o promotor",
                        "Comparar operacion esperada contra cobros registrados.",
                    ),
                    (
                        "Seguimiento semanal",
                        "Rango de fechas y estructura comercial",
                        "Detectar fallas, abonos, semanas extra y recuperaciones.",
                    ),
                    (
                        "Revision por plaza",
                        "Plaza y periodo",
                        "Comparar desempeno entre sucursales o zonas.",
                    ),
                    (
                        "Revision por promotor",
                        "Promotor y fecha",
                        "Validar venta, cobranza y clientes con incidencia.",
                    ),
                    (
                        "Cobranza especializada",
                        "Gestor y semana",
                        "Conciliar recuperacion, comisiones, folios y diferencias.",
                    ),
                ],
            ),
            bullets(
                [
                    "Revise el reporte antes del cierre, no despues de detectar diferencias en caja.",
                    "Las cifras deben cruzarse con recibos, notas y folios cuando aplique.",
                    "Si un usuario no puede filtrar una plaza o estructura, revise su alcance en Colaboradores.",
                    "Los reportes deben revisarse con la misma fecha de corte usada por la operacion.",
                ]
            ),
        ],
    },
    {
        "title": "11. Inversionistas",
        "blocks": [
            p(
                "El modulo de inversionistas permite administrar personas inversionistas, "
                "registrar inversiones, calcular utilidad, controlar fecha de retiro y "
                "conservar comprobantes."
            ),
            steps(
                [
                    "Dar de alta o validar al inversionista en el modulo Inversionistas.",
                    "Entrar a Inversiones y seleccionar Nueva inversion.",
                    "Seleccionar inversionista, fecha, monto, porcentaje de utilidad y plazo.",
                    "Revisar utilidad calculada, total esperado y fecha de retiro.",
                    "Guardar la inversion y adjuntar comprobante cuando aplique.",
                    "En caso de retiro, registrar el movimiento y conservar evidencia de pago.",
                    "Usar el dashboard para revisar resumen, disponibilidad y seguimiento.",
                ]
            ),
            matrix(
                ["Control", "Que revisar", "Responsable sugerido"],
                [
                    (
                        "Alta de inversionista",
                        "Datos completos y contacto correcto.",
                        "Administracion.",
                    ),
                    (
                        "Registro de inversion",
                        "Monto, utilidad, plazo y comprobante.",
                        "Administracion o Finanzas.",
                    ),
                    (
                        "Retiro",
                        "Fecha, total a retirar y evidencia de pago.",
                        "Finanzas o Direccion.",
                    ),
                ],
            ),
        ],
    },
    {
        "title": "12. Gastos, activos, materiales y contenido web",
        "blocks": [
            p(
                "Estos modulos sostienen controles administrativos y de comunicacion. "
                "Aunque no forman parte directa del credito, afectan conciliacion, inventario "
                "y experiencia del cliente."
            ),
            matrix(
                ["Area", "Ruta", "Uso operativo"],
                [
                    (
                        "Gastos",
                        "Otros > Gastos",
                        "Registrar concepto, fecha y monto para control administrativo.",
                    ),
                    (
                        "Activos",
                        "Otros > Activos",
                        "Mantener catalogo de activos bajo control de la empresa.",
                    ),
                    (
                        "Entrega de materiales",
                        "Otros > Entrega de materiales",
                        "Registrar material, cantidad, costo, fecha y colaborador receptor.",
                    ),
                    (
                        "FAQ",
                        "Web > FAQ",
                        "Actualizar preguntas visibles para usuarios o clientes.",
                    ),
                    (
                        "Tutoriales",
                        "Web > Tutoriales",
                        "Publicar o desactivar videos y guias de apoyo.",
                    ),
                    (
                        "Terminos y privacidad",
                        "Web > Terminos / Aviso de privacidad",
                        "Mantener textos vigentes y autorizados.",
                    ),
                ],
            ),
            p(
                "Los cambios de contenido publico deben validarse con el area responsable "
                "antes de activarse. Los gastos, activos y materiales deben capturarse con "
                "soporte para facilitar auditoria interna."
            ),
        ],
    },
    {
        "title": "13. Checklist operativo",
        "blocks": [
            matrix(
                ["Frecuencia", "Revision", "Responsable"],
                [
                    (
                        "Diaria",
                        "Usuarios activos, solicitudes pendientes, pagos del dia y cartera con incidencia.",
                        "Operacion y Administracion.",
                    ),
                    (
                        "Diaria",
                        "Cuentas vencidas, pagos parciales, fallas y observaciones de clientes.",
                        "Supervisores y responsables de cobranza.",
                    ),
                    (
                        "Semanal",
                        "Reporte de cobranza, folios utilizados, no tangible, comisiones y diferencias.",
                        "Gerente de Cobranza.",
                    ),
                    (
                        "Semanal",
                        "Reporte general por plaza, promotor y estructura comercial.",
                        "Direccion, Coordinacion o Ejecutivo.",
                    ),
                    (
                        "Mensual",
                        "Perfiles, permisos, colaboradores activos, bajas y contrasenas pendientes.",
                        "Recursos Humanos y Administracion.",
                    ),
                    (
                        "Mensual",
                        "Plazas, calendarios, reglas, mensajes, categorias y contenido publico.",
                        "Responsable administrativo.",
                    ),
                ],
            ),
            bullets(
                [
                    "Ningun usuario operativo debe compartir credenciales.",
                    "Cada rol debe conservar solo los permisos necesarios para su responsabilidad.",
                    "Los cambios de perfil deben documentarse con responsable y fecha.",
                    "Los reportes se revisan con la misma fecha de corte que usa caja o cobranza.",
                    "Los folios de cobranza se entregan, usan y concilian por gestor.",
                ]
            ),
        ],
    },
    {
        "title": "14. Respuestas rapidas y prompt sugerido",
        "blocks": [
            matrix(
                ["Pregunta operativa", "Respuesta"],
                [
                    (
                        "En que modulo doy de alta usuarios?",
                        "Para alta completa: Configuracion > Colaboradores > Nuevo colaborador. Para ajustar una cuenta existente: Configuracion > Usuarios.",
                    ),
                    (
                        "Que perfil se utiliza para dar de alta usuario?",
                        "Recursos Humanos es el perfil recomendado para altas operativas. Tambien puede hacerlo Direccion, Superusuario o un perfil con permiso a Colaboradores y Usuarios.",
                    ),
                    (
                        "Donde se dan los permisos?",
                        "Configuracion > Tipos usuario > Permisos. Ahi se define que modulos ve cada perfil.",
                    ),
                    (
                        "Donde cambio un rol administrativo?",
                        "En Configuracion > Colaboradores se edita el Puesto. Si solo se corrige la cuenta, en Configuracion > Usuarios se ajusta Tipo usuario.",
                    ),
                    (
                        "Donde cambio a Gerente de Cobranza?",
                        "En Configuracion > Colaboradores, editar colaborador y seleccionar Gerente de Cobranza como puesto. Despues confirmar que vea Cobranza > Cartera, Folios y Reporte semanal.",
                    ),
                    (
                        "Por que un gestor no ve una cuenta?",
                        "Porque el gestor solo ve cartera asignada. Revise Cobranza > Cartera y confirme que la cuenta tenga gestor asignado.",
                    ),
                    (
                        "Cuando cae una cuenta a cobranza especializada?",
                        "A partir de la semana 17 desde el inicio del credito, siempre que conserve saldo pendiente.",
                    ),
                    (
                        "Que reviso si un usuario no ve una opcion?",
                        "Colaborador correcto, tipo de usuario correcto y permisos del perfil en Tipos usuario.",
                    ),
                ],
            ),
            p("Prompt sugerido para solicitar una nueva actualizacion del manual:"),
            p(
                "Genera un manual operativo profesional de Finaer en Word y PDF. Revisalo "
                "desde el flujo real de negocio y explica modulos, responsables, perfiles, "
                "permisos, alta de colaboradores y usuarios, cambio de roles, creditos, pagos, "
                "cobranza, inversionistas, reportes, gastos, activos, materiales y contenido "
                "web. Evita nombres internos, referencias de desarrollo y detalles que no "
                "utiliza operaciones. Incluye respuestas rapidas para administracion de usuarios "
                "y permisos, y valida visualmente el PDF antes de entregarlo."
            ),
        ],
    },
]

