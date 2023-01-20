using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using System.IO;
using System.Drawing;
using Syncfusion.DocToPDFConverter;
using Syncfusion.Pdf;
using Dapper;

namespace Plataforma.pages.Loans
{
	public partial class PaymentOverdue : System.Web.UI.Page
	{
		const string pagina = "15";

		protected void Page_Load(object sender, EventArgs e)
		{
			string usuario = (string)Session["usuario"];
			string idTipoUsuario = (string)Session["id_tipo_usuario"];
			string idUsuario = (string)Session["id_usuario"];
			string path = (string)Session["path"];
			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NRAiBiAaIQQuGjN/V0Z+WE9EaFtGVmJLYVB3WmpQdldgdVRMZVVbQX9PIiBoS35RdUViW39fc3RTQmFVWUR2");


			txtUsuario.Value = usuario;//"promotor.colorado
			txtIdTipoUsuario.Value = idTipoUsuario;//5
			txtIdUsuario.Value = idUsuario;//69


			//  FASE DE PRUEBAS, QUITAR AL FINAL
			//if (usuario == string.Empty)
			//{
			//    txtUsuario.Value = "promotor.colorado";
			//    txtIdTipoUsuario.Value = "5";
			//    txtIdUsuario.Value = "69";
			//}
			//


			//  si no esta logueado
			if (usuario == string.Empty)
			{
				Response.Redirect("Login.aspx");
			}
		}

