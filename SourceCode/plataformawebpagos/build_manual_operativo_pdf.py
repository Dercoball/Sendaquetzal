from pathlib import Path
from xml.sax.saxutils import escape

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

from manual_operativo_content import BRAND, DOC_DATE, DOC_SUBTITLE, DOC_TITLE, SECTIONS, TOC, VERSION


ROOT = Path(__file__).resolve().parent
OUT_DIR = ROOT / "output" / "manuales"
PDF_PATH = OUT_DIR / "Manual_Operativo_Finaer_2026-09-16.pdf"


BLUE = colors.HexColor("#1F4E79")
LIGHT_BLUE = colors.HexColor("#D9EAF7")
PALE_BLUE = colors.HexColor("#F4F9FD")
GRID = colors.HexColor("#D9D9D9")
DARK = colors.HexColor("#333333")


def e(text):
    return escape(str(text))


def get_styles():
    base = getSampleStyleSheet()
    return {
        "title": ParagraphStyle(
            "ManualTitle",
            parent=base["Title"],
            fontName="Helvetica-Bold",
            fontSize=27,
            leading=32,
            textColor=colors.black,
            alignment=TA_LEFT,
            spaceAfter=14,
        ),
        "subtitle": ParagraphStyle(
            "ManualSubtitle",
            parent=base["Normal"],
            fontName="Helvetica",
            fontSize=11.2,
            leading=15.5,
            textColor=DARK,
            alignment=TA_LEFT,
            spaceAfter=20,
        ),
        "h1": ParagraphStyle(
            "ManualH1",
            parent=base["Heading1"],
            fontName="Helvetica-Bold",
            fontSize=14.2,
            leading=17,
            textColor=BLUE,
            spaceBefore=10,
            spaceAfter=7,
            borderWidth=0,
            borderPadding=0,
        ),
        "body": ParagraphStyle(
            "ManualBody",
            parent=base["BodyText"],
            fontName="Helvetica",
            fontSize=8.45,
            leading=11.2,
            textColor=DARK,
            alignment=TA_LEFT,
            spaceAfter=6,
        ),
        "small": ParagraphStyle(
            "ManualSmall",
            parent=base["BodyText"],
            fontName="Helvetica",
            fontSize=7.7,
            leading=9.6,
            textColor=DARK,
            alignment=TA_LEFT,
        ),
        "table_header": ParagraphStyle(
            "ManualTableHeader",
            parent=base["BodyText"],
            fontName="Helvetica-Bold",
            fontSize=7.7,
            leading=9.6,
            textColor=colors.white,
            alignment=TA_LEFT,
        ),
        "toc": ParagraphStyle(
            "ManualToc",
            parent=base["BodyText"],
            fontName="Helvetica",
            fontSize=9,
            leading=12,
            textColor=DARK,
            leftIndent=8,
            spaceAfter=2,
        ),
        "coverbrand": ParagraphStyle(
            "CoverBrand",
            parent=base["Normal"],
            fontName="Helvetica-Bold",
            fontSize=21,
            leading=24,
            textColor=BLUE,
            alignment=TA_RIGHT,
            spaceAfter=72,
        ),
        "covernote": ParagraphStyle(
            "CoverNote",
            parent=base["BodyText"],
            fontName="Helvetica",
            fontSize=9.2,
            leading=12.5,
            textColor=DARK,
            alignment=TA_LEFT,
            spaceBefore=12,
        ),
    }


def para(text, style):
    return Paragraph(e(text), style)


def bullet_list(items, styles):
    return ListFlowable(
        [ListItem(para(item, styles["body"]), leftIndent=12) for item in items],
        bulletType="bullet",
        start="circle",
        leftIndent=14,
        bulletFontName="Helvetica",
        bulletFontSize=7,
    )


def step_list(items, styles):
    return ListFlowable(
        [ListItem(para(item, styles["body"]), leftIndent=13) for item in items],
        bulletType="1",
        leftIndent=16,
        bulletFontName="Helvetica-Bold",
        bulletFontSize=7.5,
    )


