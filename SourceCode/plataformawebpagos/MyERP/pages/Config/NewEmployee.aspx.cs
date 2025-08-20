using Dapper;
using Newtonsoft.Json.Linq;
using Plataforma.Clases;
using Plataforma.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace Plataforma.pages.Config
{
    public partial class NewEmployee : System.Web.UI.Page
    {
        const string pagina = "8";

        public const int POSICION_DIRECTOR = 1;
        public const int POSICION_COORDINADOR = 2;
        public const int POSICION_EJECUTIVO = 3;
        public const int POSICION_SUPERVISOR = 4;
        public const int POSICION_PROMOTOR = 5;
        public const int SUPERUSUARIO = 6;

        protected void Page_Load(object sender, EventArgs e)
        {
            string usuario = (string)Session["usuario"];
            string idTipoUsuario = (string)Session["id_tipo_usuario"];
            string idUsuario = (string)Session["id_usuario"];
            string path = (string)Session["path"];

            txtUsuario.Value = usuario ?? string.Empty;
            txtIdTipoUsuario.Value = idTipoUsuario ?? string.Empty;
            txtIdUsuario.Value = idUsuario ?? string.Empty;
            txtPath.Value = path ?? string.Empty;

            //  si no esta logueado
            if (string.IsNullOrEmpty(usuario))
            {
                Response.Redirect(ResolveUrl("~/pages/Login.aspx"), false);
                return;
            }

            if (!IsPostBack)
            {
                var idEdit = Request.QueryString["id"];
                txtIdEmpleado.Value = string.IsNullOrEmpty(idEdit) ? "0" : idEdit;
            }
        }

        #region Helpers consulta/validación

        static Usuario GetUsuarioByLogin(string login, SqlConnection conn, SqlTransaction transaction)
        {
            const string query = @" SELECT TOP 1 id_usuario, id_empleado, id_tipo_usuario, nombre, login, password, email, telefono 
                                    FROM usuario 
                                    WHERE login = @login AND ISNULL(eliminado,0)=0 ";

            return conn.QueryFirstOrDefault<Usuario>(query, new { login }, transaction);
        }

        static int CountEmpleadoByCurp(string curp, SqlConnection conn, SqlTransaction tx, int? excludeIdEmpleado = null)
        {
            const string sql = @"SELECT COUNT(*) FROM empleado 
                                 WHERE UPPER(curp) = UPPER(@curp) 
                                 AND ISNULL(eliminado,0)=0
                                 AND (@exclude IS NULL OR id_empleado <> @exclude)";
            return conn.ExecuteScalar<int>(sql, new { curp, exclude = excludeIdEmpleado }, tx);
        }

        static int CountClienteByCurp(string curp, SqlConnection conn, SqlTransaction tx, int? excludeIdCliente = null)
        {
            const string sql = @"SELECT COUNT(*) FROM cliente 
                                 WHERE UPPER(curp) = UPPER(@curp) 
                                 AND ISNULL(eliminado,0)=0
                                 AND (@exclude IS NULL OR id_cliente <> @exclude)";
            return conn.ExecuteScalar<int>(sql, new { curp, exclude = excludeIdCliente }, tx);
        }

        static int CountUsuarioLoginUsadoPorOtro(string login, int idEmpleadoActual, SqlConnection conn, SqlTransaction tx)
        {
            const string sql = @"SELECT COUNT(*) FROM usuario 
                                 WHERE UPPER(login) = UPPER(@login)
                                 AND ISNULL(eliminado,0)=0
                                 AND ISNULL(id_empleado,0) <> @idEmpleado";
            return conn.ExecuteScalar<int>(sql, new { login, idEmpleado = idEmpleadoActual }, tx);
        }

        #endregion

        #region Registrar (INSERT)

        static void RegistrarUsuario(Usuario usuario, SqlTransaction transaccion, SqlConnection conn)
        {
            const string sql = @"
                INSERT INTO usuario (id_empleado, login, password, id_tipo_usuario, eliminado)
                OUTPUT INSERTED.id_usuario
                VALUES (@id_empleado, @login, @password, @id_tipo_usuario, 0);";

            string hash = null;
            if (!string.IsNullOrWhiteSpace(usuario.Password))
            {
                using (var md5Hash = MD5.Create())
                {
                    hash = Usuarios.GetMd5Hash(md5Hash, usuario.Password);
                }
            }
            var id = conn.ExecuteScalar<int>(sql, new
            {
                id_empleado = usuario.IdEmpleado,
                login = usuario.Login,
                password = hash,
                id_tipo_usuario = usuario.IdTipoUsuario
            }, transaccion);

            usuario.IdUsuario = id;
        }

        static void RegistrarEmpleado(Empleado oEmpleado, SqlTransaction transaccion, SqlConnection conn)
        {
            const string sql = @"
                INSERT INTO empleado (
                    id_tipo_usuario, id_comision_inicial, id_posicion, id_plaza, curp, email, nombre,
                    primer_apellido, segundo_apellido, telefono, eliminado, fecha_nacimiento, activo,
                    id_supervisor, id_ejecutivo, fecha_ingreso, nombre_aval, primer_apellido_aval,
                    segundo_apellido_aval, curp_aval, telefono_aval, monto_limite_inicial, id_coordinador,
                    limite_venta_ejercicio, limite_incremento_ejercicio, fizcalizable, ocupacion, nota_foto, fecha_baja
                )
                VALUES (
                    @id_tipo_usuario, 0, @id_posicion, @id_plaza, @curp, NULL, @nombre,
                    @primer_apellido, @segundo_apellido, @telefono, 0, NULL, 1,
                    @id_supervisor, @id_ejecutivo, @fecha_ingreso, NULL, NULL,
                    NULL, NULL, NULL, NULL, NULL,
                    @limite_venta_ejercicio, @limite_incremento_ejercicio, @fizcalizable, @ocupacion, @nota_foto, @fecha_baja
                );
                SELECT SCOPE_IDENTITY();";

            var idEmpleado = Convert.ToInt32(conn.ExecuteScalar(sql, new
            {
                id_tipo_usuario = oEmpleado.IdPosicion, // si tu modelo usa IdTipoUsuario cámbialo aquí
                id_posicion = oEmpleado.IdPosicion,
                id_plaza = oEmpleado.IdPlaza,
                curp = (object)oEmpleado.CURP ?? DBNull.Value,
                nombre = (object)oEmpleado.Nombre ?? DBNull.Value,
                primer_apellido = (object)oEmpleado.PrimerApellido ?? DBNull.Value,
                segundo_apellido = (object)oEmpleado.SegundoApellido ?? DBNull.Value,
                telefono = (object)oEmpleado.Telefono ?? DBNull.Value,
                fecha_ingreso = (object)oEmpleado.FechaIngreso ?? DBNull.Value,
                limite_venta_ejercicio = (object)oEmpleado.limite_venta_ejercicio ?? DBNull.Value,
                limite_incremento_ejercicio = (object)oEmpleado.limite_incremento_ejercicio ?? DBNull.Value,
                fizcalizable = (object)oEmpleado.fizcalizable ?? DBNull.Value,
                ocupacion = (object)oEmpleado.Ocupacion ?? DBNull.Value,
                nota_foto = (object)oEmpleado.NotaFoto ?? DBNull.Value,
                id_supervisor = (object)oEmpleado.IdSupervisor ?? DBNull.Value,
                id_ejecutivo = (object)oEmpleado.IdEjecutivo ?? DBNull.Value,
                fecha_baja = (object)oEmpleado.FechaBaja ?? DBNull.Value
            }, transaccion));

            oEmpleado.IdEmpleado = idEmpleado;

            // Dirección del empleado
            if (oEmpleado.Direccion != null)
            {
                oEmpleado.Direccion.IdCliente = null;                 // <-- clave
                oEmpleado.Direccion.IdEmpleado = oEmpleado.IdEmpleado;
                RegistrarDireccion(oEmpleado.Direccion, transaccion, conn);
            }
        }

        static void RegistraClienteAval(Cliente oCliente, SqlTransaction transaccion, SqlConnection conn)
        {
            string sql;

            if (oCliente.IdCliente > 0)
            {
                sql = @"UPDATE cliente
                        SET curp = @curp, nombre = @nombre, primer_apellido = @primer_apellido,
                            segundo_apellido = @segundo_apellido, ocupacion = @ocupacion, telefono = @telefono 
                        WHERE id_cliente = @id_cliente ";
                conn.Execute(sql, new
                {
                    curp = oCliente.Curp,
                    nombre = oCliente.Nombre,
                    primer_apellido = oCliente.PrimerApellido,
                    segundo_apellido = oCliente.SegundoApellido,
                    ocupacion = oCliente.Ocupacion,
                    telefono = oCliente.Telefono,
                    id_cliente = oCliente.IdCliente
                }, transaccion);
            }
            else
            {
                sql = @"INSERT INTO cliente 
                        (curp, nombre, primer_apellido, segundo_apellido, ocupacion, telefono,
                         /* campos omitidos -> */ email, rfc, fecha_nacimiento, genero, estado_civil, escolaridad,
                         ingreso_mensual, activo, eliminado, id_tipo_cliente, fecha_alta, fecha_baja, nota)
                        OUTPUT INSERTED.id_cliente
                        VALUES (@curp, @nombre, @primer_apellido, @segundo_apellido, @ocupacion, @telefono,
                                NULL,NULL,NULL,NULL,NULL,NULL,
                                NULL,1,0,2,GETDATE(),NULL,NULL) ";

                oCliente.IdCliente = conn.ExecuteScalar<string>(sql, new
                {
                    curp = oCliente.Curp,
                    nombre = oCliente.Nombre,
                    primer_apellido = oCliente.PrimerApellido,
                    segundo_apellido = oCliente.SegundoApellido,
                    ocupacion = oCliente.Ocupacion,
                    telefono = oCliente.Telefono
                }, transaccion).ParseStringToInt();
            }

            if (oCliente.direccion != null)
            {
                oCliente.direccion.IdCliente = oCliente.IdCliente; // siempre
                                                                   // oCliente.direccion.IdEmpleado viene seteado desde Save para alta
                RegistrarDireccion(oCliente.direccion, transaccion, conn);
            }
        }

        static void RegistrarDireccion(Direccion oDireccion, SqlTransaction transaccion, SqlConnection conn)
        {
            // coherencia: si hay IdEmpleado usamos IdEmpleado; si no, IdCliente
            // Preferir CLIENTE si viene; si no, EMPLEADO
            bool usarCliente = oDireccion.IdCliente.HasValue && oDireccion.IdCliente.Value > 0;
            bool usarEmpleado = !usarCliente && oDireccion.IdEmpleado.HasValue && oDireccion.IdEmpleado.Value > 0;

            string sqlCheck = usarCliente
                ? "SELECT COUNT(*) FROM direccion WHERE id_cliente = @id"
                : "SELECT COUNT(*) FROM direccion WHERE id_empleado = @id";

            int existe = conn.ExecuteScalar<int>(sqlCheck, new { id = usarCliente ? (object)oDireccion.IdCliente : (object)oDireccion.IdEmpleado }, transaccion /*o tx*/);

            if (existe > 0)
            {
                string sqlUpdate = usarCliente
                    ? @"UPDATE direccion
            SET calleyno=@calleyno, colonia=@colonia, municipio=@municipio, estado=@estado,
                codigo_postal=@codigo_postal, direccion_trabajo=@direccion_trabajo, ubicacion=@ubicacion
          WHERE id_cliente=@idCliente"
                    : @"UPDATE direccion
            SET calleyno=@calleyno, colonia=@colonia, municipio=@municipio, estado=@estado,
                codigo_postal=@codigo_postal, direccion_trabajo=@direccion_trabajo, ubicacion=@ubicacion
          WHERE id_empleado=@idEmpleado";

                conn.Execute(sqlUpdate, new
                {
                    calleyno = (object)oDireccion.Calle ?? DBNull.Value,
                    colonia = (object)oDireccion.Colonia ?? DBNull.Value,
                    municipio = (object)oDireccion.Municipio ?? DBNull.Value,
                    estado = (object)oDireccion.Estado ?? DBNull.Value,
                    codigo_postal = (object)oDireccion.CodigoPostal ?? DBNull.Value,
                    direccion_trabajo = (object)oDireccion.DireccionTrabajo ?? DBNull.Value,
                    ubicacion = (object)oDireccion.Ubicacion ?? DBNull.Value,
                    idEmpleado = oDireccion.IdEmpleado,
                    idCliente = oDireccion.IdCliente
                }, transaccion /*o tx*/);
            }
            else
            {
                const string sqlInsert = @"
        INSERT INTO direccion (
            id_empleado, id_aval, calleyno, colonia, municipio, estado,
            telefono, codigo_postal, id_municipio, id_estado, activo, aval,
            direccion_trabajo, id_cliente, ubicacion
        )
        OUTPUT INSERTED.id_direccion
        VALUES (
            @id_empleado, NULL, @calleyno, @colonia, @municipio, @estado,
            NULL, @codigo_postal, NULL, NULL, 1, NULL,
            @direccion_trabajo, @id_cliente, @ubicacion
        );";

                oDireccion.idDireccion = conn.ExecuteScalar<int>(sqlInsert, new
                {
                    id_empleado = (object)oDireccion.IdEmpleado ?? DBNull.Value,
                    id_cliente = (object)oDireccion.IdCliente ?? DBNull.Value,
                    calleyno = (object)oDireccion.Calle ?? DBNull.Value,
                    colonia = (object)oDireccion.Colonia ?? DBNull.Value,
                    municipio = (object)oDireccion.Municipio ?? DBNull.Value,
                    estado = (object)oDireccion.Estado ?? DBNull.Value,
                    codigo_postal = (object)oDireccion.CodigoPostal ?? DBNull.Value,
                    direccion_trabajo = (object)oDireccion.DireccionTrabajo ?? DBNull.Value,
                    ubicacion = (object)oDireccion.Ubicacion ?? DBNull.Value
                }, transaccion /*o tx*/);
            }

        }

        #endregion

        #region Actualizar (UPDATE)

        static void ActualizarEmpleado(Empleado oEmpleado, SqlTransaction tx, SqlConnection conn)
        {
            const string sql = @"
                UPDATE empleado SET
                    id_tipo_usuario = @id_tipo_usuario,
                    id_posicion = @id_posicion,
                    id_plaza = @id_plaza,
                    curp = @curp,
                    nombre = @nombre,
                    primer_apellido = @primer_apellido,
                    segundo_apellido = @segundo_apellido,
                    telefono = @telefono,
                    id_supervisor = @id_supervisor,
                    id_ejecutivo = @id_ejecutivo,
                    fecha_ingreso = @fecha_ingreso,
                    limite_venta_ejercicio = @limite_venta_ejercicio,
                    limite_incremento_ejercicio = @limite_incremento_ejercicio,
                    fizcalizable = @fizcalizable,
                    ocupacion = @ocupacion,
                    nota_foto = @nota_foto,
                    fecha_baja = @fecha_baja
                WHERE id_empleado = @id_empleado";

            conn.Execute(sql, new
            {
                id_tipo_usuario = oEmpleado.IdPosicion, // ajusta si corresponde
                id_posicion = oEmpleado.IdPosicion,
                id_plaza = oEmpleado.IdPlaza,
                curp = oEmpleado.CURP,
                nombre = oEmpleado.Nombre,
                primer_apellido = oEmpleado.PrimerApellido,
                segundo_apellido = oEmpleado.SegundoApellido,
                telefono = oEmpleado.Telefono,
                id_supervisor = oEmpleado.IdSupervisor,
                id_ejecutivo = oEmpleado.IdEjecutivo,
                fecha_ingreso = (object)oEmpleado.FechaIngreso ?? DBNull.Value,
                limite_venta_ejercicio = (object)oEmpleado.limite_venta_ejercicio ?? DBNull.Value,
                limite_incremento_ejercicio = (object)oEmpleado.limite_incremento_ejercicio ?? DBNull.Value,
                fizcalizable = (object)oEmpleado.fizcalizable ?? DBNull.Value,
                ocupacion = oEmpleado.Ocupacion,
                nota_foto = oEmpleado.NotaFoto,
                fecha_baja = (object)oEmpleado.FechaBaja ?? DBNull.Value,
                id_empleado = oEmpleado.IdEmpleado
            }, tx);

            if (oEmpleado.Direccion != null)
            {
                oEmpleado.Direccion.IdEmpleado = oEmpleado.IdEmpleado;
                ActualizarDireccion(oEmpleado.Direccion, tx, conn);
            }
        }

        static void ActualizarUsuario(Usuario usuario, SqlTransaction tx, SqlConnection conn)
        {
            // Si viene Password vacío, no lo tocamos
            string hash = null;
            if (!string.IsNullOrWhiteSpace(usuario.Password))
            {
                using (var md5Hash = MD5.Create())
                {
                    hash = Usuarios.GetMd5Hash(md5Hash, usuario.Password);
                }
            }

            // ¿Existe usuario ligado a este empleado?
            const string sqlExists = "SELECT COUNT(*) FROM usuario WHERE id_empleado = @id AND ISNULL(eliminado,0)=0";
            int existe = conn.ExecuteScalar<int>(sqlExists, new { id = usuario.IdEmpleado }, tx);

            if (existe > 0)
            {
                string sqlUpdate = @"
                    UPDATE usuario
                    SET login = @login,
                        id_tipo_usuario = @id_tipo_usuario
                        /**password**/
                    WHERE id_empleado = @id_empleado";

                if (!string.IsNullOrWhiteSpace(hash))
                {
                    sqlUpdate = sqlUpdate.Replace("/**password**/", ", password = @password");
                }
                else
                {
                    sqlUpdate = sqlUpdate.Replace("/**password**/", "");
                }

                conn.Execute(sqlUpdate, new
                {
                    login = usuario.Login,
                    id_tipo_usuario = usuario.IdTipoUsuario,
                    password = hash,
                    id_empleado = usuario.IdEmpleado
                }, tx);
            }
            else
            {
                // Si no existe, lo creamos (caso de alta retroactiva)
                RegistrarUsuario(usuario, tx, conn);
            }
        }

        static void ActualizarClienteAval(Cliente oCliente, SqlTransaction tx, SqlConnection conn)
        {
            if (oCliente == null) return;

            // Si no existe IdCliente intentamos encontrarlo por JOIN con direccion del empleado
            if (oCliente.IdCliente <= 0)
            {
                const string find = @"
                    SELECT TOP 1 c.id_cliente
                    FROM cliente c
                    INNER JOIN direccion d ON d.id_cliente = c.id_cliente
                    WHERE d.id_empleado = @idEmpleado
                    ORDER BY c.id_cliente DESC";
                int idCli = conn.ExecuteScalar<int?>(find, new { idEmpleado = oCliente.direccion?.IdEmpleado }, tx) ?? 0;
                oCliente.IdCliente = idCli;
            }

            if (oCliente.IdCliente > 0)
            {
                const string up = @"UPDATE cliente
                                    SET curp = @curp, nombre = @nombre, primer_apellido = @primer_apellido,
                                        segundo_apellido = @segundo_apellido, ocupacion = @ocupacion, telefono = @telefono
                                    WHERE id_cliente = @id_cliente";
                conn.Execute(up, new
                {
                    curp = oCliente.Curp,
                    nombre = oCliente.Nombre,
                    primer_apellido = oCliente.PrimerApellido,
                    segundo_apellido = oCliente.SegundoApellido,
                    ocupacion = oCliente.Ocupacion,
                    telefono = oCliente.Telefono,
                    id_cliente = oCliente.IdCliente
                }, tx);
            }
            else
            {
                // Crear si no existía
                RegistraClienteAval(oCliente, tx, conn);
            }

            if (oCliente.direccion != null)
            {
                oCliente.direccion.IdCliente = oCliente.IdCliente;
                ActualizarDireccion(oCliente.direccion, tx, conn);
            }
        }

        static void ActualizarDireccion(Direccion oDireccion, SqlTransaction tx, SqlConnection conn)
        {
            // Igual que en registrar, usamos la clave disponible
            bool esDireccionEmpleado = oDireccion.IdEmpleado.HasValue && oDireccion.IdEmpleado.Value > 0;

            string sqlCheck = esDireccionEmpleado
                ? "SELECT COUNT(*) FROM direccion WHERE id_empleado = @id"
                : "SELECT COUNT(*) FROM direccion WHERE id_cliente = @id";

            int existe = conn.ExecuteScalar<int>(sqlCheck, new { id = esDireccionEmpleado ? oDireccion.IdEmpleado : oDireccion.IdCliente }, tx);

            if (existe > 0)
            {
                string sqlUpdate = esDireccionEmpleado
                    ? @"UPDATE direccion
                       SET calleyno = @calleyno, colonia = @colonia, municipio = @municipio, estado = @estado,
                           codigo_postal = @codigo_postal, direccion_trabajo = @direccion_trabajo, ubicacion = @ubicacion
                       WHERE id_empleado = @idEmpleado"
                    : @"UPDATE direccion
                       SET calleyno = @calleyno, colonia = @colonia, municipio = @municipio, estado = @estado,
                           codigo_postal = @codigo_postal, direccion_trabajo = @direccion_trabajo, ubicacion = @ubicacion
                       WHERE id_cliente = @idCliente";

                conn.Execute(sqlUpdate, new
                {
                    calleyno = (object)oDireccion.Calle ?? DBNull.Value,
                    colonia = (object)oDireccion.Colonia ?? DBNull.Value,
                    municipio = (object)oDireccion.Municipio ?? DBNull.Value,
                    estado = (object)oDireccion.Estado ?? DBNull.Value,
                    codigo_postal = (object)oDireccion.CodigoPostal ?? DBNull.Value,
                    direccion_trabajo = (object)oDireccion.DireccionTrabajo ?? DBNull.Value,
                    ubicacion = (object)oDireccion.Ubicacion ?? DBNull.Value,
                    idEmpleado = oDireccion.IdEmpleado,
                    idCliente = oDireccion.IdCliente
                }, tx);
            }
            else
            {
                RegistrarDireccion(oDireccion, tx, conn);
            }
        }

        #endregion

        #region WebMethods (Guardar / Cargar / Listas)

        [WebMethod]
        public static EmpleadoRequest GetEmpleadoById(string path, int idEmpleado)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            using (var conn = new SqlConnection(strConexion))
            {
                conn.Open();

                var emp = conn.QueryFirstOrDefault<Empleado>(@"
                    SELECT e.id_empleado AS IdEmpleado, e.id_posicion AS IdPosicion, e.id_plaza AS IdPlaza,
                           e.curp AS CURP, e.nombre AS Nombre, e.primer_apellido AS PrimerApellido,
                           e.segundo_apellido AS SegundoApellido, e.telefono AS Telefono,
                           e.id_supervisor AS IdSupervisor, e.id_ejecutivo AS IdEjecutivo,
                           e.fecha_ingreso AS FechaIngreso,
                           e.limite_venta_ejercicio, e.limite_incremento_ejercicio,
                           e.fizcalizable, e.ocupacion AS Ocupacion, e.nota_foto AS NotaFoto,
                           e.fecha_baja,
                           id_posicion as IdTipoUsuario,
id_posicion as IdPuesto
                    FROM empleado e
                    WHERE e.id_empleado = @id AND ISNULL(e.eliminado,0)=0", new { id = idEmpleado });

                if (emp == null) return null;

                // Dirección del empleado
                var dir = conn.QueryFirstOrDefault<Direccion>(@"
                    SELECT TOP 1 id_direccion as idDireccion, id_empleado as IdEmpleado, id_cliente as IdCliente,
                           calleyno as Calle, colonia as Colonia, municipio as Municipio, estado as Estado,
                           codigo_postal as CodigoPostal, direccion_trabajo as DireccionTrabajo, ubicacion as Ubicacion
                    FROM direccion
                    WHERE id_empleado = @id
                    ORDER BY id_direccion DESC", new { id = idEmpleado }) ?? new Direccion { IdEmpleado = idEmpleado };

                emp.Direccion = dir;

                // Usuario
                var user = conn.QueryFirstOrDefault<Usuario>(@"
                    SELECT TOP 1 id_usuario as IdUsuario, id_empleado as IdEmpleado,
                           login as Login, id_tipo_usuario as IdTipoUsuario
                    FROM usuario
                    WHERE id_empleado = @id AND ISNULL(eliminado,0)=0", new { id = idEmpleado }) ?? new Usuario { IdEmpleado = idEmpleado };

                // Aval (asociado vía direccion.id_cliente)
                var aval = conn.QueryFirstOrDefault<Cliente>(@"
                    SELECT TOP 1 c.id_cliente as IdCliente, c.curp as Curp, c.nombre as Nombre,
                           c.primer_apellido as PrimerApellido, c.segundo_apellido as SegundoApellido,
                           c.ocupacion as Ocupacion, c.telefono as Telefono
                    FROM cliente c
                    INNER JOIN direccion d ON d.id_cliente = c.id_cliente
                    WHERE d.id_empleado = @id
                    ORDER BY c.id_cliente DESC", new { id = idEmpleado });

                if (aval != null)
                {
                    aval.direccion = conn.QueryFirstOrDefault<Direccion>(@"
                        SELECT TOP 1 id_direccion as idDireccion, id_empleado as IdEmpleado, id_cliente as IdCliente,
                               calleyno as Calle, colonia as Colonia, municipio as Municipio, estado as Estado,
                               codigo_postal as CodigoPostal, direccion_trabajo as DireccionTrabajo, ubicacion as Ubicacion
                        FROM direccion
                        WHERE id_cliente = @idCliente
                        ORDER BY id_direccion DESC", new { idCliente = aval.IdCliente }) ?? new Direccion { IdCliente = aval.IdCliente };
                }
                else
                {
                    aval = new Cliente { direccion = new Direccion { IdEmpleado = idEmpleado } };
                }

                return new EmpleadoRequest
                {
                    Colaborador = emp,
                    Aval = aval,
                    User = user
                };
            }
        }

        [WebMethod]
        public static DatosSalida Save(string path, string idUsuario, EmpleadoRequest oRequest)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            var salida = new DatosSalida();
            using (var conn = new SqlConnection(strConexion))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        // ----- Permisos -----
                        bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
                        if (!tienePermiso)
                        {
                            tx.Rollback();
                            return new DatosSalida { CodigoError = 1, MensajeError = "No tiene permisos para realizar esta operación." };
                        }

                        // ----- Null guards -----
                        if (oRequest == null)
                            oRequest = new EmpleadoRequest();

                        if (oRequest.Colaborador == null)
                            oRequest.Colaborador = new Empleado();

                        if (oRequest.User == null)
                            oRequest.User = new Usuario();

                        if (oRequest.Aval == null)
                            oRequest.Aval = new Cliente();


                        var isEdit = oRequest.Colaborador.IdEmpleado > 0;
                        int? idEmpleado = isEdit ? (int?)oRequest.Colaborador.IdEmpleado : null;

                        // Normaliza inputs
                        string curpNuevo = (oRequest.Colaborador.CURP ?? "").Trim();
                        string loginNuevo = (oRequest.User.Login ?? "").Trim();
                        string curpAvalNueva = (oRequest.Aval.Curp ?? "").Trim();

                        // ==========================
                        // VALIDACIONES (tolerantes en edición)
                        // ==========================

                        // 1) CURP Colaborador
                        if (!string.IsNullOrWhiteSpace(curpNuevo))
                        {
                            bool deboValidarCurpEmpleado = true;

                            if (isEdit)
                            {
                                // Sólo valida si CAMBIÓ la CURP
                                const string qCurp = "SELECT TOP 1 curp FROM empleado WHERE id_empleado=@id AND ISNULL(eliminado,0)=0";
                                string curpActual = conn.ExecuteScalar<string>(qCurp, new { id = idEmpleado }, tx);
                                deboValidarCurpEmpleado = !string.Equals(curpNuevo, curpActual ?? "", StringComparison.OrdinalIgnoreCase);
                            }

                            if (deboValidarCurpEmpleado &&
                                CountEmpleadoByCurp(curpNuevo, conn, tx, isEdit ? idEmpleado : null) > 0)
                            {
                                tx.Rollback();
                                return new DatosSalida { CodigoError = 1, MensajeError = "Ya existe un colaborador con el CURP especificado." };
                            }
                        }

                        // 2) CURP Aval (si viene)
                        if (!string.IsNullOrWhiteSpace(curpAvalNueva))
                        {
                            int? excludeCliente = (oRequest.Aval.IdCliente > 0) ? oRequest.Aval.IdCliente : (int?)null;

                            if (isEdit && excludeCliente == null)
                            {
                                // Si no viene IdCliente en la edición, intenta resolver el cliente del aval ligado al empleado
                                const string qCli =
                                    @"SELECT TOP 1 c.id_cliente
                              FROM cliente c
                              INNER JOIN direccion d ON d.id_cliente = c.id_cliente
                              WHERE d.id_empleado = @idEmpleado
                              ORDER BY c.id_cliente DESC";
                                excludeCliente = conn.ExecuteScalar<int?>(qCli, new { idEmpleado }, tx);
                                oRequest.Aval.IdCliente = excludeCliente ?? 0;
                            }

                            // Sólo valida si la CURP CAMBIÓ
                            bool deboValidarCurpAval = true;
                            if (isEdit && excludeCliente.HasValue)
                            {
                                const string qCurpAval = "SELECT TOP 1 curp FROM cliente WHERE id_cliente=@id";
                                string curpAvalActual = conn.ExecuteScalar<string>(qCurpAval, new { id = excludeCliente.Value }, tx);
                                deboValidarCurpAval = !string.Equals(curpAvalNueva, curpAvalActual ?? "", StringComparison.OrdinalIgnoreCase);
                            }

                            if (deboValidarCurpAval &&
                                CountClienteByCurp(curpAvalNueva, conn, tx, excludeCliente) > 0)
                            {
                                tx.Rollback();
                                return new DatosSalida { CodigoError = 1, MensajeError = "Ya existe un aval (cliente) con el CURP especificado." };
                            }
                        }

                        // 3) LOGIN usuario (si viene)
                        if (!string.IsNullOrWhiteSpace(loginNuevo))
                        {
                            if (isEdit)
                            {
                                // Verifica conflicto SÓLO si el login CAMBIÓ
                                const string qLogin = "SELECT TOP 1 login FROM usuario WHERE id_empleado=@id AND ISNULL(eliminado,0)=0";
                                string loginActual = conn.ExecuteScalar<string>(qLogin, new { id = idEmpleado }, tx);

                                bool loginCambio = !string.Equals(loginNuevo, loginActual ?? "", StringComparison.OrdinalIgnoreCase);
                                if (loginCambio &&
                                    CountUsuarioLoginUsadoPorOtro(loginNuevo, oRequest.Colaborador.IdEmpleado, conn, tx) > 0)
                                {
                                    tx.Rollback();
                                    return new DatosSalida { CodigoError = 1, MensajeError = "Ya existe un usuario con ese Login asignado a otro colaborador." };
                                }
                            }
                            else
                            {
                                var usuarioExists = GetUsuarioByLogin(loginNuevo, conn, tx);
                                if (usuarioExists != null)
                                {
                                    tx.Rollback();
                                    return new DatosSalida { CodigoError = 1, MensajeError = "Ya existe un usuario con ese Login. Verifique e intente de nuevo." };
                                }
                            }
                        }

                        // ==========================
                        // INSERT / UPDATE
                        // ==========================
                        if (!isEdit)
                        {
                            // INSERT EMPLEADO
                            RegistrarEmpleado(oRequest.Colaborador, tx, conn);

                            // Usuario
                            oRequest.User.IdEmpleado = oRequest.Colaborador.IdEmpleado;
                            oRequest.User.IdTipoUsuario = oRequest.Colaborador.IdPosicion;
                            RegistrarUsuario(oRequest.User, tx, conn);

                            // Aval
                            if (!string.IsNullOrEmpty(oRequest.Aval.Nombre))
                            {
                                RegistraClienteAval(oRequest.Aval, tx, conn);
                            }
                        }
                        else
                        {
                            // UPDATE EMPLEADO
                            ActualizarEmpleado(oRequest.Colaborador, tx, conn);

                            // Usuario (login, rol y password opcional)
                            oRequest.User.IdEmpleado = oRequest.Colaborador.IdEmpleado;
                            oRequest.User.IdTipoUsuario = oRequest.Colaborador.IdPosicion;
                            ActualizarUsuario(oRequest.User, tx, conn);

                            // Aval
                            if (oRequest.Aval.NombreAval != null)
                            {
                                // IMPORTANTÍSIMO: dirección del AVAL debe actualizarse por CLIENTE
                                if (oRequest.Aval != null && oRequest.Aval.direccion != null)
                                {
                                    // Vincula esta dirección del aval con el empleado recién creado
                                    oRequest.Aval.direccion.IdEmpleado = oRequest.Colaborador.IdEmpleado;
                                }
                                RegistraClienteAval(oRequest.Aval, tx, conn);
                            }
                        }

                        tx.Commit();

                        return new DatosSalida
                        {
                            CodigoError = 0,
                            MensajeError = "Guardado correctamente",
                            IdItem = oRequest.Colaborador.IdEmpleado.ToString()
                        };
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        Utils.Log("Error ... " + ex.Message);
                        Utils.Log(ex.StackTrace);
                        return new DatosSalida { CodigoError = 1, MensajeError = "Se ha generado un error <br/>" + ex.Message };
                    }
                }
            }
        }


        [WebMethod]
        public static List<Posicion> GetListaItemsPosiciones(string path)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            using (var conn = new SqlConnection(strConexion))
            {
                var query = @" SELECT id_posicion as IdPosicion, nombre as Nombre
                               FROM posicion 
                               WHERE ISNull(eliminado, 0) = 0 AND id_posicion <> 6 ";

                return conn.Query<Posicion>(query).ToList();
            }
        }

        [WebMethod]
        public static List<Plaza> GetListaItemsPlazas(string path)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            using (var conn = new SqlConnection(strConexion))
            {
                var query = @" SELECT id_plaza as IdPlaza, nombre as Nombre
                               FROM  plaza 
                               WHERE IsNull(activo, 1) = 1 AND ISNull(eliminado, 0) = 0";

                return conn.Query<Plaza>(query).ToList();
            }
        }

        [WebMethod]
        public static List<Empleado> GetListaItemsEmpleadoByPosicion(string path, string idTipoEmpleado)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            using (var conn = new SqlConnection(strConexion))
            {
                var query = @"
                    SELECT id_empleado AS IdEmpleado,
                           concat(nombre, ' ', primer_apellido, ' ', segundo_apellido) AS Nombre
                    FROM empleado 
                    WHERE ISNull(activo, 1) = 1  
                      AND ISNull(eliminado, 0) = 0
                      AND id_posicion = @id_posicion ";

                return conn.Query<Empleado>(query, new { id_posicion = idTipoEmpleado }).ToList();
            }
        }

        #endregion
    }
}
