from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT, TA_RIGHT
from reportlab.lib.pagesizes import letter
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import inch
from reportlab.platypus import (
    ListFlowable,
    ListItem,
    PageBreak,
    Paragraph,
    SimpleDocTemplate,
    Spacer,
    Table,
    TableStyle,
)


ROOT = Path(__file__).resolve().parent
OUT_DIR = ROOT / "output" / "manuales"
PDF_PATH = OUT_DIR / "Manual_Sistema_Finaer_2026-09-16.pdf"


BLUE = colors.HexColor("#1F4E79")
PALE_BLUE = colors.HexColor("#F4F9FD")
GRID = colors.HexColor("#D9D9D9")
DARK = colors.HexColor("#202020")


def styles():
    base = getSampleStyleSheet()
    return {
        "title": ParagraphStyle(
            "ManualTitle",
            parent=base["Title"],
            fontName="Helvetica-Bold",
            fontSize=25,
            leading=30,
            textColor=colors.black,
            alignment=TA_LEFT,
            spaceAfter=12,
        ),
        "subtitle": ParagraphStyle(
            "ManualSubtitle",
            parent=base["Normal"],
            fontName="Helvetica",
            fontSize=12,
            leading=16,
            textColor=colors.black,
            alignment=TA_LEFT,
            spaceAfter=22,
        ),
        "h1": ParagraphStyle(
            "ManualH1",
            parent=base["Heading1"],
            fontName="Helvetica-Bold",
            fontSize=15,
            leading=18,
            textColor=colors.black,
            spaceBefore=12,
            spaceAfter=8,
        ),
        "h2": ParagraphStyle(
            "ManualH2",
            parent=base["Heading2"],
            fontName="Helvetica-Bold",
            fontSize=11.5,
            leading=14,
            textColor=colors.black,
            spaceBefore=8,
            spaceAfter=5,
        ),
        "body": ParagraphStyle(
            "ManualBody",
            parent=base["BodyText"],
            fontName="Helvetica",
            fontSize=9.2,
            leading=12.4,
            textColor=DARK,
            alignment=TA_LEFT,
            spaceAfter=6,
        ),
        "small": ParagraphStyle(
            "ManualSmall",
            parent=base["BodyText"],
            fontName="Helvetica",
            fontSize=7.8,
            leading=10.3,
            textColor=DARK,
        ),
        "cell": ParagraphStyle(
            "ManualCell",
            parent=base["BodyText"],
            fontName="Helvetica",
            fontSize=7.7,
            leading=9.7,
            textColor=DARK,
        ),
        "headcell": ParagraphStyle(
            "ManualHeadCell",
            parent=base["BodyText"],
            fontName="Helvetica-Bold",
            fontSize=7.8,
            leading=9.6,
            textColor=colors.white,
            alignment=TA_CENTER,
        ),
        "footer": ParagraphStyle(
            "ManualFooter",
            parent=base["Normal"],
            fontName="Helvetica",
            fontSize=7.5,
            textColor=colors.HexColor("#555555"),
            alignment=TA_RIGHT,
        ),
    }


S = styles()


def p(text, style="body"):
    return Paragraph(text, S[style])


def bullets(items):
    return ListFlowable(
        [ListItem(p(item, "body"), leftIndent=12) for item in items],
        bulletType="bullet",
        start="circle",
        leftIndent=16,
        bulletFontSize=6,
        spaceAfter=6,
    )


def nums(items):
    return ListFlowable(
        [ListItem(p(item, "body"), leftIndent=14) for item in items],
        bulletType="1",
        leftIndent=18,
        bulletFontSize=8,
        spaceAfter=6,
    )