def manual_table(headers, rows, styles):
    data = [[Paragraph(e(h), styles["table_header"]) for h in headers]]
    for row in rows:
        data.append([Paragraph(e(value), styles["small"]) for value in row])
    table = Table(data, repeatRows=1, hAlign="LEFT")
    table.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, 0), BLUE),
                ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
                ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
                ("FONTSIZE", (0, 0), (-1, -1), 7.7),
                ("LEADING", (0, 0), (-1, -1), 9.2),
                ("GRID", (0, 0), (-1, -1), 0.25, GRID),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
                ("LEFTPADDING", (0, 0), (-1, -1), 5),
                ("RIGHTPADDING", (0, 0), (-1, -1), 5),
                ("TOPPADDING", (0, 0), (-1, -1), 4),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 4),
                ("ROWBACKGROUNDS", (0, 1), (-1, -1), [colors.white, PALE_BLUE]),
            ]
        )
    )
    return table


def header_footer(canvas, doc):
    canvas.saveState()
    width, height = letter
    canvas.setStrokeColor(LIGHT_BLUE)
    canvas.setLineWidth(0.7)
    canvas.line(doc.leftMargin, height - 0.48 * inch, width - doc.rightMargin, height - 0.48 * inch)
    canvas.setFillColor(DARK)
    canvas.setFont("Helvetica", 7.5)
    canvas.drawRightString(width - doc.rightMargin, height - 0.35 * inch, f"{BRAND} | Manual Operativo")
    canvas.setStrokeColor(LIGHT_BLUE)
    canvas.line(doc.leftMargin, 0.45 * inch, width - doc.rightMargin, 0.45 * inch)
    canvas.drawCentredString(width / 2, 0.28 * inch, f"Pagina {doc.page}")
    canvas.restoreState()


def cover(styles):
    info = Table(
        [
            [Paragraph("Empresa", styles["table_header"]), Paragraph(e(BRAND), styles["small"])],
            [Paragraph("Documento", styles["table_header"]), Paragraph("Manual operativo", styles["small"])],
            [Paragraph("Version", styles["table_header"]), Paragraph(e(VERSION), styles["small"])],
            [Paragraph("Fecha", styles["table_header"]), Paragraph(e(DOC_DATE), styles["small"])],
        ],
        colWidths=[1.35 * inch, 4.55 * inch],
        hAlign="LEFT",
    )
    info.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (0, -1), BLUE),
                ("TEXTCOLOR", (0, 0), (0, -1), colors.white),
                ("BACKGROUND", (1, 0), (1, -1), PALE_BLUE),
                ("GRID", (0, 0), (-1, -1), 0.3, GRID),
                ("LEFTPADDING", (0, 0), (-1, -1), 7),
                ("RIGHTPADDING", (0, 0), (-1, -1), 7),
                ("TOPPADDING", (0, 0), (-1, -1), 5),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
            ]
        )
    )
    return [
        Paragraph(e(BRAND), styles["coverbrand"]),
        Paragraph(e(DOC_TITLE), styles["title"]),
        Paragraph(e(DOC_SUBTITLE), styles["subtitle"]),
        info,
        Paragraph(
            "Documento de uso interno para personal operativo y administrativo. "
            "El contenido debe actualizarse cuando cambien perfiles, permisos o reglas de operacion.",
            styles["covernote"],
        ),
        PageBreak(),
    ]


def build_story():
    styles = get_styles()
    story = cover(styles)
    story.append(Paragraph("Contenido", styles["h1"]))
    for item in TOC:
        story.append(Paragraph(e(item), styles["toc"]))
    story.append(PageBreak())

    for index, section in enumerate(SECTIONS):
        if index in (3, 6, 9, 12):
            story.append(PageBreak())
        story.append(Paragraph(e(section["title"]), styles["h1"]))
        for block in section["blocks"]:
            if block["type"] == "paragraph":
                story.append(para(block["text"], styles["body"]))
            elif block["type"] == "bullets":
                story.append(bullet_list(block["items"], styles))
                story.append(Spacer(1, 4))
            elif block["type"] == "steps":
                story.append(step_list(block["items"], styles))
                story.append(Spacer(1, 4))
            elif block["type"] == "table":
                story.append(manual_table(block["headers"], block["rows"], styles))
                story.append(Spacer(1, 8))
    return story


def build():
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    doc = SimpleDocTemplate(
        str(PDF_PATH),
        pagesize=letter,
        rightMargin=0.52 * inch,
        leftMargin=0.52 * inch,
        topMargin=0.62 * inch,
        bottomMargin=0.62 * inch,
        title=f"{DOC_TITLE} - {BRAND}",
        author=BRAND,
    )
    doc.build(build_story(), onFirstPage=header_footer, onLaterPages=header_footer)
    print(PDF_PATH)


if __name__ == "__main__":
    build()
