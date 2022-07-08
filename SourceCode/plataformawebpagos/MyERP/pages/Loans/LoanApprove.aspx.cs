using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Plataforma.pages
{
    public partial class LoanApprove : System.Web.UI.Page
    {
        const string pagina = "13";



        protected void Page_Load(object sender, EventArgs e)
        {
            string usuario = (string)Session["usuario"];
            string idTipoUsuario = (string)Session["id_tipo_usuario"];
            string idUsuario = (string)Session["id_usuario"];
            string path = (string)Session["path"];



            txtUsuario.Value = usuario;
            txtIdTipoUsuario.Value = idTipoUsuario;
            txtIdUsuario.Value = idUsuario;

            //  si no esta logueado
            if (usuario == string.Empty)
            {
                Response.Redirect("Login.aspx");
            }


        }


        /// <summary>
        /// Aprobación de un préstamo por un supervisor o por un ejecutivo.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="idPrestamo"></param>
        /// <param name="idUsuario"></param>
        /// <param name="idPosicion"></param>
        /// <param name="nota"></param>
        /// <returns></returns>
        [WebMethod]
        public static DatosSalida Approve(string path, string idPrestamo, string idUsuario, string idPosicion, string nota)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<StatusPrestamo> items = new List<StatusPrestamo>();
            DatosSalida response = new DatosSalida();

            Utils.Log("\n\n******************Inicia la aprobación del préstamo ");

            SqlTransaction transaction = null;

            int r = 0;
            try
            {

                conn.Open();
                transaction = conn.BeginTransaction();


                //  Traer los datos del préstamo y cliente
                Prestamo prestamo = LoanRequest.GetDataPrestamo(path, idPrestamo);

                //  VALIDACIONES
                // 1) Validar campos vacíos
                var dataStringsValidations = ValidateCustomerData(prestamo.Cliente);
                if (dataStringsValidations.Count > 0)
                {
                    response.MensajeError = "Se necesitan valores para los siguientes campos: " + string.Join(", ", dataStringsValidations.ToArray());
                    response.CodigoError = 1;
                    return response;
                }



                //  -------Validar que las fotos esten subidas para cliente y aval
                // 2) Traer los documentos actuales del pr+éstamo
                List<Documento> documentsInLoan = GetDocumentsByCustomerId(path, prestamo.IdCliente, conn, transaction);


                //  Traer todos los tipos de documenos necesarios para un prestamo
                List<Documento> allDocumentTypes = GetDocumentCustomerTypes(path, conn, transaction);
                HashSet<int> docs = new HashSet<int>(documentsInLoan.Select(x => x.IdTipoDocumento));
                //  Restar los documentos del documento a la lista de todos los documentos necesarios
                allDocumentTypes.RemoveAll(x => docs.Contains(x.IdTipoDocumento));

                //  Lista final de documentos faltantes
                List<string> missingsDocs = new List<string>();
                foreach (var item in allDocumentTypes)
                {
                    missingsDocs.Add("<li>" + item.Nombre + "</li>");
                }

                if (allDocumentTypes.Count > 0)
                {
                    response.MensajeError = "<p>Se necesita todos los documentos solicitados. </p><ul>" + string.Join(", ", missingsDocs.ToArray()) + "</ul>";
                    response.CodigoError = 1;
                    return response;
                }
                //  -------

                //  Tipo de cliente
                TipoCliente customerType = GetCustomerTypeById(path, prestamo.Cliente.IdTipoCliente.ToString(), conn, transaction);


                Utils.Log("Núm de semanas  " + customerType.SemanasAPrestar);
                Utils.Log("GarantiasPorMonto " + customerType.GarantiasPorMonto);
                Utils.Log("prestamo.Monto " + prestamo.Monto);

                float guaranteeAmmount = prestamo.Monto * customerType.GarantiasPorMonto;
                Utils.Log("Monto A cubrir por las garantias = " + guaranteeAmmount);

                // 3) Validar que la suma de los costos de los articulos en garantia del cliente,
                //  sean mas o igual al monto del prestamo mas el porcentaje configurado
                List<Garantia> guaranteeCustomerList = GetListGuaranteeCustomer(path, idUsuario, prestamo.IdPrestamo.ToString(), conn, transaction);
                float guaranteeAmmountSumCustomer = 0;
                foreach (var item in guaranteeCustomerList)
                {
                    guaranteeAmmountSumCustomer += item.Costo;
                }
                if (guaranteeAmmountSumCustomer < guaranteeAmmount)
                {
                    response.MensajeError = "El total de las garantías del cliente no es suficiente para cubrir el monto del préstamo mas el porcentaje configurado de " +
                        customerType.GarantiasPorMonto + "<br/><br/>" +
                        "El monto del préstamo es: " + prestamo.Monto.ToString("C2") + "<br/>" +
                        "El monto a cubrir es: " + guaranteeAmmount.ToString("C2") + "<br/>" +
                        "La suma de costos de las garantías es: " + guaranteeAmmountSumCustomer.ToString("C2");

                    response.CodigoError = 1;
                    return response;
                }

                // 4) Validar que la suma de los costos de los articulos en garantia del aval,
                //  sean mas o igual al monto del prestamo mas el porcentaje configurado
                List<Garantia> guaranteeAvalList = GetListGuaranteeAval(path, idUsuario, prestamo.IdPrestamo.ToString(), conn, transaction);
                float guaranteeAmmountSumAval = 0;
                foreach (var item in guaranteeAvalList)
                {
                    guaranteeAmmountSumAval += item.Costo;
                }
                if (guaranteeAmmountSumAval < guaranteeAmmount)
                {
                    response.MensajeError = "El total de las garantías del aval no es suficiente para cubrir el monto del préstamo mas el porcentaje configurado de " +
                        customerType.GarantiasPorMonto + "<br/><br/>" +
                       "El monto del préstamo es: " + prestamo.Monto.ToString("C2") + "<br/>" +
                       "El monto a cubrir es: " + guaranteeAmmount.ToString("C2") + "<br/>" +
                       "La suma de costos de las garantías es: " + guaranteeAmmountSumAval.ToString("C2");

                    response.CodigoError = 1;
                    return response;
                }


                //  5) Validar el monto máximo al ser un préstamo inicial
                List<Prestamo> prestamosAnteriores = GetLoansByCustomerId(path, prestamo.IdCliente, conn, transaction);
                if (prestamosAnteriores.Count > 0)
                {
                    if (prestamo.Monto > customerType.PrestamoInicialMaximo)
                    {
                        response.MensajeError = "El monto para un préstamo inicial sobrepasa el monto configurado para este tipo de cliente (" + customerType.NombreTipoCliente?.Trim() + ").<br/><br/>" +
                      "El monto del préstamo es: " + prestamo.Monto.ToString("C2") + "<br/>" +
                      "El préstamo inicial máximo: " + customerType.PrestamoInicialMaximo.ToString("C2") + "<br/>";

                        response.CodigoError = 1;
                        return response;
                    }

                }




                // 6) Validar que el promotor no exceda el límite de crédito que puede otorgar y creación de alerta de límite de crédito
                if (idPosicion == Employees.POSICION_SUPERVISOR.ToString())
                {

                    //  Traer datos del empleado promotor
                    Empleado empleado = GetItemEmployee(prestamo.IdEmpleado.ToString(), conn, transaction);

                    if (empleado.MontoLimiteInicial < prestamo.Monto)
                    {
                        //  Insertar solicitud de aumento de limite de credito

                        int rowsAffected = InsertarSolicitudLimiteCredito(idPrestamo, idUsuario, conn, transaction);
                        Utils.Log("rowsAffected InsertarSolicitudLimiteCredito " + r);

                        transaction.Commit();

                        Utils.Log("\n\n******************Fin de la aprobación del préstamo debido a que el supervisor necesita aumento de crédito. ");

                        response.MensajeError = "El monto límite de crédito con el que cuenta no es suficiente para aprobar el préstamo. " +
                            "<br/>Monto límite del promotor: " + empleado.MontoLimiteInicial.ToString("C2") +
                            "<br/>Monto a solicitar: " + prestamo.Monto.ToString("C2") +
                            "<br/>Se ha generado una solicitud de aumento de crédito que debera ser aprobada.";
                        response.CodigoError = 2;
                        return response;

                    }


                }

                //  Para recalcular el monto a prestar restandole el monto pendiente de un anterior préstamo si es que tuviese uno
                //float newMonto = prestamo.Monto;

                // Traer al prestamo actual(anterior, no este nuevo que estamos aprobando)
                Prestamo currentLoan = GetPrestamoByIdCliente(prestamo.IdCliente.ToString(), conn, transaction);
                Prestamo deudaActual = null;

                if (currentLoan != null)
                {

                    //   Tiene un prestamo activo anterior 
                    //  Traer pagos del prestamo actual, con fecha de hasta este fin de semana
                    LoanValidation validations = new LoanValidation();
                    LoanValidation.WeekData currentWeek = validations.GetFechas();


                    // Traer pagos sin importar el status
                    List<Pago> paymentsByCurrentLoan = GetPaymentsByIdPrestamoAndDate(false, currentLoan.IdPrestamo.ToString(), currentWeek.fechaFinal, conn, transaction);

                    //  7) Validar que préstamo anterior se encuentre en la semana>=10, 
                    if (paymentsByCurrentLoan.Count < 10)
                    {
                        response.MensajeError = "El cliente cuenta con un préstamo activo en la semana número " + paymentsByCurrentLoan.Count + ".<br/>" +
                       "Para poder ser aprobado debe estar en la semana 10 o posterior.<br/><br/>" +
                       "Por lo tanto no es posible aprobar este nuevo préstamo.<br/>";

                        response.CodigoError = 1;
                        return response;

                    }

                    //  8) Validar que no tenga pagos en falla o abonados en prestamo actual sin importar la semana
                    var paymentsInFail = paymentsByCurrentLoan.Find(x => x.IdStatusPago == Pago.STATUS_PAGO_ABONADO || x.IdStatusPago == Pago.STATUS_PAGO_FALLA);
                    if (paymentsInFail != null)
                    {
                        response.MensajeError = "El cliente cuenta con un préstamo activo y este tiene pagos con status de falla o abonado.<br/><br/>" +
                       "Por lo tanto no es posible aprobar este nuevo préstamo.<br/>";

                        response.CodigoError = 1;
                        return response;
                    }


                    //  Obtener la deuda actual
                    deudaActual = CurrentDebtByIdLoan(currentLoan.IdPrestamo.ToString(), conn, transaction);


                    //  Validar que el monto de la deuda actual sea menor al monto del prestamo nuevo solicitado
                    if (prestamo.Monto < deudaActual.Saldo)
                    {
                        response.MensajeError = "El cliente cuenta con un préstamo activo con un saldo total por pagar de " + deudaActual.Saldo.ToString("C2") + " .<br/><br/>" +
                        "El nuevo préstamo es por la cantidad de " + prestamo.Monto.ToString("C2") + ", no alcanza a cubrir la deuda actual.<br/><br/>" +
                        "Por lo tanto no es posible aprobar este nuevo préstamo.<br/>";

                        response.CodigoError = 1;
                        return response;
                    }





                }

                //  Actualizar status y monto del préstamo

                if (idPosicion == Employees.POSICION_SUPERVISOR.ToString())
                {
                    
                    //  -1 para montoConInteres porque no queremos actualizar ese valor aún
                    int rowsAffectedStatusPrestamo = UpdateStatusPrestamo(idPrestamo, idUsuario, nota, -1,  Prestamo.STATUS_PENDIENTE_EJECUTIVO, prestamo.Monto, conn, transaction);

                    Utils.Log("rowsAffected UpdateStatusPrestamo POSICION_SUPERVISOR " + rowsAffectedStatusPrestamo);

                }
                else if (idPosicion == Employees.POSICION_EJECUTIVO.ToString())
                {

                    //  Si se esta cubriendo deuda restante anterior de un prestamo con el nuevo que se esta aprobando
                    if (currentLoan != null && deudaActual != null)
                    {
                        //  Nuevo monto
                        prestamo.Monto = prestamo.Monto - deudaActual.Saldo;
                        
                        //  Efectuar los pagos para cada Pago semanal pendiente
                        int rowsPago = UpdatePagos(currentLoan.IdPrestamo.ToString(), idPrestamo, conn, transaction);
                        Utils.Log("\nrowsPago affected " + rowsPago);


                        //  Pasar a status pagado el prestamo anterior
                        string notaPagoPrestamoAnterior = "Se da por pagado el prestamo anterior No. " + currentLoan.IdPrestamo + ". El saldo se cubre con el nuevo prestamo. " ;
                        
                        int rowsAffectedStatusPrestamoAnterior = UpdateStatusPrestamoPagado(currentLoan.IdPrestamo.ToString(), idUsuario, notaPagoPrestamoAnterior, Prestamo.STATUS_PAGADO, conn, transaction);                        

                        Utils.Log("\nrowsPago rowsAffectedStatusPrestamoAnterior " + rowsAffectedStatusPrestamoAnterior);

                    }

                    float pagoAmmountWithInteres = prestamo.Monto + (prestamo.Monto * customerType.PorcentajeSemanal / 100);

                    int rowsAffectedStatusPrestamo = UpdateStatusPrestamo(idPrestamo, idUsuario, nota, pagoAmmountWithInteres, Prestamo.STATUS_APROBADO, prestamo.Monto, conn, transaction);

                    Utils.Log("rowsAffected UpdateStatusPrestamo POSICION_EJECUTIVO " + rowsAffectedStatusPrestamo);

                    //  Pago semanal incluyendo interes
                    float pagoAmmount = pagoAmmountWithInteres  / customerType.SemanasAPrestar;


                    //  Generar semanas para pagos, Generar calendario de pagos de acuerdo al num. de semanas del tipo de cliente
                    //DateTime startDate = DateTime.Now;   //Tomar fecha de aprobacion para semanas de pago
                    DateTime startDate = new DateTime(2022, 06, 15);    //TODO: CAMBIAR ESTE TEST


                    //  Se agrega la semana extra por si le aplica
                    int numSemanas = customerType.SemanasAPrestar;
                    if (customerType.SemanasExtra == 1)
                    {
                        numSemanas++;
                    }

                    for (int i = 0; i < numSemanas; i++)
                    {

                        DateTime nextDate = startDate.AddDays(7);

                        Pago pago = new Pago();
                        pago.IdPrestamo = int.Parse(idPrestamo);
                        pago.Monto = pagoAmmount;
                        pago.NumeroSemana = (i + 1);
                        pago.Fecha = nextDate;
                        pago.IdUsuario = int.Parse(idUsuario);

                        bool isExtraWeek = (customerType.SemanasExtra == 1 && (i + 1) == numSemanas);

                        pago.SemanaExtra = isExtraWeek ? 1 : 0;

                        InsertPago(pago, conn, transaction);

                        startDate = nextDate;

                    }


                    //  Pasar el status del cliente a ACTIVO
                    int rowsAffectedPrestamo = UpdateStatusCustomer(prestamo.IdCliente, idUsuario, Cliente.STATUS_ACTIVO, conn, transaction);
                    Utils.Log("rowsAffectedCliente ... " + rowsAffectedPrestamo);



                }


                //  Actualizar relacion de aprobaciones
                int rowsAffectedRPA = UpdateRelacionPrestamoAprobacion(idPrestamo, idUsuario, nota, idPosicion.ToString(), conn, transaction);


                transaction.Commit();


                Utils.Log("\n\n******************Fin de la aprobación del préstamo ");


                return response;
            }
            catch (Exception ex)
            {

                transaction.Rollback();

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                response.MensajeError = "Se ha generado un error inesperado. No se pudo completar la operación. Por favor intente mas tarde.";
                response.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return response;


        }

        public static int UpdateStatusCustomer(string idCliente, string idUsuario, int idStatus,
          SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sql = @"  UPDATE cliente
                            SET id_status_cliente = @id_status_cliente
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

                throw ex;

            }


            return r;


        }

        /// <summary>
        /// Traer los pagos de un préstamo con fecha <= a este fin de semana actual 
        /// </summary>
        /// <param name="byStatus"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        [WebMethod]
        public static List<Pago> GetPaymentsByIdPrestamoAndDate(bool byStatus, string idPrestamo, string fechaFinal, SqlConnection conn, SqlTransaction transaction)
        {

            List<Pago> items = new List<Pago>();

            try
            {

                var sqlStatus = "";
                if (byStatus) // solo los del status indicado
                {
                    sqlStatus = " AND p.id_status_pago IN(1, 2, 3)  ";

                }


                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, p.monto, p.saldo, p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,                                    
                                     FORMAT(p.fecha, 'dd/MM/yyyy') fechastr
                                    FROM pago p
                                    JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)                                            
                                    WHERE p.id_prestamo = @id_prestamo AND (p.fecha <= '" + fechaFinal + @"') "
                                    + sqlStatus
                                    + " ORDER BY p.id_pago ";

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
                        item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        item.IdStatusPago = int.Parse(ds.Tables[0].Rows[i]["id_status_pago"].ToString());

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



        }

        /// <summary>
        /// Traer saldo o deuda actual de un préstamo 
        /// </summary>
        /// <param name="byStatus"></param>
        /// <param name="idPrestamo"></param>
        /// <param name="fechaFinal"></param>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        [WebMethod]
        public static Prestamo CurrentDebtByIdLoan(string idPrestamo, SqlConnection conn, SqlTransaction transaction)
        {

            Prestamo item = null;

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT IsNull(SUM(saldo), 0) saldo
                                    FROM pago                                    
                                    WHERE id_prestamo = @id_prestamo ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("id_prestamo", idPrestamo);
                adp.SelectCommand.Transaction = transaction;

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    item = new Prestamo();
                    item.Saldo = float.Parse(ds.Tables[0].Rows[0]["saldo"].ToString());
                }


            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
            }

            return item;


        }



        //  Buscar prestamos aprobados (4) del cliente
        public static Prestamo GetPrestamoByIdCliente(string idCliente, SqlConnection conn, SqlTransaction transaction)
        {

            Prestamo item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT TOP 1 p.id_prestamo, c.curp
                                FROM prestamo p JOIN cliente c ON (c.id_cliente = p.id_cliente)
                                WHERE p.id_cliente = @id_cliente AND p.id_status_prestamo IN (4) ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("idCliente =  " + idCliente);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_cliente", idCliente);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {

                    item = new Prestamo();
                    item.IdPrestamo = int.Parse(ds.Tables[0].Rows[0]["id_prestamo"].ToString());

                }



                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return item;
            }



        }

        public static int UpdateRelacionPrestamoAprobacion(string idPrestamo, string idUsuario, string nota, string idPosicion,
          SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sqlActualizaPosicion = idPosicion == Employees.POSICION_SUPERVISOR.ToString() ? ", id_supervisor = @id_supervisor " : ", id_ejecutivo = @id_ejecutivo ";


                string sql = @"  UPDATE relacion_prestamo_aprobacion
                                SET fecha = @fecha, notas_generales = @notas_generales, status_aprobacion = @status_aprobacion  "
                                + sqlActualizaPosicion
                                + @"WHERE id_prestamo = @id_prestamo AND
                                id_posicion = " + idPosicion + " ";


                Utils.Log("ACTUALIZAR RelacionPrestamoAprobacion " + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmd.Parameters.AddWithValue("@notas_generales", nota);
                cmd.Parameters.AddWithValue("@id_supervisor", idUsuario);
                cmd.Parameters.AddWithValue("@id_ejecutivo", idUsuario);
                cmd.Parameters.AddWithValue("@status_aprobacion", "Aprobado");
                cmd.Transaction = transaction;

                r += cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {


                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

                throw ex;


            }


            return r;


        }

        public static int UpdateStatusPrestamoPagado(string idPrestamo, string idUsuario, string nota, int idStatus, 
        SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sql = @"  UPDATE prestamo
                            SET activo = 0, id_status_prestamo = @id_status_prestamo,
                                notas_generales = @notas_generales, id_usuario = @id_usuario
                            WHERE
                            id_prestamo = @id_prestamo ";

                Utils.Log("\nMétodo-> " +
              System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmdUpdatePrestamo = new SqlCommand(sql, conn);
                cmdUpdatePrestamo.CommandType = CommandType.Text;

                cmdUpdatePrestamo.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmdUpdatePrestamo.Parameters.AddWithValue("@notas_generales", nota);
                cmdUpdatePrestamo.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmdUpdatePrestamo.Parameters.AddWithValue("@id_status_prestamo", idStatus);
                cmdUpdatePrestamo.Transaction = transaction;

                r += cmdUpdatePrestamo.ExecuteNonQuery();

            }
            catch (Exception ex)
            {


                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

                throw ex;


            }


            return r;


        }


        public static int UpdateStatusPrestamo(string idPrestamo, string idUsuario, string nota, float ammountWithInteres, int idStatus, float monto,
           SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sqlMontoWithinteres = "";
                if (ammountWithInteres != -1)
                {
                    sqlMontoWithinteres = " ,monto_con_interes = @monto_con_interes ";
                }

                string sql = @"  UPDATE prestamo
                            SET activo = 1, id_status_prestamo = @id_status_prestamo, fecha_aprobacion = @fecha_aprobacion " + sqlMontoWithinteres + @"
                                ,monto = @monto,
                                notas_generales = @notas_generales, id_usuario = @id_usuario
                            WHERE
                            id_prestamo = @id_prestamo ";

                Utils.Log("\nMétodo-> " +
              System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmdUpdatePrestamo = new SqlCommand(sql, conn);
                cmdUpdatePrestamo.CommandType = CommandType.Text;

                cmdUpdatePrestamo.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmdUpdatePrestamo.Parameters.AddWithValue("@notas_generales", nota);
                cmdUpdatePrestamo.Parameters.AddWithValue("@fecha_aprobacion", DateTime.Now);
                cmdUpdatePrestamo.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmdUpdatePrestamo.Parameters.AddWithValue("@monto", monto);
                cmdUpdatePrestamo.Parameters.AddWithValue("@monto_con_interes", ammountWithInteres);
                cmdUpdatePrestamo.Parameters.AddWithValue("@id_status_prestamo", idStatus);
                cmdUpdatePrestamo.Transaction = transaction;

                r += cmdUpdatePrestamo.ExecuteNonQuery();

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
        public static int UpdatePagos(string idPrestamo, string idPrestamoAdelanto, SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sql = @" UPDATE pago SET pagado = monto, saldo = 0, id_status_pago = 4, pagado_con_adelanto = 1, fecha_registro_pago = @fecha_registro_pago,
                                    id_prestamo_adelanto = @id_prestamo_adelanto
                                    WHERE pagado < monto OR saldo > 0 AND id_prestamo = @id_prestamo AND IsNull(semana_extra, 0) = 0  ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmd.Parameters.AddWithValue("@fecha_registro_pago", DateTime.Now);
                cmd.Parameters.AddWithValue("@id_prestamo_adelanto", idPrestamoAdelanto);
                cmd.Transaction = transaction;

                r = cmd.ExecuteNonQuery();

                Utils.Log("Pago actualizado -> OK ");

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
            }


            return r;

        }

        [WebMethod]
        public static int InsertarSolicitudLimiteCredito(string idPrestamo, string idUsuario, SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sql = "";

                sql = @" INSERT INTO solicitud_aumento_credito (id_prestamo, fecha, id_usuario, id_status_solicitud_aumento_credito) 
                    VALUES (@id_prestamo, @fecha, @id_usuario, @id_status_solicitud_aumento_credito) ";



                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                cmd.Parameters.AddWithValue("@id_status_solicitud_aumento_credito", SolicitudAumentoCredito.STATUS_SOLICITUD_CREADA);
                cmd.Transaction = transaction;

                r = cmd.ExecuteNonQuery();


                Utils.Log("SolicitudLimiteCredito Insertada -> OK ");


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
        public static int InsertPago(Pago item, SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sql = "";

                sql = @" INSERT INTO pago (id_prestamo, monto, fecha, id_status_pago, id_usuario, numero_semana, pagado, saldo, semana_extra, pagado_con_adelanto, id_prestamo_adelanto, es_recuperado) 
                    VALUES (@id_prestamo, @monto, @fecha, @id_status_pago, @id_usuario, @numero_semana, @pagado, @saldo, @semana_extra, 0, 0, 0) ";



                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_prestamo", item.IdPrestamo);
                cmd.Parameters.AddWithValue("@monto", Math.Round(item.Monto, 2));
                cmd.Parameters.AddWithValue("@pagado", 0);
                cmd.Parameters.AddWithValue("@saldo", Math.Round(item.Monto, 2));
                cmd.Parameters.AddWithValue("@fecha", item.Fecha);
                cmd.Parameters.AddWithValue("@numero_semana", item.NumeroSemana);
                cmd.Parameters.AddWithValue("@id_status_pago", Pago.STATUS_PAGO_PENDIENTE);
                cmd.Parameters.AddWithValue("@id_usuario", item.IdUsuario);
                cmd.Parameters.AddWithValue("@semana_extra", item.SemanaExtra);
                cmd.Transaction = transaction;

                r = cmd.ExecuteNonQuery();


                Utils.Log("Pago Insertado -> OK ");


            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
            }


            return r;

        }


        [WebMethod]
        public static List<Documento> GetDocumentsByCustomerId(string path, string idCliente, SqlConnection conn, SqlTransaction transaction)
        {

            List<Documento> items = new List<Documento>();


            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT id_documento_colaborador, 
                                    nombre, url, id_cliente, id_tipo_documento, fecha_ingreso
                                  FROM documento    
                                  WHERE id_cliente = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("idCliente =  " + idCliente);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", idCliente);
                adp.SelectCommand.Transaction = transaction;

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Documento item = new Documento();
                        item.IdDocumento = int.Parse(ds.Tables[0].Rows[i]["id_documento_colaborador"].ToString());
                        item.IdTipoDocumento = int.Parse(ds.Tables[0].Rows[i]["id_tipo_documento"].ToString());
                        item.Url = ds.Tables[0].Rows[i]["url"].ToString();
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();

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


        }

        [WebMethod]
        public static List<Documento> GetDocumentCustomerTypes(string path, SqlConnection conn, SqlTransaction transaction)
        {

            List<Documento> items = new List<Documento>();


            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT id_tipo_documento, nombre
                                  FROM tipo_documento WHERE id_tipo_documento <> 5 ";   //todos excepto antecedentes penales porque es para empleado

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Transaction = transaction;

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Documento item = new Documento();

                        item.IdTipoDocumento = int.Parse(ds.Tables[0].Rows[i]["id_tipo_documento"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();

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



        }


        public static List<string> ValidateCustomerData(Cliente prestamo)
        {

            List<string> errsList = new List<string>();

            //  Customer
            if (string.IsNullOrEmpty(prestamo.Curp))
            {
                errsList.Add("Curp cliente");
            }
            if (string.IsNullOrEmpty(prestamo.Nombre))
            {
                errsList.Add("Nombre cliente");
            }
            if (string.IsNullOrEmpty(prestamo.PrimerApellido))
            {
                errsList.Add("Primer apellido cliente");
            }

            if (string.IsNullOrEmpty(prestamo.Telefono))
            {
                errsList.Add("Teléfono cliente");
            }
            if (string.IsNullOrEmpty(prestamo.Ocupacion))
            {
                errsList.Add("Ocupación cliente");
            }
            if (string.IsNullOrEmpty(prestamo.NotaFotografiaCliente))
            {
                errsList.Add("Nota fotografía cliente");
            }




            //  customer address
            if (string.IsNullOrEmpty(prestamo.direccion.Calle))
            {
                errsList.Add("Calle cliente");
            }
            if (string.IsNullOrEmpty(prestamo.direccion.Colonia))
            {
                errsList.Add("Colonia cliente");
            }
            if (string.IsNullOrEmpty(prestamo.direccion.Municipio))
            {
                errsList.Add("Municipio cliente");
            }
            if (string.IsNullOrEmpty(prestamo.direccion.Estado))
            {
                errsList.Add("Estado cliente");
            }
            if (string.IsNullOrEmpty(prestamo.direccion.CodigoPostal))
            {
                errsList.Add("Código postal cliente");
            }

            if (string.IsNullOrEmpty(prestamo.direccion.DireccionTrabajo))
            {
                errsList.Add("Direccion trabajo cliente");
            }
            if (string.IsNullOrEmpty(prestamo.direccion.Ubicacion))
            {
                errsList.Add("Ubicacion cliente");
            }


            //  aval
            if (string.IsNullOrEmpty(prestamo.CurpAval))
            {
                errsList.Add("Curp aval");
            }
            if (string.IsNullOrEmpty(prestamo.NombreAval))
            {
                errsList.Add("Nombre aval");
            }
            if (string.IsNullOrEmpty(prestamo.PrimerApellidoAval))
            {
                errsList.Add("Primer apellido aval");
            }

            if (string.IsNullOrEmpty(prestamo.TelefonoAval))
            {
                errsList.Add("Teléfono aval");
            }
            if (string.IsNullOrEmpty(prestamo.OcupacionAval))
            {
                errsList.Add("Ocupación aval");
            }
            if (string.IsNullOrEmpty(prestamo.NotaFotografiaAval))
            {
                errsList.Add("Nota fotografía aval");
            }


            //  AVAL address
            if (string.IsNullOrEmpty(prestamo.direccionAval.Calle))
            {
                errsList.Add("Calle aval");
            }
            if (string.IsNullOrEmpty(prestamo.direccionAval.Colonia))
            {
                errsList.Add("Colonia aval");
            }
            if (string.IsNullOrEmpty(prestamo.direccionAval.Municipio))
            {
                errsList.Add("Municipio aval");
            }
            if (string.IsNullOrEmpty(prestamo.direccionAval.Estado))
            {
                errsList.Add("Estado aval");
            }
            if (string.IsNullOrEmpty(prestamo.direccionAval.CodigoPostal))
            {
                errsList.Add("Código postal aval");
            }

            if (string.IsNullOrEmpty(prestamo.direccionAval.DireccionTrabajo))
            {
                errsList.Add("Direccion trabajo aval");
            }
            if (string.IsNullOrEmpty(prestamo.direccionAval.Ubicacion))
            {
                errsList.Add("Ubicación aval");
            }




            return errsList;

        }

        /// <summary>
        /// Rechazo de un préstamo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="idPrestamo"></param>
        /// <param name="idUsuario"></param>
        /// <param name="idEmpleado"></param>
        /// <returns></returns>
        [WebMethod]
        public static DatosSalida Reject(string path, string idPrestamo, string idUsuario, string nota, string idPosicion)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<StatusPrestamo> items = new List<StatusPrestamo>();
            DatosSalida response = new DatosSalida();


            SqlTransaction transaccion = null;

            int r = 0;
            try
            {

                conn.Open();
                transaccion = conn.BeginTransaction();

                //  Validaciones



                //  Actualizar status del préstamo
                string sql = @"  UPDATE prestamo
                            SET id_status_prestamo = @id_status_prestamo, notas_generales = @notas_generales
                            WHERE
                            id_prestamo = @id_prestamo ";

                Utils.Log("Actualizar NUEVO PRESTAMO " + sql);

                SqlCommand cmdUpdatePrestamo = new SqlCommand(sql, conn);
                cmdUpdatePrestamo.CommandType = CommandType.Text;

                cmdUpdatePrestamo.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmdUpdatePrestamo.Parameters.AddWithValue("@notas_generales", nota);
                cmdUpdatePrestamo.Parameters.AddWithValue("@id_status_prestamo", Prestamo.STATUS_RECHAZADO);
                cmdUpdatePrestamo.Transaction = transaccion;
                r += cmdUpdatePrestamo.ExecuteNonQuery();

                string sqlActualizaPosicion = idPosicion == Employees.POSICION_SUPERVISOR.ToString() ? ", id_supervisor = @id_supervisor " : ", id_ejecutivo = @id_ejecutivo ";

                sql = @"  UPDATE relacion_prestamo_aprobacion
                                SET fecha = @fecha, notas_generales = @notas_generales, status_aprobacion = @status_aprobacion  "
                                + sqlActualizaPosicion
                                + @"WHERE id_prestamo = @id_prestamo AND
                                id_posicion = " + idPosicion + " ";


                Utils.Log("ACTUALIZAR RelacionPrestamoAprobacion " + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmd.Parameters.AddWithValue("@notas_generales", nota);
                cmd.Parameters.AddWithValue("@id_supervisor", idUsuario);
                cmd.Parameters.AddWithValue("@id_ejecutivo", idUsuario);
                cmd.Parameters.AddWithValue("@status_aprobacion", "Rechazado");
                cmd.Transaction = transaccion;

                r += cmd.ExecuteNonQuery();

                response.MensajeError = "";
                response.CodigoError = 0;

                transaccion.Commit();

                return response;
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                response.MensajeError = "Se ha generado un error <br/>" + ex.Message + " ... " + ex.StackTrace.ToString();
                response.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return response;


        }


        [WebMethod]
        public static TipoCliente GetCustomerTypeById(string path, string id, SqlConnection conn, SqlTransaction transaction)
        {

            TipoCliente item = new TipoCliente();

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT id_tipo_cliente as id, tipo_cliente, IsNull(prestamo_inicial_maximo, 0) prestamo_inicial_maximo, 
                                    IsNull(porcentaje_semanal, 0) porcentaje_semanal, IsNull(semanas_a_prestar, 0) semanas_a_prestar, 
                                    IsNull(garantias_por_monto, 0) garantias_por_monto,
                                    fechas_pago, IsNull(cantidad_para_renovar, 0) cantidad_para_renovar, 
                                    IsNull(semana_extra, 0) semana_extra, IsNull(fecha_pago_lunes, 0) fecha_pago_lunes, 
                                    IsNull(fecha_pago_martes, 0) fecha_pago_martes, IsNull(fecha_pago_miercoles, 0) fecha_pago_miercoles, 
                                    IsNull(fecha_pago_jueves, 0) fecha_pago_jueves, IsNull(fecha_pago_viernes, 0) fecha_pago_viernes, 
                                    IsNull(fecha_pago_sabado, 0) fecha_pago_sabado, IsNull(fecha_pago_domingo, 0) fecha_pago_domingo
                                FROM tipo_cliente
                                WHERE id_tipo_cliente =  @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id=  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", id);
                adp.SelectCommand.Transaction = transaction;

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new TipoCliente();

                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.NombreTipoCliente = (ds.Tables[0].Rows[i]["tipo_cliente"].ToString());
                        item.PrestamoInicialMaximo = float.Parse(ds.Tables[0].Rows[i]["prestamo_inicial_maximo"].ToString());
                        item.PorcentajeSemanal = float.Parse(ds.Tables[0].Rows[i]["porcentaje_semanal"].ToString());
                        item.SemanasAPrestar = int.Parse(ds.Tables[0].Rows[i]["semanas_a_prestar"].ToString());
                        item.GarantiasPorMonto = float.Parse(ds.Tables[0].Rows[i]["garantias_por_monto"].ToString());
                        item.FechasDePago = (ds.Tables[0].Rows[i]["fechas_pago"].ToString());
                        item.CantidadParaRenovar = float.Parse(ds.Tables[0].Rows[i]["cantidad_para_renovar"].ToString());
                        item.SemanasExtra = int.Parse(ds.Tables[0].Rows[i]["semana_extra"].ToString());

                        item.FechaPagoLunes = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_lunes"].ToString());
                        item.FechaPagoMartes = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_martes"].ToString());
                        item.FechaPagoMiercoles = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_miercoles"].ToString());
                        item.FechaPagoJueves = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_jueves"].ToString());
                        item.FechaPagoViernes = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_viernes"].ToString());
                        item.FechaPagoSabado = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_sabado"].ToString());
                        item.FechaPagoDomingo = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_domingo"].ToString());


                    }
                }

                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return item;
            }


        }


        public static Prestamo GetPrestamoById(string path, string idCliente, SqlConnection conn, SqlTransaction transaction)
        {

            Prestamo prestamoData = null;

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT id_prestamo, fecha_solicitud, monto, 
                                    id_status_prestamo, id_cliente, id_usuario
                                    FROM prestamo WHERE id_prestamo = @id_prestamo
                                    ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("id_prestamo", idCliente);
                adp.SelectCommand.Transaction = transaction;

                Utils.Log("\nMétodo-> " +
                        System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        prestamoData = new Prestamo();
                        prestamoData.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        prestamoData.IdStatusPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_status_prestamo"].ToString());
                        prestamoData.Color = ds.Tables[0].Rows[i]["color"].ToString();
                        prestamoData.NombreStatus = "<span class='" + prestamoData.Color + "'>" + ds.Tables[0].Rows[i]["nombre_status_prestamo"].ToString() + "</span>";
                        prestamoData.FechaSolicitud = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();
                        prestamoData.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        prestamoData.MontoFormateadoMx = prestamoData.Monto.ToString("C2");


                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return prestamoData;
            }

            return prestamoData;

        }

        [WebMethod]
        public static Cliente GetLoanDataByCustomerId(string path, string idCliente, SqlConnection conn, SqlTransaction transaction)
        {

            Cliente item = new Cliente();

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT c.id_cliente , c.nombre, c.primer_apellido, c.segundo_apellido, c.id_tipo_cliente,
                                concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                c.telefono , c.curp, c.ocupacion, c.activo, 
                                c.curp_aval, c.nombre_aval, c.primer_apellido_aval, c.segundo_apellido_aval, c.ocupacion_aval, c.telefono_aval,
                                tc.tipo_cliente, p.id_prestamo, p.monto,
                                FORMAT(fecha_solicitud, 'yyyy-MM-dd') fecha_solicitud
                                FROM cliente c 
                                JOIN tipo_cliente tc ON (tc.id_tipo_cliente = c.id_tipo_cliente) 
                                JOIN prestamo p ON (p.id_cliente = c.id_cliente) 
                                WHERE c.id_cliente = @id
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_empleado =  " + idCliente);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_cliente", idCliente);
                adp.SelectCommand.Transaction = transaction;

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Cliente();

                        item.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id_tipo_cliente"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.Curp = ds.Tables[0].Rows[i]["curp"].ToString();
                        item.Ocupacion = ds.Tables[0].Rows[i]["ocupacion"].ToString();

                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.FechaSolicitud = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();

                        item.CurpAval = ds.Tables[0].Rows[i]["curp_aval"].ToString();
                        item.NombreAval = ds.Tables[0].Rows[i]["nombre_aval"].ToString();
                        item.PrimerApellidoAval = ds.Tables[0].Rows[i]["primer_apellido_aval"].ToString();
                        item.SegundoApellidoAval = ds.Tables[0].Rows[i]["segundo_apellido_aval"].ToString();
                        item.TelefonoAval = ds.Tables[0].Rows[i]["telefono_aval"].ToString();
                        item.OcupacionAval = ds.Tables[0].Rows[i]["ocupacion_aval"].ToString();

                    }
                }





                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return item;
            }



        }

        [WebMethod]
        public static DatosSalida ApproveBySupervisor(string path, string idPrestamos, string idUsuario, string idEmpleado)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<StatusPrestamo> items = new List<StatusPrestamo>();
            DatosSalida response = new DatosSalida();


            SqlTransaction transaccion = null;

            int r = 0;
            try
            {

                conn.Open();
                transaccion = conn.BeginTransaction();

                //  Guardar prestamo
                string sql = @"  UPDATE prestamo
                            SET id_status_prestamo = @id_status_prestamo
                            WHERE
                            id_prestamo = @id_prestamo ";

                Utils.Log("Actualizar NUEVO PRESTAMO " + sql);

                SqlCommand cmdUpdatePrestamo = new SqlCommand(sql, conn);
                cmdUpdatePrestamo.CommandType = CommandType.Text;

                cmdUpdatePrestamo.Parameters.AddWithValue("@id_prestamo", idPrestamos);
                cmdUpdatePrestamo.Parameters.AddWithValue("@id_status_prestamo", Prestamo.STATUS_PENDIENTE_EJECUTIVO);
                cmdUpdatePrestamo.Transaction = transaccion;
                cmdUpdatePrestamo.ExecuteNonQuery();


                return response;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                response.MensajeError = "Se ha generado un error <br/>" + ex.Message + " ... " + ex.StackTrace.ToString();
                response.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return response;


        }




        [WebMethod]
        public static DatosSalida UpdateCustomer(string path, Cliente item, Direccion itemAddress, string accion,
            string idUsuario, string idTipoUsuario, string idPrestamo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");


            //verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            DatosSalida salida = new DatosSalida();
            SqlTransaction transaccion = null;



            int r = 0;
            try
            {

                conn.Open();
                transaccion = conn.BeginTransaction();


                string sql = "";

                sql = @"  UPDATE cliente
                                SET curp = @curp, nombre = @nombre, primer_apellido = @primer_apellido,
                                segundo_apellido = @segundo_apellido, nota_fotografia = @nota_fotografia, 
                                ocupacion = @ocupacion, telefono = @telefono 
                          WHERE
                                id_cliente = @id_cliente ";


                Utils.Log("ACTUALIZAR CLIENTE " + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@curp", item.Curp);
                cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                cmd.Parameters.AddWithValue("@primer_apellido", item.PrimerApellido);
                cmd.Parameters.AddWithValue("@segundo_apellido", item.SegundoApellido);
                cmd.Parameters.AddWithValue("@ocupacion", item.Ocupacion);
                cmd.Parameters.AddWithValue("@telefono", item.Telefono);
                cmd.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmd.Parameters.AddWithValue("@nota_fotografia", item.NotaFotografiaCliente);
                //cmd.Parameters.AddWithValue("@nota_fotografia_aval", item.NotaFotografiaAval);
                cmd.Transaction = transaccion;

                r += cmd.ExecuteNonQuery();

                //  Guardar direccion cliente
                sql = @"  UPDATE direccion
                             SET calleyno = @calleyno, colonia = @colonia, municipio = @municipio, estado = @estado,
                                codigo_postal = @codigo_postal, direccion_trabajo = @direccion_trabajo, ubicacion = @ubicacion
                            WHERE id_cliente = @id_cliente AND ISNULL(aval, 0) = 0
                        ";


                Utils.Log("ACTUALIZAR DIRECCION CLIENTE " + sql);

                SqlCommand cmdAddressEmployee = new SqlCommand(sql, conn);
                cmdAddressEmployee.CommandType = CommandType.Text;

                cmdAddressEmployee.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmdAddressEmployee.Parameters.AddWithValue("@calleyno", itemAddress.Calle);
                cmdAddressEmployee.Parameters.AddWithValue("@colonia", itemAddress.Colonia);
                cmdAddressEmployee.Parameters.AddWithValue("@municipio", itemAddress.Municipio);
                cmdAddressEmployee.Parameters.AddWithValue("@estado", itemAddress.Estado);
                cmdAddressEmployee.Parameters.AddWithValue("@codigo_postal", itemAddress.CodigoPostal);
                cmdAddressEmployee.Parameters.AddWithValue("@direccion_trabajo", itemAddress.DireccionTrabajo);
                cmdAddressEmployee.Parameters.AddWithValue("@ubicacion", itemAddress.Ubicacion);
                cmdAddressEmployee.Transaction = transaccion;

                r = cmdAddressEmployee.ExecuteNonQuery();

                Utils.Log("Guardado -> OK ");

                DatosSalida dataUpdateNotas = UpdateRelacionPrestamoAprobacion(path, idTipoUsuario, item.NotaCliente, item.NotaAval,
                    idUsuario, idPrestamo, conn, transaccion, 1);


                transaccion.Commit();


                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = item.IdCliente.ToString();

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return salida;


        }



        [WebMethod]
        public static DatosSalida UpdateCustomerAval(string path, Cliente item, Direccion itemAddressAval, string accion, string idUsuario,
            string idTipoUsuario, string idPrestamo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");


            //verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            DatosSalida salida = new DatosSalida();
            SqlTransaction transaccion = null;


            int r = 0;
            try
            {

                conn.Open();
                transaccion = conn.BeginTransaction();


                string sql = "";

                sql = @"  UPDATE cliente
                                SET curp_aval = @curp_aval, nombre_aval = @nombre_aval, primer_apellido_aval = @primer_apellido_aval, 
                                segundo_apellido_aval = @segundo_apellido_aval, ocupacion_aval = @ocupacion_aval, 
                                nota_fotografia_aval = @nota_fotografia_aval, 
                                telefono_aval = @telefono_aval 
                                WHERE
                                id_cliente = @id_cliente ";


                Utils.Log("ACTUALIZAR CLIENTE " + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;


                cmd.Parameters.AddWithValue("@curp_aval", item.CurpAval);
                cmd.Parameters.AddWithValue("@nombre_aval", item.NombreAval);
                cmd.Parameters.AddWithValue("@primer_apellido_aval", item.PrimerApellidoAval);
                cmd.Parameters.AddWithValue("@segundo_apellido_aval", item.SegundoApellidoAval);
                cmd.Parameters.AddWithValue("@telefono_aval", item.TelefonoAval);
                cmd.Parameters.AddWithValue("@ocupacion_aval", item.OcupacionAval);
                cmd.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmd.Parameters.AddWithValue("@nota_fotografia_aval", item.NotaFotografiaAval);

                cmd.Transaction = transaccion;

                r += cmd.ExecuteNonQuery();

                //  Guardar direccion aval
                sql = @"  UPDATE direccion
                             SET calleyno = @calleyno, colonia = @colonia, municipio = @municipio, estado = @estado,
                                codigo_postal = @codigo_postal, direccion_trabajo = @direccion_trabajo, ubicacion = @ubicacion
                            WHERE id_cliente = @id_cliente AND ISNULL(aval, 0) = 1
                        ";



                Utils.Log("update customer aval" + sql);

                SqlCommand cmdAddressEmployeeAval = new SqlCommand(sql, conn);
                cmdAddressEmployeeAval.CommandType = CommandType.Text;

                cmdAddressEmployeeAval.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@calleyno", itemAddressAval.Calle);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@colonia", itemAddressAval.Colonia);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@municipio", itemAddressAval.Municipio);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@estado", itemAddressAval.Estado);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@codigo_postal", itemAddressAval.CodigoPostal);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@direccion_trabajo", itemAddressAval.DireccionTrabajo);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@ubicacion", itemAddressAval.Ubicacion);

                cmdAddressEmployeeAval.Transaction = transaccion;

                r = cmdAddressEmployeeAval.ExecuteNonQuery();

                Utils.Log("Guardado -> OK ");

                DatosSalida dataUpdateNotas = UpdateRelacionPrestamoAprobacion(path, idTipoUsuario, item.NotaCliente,
                    item.NotaAval, idUsuario, idPrestamo, conn, transaccion, 2);


                transaccion.Commit();


                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = item.IdCliente.ToString();

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return salida;


        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="notas"></param>
        /// <param name="idPosicion">Es el puesto de la persona que hace el cambio, supervisor o coordinador</param>
        /// <returns></returns>
        [WebMethod]
        public static DatosSalida UpdateRelacionPrestamoAprobacion(string path, string idPosicion,
                string notaCliente, string notaAval, string idUsuario, string idPrestamo, SqlConnection conn, SqlTransaction transaction,
                int tipo)
        {
            //tipo 1 cliente, 2 aval

            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            DatosSalida salida = new DatosSalida();

            int r = 0;
            try
            {

                string sqlActualizaPosicion = idPosicion == Employees.POSICION_SUPERVISOR.ToString() ? ", id_supervisor = @id_supervisor " : ", id_ejecutivo = @id_ejecutivo ";
                string sqlActualizaNotaCliente = tipo == 1 ? ", notas_cliente = @notas_cliente " : " ";
                string sqlActualizaNotaAval = tipo == 2 ? ", notas_aval = @notas_aval " : " ";

                string sql = "";

                sql = @"  UPDATE relacion_prestamo_aprobacion
                                SET fecha = @fecha "
                                + sqlActualizaPosicion
                                + sqlActualizaNotaCliente
                                + sqlActualizaNotaAval
                                + @"
                                WHERE id_prestamo = @id_prestamo AND
                                id_posicion = " + idPosicion + " ";


                Utils.Log("ACTUALIZAR RelacionPrestamoAprobacion " + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmd.Parameters.AddWithValue("@id_supervisor", idUsuario);
                cmd.Parameters.AddWithValue("@id_ejecutivo", idUsuario);
                cmd.Parameters.AddWithValue("@notas_cliente", notaCliente == null ? "" : notaCliente);
                cmd.Parameters.AddWithValue("@notas_aval", notaAval == null ? "" : notaAval);
                cmd.Transaction = transaction;

                r += cmd.ExecuteNonQuery();


                Utils.Log("Guardado -> OK ");



                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
            }


            return salida;


        }



        /// <summary>
        /// Obtener lista de prestamos de un cliente
        /// </summary>
        /// <param name="path"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [WebMethod]
        public static List<Prestamo> GetLoansByCustomerId(string path, string customerId, SqlConnection conn, SqlTransaction transaction)
        {



            //  Lista de datos a devolver
            List<Prestamo> items = new List<Prestamo>();


            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT c.id_cliente,
                     concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                     c.telefono , c.curp, c.ocupacion, c.activo, p.id_prestamo, p.monto,
                     FORMAT(p.fecha_solicitud, 'dd/MM/yyyy') fecha_solicitud,
                     st.nombre nombre_status_prestamo, st.color
                     FROM cliente c 
                     JOIN prestamo p ON (p.id_cliente = c.id_cliente) 
                     JOIN status_prestamo st ON (st.id_status_prestamo = p.id_status_prestamo) 
                     WHERE p.id_cliente = @id_cliente ORDER BY p.id_prestamo DESC ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("id_cliente", customerId);
                adp.SelectCommand.Transaction = transaction;

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        Prestamo item = new Prestamo();
                        item.Cliente = new Cliente();
                        item.Cliente.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.Cliente.Curp = ds.Tables[0].Rows[i]["curp"].ToString();
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());

                        item.Cliente.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();

                        item.Color = ds.Tables[0].Rows[i]["color"].ToString();
                        item.NombreStatus = "<span class='" + item.Color + "'>" + ds.Tables[0].Rows[i]["nombre_status_prestamo"].ToString() + "</span>";


                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2");//moneda Mx -> $ 2.00

                        item.FechaSolicitud = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());



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



        }

        public static List<Garantia> GetListGuaranteeCustomer(string path, string idUsuario, string idPrestamo, SqlConnection conn, SqlTransaction transaction)
        {

            List<Garantia> items = new List<Garantia>();

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT id_garantia_prestamo, nombre, numero_serie, IsNull(costo, 0) costo, fotografia, 
                            FORMAT(fecha_registro, 'dd/MM/yyyy') fecha_registro
                            FROM  garantia_prestamo 
                            WHERE id_prestamo = @id_prestamo AND
                            aval = 0 AND eliminado = 0 
                            ORDER by id_garantia_prestamo";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.SelectCommand.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                adp.SelectCommand.Transaction = transaction;


                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Garantia item = new Garantia();
                        item.IdGarantia = int.Parse(ds.Tables[0].Rows[i]["id_garantia_prestamo"].ToString());

                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NumeroSerie = ds.Tables[0].Rows[i]["numero_serie"].ToString();
                        item.Costo = float.Parse(ds.Tables[0].Rows[i]["costo"].ToString());
                        item.CostoFormateadoMx = item.Costo.ToString("C2");
                        item.Fecha = ds.Tables[0].Rows[i]["fecha_registro"].ToString();
                        item.Imagen = "<img src='../../img/upload.png' class='img-fluid garantias' id='img_garantia_" + item.IdGarantia + "' data-idgarantia='" + item.IdGarantia + "' />";

                        string botones = "<a href='#'  onclick='panelGuarantee.delete(" + item.IdGarantia + ")'   class='btn btn-outline-primary boton-ocultable'> <span class='fa fa-remove mr-1'></span>Eliminar</a>";

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



        }


        [WebMethod]
        public static Empleado GetItemEmployee(string id, SqlConnection conn, SqlTransaction transaction)
        {

            Empleado item = new Empleado();

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT 
                                    id_empleado, id_tipo_usuario, id_comision_inicial, 
                                    id_posicion, id_plaza, curp, email, 
                                    nombre, primer_apellido, segundo_apellido, telefono, eliminado, 
                                    activo, IsNull(id_supervisor, 0) id_supervisor, IsNull(id_ejecutivo, 0) id_ejecutivo, 
                                    IsNull(id_coordinador, 0) id_coordinador, FORMAT(fecha_ingreso, 'yyyy-MM-dd') fecha_ingreso, 
                                    FORMAT(fecha_nacimiento, 'yyyy-MM-dd') fecha_nacimiento,
                                    monto_limite_inicial,
                                    nombre_aval, primer_apellido_aval, segundo_apellido_aval, curp_aval, telefono_aval,
                                    concat(nombre ,  ' ' , primer_apellido , ' ' , segundo_apellido) AS nombre_completo,
                                    concat(nombre_aval ,  ' ' , primer_apellido_aval , ' ' , segundo_apellido_aval) AS nombre_completo_aval
                                FROM empleado
                                WHERE id_empleado =  @id_empleado 
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_empleado =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_empleado", id);
                adp.SelectCommand.Transaction = transaction;

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Empleado();

                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        item.IdPosicion = int.Parse(ds.Tables[0].Rows[i]["id_posicion"].ToString());
                        item.IdPlaza = int.Parse(ds.Tables[0].Rows[i]["id_plaza"].ToString());
                        item.IdComisionInicial = int.Parse(ds.Tables[0].Rows[i]["id_comision_inicial"].ToString());

                        item.IdSupervisor = int.Parse(ds.Tables[0].Rows[i]["id_supervisor"].ToString());
                        item.IdEjecutivo = int.Parse(ds.Tables[0].Rows[i]["id_ejecutivo"].ToString());
                        item.IdCoordinador = int.Parse(ds.Tables[0].Rows[i]["id_coordinador"].ToString());

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.CURP = ds.Tables[0].Rows[i]["curp"].ToString();
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();
                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();
                        item.MontoLimiteInicial = float.Parse(ds.Tables[0].Rows[i]["monto_limite_inicial"].ToString());
                        item.FechaIngreso = ds.Tables[0].Rows[i]["fecha_ingreso"].ToString();
                        item.FechaNacimiento = ds.Tables[0].Rows[i]["fecha_nacimiento"].ToString();

                        item.CURPAval = ds.Tables[0].Rows[i]["curp_aval"].ToString();
                        item.NombreAval = ds.Tables[0].Rows[i]["nombre_aval"].ToString();
                        item.NombreCompletoAval = ds.Tables[0].Rows[i]["nombre_completo_aval"].ToString();
                        item.PrimerApellidoAval = ds.Tables[0].Rows[i]["primer_apellido_aval"].ToString();
                        item.SegundoApellidoAval = ds.Tables[0].Rows[i]["segundo_apellido_aval"].ToString();
                        item.TelefonoAval = ds.Tables[0].Rows[i]["telefono_aval"].ToString();



                    }
                }





                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        public static List<Garantia> GetListGuaranteeAval(string path, string idUsuario, string idPrestamo, SqlConnection conn, SqlTransaction transaction)
        {

            List<Garantia> items = new List<Garantia>();

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT id_garantia_prestamo, nombre, numero_serie, IsNull(costo, 0) costo, fotografia, 
                            FORMAT(fecha_registro, 'dd/MM/yyyy') fecha_registro
                            FROM  garantia_prestamo
                            WHERE id_prestamo = @id_prestamo AND
                            aval = 1 AND eliminado = 0 
                            ORDER by id_garantia_prestamo";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.SelectCommand.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                adp.SelectCommand.Transaction = transaction;


                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Garantia item = new Garantia();
                        item.IdGarantia = int.Parse(ds.Tables[0].Rows[i]["id_garantia_prestamo"].ToString());

                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NumeroSerie = ds.Tables[0].Rows[i]["numero_serie"].ToString();
                        item.Costo = float.Parse(ds.Tables[0].Rows[i]["costo"].ToString());
                        item.CostoFormateadoMx = item.Costo.ToString("C2");
                        item.Fecha = ds.Tables[0].Rows[i]["fecha_registro"].ToString();
                        item.Imagen = "<img src='../../img/upload.png' class='img-fluid garantias' id='img_garantia_" + item.IdGarantia + "' data-idgarantia='" + item.IdGarantia + "' />";

                        string botones = "<a href='#'  onclick='panelGuarantee.delete(" + item.IdGarantia + ")'   class='btn btn-outline-primary boton-ocultable'> <span class='fa fa-remove mr-1'></span>Eliminar</a>";

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



        }



    }



}