def tbl(headers, rows, widths):
    data = [[p(h, "headcell") for h in headers]]
    for row in rows:
        data.append([p(str(value), "cell") for value in row])
    table = Table(data, colWidths=widths, repeatRows=1, hAlign="CENTER")
    style = [
        ("BACKGROUND", (0, 0), (-1, 0), BLUE),
        ("GRID", (0, 0), (-1, -1), 0.35, GRID),
        ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
        ("LEFTPADDING", (0, 0), (-1, -1), 5),
        ("RIGHTPADDING", (0, 0), (-1, -1), 5),
        ("TOPPADDING", (0, 0), (-1, -1), 5),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
    ]
    for r in range(2, len(data), 2):
        style.append(("BACKGROUND", (0, r), (-1, r), PALE_BLUE))
    table.setStyle(TableStyle(style))
    return table


def footer(canvas, doc):
    canvas.saveState()
    canvas.setFont("Helvetica", 7.5)
    canvas.setFillColor(colors.HexColor("#555555"))
    canvas.drawRightString(
        doc.pagesize[0] - doc.rightMargin,
        0.42 * inch,
        f"Manual del Sistema Finaer | Página {doc.page}",
    )
    canvas.restoreState()


def section(story, title):
    story.append(Paragraph(title, S["h1"]))


def subsection(story, title):
    story.append(Paragraph(title, S["h2"]))


