
using iTextSharp.text;
using iTextSharp.text.pdf;
using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Plataforma
{
	/// <summary>
	/// Summary description for Download
	/// </summary>
	public class Download : IHttpHandler
	{

		const string TIPO_DOCUMENTO = "1";
		const string TIPO_EVIDENCIA = "2";

		const string TIPO_DESCARGA_DOCUMENTOS = "1";
		const string TIPO_DESCARGA_CSV = "2";
		const string TIPO_DESCARGA_ORDENTRABAJO = "3";

		public void ProcessRequest(HttpContext context)
		{

			try
			{

				string path = context.Request.QueryString["path"];
				string id_documento = context.Request.QueryString["id_documento"];

				string tipoDescarga = context.Request.QueryString["tipo_descarga"];

				if (tipoDescarga == TIPO_DESCARGA_CSV)
				{
					//GENERAR CSV DE NOMINA
					var recordsCvsList = GetRecordsCSV(path);
					var csv = new StringBuilder();

					foreach (var row in recordsCvsList)
					{
						var first = row.Clave;
						var second = row.Horas;
						var third = row.Dias;
						var newLine = string.Format("{0}\t{1}\t{2}", first, second, third);
						csv.AppendLine(newLine);


					}


					DateTime ahora = DateTime.Now;
					string nombreArchivo = context.Server.MapPath("Descargas") + "/CSV_" + ahora.ToString("yyy_MM_dd_hhmmss") + ".csv";

					File.WriteAllText(nombreArchivo, csv.ToString());

					context.Response.ContentType = "application/octet-stream";
					context.Response.AddHeader("content-disposition", "attachment;filename=" + Path.GetFileName(nombreArchivo));
					context.Response.TransmitFile(nombreArchivo);


				}
				

				//else if (tipoDescarga == TIPO_DESCARGA_ORDENTRABAJO)
				//{
					
				//	string idRequisicion = context.Request.QueryString["id_requisicion"];

				//	Requisicion item = Plataforma.pages.MantenimientoListadoFallas.GetItem(path, idRequisicion);
				//	Equipo itemEquipo = Plataforma.pages.PanelEquipos.GetItem(path, item.IdEquipo.ToString());
				//	var itemsDiagnosticos = Plataforma.pages.MantenimientoListadoFallas.GetItemsUsuarios(path, "1", item.IdRequisicion.ToString());
				//	var itemsRefacciones = Plataforma.pages.MantenimientoPanelSolicitudRefacciones.GetItems(path, "1", item.IdRequisicion.ToString());

				//	Utils.Log("descripcion " + item.Descripcion);


				//	DateTime ahora = DateTime.Now;

				//	// Creamos el documento con el tamaño de página tradicional
				//	Document doc = new Document(PageSize.LETTER);
				//	// Indicamos donde vamos a guardar el documento

				//	string dirFullPath = context.Server.MapPath("/pages/Descargas");
				//	string nuevoArchivo = dirFullPath + "/OrdenTrabajo_" + idRequisicion + ".pdf";

				//	PdfWriter writer = PdfWriter.GetInstance(doc,
				//								new FileStream(nuevoArchivo, FileMode.Create));

				//	// Le colocamos el título y el autor
				//	// **Nota: Esto no será visible en el documento
				//	doc.AddTitle("Orden de trabajo");
				//	doc.AddCreator("Sistema_Plataforma_Urbanissa");

				//	// Abrimos el archivo
				//	doc.Open();


				//	// Creamos el tipo de Font que vamos utilizar
				//	iTextSharp.text.Font _standardFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
				//	iTextSharp.text.Font negrita = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

				//	iTextSharp.text.Font negritaTitulos = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

				//	//*************TABLA DEL ENCABEZADO
				//	PdfPTable tblPruebaHeader = new PdfPTable(4);
				//	tblPruebaHeader.WidthPercentage = 100;

				//	string imageURL = context.Server.MapPath("~/img/logo_small.jpg");
				//	iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
				//	jpg.ScaleToFit(140f, 120f);
				//	jpg.SpacingBefore = 10f;
				//	jpg.SpacingAfter = 1f;
				//	jpg.Alignment = Element.ALIGN_LEFT;

				//	PdfPCell cellimagen = new PdfPCell(jpg);
				//	cellimagen.BorderWidth = 0;
				//	cellimagen.Rowspan = 10;

				//	tblPruebaHeader.AddCell(cellimagen);

				//	PdfPCell t0 = new PdfPCell(new Phrase("", negrita));
				//	t0.BorderWidth = 0;
				//	t0.Colspan = 2;

				//	PdfPCell t1 = new PdfPCell(new Phrase("Orden de trabajo", negritaTitulos));
				//	t1.BorderWidth = 0;
				//	t1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
				//	t1.Colspan = 2;

				//	PdfPCell t2 = new PdfPCell(new Phrase("", negrita));
				//	t2.BorderWidth = 0;
				//	t2.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
				//	t2.Colspan = 2;

				//	PdfPCell t3 = new PdfPCell(new Phrase("", negrita));
				//	t3.BorderWidth = 0;
				//	t3.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
				//	t3.Colspan = 2;

				//	PdfPCell t4 = new PdfPCell(new Phrase("", negrita));
				//	t4.BorderWidth = 0;
				//	t4.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
				//	t4.Colspan = 2;


				//	PdfPCell labelFechaIngreso = new PdfPCell(new Phrase("Fecha", negrita));
				//	labelFechaIngreso.BorderWidth = 0;
				//	labelFechaIngreso.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;

				//	PdfPCell valorFechaIngreso = new PdfPCell(new Phrase(item.FechaCreacion.ToString("dd/MM/yyyy"), _standardFont));
				//	valorFechaIngreso.BorderWidth = 0;
				//	valorFechaIngreso.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
				//	valorFechaIngreso.BorderWidthBottom = 0.75f;


				//	PdfPCell labelNumOrden = new PdfPCell(new Phrase("Número de orden", negrita));
				//	labelNumOrden.BorderWidth = 0;
				//	labelNumOrden.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;

				//	PdfPCell vaorNumeroOrden = new PdfPCell(new Phrase(item.IdRequisicion.ToString(), _standardFont));
				//	vaorNumeroOrden.BorderWidth = 0;
				//	vaorNumeroOrden.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
				//	vaorNumeroOrden.BorderWidthBottom = 0.75f;


				//	PdfPCell  labelGeneradoPor = new PdfPCell(new Phrase("Generado por", negrita));
				//	labelGeneradoPor.BorderWidth = 0;
				//	labelGeneradoPor.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;

				//	PdfPCell valorGeneradoPor = new PdfPCell(new Phrase(item.NombreUsuario.ToString(), _standardFont));
				//	valorGeneradoPor.BorderWidth = 0;
				//	valorGeneradoPor.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
				//	valorGeneradoPor.BorderWidthBottom = 0.75f;


				//	PdfPCell celdaVacia = new PdfPCell(new Phrase(" ", _standardFont));
				//	celdaVacia.BorderWidth = 0;

				//	PdfPCell celdaVaciaConLineaAbajo = new PdfPCell(new Phrase(" ", _standardFont));
				//	celdaVaciaConLineaAbajo.BorderWidth = 0;
				//	celdaVaciaConLineaAbajo.BorderWidthBottom = 0.75f;

				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(t0);
				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(t1);
				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(t2);
				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(t3);
				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(t4);


				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(celdaVacia);

				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(labelFechaIngreso);//fecha ingreso
				//	tblPruebaHeader.AddCell(valorFechaIngreso);

				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(labelNumOrden);//num orden
				//	tblPruebaHeader.AddCell(vaorNumeroOrden);


				//	tblPruebaHeader.AddCell(celdaVacia);
				//	tblPruebaHeader.AddCell(labelGeneradoPor);//Usuario que genera la orden
				//	tblPruebaHeader.AddCell(valorGeneradoPor);


				//	doc.Add(tblPruebaHeader);

				//	iTextSharp.text.Font fuenteEnormaParaEspacio = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 20, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
				//	Paragraph parrafoParaEspacio = new Paragraph(new Phrase(" ", fuenteEnormaParaEspacio));
				//	doc.Add(parrafoParaEspacio);//insertar espacio vacío

				//	//****TABLA DATOS DEL CLIENTE

				//	//float[] columnWidths = { 1, 4, 1, 4 };
				//	PdfPTable tablaDatosCliente = new PdfPTable(4);//4 columnas
				//	tablaDatosCliente.WidthPercentage = 100;

				   


				//	////****TABLA DATOS DEL VEHICULO

				//	PdfPTable tablaDatosVehiculo = new PdfPTable(5);
				//	tablaDatosVehiculo.WidthPercentage = 100;

				//	PdfPCell columna1_Vehiculo = new PdfPCell(new Phrase("Datos Equipo", negritaTitulos));
				//	columna1_Vehiculo.BorderWidth = 0;
				//	columna1_Vehiculo.BorderWidthBottom = 0.75f;

				//	PdfPCell columna2_Vehiculo = new PdfPCell(new Phrase("", _standardFont));
				//	columna2_Vehiculo.BorderWidth = 0;
				//	columna2_Vehiculo.BorderWidthBottom = 0.75f;
				//	columna2_Vehiculo.Colspan = 4;


				//	//Añadimos las celdas a la tabla
				//	tablaDatosVehiculo.AddCell(columna1_Vehiculo);
				//	tablaDatosVehiculo.AddCell(columna2_Vehiculo);

				//	PdfPCell tituloNombreEquipo = new PdfPCell(new Phrase("Equipo", negrita));
				//	tituloNombreEquipo.BorderWidth = 0;


				//	PdfPCell celdaTituloNoEconomico = new PdfPCell(new Phrase("No. económico ", negrita)); 
				//	celdaTituloNoEconomico.BorderWidth = 0;


				//	PdfPCell celdaTituloMarca = new PdfPCell(new Phrase("Marca", negrita));
				//	celdaTituloMarca.BorderWidth = 0;


				//	PdfPCell celdaTituloModelo = new PdfPCell(new Phrase("Modelo", negrita));
				//	celdaTituloModelo.BorderWidth = 0;

				//	PdfPCell celdaTituloHorometro= new PdfPCell(new Phrase("Horometro/Odómetro", negrita));
				//	celdaTituloHorometro.BorderWidth = 0;


				//	PdfPCell valorNombreEquipo = new PdfPCell(new Phrase(item.NombreEquipo, _standardFont));
				//	valorNombreEquipo.BorderWidth = 0;

				//	PdfPCell valorNoEconomico = new PdfPCell(new Phrase(itemEquipo.NumeroEconomico, _standardFont));
				//	valorNoEconomico.BorderWidth = 0;

				//	PdfPCell valorMarca = new PdfPCell(new Phrase(itemEquipo.NombreMarca, _standardFont));
				//	valorMarca.BorderWidth = 0;

				//	PdfPCell valorModelo = new PdfPCell(new Phrase(itemEquipo.NombreModelo, _standardFont));
				//	valorModelo.BorderWidth = 0;


				//	PdfPCell valorHorometro = new PdfPCell(new Phrase(item.Orometro, _standardFont));
				//	valorHorometro.BorderWidth = 0;

				//	//fila
				//	tablaDatosVehiculo.AddCell(tituloNombreEquipo);
				//	tablaDatosVehiculo.AddCell(celdaTituloNoEconomico);
				//	tablaDatosVehiculo.AddCell(celdaTituloMarca);
				//	tablaDatosVehiculo.AddCell(celdaTituloModelo);
				//	tablaDatosVehiculo.AddCell(celdaTituloHorometro);


				//	//fila
				//	tablaDatosVehiculo.AddCell(valorNombreEquipo);
				//	tablaDatosVehiculo.AddCell(valorNoEconomico);
				//	tablaDatosVehiculo.AddCell(valorMarca);
				//	tablaDatosVehiculo.AddCell(valorModelo);
				//	tablaDatosVehiculo.AddCell(valorHorometro);



				//	tablaDatosVehiculo.AddCell(celdaVaciaConLineaAbajo);
				//	tablaDatosVehiculo.AddCell(celdaVaciaConLineaAbajo);
				//	tablaDatosVehiculo.AddCell(celdaVaciaConLineaAbajo);

				//	doc.Add(tablaDatosVehiculo);
				//	//*****fin de tabla datos del vehiculo


				//	doc.Add(parrafoParaEspacio);//insertar espacio vacío


				//	PdfPTable tabla_Servicio = new PdfPTable(2);
				//	tabla_Servicio.WidthPercentage = 100;

				//	PdfPCell columna1_Servicio = new PdfPCell(new Phrase("Datos de la falla", negritaTitulos));
				//	columna1_Servicio.BorderWidth = 0;
				//	columna1_Servicio.BorderWidthBottom = 0.75f;
				//	columna1_Servicio.Colspan = 2;


				//	//Añadimos las celdas a la tabla
				//	tabla_Servicio.AddCell(columna1_Servicio);

				//	PdfPCell tituloServicio1 = new PdfPCell(new Phrase("Descripción", negrita));
				//	tituloServicio1.BorderWidth = 0;
				//	tituloServicio1.Colspan = 2;


				//	PdfPCell contenidoServicio1 = new PdfPCell(new Phrase(item.Descripcion, _standardFont));
				//	contenidoServicio1.BorderWidth = 0;
				//	contenidoServicio1.Colspan = 2;
					



				//	PdfPCell tituloNivel = new PdfPCell(new Phrase("Nivel de prioridad", negrita));
				//	tituloNivel.BorderWidth = 0;


				//	PdfPCell tituloDetiene = new PdfPCell(new Phrase("Detiene la operación", negrita));
				//	tituloDetiene.BorderWidth = 0;

				//	PdfPCell valorDetiene = new PdfPCell(new Phrase(item.DetieneOperacion == 1 ? "Sí": "No", _standardFont));
				//	valorDetiene.BorderWidth = 0;

				//	string nombreNivelPrioridad = "";
				//	switch (item.IdNivelPrioridad)
				//	{
				//		case 1:
				//			{
				//				nombreNivelPrioridad = "Rojo";
				//				break;
				//			}
				//		case 2:
				//			{
				//				nombreNivelPrioridad = "Amarrillo";
				//				break;
				//			}
				//		case 3:
				//			{
				//				nombreNivelPrioridad = "Verde";
				//				break;
				//			}

				//	}

				//	PdfPCell valorNivel = new PdfPCell(new Phrase(nombreNivelPrioridad, _standardFont));
				//	valorNivel.BorderWidth = 0;
				//	if (item.IdNivelPrioridad == 1) valorNivel.BackgroundColor = BaseColor.RED;
				//	if (item.IdNivelPrioridad == 2) valorNivel.BackgroundColor = BaseColor.YELLOW;
				//	if (item.IdNivelPrioridad == 3) valorNivel.BackgroundColor = BaseColor.GREEN;

				//	//fila
				//	tabla_Servicio.AddCell(tituloServicio1);
				//	tabla_Servicio.AddCell(contenidoServicio1);


				//	tabla_Servicio.AddCell(celdaVaciaConLineaAbajo);
				//	tabla_Servicio.AddCell(celdaVaciaConLineaAbajo);

				//	//fila
				//	tabla_Servicio.AddCell(tituloDetiene);
				//	tabla_Servicio.AddCell(tituloNivel);

				//	tabla_Servicio.AddCell(valorDetiene);
				//	tabla_Servicio.AddCell(valorNivel);


				//	doc.Add(tabla_Servicio);

				//	doc.Add(parrafoParaEspacio);//insertar espacio vacío


				//	PdfPTable tablaDiagnostico = new PdfPTable(2);
				//	tablaDiagnostico.WidthPercentage = 100;

				//	PdfPCell columnaTituloDiagnostico = new PdfPCell(new Phrase("Diagnóstico", negritaTitulos));
				//	columnaTituloDiagnostico.BorderWidth = 0;
				//	columnaTituloDiagnostico.BorderWidthBottom = 0.75f;
				//	columnaTituloDiagnostico.Colspan = 2;


				//	tablaDiagnostico.AddCell(columnaTituloDiagnostico);



				//	foreach(var itemDiag in itemsDiagnosticos)
				//	{

				//		tablaDiagnostico.AddCell(new PdfPCell(new Phrase(itemDiag.NombreUsuario, negrita)));
				//		tablaDiagnostico.AddCell(new PdfPCell(new Phrase(itemDiag.FechaFormateadaMx, _standardFont)));

				//		PdfPCell celdaDiagnostico = new PdfPCell(new Phrase(itemDiag.Diagnostico, _standardFont));
				//		celdaDiagnostico.Colspan = 2;

				//		tablaDiagnostico.AddCell(celdaDiagnostico);

				//	}
					
				//	doc.Add(tablaDiagnostico);

				//	doc.Add(parrafoParaEspacio);//insertar espacio vacío
				//	doc.Add(parrafoParaEspacio);//insertar espacio vacío

				//	//	Tabla refacciones
				//	PdfPTable tablaRefacciones = new PdfPTable(6);
    //                tablaRefacciones.WidthPercentage = 100;

				//	PdfPCell columnaTituloRefacciones = new PdfPCell(new Phrase("Refacciones", negritaTitulos));
				//	columnaTituloRefacciones.BorderWidth = 0;
				//	columnaTituloRefacciones.BorderWidthBottom = 0.75f;
				//	columnaTituloRefacciones.Colspan = 6;


				//	PdfPCell columnaFechaSolicitud = new PdfPCell(new Phrase("Fecha Solicitud", negrita));
    //                columnaFechaSolicitud.BorderWidth = 0;
    //                columnaFechaSolicitud.BorderWidthBottom = 0.75f;


				//	PdfPCell columnaMarca = new PdfPCell(new Phrase("Marca/Proveedor", negrita));
				//	columnaMarca.BorderWidth = 0;
				//	columnaMarca.BorderWidthBottom = 0.75f;

				//	PdfPCell columnaCantidad = new PdfPCell(new Phrase("Cantidad", negrita));
				//	columnaCantidad.BorderWidth = 0;
				//	columnaCantidad.BorderWidthBottom = 0.75f;


				//	PdfPCell columnaNumParte = new PdfPCell(new Phrase("Número de parte", negrita));
				//	columnaNumParte.BorderWidth = 0;
				//	columnaNumParte.BorderWidthBottom = 0.75f;

    //                PdfPCell colDescripcion = new PdfPCell(new Phrase("Descripción", negrita));
				//	colDescripcion.BorderWidth = 0;
				//	colDescripcion.BorderWidthBottom = 0.75f;


				//	PdfPCell colRequisiciokEK= new PdfPCell(new Phrase("Requisición EK", negrita));
				//	colRequisiciokEK.BorderWidth = 0;
				//	colRequisiciokEK.BorderWidthBottom = 0.75f;


				//	//	row
				//	tablaRefacciones.AddCell(columnaTituloRefacciones);

				//	//row
				//	tablaRefacciones.AddCell(columnaFechaSolicitud);
				//	tablaRefacciones.AddCell(columnaMarca);
				//	tablaRefacciones.AddCell(columnaCantidad);
				//	tablaRefacciones.AddCell(columnaNumParte);
				//	tablaRefacciones.AddCell(colDescripcion);
				//	tablaRefacciones.AddCell(colRequisiciokEK);



				//	foreach (var itemDiag in itemsRefacciones)
    //                {

				//		tablaRefacciones.AddCell(new PdfPCell(new Phrase(itemDiag.FechaFormateadaMx, _standardFont)));
				//		tablaRefacciones.AddCell(new PdfPCell(new Phrase(itemDiag.NombreMarca, _standardFont)));
				//		tablaRefacciones.AddCell(new PdfPCell(new Phrase(itemDiag.Cantidad.ToString(), _standardFont)));
				//		tablaRefacciones.AddCell(new PdfPCell(new Phrase(itemDiag.NumeroParte, _standardFont)));
				//		tablaRefacciones.AddCell(new PdfPCell(new Phrase(itemDiag.Descripcion, _standardFont)));
				//		tablaRefacciones.AddCell(new PdfPCell(new Phrase(itemDiag.ValorCaptura, _standardFont)));


				//	}


    //                //
    //                doc.Add(tablaRefacciones);



				//	doc.Add(parrafoParaEspacio);//insertar espacio vacío

				   



				//	doc.Close();

				//	writer.Close();


				//	context.Response.ContentType = "application/octet-stream";
				//	context.Response.AddHeader("content-disposition", "attachment;filename=" + Path.GetFileName(nuevoArchivo));
				//	context.Response.TransmitFile(nuevoArchivo);
				//	context.Response.End();


				//}
				else
				{

					try
					{

						string[] datosDocumento = GetDocumento(path, id_documento);

						if (datosDocumento != null)
						{
							string docB64 = datosDocumento[0];
							string nombreDocumento = datosDocumento[1];
							byte[] decoded = Convert.FromBase64String(docB64);



							string dirFullPath = context.Server.MapPath("~/pages/Descargas");
							string nuevoArchivo = dirFullPath + "/" + nombreDocumento;
							File.WriteAllBytes(nuevoArchivo, decoded);



							context.Response.ContentType = "application/octet-stream";
							context.Response.AddHeader("content-disposition", "attachment;filename=" + Path.GetFileName(nuevoArchivo));
							context.Response.WriteFile(nuevoArchivo);
							context.Response.End();
						}

					}
					catch (Exception ex)
					{
						context.Response.ContentType = "text/plain";
						context.Response.Write(ex.Message);
					}

				}

			}
			catch (Exception ex)
			{
				context.Response.ContentType = "text/plain";
				context.Response.Write(ex.Message);
			}
		}

		public class RowCsv
		{
			public string Clave;
			public string Dias;
			public string Horas;
		}

		public static List<RowCsv> GetRecordsCSV(string path)
		{

			string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

			SqlConnection conn = new SqlConnection(strConexion);
			List<RowCsv> items = new List<RowCsv>();

			try
			{
				conn.Open();
				DataSet ds = new DataSet();
				string query = @" 
							select clave, jue+vie+sab+dom+lun+mar+mie as dias, horas_extras_laboradas as horas
							from 
							(
								select e.clave, pnom.horas_extras_laboradas,
								case when  (pnom.jue = '1' or pnom.jue = '10') then 1 else 0 end as jue,
								case when  (pnom.vie = '1' or pnom.vie = '10') then 1 else 0 end as vie,
								case when  (pnom.sab = '1' or pnom.sab = '10') then 1 else 0 end as sab,
								case when  (pnom.dom = '1' or pnom.dom = '10') then 1 else 0 end as dom,
								case when  (pnom.lun = '1' or pnom.lun = '10') then 1 else 0 end as lun,
								case when  (pnom.mar = '1' or pnom.mar = '10') then 1 else 0 end as mar,
								case when  (pnom.mie = '1' or pnom.mie = '10') then 1 else 0 end as mie
								FROM  empleado e  JOIN puesto p ON (p.id_puesto = e.id_puesto)  
								JOIN departamento d ON (d.id_departamento = e.id_departamento)  
								JOIN prenomina pre ON (pre.id_obra = e.id_departamento)  
								left JOIN detalle_prenomina pnom ON (pnom.id_prenomina = pre.id_prenomina  and pnom.id_empleado = e.id_empleado)  
								WHERE IsNull(pre.activo, 1) = 1 
								AND pre.id_status_prenomina in ( " + Prenomina.STATUS_PRENOMINA_ENVIADA
								+ ", "
								+ Prenomina.STATUS_PRENOMINA_APROBADA +
								"		) as q 	";

				SqlDataAdapter adp = new SqlDataAdapter(query, conn);
				//adp.SelectCommand.Parameters.AddWithValue("id_obra", id_obra);
				//adp.SelectCommand.Parameters.AddWithValue("id_prenomina", id_prenomina);

				Log("\nMétodo-> " +
				System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

				adp.Fill(ds);

				if (ds.Tables[0].Rows.Count > 0)
				{
					for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
					{
						RowCsv item = new RowCsv();
						item.Clave = ds.Tables[0].Rows[i]["clave"].ToString();
						item.Dias = ds.Tables[0].Rows[i]["dias"].ToString();
						item.Horas = ds.Tables[0].Rows[i]["horas"].ToString();

						items.Add(item);


					}
				}


				return items;
			}
			catch (Exception ex)
			{
				Log("Error ... " + ex.Message);
				Log(ex.StackTrace);
				return items;
			}

			finally
			{
				conn.Close();
			}

		}

		public static string[] GetDocumento(string path, string id_documento)
		{

			string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

			SqlConnection conn = new SqlConnection(strConexion);

			string[] salida = new string[2];
			try
			{
				conn.Open();
				DataSet ds = new DataSet();
				string query = "SELECT contenido_b64, descripcion_documento " +
					"  FROM documento_requisicion" +
					"  WHERE id_documento_requisicion = @id_documento " +
					"   ";


				SqlDataAdapter adp = new SqlDataAdapter(query, conn);
				adp.SelectCommand.Parameters.AddWithValue("@id_documento", id_documento);

				adp.Fill(ds);

				if (ds.Tables[0].Rows.Count > 0)
				{

					salida[0] = ds.Tables[0].Rows[0][0].ToString();
					salida[1] = ds.Tables[0].Rows[0][1].ToString();


				}


			}
			catch (Exception ex)
			{
				Log(ex.Message);

				return null;
			}

			finally
			{
				conn.Close();
			}

			return salida;

		}



		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		private string ObtenerDiaSemana(int dia)
		{
			string nombreDia = "";
			switch (dia)
			{
				case 0: nombreDia = "DOMINGO"; break;
				case 1: nombreDia = "LUNES"; break;
				case 2: nombreDia = "MARTES"; break;
				case 3: nombreDia = "MIERCOLES"; break;
				case 4: nombreDia = "JUEVES"; break;
				case 5: nombreDia = "VIERNES"; break;
				case 6: nombreDia = "SÁBADO"; break;
			}
			return nombreDia;
		}


		public static void Log(string texto)
		{
			System.Diagnostics.Debug.Print(texto);


		}
	}
}