		[WebMethod]
		public static List<Pago> GetListaItems(string path, string idUsuario, string idTipoUsuario, string idStatus,
				string fechaInicial, string fechaFinal, int idPlaza, int idEjecutivo, int idSupervisor, int idPromotor, string typeFilter)
		{

			string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


			// verificar que tenga permisos para usar esta pagina
			bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
			if (!tienePermiso)
			{
				return null;//No tiene permisos
			}


			//  Lista de datos a devolver
			List<Pago> items = new List<Pago>();


			SqlConnection conn = new SqlConnection(strConexion);

			try
			{

				conn.Open();

				//  Traer datos del usuario para saber su id_empleado
				Usuario user = Usuarios.GetUsuario(path, idUsuario);

				//  Filtro status 
				var sqlPlaza = "";
				if (idPlaza > 0)
				{
					var sqlEmpleado = "";
					var empleados = conn.Query<Empleado>("SELECT id_empleado IdEmpleado, id_plaza IdPlaza, id_posicion IdPosicion, id_supervisor IdSupervisor, id_ejecutivo IdEjecutivo FROM empleado WHERE id_plaza = " + idPlaza).ToList();
					List<Empleado> empleadosFiltrados = new List<Empleado>();

					switch (typeFilter)
					{
						case "promotor":
							empleadosFiltrados = empleados.Where(w => w.IdEmpleado == idPromotor).ToList();
							break;
						case "supervisor":
							//obtenemos el supervisor
							var supervisor = empleados.Where(w => w.IdEmpleado == idSupervisor && w.IdPosicion == 4).ToList();

							//obtenemos los promotores asignados al supervisor
							var promotores = empleados.Where(w => w.IdSupervisor == idSupervisor && w.IdPosicion == 5).ToList();

							//empleadosFiltrados.AddRange(supervisor);
							empleadosFiltrados.AddRange(promotores);
							break;
						case "ejecutivo":
							//obtenemos el ejecutivo
							var ejecutivo = empleados.Where(w => w.IdEmpleado == idEjecutivo && w.IdPosicion == 3).ToList();

							//obtenemos los supervisores
							var supervisores = empleados.Where(w => w.IdEjecutivo == idEjecutivo && w.IdPosicion == 4).ToList();
							var supervisoresIDs = supervisores.Select(s => s.IdEmpleado).ToList();

							//obtenemos los promotores
							var promotoresEjecutivo = empleados.Where(w => supervisoresIDs.Contains(w.IdSupervisor) && w.IdPosicion == 5).ToList();

							//empleadosFiltrados.AddRange(ejecutivo);
							//empleadosFiltrados.AddRange(supervisores);
							empleadosFiltrados.AddRange(promotoresEjecutivo);

							break;
					}
					if (empleadosFiltrados.Count > 0)
					{
						sqlEmpleado = string.Join<int>(",", empleadosFiltrados.Select(s => s.IdEmpleado).ToList());
					}
					else
					{
						sqlEmpleado = string.Join<int>(",", empleados.Select(s => s.IdEmpleado).ToList());
					}

					sqlPlaza = " AND p.id_empleado IN (" + sqlEmpleado + ") ";
				}


				int idStatusPagoFalla = Pago.STATUS_PAGO_FALLA;    // PARA EL subquery de totalizado de fallas


				DataSet ds = new DataSet();
				string query = @"SELECT 
									pre.id_prestamo,
									concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
									concat(c.nombre_aval ,  ' ' , c.primer_apellido_aval, ' ' , c.segundo_apellido_aval) AS nombre_completo_aval,
									c.id_cliente,
									c.telefono,
									c.telefono_aval,
									dc.calleyno,
									da.calleyno calleyno_aval,
									pre.monto,
									pre.fecha_solicitud,
									pre.fecha_aprobacion,
									pre.id_status_prestamo,
									st.color,
									st.nombre nombre_status_prestamo,
									(SELECT SUBSTRING( 
												(
												SELECT ', ' + CAST(p2.numero_semana AS VARCHAR(5)) AS 'data()'
												FROM pago p2 WHERE p2.id_prestamo = pre.id_prestamo
												AND  p2.id_status_pago = 2 AND p2.saldo > 0
												FOR XML PATH('') 
												), 2 , 9999)) As semanas_falla,
									IsNull( (SELECT SUM(f.saldo) FROM pago f WHERE f.id_prestamo = pre.id_prestamo AND  f.id_status_pago = 2 ) , 0)  total_falla,
									IsNull( (SELECT SUM(f.pagado) FROM pago f WHERE f.id_prestamo = pre.id_prestamo AND  f.id_status_pago = 2 ) , 0)  total_pagado,
									p.id_pago,
									p.numero_semana,
									p.monto montopago
								FROM 
									prestamo pre 
									INNER JOIN cliente c ON (c.id_cliente = pre.id_cliente) 
									LEFT JOIN direccion dc ON (dc.id_cliente = c.id_cliente AND dc.aval = 0)
									LEFT JOIN direccion da ON (da.id_cliente = c.id_cliente AND da.aval = 1)
									JOIN status_prestamo st ON (st.id_status_prestamo = pre.id_status_prestamo)
									INNER JOIN pago p ON (
										p.id_prestamo = pre.id_prestamo 
										AND p.fecha < CAST(GETDATE() as DATE) 
										AND p.fecha <= CAST(GETDATE() as DATE) 
										AND p.id_status_pago IN (2) 
										AND p.numero_semana = (SELECT MIN(numero_semana) FROM pago p2 WHERE p2.id_prestamo = pre.id_prestamo AND p2.id_status_pago = 2)
									)
								WHERE
									pre.id_status_prestamo = 4" + sqlPlaza;

				SqlDataAdapter adp = new SqlDataAdapter(query, conn);

				Utils.Log("\nMétodo-> " +
				System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

				adp.Fill(ds);

				if (ds.Tables[0].Rows.Count > 0)
				{
					for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
					{
						Pago item = new Pago();
						item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
						item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
						item.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
						item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
						item.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
						item.CalleCliente = ds.Tables[0].Rows[i]["calleyno"].ToString();
						item.TelefonoCliente = ds.Tables[0].Rows[i]["telefono"].ToString();
						item.NombreAval = ds.Tables[0].Rows[i]["nombre_completo_aval"].ToString();
						item.CalleAval = ds.Tables[0].Rows[i]["calleyno_aval"].ToString();
						item.TelefonoAval = ds.Tables[0].Rows[i]["telefono_aval"].ToString();
						item.MontoPrestamo = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
						item.Monto = float.Parse(ds.Tables[0].Rows[i]["montopago"].ToString());
						item.MontoFormateadoMx = item.Monto.ToString("C2"); //moneda Mx -> $ 2,233.00
						item.Pagado = float.Parse(ds.Tables[0].Rows[i]["total_pagado"].ToString());

						item.TotalFalla = float.Parse(ds.Tables[0].Rows[i]["total_falla"].ToString());
						item.TotalFallaFormateadoMx = item.TotalFalla.ToString("C2");
						item.Fecha = DateTime.Parse(ds.Tables[0].Rows[i]["fecha_solicitud"].ToString());
						item.FechaStr = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();

						item.SemanasFalla = ds.Tables[0].Rows[i]["semanas_falla"].ToString();

						item.Color = ds.Tables[0].Rows[i]["color"].ToString();
						item.Status = "<span class='" + item.Color + "'>" + ds.Tables[0].Rows[i]["nombre_status_prestamo"].ToString() + "</span>";


						string botones = "";

						botones += "<button data-idcliente = " + item.IdCliente + "  data-idprestamo = " + item.IdPrestamo + " onclick='payments.view(" + item.IdPago + ")'  class='btn btn-outline-primary'> <span class='fa fa-folder-open mr-1'></span>Abrir</button>";

						item.Accion = botones;

						items.Add(item);


					}
				}


				return items;
			}
			catch (Exception ex)
			{
				Utils.Log("Error ... " + ex.Message);
				Utils.Log(ex.StackTrace);
				return items;
			}

			finally
			{
				conn.Close();
			}

		}

		[WebMethod]
		public static Pago GetPaymentByIdPrestamo(string path, string idPrestamo, string idUsuario)
		{

			string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


			// verificar que tenga permisos para usar esta pagina
			bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
			if (!tienePermiso)
			{
				return null;//No tiene permisos
			}


			Pago item = new Pago();


			SqlConnection conn = new SqlConnection(strConexion);

			try
			{

				conn.Open();
				DataSet ds = new DataSet();
				string query = @"SELECT 
									pre.id_prestamo,
									concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
									concat(c.nombre_aval ,  ' ' , c.primer_apellido_aval, ' ' , c.segundo_apellido_aval) AS nombre_completo_aval,
									c.id_cliente,
									c.telefono,
									c.telefono_aval,
									dc.calleyno,
									da.calleyno calleyno_aval,
									pre.monto,
									pre.fecha_solicitud,
									pre.fecha_aprobacion,
									pre.id_status_prestamo,
									st.color,
									st.nombre nombre_status_prestamo,
									(SELECT SUBSTRING( 
												(
												SELECT ', ' + CAST(p2.numero_semana AS VARCHAR(5)) AS 'data()'
												FROM pago p2 WHERE p2.id_prestamo = pre.id_prestamo
												AND  p2.id_status_pago = 2 AND p2.saldo > 0
												FOR XML PATH('') 
												), 2 , 9999)) As semanas_falla,
									IsNull( (SELECT SUM(f.saldo) FROM pago f WHERE f.id_prestamo = pre.id_prestamo AND  f.id_status_pago = 2 ) , 0)  total_falla,
									IsNull( (SELECT SUM(f.pagado) FROM pago f WHERE f.id_prestamo = pre.id_prestamo AND  f.id_status_pago = 2 ) , 0)  total_pagado,
									p.id_pago,
									p.numero_semana,
									p.monto montopago
								FROM 
									prestamo pre 
									INNER JOIN cliente c ON (c.id_cliente = pre.id_cliente) 
									LEFT JOIN direccion dc ON (dc.id_cliente = c.id_cliente AND dc.aval = 0)
									LEFT JOIN direccion da ON (da.id_cliente = c.id_cliente AND da.aval = 1)
									JOIN status_prestamo st ON (st.id_status_prestamo = pre.id_status_prestamo)
									INNER JOIN pago p ON (
										p.id_prestamo = pre.id_prestamo 
										AND p.fecha < CAST(GETDATE() as DATE) 
										AND p.fecha <= CAST(GETDATE() as DATE) 
										AND p.id_status_pago IN (2) 
										AND p.numero_semana = (SELECT MIN(numero_semana) FROM pago p2 WHERE p2.id_prestamo = pre.id_prestamo  AND p2.id_status_pago = 2)
									)
								WHERE
									pre.id_prestamo = @id_prestamo ";

				SqlDataAdapter adp = new SqlDataAdapter(query, conn);
				adp.SelectCommand.Parameters.AddWithValue("id_prestamo", idPrestamo);

				Utils.Log("\nMétodo-> " +
				System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

				adp.Fill(ds);

				if (ds.Tables[0].Rows.Count > 0)
				{
					for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
					{
						item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
						item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
						item.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
						item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
						item.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
						item.CalleCliente = ds.Tables[0].Rows[i]["calleyno"].ToString();
						item.TelefonoCliente = ds.Tables[0].Rows[i]["telefono"].ToString();
						item.NombreAval = ds.Tables[0].Rows[i]["nombre_completo_aval"].ToString();
						item.CalleAval = ds.Tables[0].Rows[i]["calleyno_aval"].ToString();
						item.TelefonoAval = ds.Tables[0].Rows[i]["telefono_aval"].ToString();
						item.MontoPrestamo = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
						item.Monto = float.Parse(ds.Tables[0].Rows[i]["montopago"].ToString());
						item.MontoFormateadoMx = item.Monto.ToString("C2"); //moneda Mx -> $ 2,233.00
						item.Pagado = float.Parse(ds.Tables[0].Rows[i]["total_pagado"].ToString());

						item.TotalFalla = float.Parse(ds.Tables[0].Rows[i]["total_falla"].ToString());
						item.TotalFallaFormateadoMx = item.TotalFalla.ToString("C2");
						item.Fecha = DateTime.Parse(ds.Tables[0].Rows[i]["fecha_solicitud"].ToString());
						item.FechaStr = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();

						item.SemanasFalla = ds.Tables[0].Rows[i]["semanas_falla"].ToString();

						item.Color = ds.Tables[0].Rows[i]["color"].ToString();
						item.Status = "<span class='" + item.Color + "'>" + ds.Tables[0].Rows[i]["nombre_status_prestamo"].ToString() + "</span>";

					}
				}


			}
			catch (Exception ex)
			{
				Utils.Log("Error ... " + ex.Message);
				Utils.Log(ex.StackTrace);
			}

			finally
			{
				conn.Close();
			}

			return item;


		}

		[WebMethod]
		public static string GenerateReport(string path, int idPrestamo, int idCliente, int idUsuario, int idPosicion,double abono, double abonoPactado, string semanas)
		{
			string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
			SqlConnection conn = new SqlConnection(strConexion);

			try
			{

				conn.Open();
				DataSet ds = new DataSet();
				string query = @"SELECT 
						pre.fecha_solicitud,
						concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo
					  FROM
					   prestamo pre 
					   INNER JOIN cliente c ON (c.id_cliente = pre.id_cliente)
					   WHERE id_prestamo = @id_prestamo ";

				SqlDataAdapter adp = new SqlDataAdapter(query, conn);
				adp.SelectCommand.Parameters.AddWithValue("id_prestamo", idPrestamo);

				Utils.Log("\nMétodo-> " +
				System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

				adp.Fill(ds);
				DateTime FechaCredito = DateTime.Now;
				string NombreCliente = "";
				if (ds.Tables[0].Rows.Count > 0)
				{
					for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
					{
						FechaCredito = DateTime.Parse(ds.Tables[0].Rows[i]["fecha_solicitud"].ToString());
						NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
					}
				}

				var pathFile = HttpContext.Current.Server.MapPath("~/plantillas/ticket_pago_01.docx");
				WordDocument document = new WordDocument(pathFile);
				string[] fieldNames = new string[] { "FechaPago", "NombreCliente", "FechaCredito", "SemanaPago", "AbonoPactado", "AbonoRecibido", "Promotor" };
				string[] fieldValues = new string[] { DateTime.Now.ToString("dd/MM/yyyy"), NombreCliente, FechaCredito.ToString("dd/MM/yyyy"), semanas, abonoPactado.ToString("C2"), abono.ToString("C2"), (string)HttpContext.Current.Session["usuario"] };
				document.MailMerge.Execute(fieldNames, fieldValues);
				//document.Save("Sample.docx", FormatType.Docx);

				DocToPDFConverter converter = new DocToPDFConverter();
				PdfDocument pdfDocument = converter.ConvertToPDF(document);
				MemoryStream stream = new MemoryStream();
				pdfDocument.Save(stream);
				pdfDocument.Close(true);
				document.Close();

				return "data:application/pdf;base64," + Convert.ToBase64String(stream.ToArray());


			}
			catch(Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		/// <summary>
		/// Capturar los datos del pago
		/// </summary>
		/// <param name="path"></param>
		/// <param name="idPrestamo"></param>
		/// <param name="idUsuario"></param>
		/// <param name="idPosicion"></param>
		/// <param name="nota"></param>
		/// <returns></returns>
		[WebMethod]
		public static DatosSalida SavePayment(string path, int idPrestamo, int idCliente, string idUsuario, double abono, string notaCliente, string notaAval)
		{

			string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
			double abonoRecibido = abono;

			SqlConnection conn = new SqlConnection(strConexion);
			List<StatusPrestamo> items = new List<StatusPrestamo>();
			DatosSalida response = new DatosSalida();


			SqlTransaction transaccion = null;

			int r = 0;
			try
			{

				conn.Open();
				transaccion = conn.BeginTransaction();

				#region Abono de saldo recuperado
				if (abono > 0)
				{
					//3.  Abonar lo recuperado a el o los pagos en status abono hasta completar lo abonado
					//DataSet ds = new DataSet();
					List<Pago> pagosFalla = GetPaymentsByIdPrestamoAndStatus(idPrestamo.ToString(), new int[] { Pago.STATUS_PAGO_FALLA }, conn, transaccion);
					//  Acomodar el monto recuperado en los registros de fallas
					if (pagosFalla.Count > 0 && abono > 0)
					{
						int i = 0;

						Utils.Log("recuperado ... " + abono);

						while (abono > 0 && i < pagosFalla.Count)
						{
							Pago pago = pagosFalla[i];
							Utils.Log("idPago ... " + pago.IdPago);
							Utils.Log("Monto ... " + pago.Monto);
							Utils.Log("Saldo ... " + pago.Saldo);

							double montoAAbonar = abono;
							bool completo = false;
							if (abono >= pago.Saldo)
							{
								montoAAbonar = pago.Saldo;
								completo= true;
							}
							Utils.Log("montoAAbonar ... " + montoAAbonar);

							int rowsUpdateds = UpdatePago(pago.IdPago, montoAAbonar, true, completo,notaCliente, notaAval, conn, transaccion);

							Utils.Log("rowsAffected de pago " + pago.IdPago + " ... " + rowsUpdateds);

							abono -= montoAAbonar;

							Utils.Log("recuperado restamte ... " + abono);

							i++;
						}

						//  Revisar que si uno de los pagos de la lista de los que estaban en falla, pasan se cubren en su totalidad
						//  Cambiarlos de status a abonado
						foreach (var item in pagosFalla)
						{
							Utils.Log("Revisar status y actualizar de pago " + item.IdPago);

							int rowsAfcected = UpdateStatusPago(item.IdPago, conn, transaccion);

							Utils.Log("rowsAfcected UpdateStatusPago ... " + rowsAfcected);
						}

					}

				}
				#endregion

				//  Revisar que si todos los pagos  han sido realizados, abonado o pagado normal pasar el status del prestamo a pagado.                    
				List<Pago> pagosDistintosAFalla = GetPaymentsByIdPrestamoAndStatus(idPrestamo.ToString(),
						new int[] { Pago.STATUS_PAGO_FALLA, Pago.STATUS_PAGO_PENDIENTE }, conn, transaccion);

				if (pagosDistintosAFalla.Count < 1)
				{
					Utils.Log("Actualizar status de préstamo a Pagado");

					string nota = "Todos los pagos del préstamo se encuentran cubiertos, el préstamo se pasa a status pagado." + DateTime.Now.ToString("g");

					int rowsAffectedPrestamo = UpdateStatusPrestamo(idPrestamo.ToString(), idUsuario.ToString(), nota, conn, transaccion);

					Utils.Log("rowsAffectedPrestamo  ... " + rowsAffectedPrestamo);

					//  Pasar el status del cliente a inactivo
					rowsAffectedPrestamo = UpdateStatusCustomer(idCliente.ToString(), idUsuario.ToString(), Cliente.STATUS_INACTIVO, conn, transaccion);
					Utils.Log("rowsAffectedCliente ... " + rowsAffectedPrestamo);


				}
				//
				response.IdItem = abonoRecibido.ToString();
				transaccion.Commit();

				return response;
			}
			catch (Exception ex)
			{
				try
				{
					transaccion.Rollback();
				}
				catch (Exception ex_)
				{

					r = -1;
					response.MensajeError = "Se ha generado un error, no se pudo finalizar la operación.";
					response.CodigoError = 1;
				}


				Utils.Log("Error ... " + ex.Message);
				Utils.Log(ex.StackTrace);
				r = -1;
				response.MensajeError = "Se ha generado un error, no se pudo finalizar la operación.";
				response.CodigoError = 1;
			}

			finally
			{
				conn.Close();
			}

			return response;


		}

		public static List<Pago> GetPaymentsByIdPrestamoAndStatus(string idPrestamo, int[] idsStatus, SqlConnection conn, SqlTransaction transaction)
		{

			List<Pago> items = new List<Pago>();

			try
			{

				DataSet ds = new DataSet();
				string query = @" SELECT p.id_pago, p.id_prestamo, IsNull(p.monto, 0) monto, IsNull(p.saldo, 0) saldo, 
                                    IsNull(p.pagado, 0) pagado,
                                    p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                     FORMAT(p.fecha, 'dd/MM/yyyy') fechastr   
                                    FROM pago p
                                    WHERE p.id_prestamo = @id_prestamo
                                        AND p.id_status_pago IN (" + string.Join(",", idsStatus) + @")
                                    ORDER BY p.numero_semana  ";

				SqlDataAdapter adp = new SqlDataAdapter(query, conn);
				adp.SelectCommand.Parameters.AddWithValue("id_prestamo", idPrestamo);
				adp.SelectCommand.Transaction = transaction;

				Utils.Log("\nMétodo-> " +
				System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

				adp.Fill(ds);

				if (ds.Tables[0].Rows.Count > 0)
				{
					for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
					{
						Pago item = new Pago();

						item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
						item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
						item.IdStatusPago = int.Parse(ds.Tables[0].Rows[i]["id_status_pago"].ToString());
						item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
						item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
						item.Saldo = float.Parse(ds.Tables[0].Rows[i]["saldo"].ToString());
						item.Pagado = float.Parse(ds.Tables[0].Rows[i]["pagado"].ToString());


						items.Add(item);

					}
				}


			}
			catch (Exception ex)
			{

				Utils.Log("Error ... " + ex.Message);
				Utils.Log(ex.StackTrace);

				throw ex;
			}



			return items;


		}

		[WebMethod]
		public static int UpdatePago(int idPago, double abono, bool esRecuperacion, bool esCompleto, string notaCliente, string notaAval,SqlConnection conn, SqlTransaction transaction)
		{

			int r = 0;
			try
			{
				string sqlEsRecuperacion = "";
				string sql = "";
				if (esRecuperacion)
				{
					sqlEsRecuperacion += " ,fecha_registro_pago = @fecha_registro_pago,es_recuperado = 1, nota_vencida_cliente = @nota_vencida_cliente, nota_vencida_aval= @nota_vencida_aval ";
				}

				if (esCompleto)
				{
					sql = @" UPDATE pago SET pagado = monto, saldo = 0 " + sqlEsRecuperacion +
							 @" WHERE id_pago = @id_pago ";
				}
				else
				{
					sql = @" UPDATE pago SET pagado = pagado+@abono, saldo = saldo-@abono " + sqlEsRecuperacion +
							 @" WHERE id_pago = @id_pago ";
				}

				Utils.Log("\nMétodo-> " +
				System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.CommandType = CommandType.Text;

				cmd.Parameters.AddWithValue("@abono", abono);
				cmd.Parameters.AddWithValue("@id_pago", idPago);
				cmd.Parameters.AddWithValue("@fecha_registro_pago", DateTime.Now);
				cmd.Parameters.AddWithValue("@nota_vencida_cliente", notaCliente);
				cmd.Parameters.AddWithValue("@nota_vencida_aval", notaAval);
				cmd.Transaction = transaction;

				r = cmd.ExecuteNonQuery();


				Utils.Log("Pago actualizado -> OK ");


			}
			catch (Exception ex)
			{

				Utils.Log("Error ... " + ex.Message);
				Utils.Log(ex.StackTrace);

				throw ex;

			}


			return r;

		}

		[WebMethod]
		public static int UpdateStatusPago(int idPago, SqlConnection conn, SqlTransaction transaction)
		{

			int r = 0;
			try
			{

				string sql = @" UPDATE pago SET saldo = 0, pagado = monto, id_status_pago = @id_status_pago
                            WHERE id_pago = @id_pago AND pagado >= monto ";


				Utils.Log("\nMétodo-> " +
				System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

				SqlCommand cmd = new SqlCommand(sql, conn);
				cmd.CommandType = CommandType.Text;

				cmd.Parameters.AddWithValue("@id_pago", idPago);
				cmd.Parameters.AddWithValue("@id_status_pago", Pago.STATUS_PAGO_ABONADO);
				cmd.Transaction = transaction;

				r = cmd.ExecuteNonQuery();


				Utils.Log("Status Pago actualizado  " + (r > 0).ToString());


			}
			catch (Exception ex)
			{

				Utils.Log("Error ... " + ex.Message);
				Utils.Log(ex.StackTrace);

				throw ex;

			}


			return r;

		}

		public static int UpdateStatusPrestamo(string idPrestamo, string idUsuario, string nota,
			SqlConnection conn, SqlTransaction transaction)
		{

			int r = 0;
			try
			{

				string sql = @"  UPDATE prestamo
                            SET id_status_prestamo = @id_status_prestamo, notas_generales = @notas_generales, id_usuario = @id_usuario
                            WHERE
                            id_prestamo = @id_prestamo ";


				SqlCommand cmdUpdatePrestamo = new SqlCommand(sql, conn);
				cmdUpdatePrestamo.CommandType = CommandType.Text;

				cmdUpdatePrestamo.Parameters.AddWithValue("@id_prestamo", idPrestamo);
				cmdUpdatePrestamo.Parameters.AddWithValue("@notas_generales", nota);
				cmdUpdatePrestamo.Parameters.AddWithValue("@id_usuario", idUsuario);
				cmdUpdatePrestamo.Parameters.AddWithValue("@id_status_prestamo", Prestamo.STATUS_PAGADO);
				cmdUpdatePrestamo.Transaction = transaction;

				r += cmdUpdatePrestamo.ExecuteNonQuery();

			}
			catch (Exception ex)
			{

				Utils.Log("Error ... " + ex.Message);
				Utils.Log(ex.StackTrace);
				r = -1;

			}


			return r;


		}


		public static int UpdateStatusCustomer(string idCliente, string idUsuario, int idStatus,
			SqlConnection conn, SqlTransaction transaction)
		{

			int r = 0;
			try
			{

				string sql = @"  UPDATE cliente
                            SET id_status_cliente = @id_status_cliente, id_usuario = @id_usuario
                            WHERE
                                id_cliente = @id_cliente ";


				using (SqlCommand cmdUpdate = new SqlCommand(sql, conn))
				{
					cmdUpdate.CommandType = CommandType.Text;

					cmdUpdate.Parameters.AddWithValue("@id_cliente", idCliente);
					cmdUpdate.Parameters.AddWithValue("@id_usuario", idUsuario);
					cmdUpdate.Parameters.AddWithValue("@id_status_cliente", idStatus);
					cmdUpdate.Transaction = transaction;

					r += cmdUpdate.ExecuteNonQuery();
				}

			}
			catch (Exception ex)
			{

				Utils.Log("Error ... " + ex.Message);
				Utils.Log(ex.StackTrace);
				r = -1;

			}


			return r;


		}
	}
}