def build():
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    doc = SimpleDocTemplate(
        str(PDF_PATH),
        pagesize=letter,
        leftMargin=0.62 * inch,
        rightMargin=0.62 * inch,
        topMargin=0.62 * inch,
        bottomMargin=0.68 * inch,
        title="Manual del Sistema Finaer",
        author="Codex",
    )
    story = []

    story.append(Paragraph("Manual del Sistema Finaer", S["title"]))
    story.append(Paragraph("Instructivo operativo para usuarios, perfiles, permisos y módulos", S["subtitle"]))
    story.append(
        tbl(
            ["Campo", "Detalle"],
            [
                ("Sistema", "Finaer / Plataforma Web de Pagos"),
                ("Fecha de elaboración", "16 de septiembre de 2026"),
                ("Audiencia", "Dirección, administración, gerencia de cobranza, recursos humanos, supervisores, promotores, gestores y soporte"),
                ("Alcance", "Acceso, usuarios, colaboradores, permisos, clientes, préstamos, pagos, cobranza, inversionistas, reportes, activos, comisiones y contenido web"),
            ],
            [1.55 * inch, 5.55 * inch],
        )
    )
    story.append(Spacer(1, 0.18 * inch))
    story.append(
        p(
            "Este manual explica dónde se realiza cada operación dentro del sistema y qué perfil debe utilizarse para ejecutarla. La guía distingue puesto, tipo de usuario y permiso, porque el menú visible y el acceso a cada página dependen de la matriz de permisos del perfil."
        )
    )
    story.append(PageBreak())

    section(story, "1 Resumen del sistema")
    story.append(
        p(
            "Finaer es un sistema web administrativo para una operación de microcréditos. Centraliza clientes y avales, solicitudes de préstamo, aprobación, pagos, cartera vencida, cobranza especializada, inversionistas, comisiones, activos, materiales, reportes y contenido público."
        )
    )
    story.append(
        tbl(
            ["Capa", "Qué contiene", "Archivos o carpetas"],
            [
                ("Presentación", "Pantallas WebForms, tablas, formularios y modales.", "pages/*.aspx, js/app/*"),
                ("Lógica", "Métodos WebMethod, validaciones y SQL de cada pantalla.", "pages/*/*.aspx.cs"),
                ("Dominio", "Clases de clientes, empleados, préstamos, pagos, cobranza e inversión.", "Clases/*.cs"),
                ("Datos", "Consultas SQL, Dapper, SqlClient y migraciones.", "flyway/migrations/*.sql"),
                ("Archivos", "Documentos, fotos, garantías y comprobantes.", "Uploads, FileUploader.ashx"),
            ],
            [1.25 * inch, 3.25 * inch, 2.6 * inch],
        )
    )

    section(story, "2 Acceso y navegación")
    story.append(nums(["Abrir Login.aspx.", "Capturar Login y Contraseña.", "Presionar Entrar.", "El sistema valida password, registra bitacora_login y redirige a la pagina_entrada del puesto."]))
    story.append(
        tbl(
            ["Dato de sesión", "Uso"],
            [
                ("usuario", "Login activo."),
                ("id_usuario", "Valida permisos y registra movimientos."),
                ("id_empleado", "Relaciona usuario con colaborador, plaza y jerarquía."),
                ("id_tipo_usuario", "Define perfil, menú y permisos."),
                ("path", "Selecciona la conexión, normalmente connbd."),
            ],
            [1.55 * inch, 5.55 * inch],
        )
    )
    story.append(p("El menú se construye con los permisos del tipo de usuario. Si el perfil no tiene permiso activo, la opción no aparece y la página rechaza el acceso al validar TienePermisoPagina."))

    section(story, "3 Roles y visibilidad")
    story.append(
        tbl(
            ["ID", "Perfil", "Uso operativo", "Regla importante"],
            [
                ("1", "Director", "Consulta directiva y operación de alto nivel.", "Si tiene plaza se acota a esa plaza; si no, ve todas."),
                ("2", "Coordinador", "Supervisión regional.", "Revisar matriz vigente."),
                ("3", "Ejecutivo", "Revisión final de créditos.", "Ve estructura bajo su jerarquía."),
                ("4", "Supervisor", "Validación de campo.", "Ve promotores bajo supervisión."),
                ("5", "Promotor", "Captura solicitudes y pagos corrientes.", "Ve su propia cartera."),
                ("6", "Superusuario", "Administración superior.", "Oculto en listados comunes."),
                ("7", "Gestor de Cobranza", "Recuperación de cartera asignada.", "Sólo ve cobranza_asignacion activa."),
                ("9", "Capturista", "Captura operativa.", "Administrativo."),
                ("10", "Gerente de Cobranza", "Control de cobranza.", "Sólo Cartera, Folios y Reporte semanal según V12."),
                ("11", "Recursos Humanos", "Colaboradores y puestos.", "Permisos 8 y 10; administrativo."),
            ],
            [0.35 * inch, 1.28 * inch, 2.45 * inch, 3.02 * inch],
        )
    )
    story.append(p("Director, Capturista, Gerente de Cobranza y Recursos Humanos son puestos administrativos. Plaza, Supervisor y Ejecutivo son opcionales para ellos en el alta de colaborador; para perfiles operativos deben completarse."))

    section(story, "4 Alta de colaboradores y usuarios")
    subsection(story, "4.1 Alta integral de colaborador")
    story.append(p("Ruta recomendada: Configuración > Colaboradores > Nuevo. Abre Config/NewEmployee.aspx y permite crear empleado, aval, documentos y usuario ligado."))
    story.append(nums(["Seleccionar Fecha de ingreso y Puesto.", "Capturar Plaza, Supervisor y Ejecutivo si el puesto es operativo.", "Capturar datos personales, domicilio, login y contraseña.", "Capturar aval y documentos cuando aplique.", "Guardar. El sistema valida CURP y Login duplicados."]))
    story.append(
        tbl(
            ["Dato", "Regla"],
            [
                ("Puesto", "Alimenta empleado.id_posicion y usuario.id_tipo_usuario."),
                ("Plaza", "Obligatoria para operativos; opcional para administrativos."),
                ("Login", "Debe ser único."),
                ("Contraseña", "Se guarda con hash MD5."),
                ("Aval", "Se registra como cliente aval cuando se captura."),
            ],
            [1.55 * inch, 5.55 * inch],
        )
    )
    subsection(story, "4.2 Alta rápida de usuario")
    story.append(p("Ruta: Configuración > Usuarios. Permite crear o editar un acceso, cambiar contraseña y vincular un empleado. Usar cuando el colaborador ya existe o sólo se ajusta el acceso."))
    story.append(nums(["Presionar Nuevo o Editar.", "Capturar Nombre, Login, Email y Teléfono.", "Seleccionar Tipo usuario y vincular Empleado.", "Guardar.", "Para contraseña, usar el botón Contraseña de la tabla."]))

    section(story, "5 Permisos y cambio de roles")
    story.append(p("Ruta de permisos: Configuración > Puestos, botón Permisos. Esta pantalla administra la matriz permisos_tipo_usuario. La pantalla Usuarios contiene código histórico para permisos directos por usuario, pero la navegación vigente usa permisos por perfil."))
    story.append(
        tbl(
            ["Tabla", "Función"],
            [
                ("permisos", "Catálogo de páginas y grupos de menú."),
                ("permisos_tipo_usuario", "Matriz principal de permisos por perfil."),
                ("usuario.id_tipo_usuario", "Perfil usado para validar acceso."),
                ("posicion.pagina_entrada", "Pantalla inicial después del login."),
                ("permisos_usuario", "Histórico de permisos directos por usuario."),
            ],
            [1.75 * inch, 5.35 * inch],
        )
    )
    story.append(
        tbl(
            ["Caso", "Instrucción"],
            [
                ("Cambiar rol", "Editar Puesto en Colaboradores o Tipo usuario en Usuarios; validar permisos en Puestos."),
                ("Gerente de Cobranza", "Usar perfil 10. Debe conservar sólo permisos 90, 92 y 93."),
                ("Recursos Humanos", "Usar perfil 11. Permisos 8 y 10."),
                ("Después del cambio", "Cerrar sesión y volver a iniciar para reconstruir menú."),
            ],
            [1.7 * inch, 5.4 * inch],
        )
    )

    section(story, "6 Mapa de módulos")
    story.append(
        tbl(
            ["Módulo", "Qué administra", "Pantallas clave"],
            [
                ("Configuración", "Catálogos, usuarios, colaboradores, puestos, plazas, mensajes y comisiones.", "pages/Config/*"),
                ("Clientes", "Clientes, avales, historial y estatus.", "Customers.aspx, CustomerHistory.aspx"),
                ("Préstamos", "Solicitud, aprobación, garantías y aumento de crédito.", "LoanRequest.aspx, LoanApprove.aspx, CreditIncreaseRequest.aspx"),
                ("Pagos", "Pagos corrientes y vencidos.", "Payments.aspx, PaymentsOverdue.aspx"),
                ("Cobranza", "Cartera, gestores, folios y reporte semanal.", "Cartera.aspx, Folios.aspx, Reporte.aspx"),
                ("Inversionistas", "Inversionistas, inversiones, retiros, utilidades y dashboard.", "Investors.aspx, Investments.aspx, Utilities.aspx"),
                ("Reportes", "Determinación de fondos y cierre semanal.", "ReportDefault.aspx"),
                ("Activos", "Activos, materiales y calendario.", "Assets.aspx, DeliveryMaterials.aspx, Calendar.aspx"),
                ("Comisiones", "Reglas y evaluación de empleados.", "ConfigRules.aspx, EmployeeEvaluation.aspx"),
                ("Web", "FAQ, acerca de, términos, privacidad y tutoriales.", "pages/Web/*"),
            ],
            [1.15 * inch, 3.1 * inch, 2.85 * inch],
        )
    )

    section(story, "7 Operación principal")
    subsection(story, "Clientes y avales")
    story.append(p("Ruta: Clientes. Permite consultar clientes, filtrar por plaza y jerarquía, abrir historial, condonar, enviar a demanda, reactivar o eliminar lógicamente registros capturados por error."))
    subsection(story, "Préstamos")
    story.append(p("LoanRequest.aspx es la bandeja de solicitudes. LoanApprove.aspx concentra datos generales, cliente, avales, validación de supervisor, dictamen de ejecutivo y garantías. CreditIncreaseRequest.aspx se usa para montos superiores al límite autorizado."))
    subsection(story, "Pagos")
    story.append(p("Payments.aspx registra pagos corrientes y abonos. PaymentsOverdue.aspx consulta cartera vencida según filtros de visibilidad por perfil."))

    section(story, "8 Cobranza especializada")
    story.append(p("El módulo Cobranza opera cuentas que caen a recuperación por semanas transcurridas desde el crédito. El parámetro semana_caida_cobranza tiene valor por defecto 17. También usa monto_multa y porcentaje_gasto_cobranza."))
    story.append(
        tbl(
            ["Pantalla", "Uso"],
            [
                ("Cartera.aspx", "Consulta cartera, calcula adeudo, multas, gasto de cobranza, bucket y total exigible; permite asignar gestor, observaciones y registrar cobro."),
                ("Folios.aspx", "Gerencia asigna blocks de folios por gestor con prefijo y rango."),
                ("Reporte.aspx", "Reporte semanal de cobros, comisiones, no tangibles y diferencia de efectivo."),
            ],
            [1.55 * inch, 5.55 * inch],
        )
    )
    story.append(
        tbl(
            ["Perfil", "Puede hacer"],
            [
                ("Gerente de Cobranza", "Ver cartera, asignar gestor, asignar folios y revisar reporte semanal."),
                ("Director", "Se considera gerencia de cobranza para asignaciones."),
                ("Gestor", "Ver sólo cuentas asignadas y registrar recuperación si tiene permiso."),
            ],
            [1.65 * inch, 5.45 * inch],
        )
    )

    section(story, "9 Inversionistas, reportes, activos y web")
    story.append(
        tbl(
            ["Área", "Operación"],
            [
                ("Inversionistas", "Alta, suspensión o baja de inversionistas; registro de inversiones, retiros, utilidades y dashboard."),
                ("Reportes", "ReportDefault.aspx consolida promotores, fallas, efectivo, gastos y concentrado de caja."),
                ("Gastos", "Bills.aspx registra concepto, fecha, monto y comprobante."),
                ("Activos", "Activos fijos, entrega de materiales y calendario."),
                ("Comisiones", "Reglas de evaluación y evaluación de colaboradores."),
                ("Web", "FAQ, acerca de, términos, aviso de privacidad y tutoriales."),
            ],
            [1.45 * inch, 5.65 * inch],
        )
    )

    section(story, "10 Respuestas rápidas")
    story.append(
        tbl(
            ["Pregunta", "Respuesta"],
            [
                ("¿Dónde doy de alta usuarios?", "Configuración > Usuarios; para expediente completo, Configuración > Colaboradores > Nuevo."),
                ("¿Qué perfil se usa para dar de alta usuario?", "Un perfil con permiso a Usuarios; normalmente administración o superusuario."),
                ("¿Dónde se dan permisos?", "Configuración > Puestos > Permisos."),
                ("¿Dónde cambio Gerente de Cobranza?", "En Colaboradores cambiando Puesto a 10, o en Usuarios corrigiendo Tipo usuario a 10."),
                ("¿Qué ve Gerente de Cobranza?", "Sólo Cobranza > Cartera, Folios y Reporte semanal."),
                ("¿Por qué un gestor no ve cuentas?", "Porque sólo ve cuentas asignadas activas en cobranza_asignacion."),
            ],
            [2.05 * inch, 5.05 * inch],
        )
    )

    section(story, "11 Prompt para actualizar este manual")
    story.append(
        p(
            "Revisa el proyecto Finaer completo y genera un manual profesional en Word y PDF. Explica módulos, rutas, roles, puestos, tipos de usuario, permisos por perfil, alta de usuarios y colaboradores, cambio de roles, visibilidad, préstamos, pagos, cobranza, inversionistas, reportes, activos, comisiones y contenido web. Contrasta el contenido contra código fuente, migraciones SQL, ASPX, code-behind C# y scripts JS. Incluye matriz de perfiles, guía para dar de alta usuarios, dónde asignar permisos, cómo configurar Gerente de Cobranza y Recursos Humanos, respuestas rápidas y revisión visual antes de entregar."
        )
    )

    doc.build(story, onFirstPage=footer, onLaterPages=footer)
    print(PDF_PATH)


if __name__ == "__main__":
    build()
