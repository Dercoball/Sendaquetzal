using Dapper;
using Plataforma.Clases;
using Plataforma.Extensions;
using Plataforma.pages.Controles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Services;

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

            if (!Page.IsPostBack)
            {
                hfIdPrestamo.Value = Request.QueryString["id"] == null
                    ? "0"
                    : Request.QueryString["id"].ToString();
            }
        }

        #region Privados
        static List<Documento> ObtenerDocumentos(int IdCliente, SqlConnection oConexion)
        {
            var sql = $@"SELECT id_documento_colaborador {nameof(Documento.IdDocumento)}
                                              ,nombre  {nameof(Documento.Nombre)}
                                              ,id_tipo_documento {nameof(Documento.IdTipoDocumento)}
                                              ,contenido {nameof(Documento.Contenido)}
                                              ,url {nameof(Documento.Url)}
                                              ,fecha_ingreso {nameof(Documento.Fecha)}
                                              ,id_cliente {nameof(Documento.IdCliente)}
                                              ,extension {nameof(Documento.Extension)}
            FROM documento
            WHERE id_cliente = {IdCliente}";

            return oConexion.Query<Documento>(sql)
                    .ToList() ?? new List<Documento>();
        }

        static Direccion ObtenerDetalleDireccion(int IdCliente, SqlConnection oConexion)
        {
            var sql = $@"SELECT id_direccion {nameof(Direccion.idDireccion)}
                                            ,calleyno  {nameof(Direccion.Calle)}
                                            ,colonia  {nameof(Direccion.Colonia)}
                                            ,municipio  {nameof(Direccion.Municipio)}
                                            ,estado  {nameof(Direccion.Estado)}
                                            ,codigo_postal  {nameof(Direccion.CodigoPostal)}
                                            ,direccion_trabajo  {nameof(Direccion.DireccionTrabajo)}
                                            ,id_cliente  {nameof(Direccion.IdCliente)}
                                            ,ubicacion  {nameof(Direccion.Ubicacion)}
            FROM direccion
            WHERE id_cliente = {IdCliente}";

            return oConexion.Query<Direccion>(sql)
                    .FirstOrDefault() ?? new Direccion();
        }

        static Cliente ObtenerDetalleCliente(int IdCliente,
            SqlConnection oConexion)
        {
            var sql = $@"SELECT id_cliente {nameof(Cliente.IdCliente)}
                                              ,curp {nameof(Cliente.Curp)}
                                              ,nombre {nameof(Cliente.Nombre)}
                                              ,primer_apellido {nameof(Cliente.PrimerApellido)}
                                              ,segundo_apellido {nameof(Cliente.SegundoApellido)}
                                              ,ocupacion {nameof(Cliente.Ocupacion)}
                                              ,telefono {nameof(Cliente.Telefono)}
                                              ,id_tipo_cliente {nameof(Cliente.IdTipoCliente)}
                                              ,activo {nameof(Cliente.Activo)}
                                              ,id_status_cliente {nameof(Cliente.IdStatusCliente)}
            FROM cliente
            WHERE id_cliente = {IdCliente}";

            return oConexion.Query<Cliente>(sql)
                    .FirstOrDefault() ?? new Cliente();
        }

        static Prestamo ObtenerDetallePrestamo(int idPrestamo, SqlConnection oConexion)
        {
            var sql = $@"SELECT id_prestamo {nameof(Prestamo.IdPrestamo)}
            ,fecha_solicitud {nameof(Prestamo.FechaSolicitud)}
            ,monto {nameof(Prestamo.Monto)}
            ,id_status_prestamo {nameof(Prestamo.IdStatusPrestamo)}
            ,id_cliente {nameof(Prestamo.IdCliente)}
            ,notas_generales  {nameof(Prestamo.NotasGenerales)}
            ,id_tipo_cliente {nameof(Prestamo.IdTipoCliente)}
            ,id_aval {nameof(Prestamo.IdAval)}
            ,monto_por_renovacion {nameof(Prestamo.MontoPorRenovacion)}
            ,ubicacion_confirmada {nameof(Prestamo.UbicacionConfirmada)}
            ,nota_ejecutivo  {nameof(Prestamo.NotasEjecutivo)}
            FROM prestamo
            WHERE id_prestamo = {idPrestamo}";

            return oConexion.Query<Prestamo>(sql)
                    .FirstOrDefault() ?? new Prestamo();
        }
        static void RegistrarPrestamo(Prestamo oPrestamo, SqlTransaction transaccion, SqlConnection conn)
        {
            string sql = "";

            if (oPrestamo.IdPrestamo > 0)
            {
                sql = @"UPDATE prestamo
                                SET fecha_solicitud = @fecha_solicitud,
                                      monto = @monto,
                                      monto_por_renovacion =@monto_por_renovacion
                          WHERE
                                id_prestamo = @id_prestamo ";
                Utils.Log("ACTUALIZAR PRESTAMO " + sql);
            }
            else
            {
                sql = @" INSERT INTO prestamo 
                            OUTPUT INSERTED.id_prestamo
                    VALUES (@fecha_solicitud, @monto, 1, @id_cliente, @id_usuario,NULL,NULL,1,NULL,NULL,@id_tipo_cliente,@id_aval,@monto_por_renovacion,null ,null) ";
                Utils.Log("INSERTAR PRESTAMO " + sql);
            }

            var cmd = new SqlCommand(sql, conn);
            cmd.Transaction = transaccion;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@fecha_solicitud", oPrestamo.FechaSolicitud);
            cmd.Parameters.AddWithValue("@monto", oPrestamo.Monto);
            cmd.Parameters.AddWithValue("@id_cliente", oPrestamo.IdCliente);
            cmd.Parameters.AddWithValue("@id_usuario", oPrestamo.idUsuario);
            cmd.Parameters.AddWithValue("@id_tipo_cliente", oPrestamo.IdTipoCliente);
            cmd.Parameters.AddWithValue("@id_aval", oPrestamo.IdAval);
            cmd.Parameters.AddWithValue("@monto_por_renovacion", oPrestamo.MontoPorRenovacion);

            if (oPrestamo.IdPrestamo > 0)
            {
                cmd.Parameters.AddWithValue("@id_prestamo", oPrestamo.IdPrestamo);

                var rows = cmd.ExecuteNonQuery();
            }
            else
            {
                oPrestamo.IdPrestamo = cmd.ExecuteScalar().ToString().ParseStringToInt();
            }
        }

        static void RegistraClienteAval(Cliente oCliente, SqlTransaction transaccion, SqlConnection conn)
        {
            string sql = "";

            if (oCliente.IdCliente > 0)
            {
                sql = @"UPDATE cliente
                                SET curp = @curp, nombre = @nombre, primer_apellido = @primer_apellido,
                                segundo_apellido = @segundo_apellido,
                                ocupacion = @ocupacion, telefono = @telefono 
                          WHERE
                                id_cliente = @id_cliente ";
                Utils.Log("ACTUALIZAR CLIENTE " + sql);
            }
            else
            {
                sql = @" INSERT INTO cliente (curp,nombre,primer_apellido,segundo_apellido,ocupacion,telefono,id_tipo_cliente,curp_aval,nombre_aval,primer_apellido_aval
                    ,segundo_apellido_aval,ocupacion_aval,telefono_aval,activo,eliminado,id_status_cliente,nota_fotografia,nota_fotografia_aval,mensaje)
                            OUTPUT INSERTED.id_cliente
                    VALUES (@curp, @nombre, @primer_apellido, @segundo_apellido, @ocupacion, @telefono,NULL,NULL,NULL,NULL,NULL,NULL,NULL,1,0,2,NULL,NULL, NULL) ";
                Utils.Log("INSERTAR CLIENTE " + sql);
            }

            var cmd = new SqlCommand(sql, conn);
            cmd.Transaction = transaccion;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@curp", oCliente.Curp);
            cmd.Parameters.AddWithValue("@nombre", oCliente.Nombre);
            cmd.Parameters.AddWithValue("@primer_apellido", oCliente.PrimerApellido);
            cmd.Parameters.AddWithValue("@segundo_apellido", oCliente.SegundoApellido);
            cmd.Parameters.AddWithValue("@ocupacion", oCliente.Ocupacion);
            cmd.Parameters.AddWithValue("@telefono", oCliente.Telefono);

            if (oCliente.IdCliente > 0)
            {
                cmd.Parameters.AddWithValue("@id_cliente",
                    oCliente.IdCliente);
                var rows = cmd.ExecuteNonQuery();
            }
            else
            {
                oCliente.IdCliente = cmd.ExecuteScalar().ToString().ParseStringToInt();
            }

            oCliente.direccion.IdCliente = oCliente.IdCliente;


            RegistrarDireccion(oCliente.direccion, transaccion, conn);
        }

        static void RegistrarDireccion(Direccion oDireccion, SqlTransaction transaccion, SqlConnection conn)
        {
            string sql = $"SELECT COUNT(*) FROM direccion WHERE id_cliente = {oDireccion.IdCliente}";

            var iExisteDireccion = conn.ExecuteScalar<int>(sql, transaction: transaccion);

            if (iExisteDireccion > 0)
            {
                sql = @"  UPDATE direccion
                             SET calleyno = @calleyno, 
                                    colonia = @colonia, 
                                    municipio = @municipio, 
                                    estado = @estado,
                                   codigo_postal = @codigo_postal, 
                                   direccion_trabajo = @direccion_trabajo, 
                                   ubicacion = @ubicacion
                            WHERE id_cliente = @IdCliente
                        ";

                Utils.Log("ACTUALIZAR DIRECCIÓN " + sql);
            }
            else
            {
                sql = @" INSERT INTO direccion 
                      OUTPUT INSERTED.id_direccion
                    VALUES (NULL, NULL,@calleyno, @colonia, @municipio, @estado,NULL, @codigo_postal,NULL,NULL,1,NULL, @direccion_trabajo,@IdCliente,  @ubicacion);";
                Utils.Log("INSERTAR id_direccion " + sql);
            }

            var cmd = new SqlCommand(sql, conn);
            cmd.Transaction = transaccion;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@calleyno", oDireccion.Calle);
            cmd.Parameters.AddWithValue("@colonia", oDireccion.Colonia);
            cmd.Parameters.AddWithValue("@municipio", oDireccion.Municipio);
            cmd.Parameters.AddWithValue("@estado", oDireccion.Estado);
            cmd.Parameters.AddWithValue("@codigo_postal", oDireccion.CodigoPostal);
            cmd.Parameters.AddWithValue("@direccion_trabajo", oDireccion.DireccionTrabajo);
            cmd.Parameters.AddWithValue("@ubicacion", oDireccion.Ubicacion);
            cmd.Parameters.AddWithValue("@IdCliente", oDireccion.IdCliente);

            if (iExisteDireccion > 0)
            {
                var rows = cmd.ExecuteNonQuery();
            }
            else
            {
                oDireccion.idDireccion = cmd.ExecuteScalar().ToString().ParseStringToInt();
            }
        }

        static void RegistrarDocumento(Documento oDocumento,
            int IdPrestamo,
            string sTipo,
            SqlTransaction transaccion,
            SqlConnection conn)
        {
            string sql = "";

            if (oDocumento.IdDocumento <= 0)
            {
                sql = @" INSERT INTO Documento 
                      OUTPUT INSERTED.id_documento_colaborador
                    VALUES (@nombre, @id_tipo_documento,null, 0, null, null,NULL, @fecha_ingreso,@id_cliente,  @extension);";
                Utils.Log("INSERTAR Documento " + sql);

                var cmd = new SqlCommand(sql, conn);
                cmd.Transaction = transaccion;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@nombre", oDocumento.Nombre);
                cmd.Parameters.AddWithValue("@id_tipo_documento", oDocumento.IdTipoDocumento);
                cmd.Parameters.AddWithValue("@fecha_ingreso", DateTime.Now);
                cmd.Parameters.AddWithValue("@id_cliente", oDocumento.IdCliente);
                cmd.Parameters.AddWithValue("@extension", oDocumento.Extension);
                oDocumento.IdDocumento = cmd.ExecuteScalar().ToString().ParseStringToInt();
            }

            if (oDocumento.Contenido.IndexOf('.') < 0)
            {
                var ls_Archivo = GuardarImagenServidor($"{HttpContext.Current.Server.MapPath("~/Uploads/Prestamos")}/{IdPrestamo}/{sTipo}",
                        oDocumento.Contenido,
                        oDocumento.IdDocumento);
                conn.Execute($"UPDATE Documento SET contenido ='/Uploads/Prestamos/{IdPrestamo}/{sTipo}/{ls_Archivo}' WHERE id_documento_colaborador = {oDocumento.IdDocumento}",
                    transaction: transaccion);
            }

        }
        #endregion

        #region Metodos 
        [WebMethod]
        public static object ObtenrContadoresCliente(string path, int IdCliente) {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var oConexion = new SqlConnection(strConexion);

            return new
            {
                iContadorVecesAval = oConexion.Query<int>($"SELECT COUNT(*) FROM prestamo WHERE id_aval = {IdCliente}").FirstOrDefault(),
                iContadorCompletados = oConexion.Query<int>($"SELECT COUNT(*) FROM prestamo WHERE id_cliente = {IdCliente} AND id_status_prestamo = 4").FirstOrDefault(),
                iContadorRechazados = oConexion.Query<int>($"SELECT COUNT(*) FROM prestamo WHERE id_cliente = {IdCliente} AND id_status_prestamo = 3").FirstOrDefault()
            };
        }

        [WebMethod]
        public static object BuscarClientePorCURP(string path, string sCURP)
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var oConexion = new SqlConnection(strConexion);
            var oCliente = new Cliente();

            try
            {
                var sql = $@"SELECT id_cliente {nameof(Cliente.IdCliente)}
                                              ,curp {nameof(Cliente.Curp)}
                                              ,nombre {nameof(Cliente.Nombre)}
                                              ,primer_apellido {nameof(Cliente.PrimerApellido)}
                                              ,segundo_apellido {nameof(Cliente.SegundoApellido)}
                                              ,ocupacion {nameof(Cliente.Ocupacion)}
                                              ,telefono {nameof(Cliente.Telefono)}
                                              ,id_tipo_cliente {nameof(Cliente.IdTipoCliente)}
                                              ,activo {nameof(Cliente.Activo)}
                                              ,id_status_cliente {nameof(Cliente.IdStatusCliente)}
                            FROM cliente
                            WHERE curp = '{sCURP}'";

                oCliente =  oConexion.Query<Cliente>(sql)
                        .FirstOrDefault() ?? new Cliente();

                if (oCliente.IdCliente > 0)
                {
                    oCliente.direccion = ObtenerDetalleDireccion(oCliente.IdCliente, oConexion);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }


            return oCliente;
        }

        [WebMethod]
        public static object RechazoPrestamo(string path, Prestamo oPrestamo)
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var conn = new SqlConnection(strConexion);
            var salida = new DatosSalida();

            try
            {
                conn.Open();
                var sql = @"UPDATE prestamo
                    SET ubicacion_confirmada = @ubicacion_confirmada,
                           notas_generales = @notas_generales,
                           id_status_prestamo = 3
                    where id_prestamo  = @id_prestamo";

                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ubicacion_confirmada", oPrestamo.UbicacionConfirmada);
                cmd.Parameters.AddWithValue("@notas_generales", oPrestamo.NotasGenerales);
                cmd.Parameters.AddWithValue("@id_prestamo", oPrestamo.IdPrestamo);
                var rows = cmd.ExecuteNonQuery();

                Utils.Log("RechazoPrestamo -> OK ");
                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = String.Empty;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
            }

            return salida;
        }

        [WebMethod]
        public static object AprobacionEjecutivo(string path,
            string sNotaEjecutivo,
            int IdPrestamo)
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var conn = new SqlConnection(strConexion);
            var salida = new DatosSalida();

            try
            {
                conn.Open();
                var sql = @"UPDATE prestamo
                    SET  nota_ejecutivo = @nota_ejecutivo,
                           id_status_prestamo = 4
                    where id_prestamo  = @id_prestamo";

                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@nota_ejecutivo", sNotaEjecutivo);
                cmd.Parameters.AddWithValue("@id_prestamo", IdPrestamo);
                var rows = cmd.ExecuteNonQuery();

                Utils.Log("Aprobación Supervisor -> OK ");
                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = String.Empty;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
            }

            return salida;
        }

        [WebMethod]
        public static object AprobacionSupervisor(string path,
            Prestamo oPrestamo,
            float fMontoGarantia)
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var conn = new SqlConnection(strConexion);
            var salida = new DatosSalida();

            try
            {
                conn.Open();

                var oTipoCliente = conn.Query<TipoCliente>($"SELECT garantias_por_monto GarantiasPorMonto  FROM  tipo_cliente WHERE id_tipo_cliente = {oPrestamo.IdTipoCliente}")
                    .FirstOrDefault() ?? new TipoCliente();

                if (fMontoGarantia < oTipoCliente.GarantiasPorMonto)
                {
                    salida.MensajeError = $"El monto de garantia mínimo es de {oTipoCliente.GarantiasPorMonto.ToString("C2")}";
                    salida.CodigoError = 2;

                    return salida;
                }

                var sql = @"UPDATE prestamo
                    SET ubicacion_confirmada = @ubicacion_confirmada,
                           notas_generales = @notas_generales,
                           id_status_prestamo = 2
                    where id_prestamo  = @id_prestamo";

                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ubicacion_confirmada", oPrestamo.UbicacionConfirmada);
                cmd.Parameters.AddWithValue("@notas_generales", oPrestamo.NotasGenerales);
                cmd.Parameters.AddWithValue("@id_prestamo", oPrestamo.IdPrestamo);
                var rows = cmd.ExecuteNonQuery();

                Utils.Log("Aprobación Supervisor -> OK ");
                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = String.Empty;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
            }

            return salida;
        }

        /// <summary>
        /// Detalle del prestamo
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="path"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [WebMethod]
        public static object DetallePrestamo(int Id, string path, string idUsuario)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var conn = new SqlConnection(strConexion);

            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            //verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            var oResponsePrestamo = new PrestamoRequest();
            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            try
            {
                oResponsePrestamo.Prestamo = ObtenerDetallePrestamo(Id, conn);
                oResponsePrestamo.Cliente = ObtenerDetalleCliente
                    (oResponsePrestamo.Prestamo.IdCliente.ParseStringToInt(), conn);
                oResponsePrestamo.Aval = ObtenerDetalleCliente
                    (oResponsePrestamo.Prestamo.IdAval, conn);
                oResponsePrestamo.Cliente.direccion = ObtenerDetalleDireccion(oResponsePrestamo.Prestamo.IdCliente.ParseStringToInt(),
                    conn);
                oResponsePrestamo.Aval.direccion = ObtenerDetalleDireccion(oResponsePrestamo.Aval.IdCliente,
                    conn);
                oResponsePrestamo.DocumentosAval = ObtenerDocumentos(oResponsePrestamo.Prestamo.IdCliente.ParseStringToInt(),
                    conn);
                oResponsePrestamo.DocumentosCliente = ObtenerDocumentos(oResponsePrestamo.Aval.IdCliente,
                    conn);
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
            }

            return oResponsePrestamo;
        }


        /// <summary>
        /// Listado de garantias
        /// </summary>
        /// <param name="path"></param>
        /// <param name="idPrestamo"></param>
        /// <returns></returns>
        [WebMethod]
        public static List<Garantia> ListadoGarantias(string path, int idPrestamo)
        {
            var oListadoGarantias = new List<Garantia>();
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var conn = new SqlConnection(strConexion);

            oListadoGarantias = conn.Query<Garantia>($"SELECT * FROM garantia_prestamo WHERE id_prestamo = {idPrestamo}").ToList() ?? new List<Garantia>();

            return oListadoGarantias;
        }


        /// <summary>
        /// Nueva garantia
        /// </summary>
        /// <param name="path"></param>
        /// <param name="oGarantia"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [WebMethod]
        public static object SaveGarantia(string path,
            Garantia oGarantia,
            string idUsuario)
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var conn = new SqlConnection(strConexion);

            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            //verificar que tenga permisos para usar esta pagina
            var tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            var salida = new DatosSalida();
            string sql = "";

            if (oGarantia.id_garantia_prestamo > 0)
            {
                sql = @"  UPDATE garantia_prestamo
                             SET numero_serie = @numero_serie
                                    costo = @costo,
                                    nombre = @nombre
                            WHERE id_garantia_prestamo = @id_garantia_prestamo
                        ";

                Utils.Log("ACTUALIZAR Documento " + sql);
            }
            else
            {
                sql = @" INSERT INTO garantia_prestamo 
                      OUTPUT INSERTED.id_garantia_prestamo
                    VALUES (@id_prestamo, @nombre,@numero_serie,@costo,NULL, GETDATE(), 0,@id_usuario);";
                Utils.Log("INSERTAR Documento " + sql);
            }
            try
            {
                conn.Open();
                var cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id_prestamo", oGarantia.id_prestamo);
                cmd.Parameters.AddWithValue("@nombre", oGarantia.nombre);
                cmd.Parameters.AddWithValue("@numero_serie", oGarantia.numero_serie);
                cmd.Parameters.AddWithValue("@costo", oGarantia.costo);
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                if (oGarantia.id_garantia_prestamo > 0)
                {
                    cmd.Parameters.AddWithValue("@id_garantia_prestamo", oGarantia.id_garantia_prestamo);
                    var rows = cmd.ExecuteNonQuery();
                }
                else
                {
                    oGarantia.id_garantia_prestamo = cmd.ExecuteScalar().ToString().ParseStringToInt();
                }

                if (oGarantia.fotografia.IndexOf('.') < 0)
                {
                    var ls_Archivo = GuardarImagenServidor($"{HttpContext.Current.Server.MapPath("~/Uploads/Prestamos")}/{oGarantia.id_prestamo}",
                            oGarantia.fotografia,
                            oGarantia.id_garantia_prestamo);
                    conn.Execute($"UPDATE garantia_prestamo SET fotografia ='{ls_Archivo}' WHERE id_garantia_prestamo = {oGarantia.id_garantia_prestamo}");
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
            }
            finally
            {
                conn.Close();
            }

            return salida;
        }

        static string GuardarImagenServidor(string sDirectorioPath,
            string sFotografia,
            int Id)
        {
            var ls_Extension = sFotografia.Split(';')[0].Split('/')[1];
            var ls_Archivo = string.Empty;

            try
            {
                //Agregamos la imagen de la garantia
                if (!Directory.Exists(sDirectorioPath))
                {
                    Directory.CreateDirectory(sDirectorioPath);
                }

                //Agregamos el archivo
                ls_Archivo = $"{Id}.{ls_Extension}";
                var ls_RutaArchivo = $"{sDirectorioPath}/{ls_Archivo}";

                if (!File.Exists(ls_RutaArchivo))
                {
                    var larr_fotografia = Convert.FromBase64String(sFotografia.Split(';')[1].Split(',')[1]);
                    using (var imageFile = new FileStream(ls_RutaArchivo, FileMode.Create))
                    {
                        imageFile.Write(larr_fotografia, 0, larr_fotografia.Length);
                        imageFile.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return ls_Archivo;
        }


        /// <summary>
        /// Borra un datod e la garantia
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [WebMethod]
        public static object DeleteGarantia(int Id, string path)
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var conn = new SqlConnection(strConexion);
            var salida = new DatosSalida();

            try
            {
                conn.Open();
                conn.Execute($"DELETE garantia_prestamo  where id_garantia_prestamo  = {Id}");
                Utils.Log("Guardado -> OK ");
                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = String.Empty;

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
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
        /// Metodo para poder salvar los datos del cliente, aval y los datos del prestamo
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Request"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [WebMethod]
        public static DatosSalida SaveCustomerOrAval(string path,
            PrestamoRequest Request,
            string idUsuario)
        {

            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var conn = new SqlConnection(strConexion);

            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            //verificar que tenga permisos para usar esta pagina
            var tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            var salida = new DatosSalida();
            SqlTransaction transaccion = null;

            try
            {
                conn.Open();
                transaccion = conn.BeginTransaction();

                RegistraClienteAval(Request.Cliente, transaccion, conn);
                RegistraClienteAval(Request.Aval, transaccion, conn);
                Request.Prestamo.idUsuario = idUsuario.ParseStringToInt();
                Request.Prestamo.IdCliente = Request.Cliente.IdCliente.ToString();
                Request.Prestamo.IdAval = Request.Aval.IdCliente;

                RegistrarPrestamo(Request.Prestamo, transaccion, conn);

                if (Request.Cliente.IdCliente > 0)
                {
                    Request.DocumentosCliente
                        .ForEach(f =>
                        {
                            f.Extension = System.IO.Path.GetExtension(f.Nombre);
                            f.IdCliente = Request.Cliente.IdCliente;
                            RegistrarDocumento(f, Request.Prestamo.IdPrestamo, "Cliente", transaccion, conn);
                        });
                }

                if (Request.Aval.IdCliente > 0)
                {
                    Request.DocumentosAval
                        .ForEach(f =>
                        {
                            f.Extension = System.IO.Path.GetExtension(f.Nombre);
                            f.IdCliente = Request.Aval.IdCliente;
                            RegistrarDocumento(f, Request.Prestamo.IdPrestamo, "Aval", transaccion, conn);
                        });
                }

                Utils.Log("Guardado -> OK ");
                transaccion.Commit();
                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = String.Empty;

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
                transaccion.Rollback();
            }
            finally
            {
                conn.Close();
            }

            return salida;
        }
        #endregion

    }
}