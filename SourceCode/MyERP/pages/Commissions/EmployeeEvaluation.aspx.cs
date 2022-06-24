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
    public partial class EmployeeEvaluation : System.Web.UI.Page
    {
        const string pagina = "18";



        protected void Page_Load(object sender, EventArgs e)
        {
            string usuario = (string)Session["usuario"];
            string idTipoUsuario = (string)Session["id_tipo_usuario"];
            string idUsuario = (string)Session["id_usuario"];
            string path = (string)Session["path"];



            txtUsuario.Value = usuario;//"promotor.colorado
            txtIdTipoUsuario.Value = idTipoUsuario;//5
            txtIdUsuario.Value = idUsuario;//69

            //  si no esta logueado
            if (usuario == string.Empty)
            {
                Response.Redirect("Login.aspx");
            }


        }


        [WebMethod]
        public static List<Empleado> GetListaItems(string path, string idUsuario, string idPlaza, string idEjecutivo,
            string idPromotor, string idSupervisor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();


            //  Filtro plaza
            var sqlPlaza = "";
            if (idPlaza != "" && idPlaza != "-1")
            {
                sqlPlaza = " AND plaza.id_plaza = '" + idPlaza + "'";
            }

            var sqlEjecutivo = "";
            if (idEjecutivo != "" && idEjecutivo != "-1")
            {
                sqlEjecutivo = " AND superv.id_ejecutivo = '" + idEjecutivo + "'";
            }

            var sqlPromotor = "";
            if (idPromotor != "" && idPromotor != "-1")
            {
                sqlPromotor = " AND e.id_empleado = '" + idPromotor + "'";
            }

            var sqlSupervisor = "";
            if (idSupervisor != "" && idSupervisor != "-1")
            {
                sqlSupervisor = " AND e.id_supervisor = '" + idSupervisor + "'";
            }


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT e.id_empleado , e.nombre, e.primer_apellido, e.segundo_apellido, 
                     concat(e.nombre ,  ' ' , e.primer_apellido , ' ' , e.segundo_apellido) AS nombre_completo,
                     concat(superv.nombre ,  ' ' , superv.primer_apellido , ' ' , superv.segundo_apellido) AS nombre_completo_supervisor,
                     concat(ejec.nombre ,  ' ' , ejec.primer_apellido , ' ' , ejec.segundo_apellido) AS nombre_completo_ejecutivo,
                     FORMAT(e.fecha_ingreso, 'dd/MM/yyyy') fecha_ingreso, IsNull(e.id_comision_inicial, 0) id_comision_inicial,
                     plaza.nombre nombre_plaza, comis.nombre nombre_comision,
                     IsNull(( SELECT SUM(valor.ponderacion) total  FROM relacion_evaluacion_colaborador_reglas valor
                        WHERE valor.id_empleado = e.id_empleado AND comis.id_comision = valor.id_comision
                        AND IsNull(valor.completado, 0) = 1
                    ), 0)  porcentaje_total_completado
                     FROM empleado e 
                     join empleado superv ON (e.id_supervisor = superv.id_empleado)
                     join empleado ejec ON (superv.id_ejecutivo = ejec.id_empleado)
                     JOIN plaza plaza ON (plaza.id_plaza = e.id_plaza) 
                     JOIN comision comis ON (comis.id_comision = e.id_comision_inicial) 
                     WHERE isnull(e.eliminado, 0) != 1 " +
                    sqlPlaza +
                    sqlEjecutivo +
                    sqlSupervisor +
                    sqlPromotor +
                    @"";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Empleado item = new Empleado();
                        item.nivelNomision = new Comision();

                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        item.IdComisionInicial = int.Parse(ds.Tables[0].Rows[i]["id_comision_inicial"].ToString());
                        item.nivelNomision.Porcentaje = int.Parse(ds.Tables[0].Rows[i]["porcentaje_total_completado"].ToString());
                        item.nivelNomision.PorcentajeStr = item.nivelNomision.Porcentaje.ToString() + "%";
                        item.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.NombreCompletoSupervisor = ds.Tables[0].Rows[i]["nombre_completo_supervisor"].ToString();
                        item.NombreCompletoEjecutivo = ds.Tables[0].Rows[i]["nombre_completo_ejecutivo"].ToString();
                        item.NombrePlaza = ds.Tables[0].Rows[i]["nombre_plaza"].ToString();
                        item.NombreComision = ds.Tables[0].Rows[i]["nombre_comision"].ToString();
                        item.FechaIngresoMx = ds.Tables[0].Rows[i]["fecha_ingreso"].ToString();

                        string botones = "";
                        botones += "<button  onclick='employeeEvaluation.open(" + item.IdComisionInicial +
                            ", \"" + item.NombreComision + "\"" +
                            ", \"" + item.NombreCompleto + "\"," + item.IdEmpleado + ")'   class='btn btn-outline-primary'> <span class='fa fa-check mr-1'></span>Evaluar</button>";

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
        public static DatosSalida SaveEvaluation(string path, List<ValorReglaEvaluacionModulo> data, string accion, string idUsuario,
            string idComision, string idEmpleado)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            DatosSalida salida = new DatosSalida();
            SqlTransaction transaccion = null;

            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");


            int r = 0;
            try
            {

                string sql = "";

                conn.Open();
                transaccion = conn.BeginTransaction();

                //  eliminar registro anterior
                sql = @" DELETE FROM relacion_evaluacion_colaborador_reglas 
                                WHERE id_comision = @id_comision AND id_empleado = @id_empleado
                          ";


                Utils.Log("\n" + sql);

                SqlCommand cmdDelete = new SqlCommand(sql, conn);
                cmdDelete.CommandType = CommandType.Text;
                cmdDelete.Transaction = transaccion;

                cmdDelete.Parameters.AddWithValue("@id_comision", idComision);
                cmdDelete.Parameters.AddWithValue("@id_empleado", idEmpleado);
                r = cmdDelete.ExecuteNonQuery();


                //  insertar nuevas calificaciones
                var totalPonderacion = 0;
                foreach (var item in data)
                {

                    if (item.IdComision != 0)
                    {
                        sql = @" INSERT INTO relacion_evaluacion_colaborador_reglas 
                                (id_comision, id_empleado, ponderacion, completado, id_regla_evaluacion)
                            VALUES
                                (@id_comision, @id_empleado, @ponderacion, @completado, @id_regla_evaluacion);
                          ";



                        Utils.Log("\n" + sql);

                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.Transaction = transaccion;

                        cmd.Parameters.AddWithValue("@id_comision", item.IdComision);
                        cmd.Parameters.AddWithValue("@id_empleado", item.IdEmpleado);
                        cmd.Parameters.AddWithValue("@ponderacion", item.Ponderacion);
                        cmd.Parameters.AddWithValue("@completado", item.Completado);
                        cmd.Parameters.AddWithValue("@id_regla_evaluacion", item.IdReglaEvaluacionModulo);
                        totalPonderacion += item.Ponderacion;

                        r = cmd.ExecuteNonQuery();
                    }

                }

                //  Si tiene el 100% subirlo de nivel
                if (totalPonderacion == 100)
                {

                }



                transaccion.Commit();


                Utils.Log("Guardado -> OK ");


                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;

            }
            catch (Exception ex)
            {
                try
                {
                    transaccion.Rollback();
                }
                catch (Exception excep)
                {

                }

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                salida.MensajeError = "No se pudo completar la operación.";
                salida.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return salida;


        }



        public class Data
        {
            public string Table;
            public EvaluacionModulo evaluacion;
            public int totalEvaluacionCompletada;
            public List<ValorReglaEvaluacionModulo> Array = new List<ValorReglaEvaluacionModulo>();
        }


        [WebMethod]
        public static Data GetItemsRulesByCommissionAndEmployee(string path, string idUsuario, string idEmpleado,
            string idComision)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            string html = "";
            Data data = new Data();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT r.id_regla_evaluacion_modulo, r.descripcion, IsNull(r.ponderacion, 0) ponderacion, 
                                    IsNull(r.id_comision, 0) id_comision, c.nombre,
                                    IsNull(r.ponderacion, 0) ponderacion, IsNull(valor.completado , 0) completado
                                    FROM regla_evaluacion_modulo r
                                    JOIN comision c ON (c.id_comision = r.id_comision)
                                    LEFT JOIN relacion_evaluacion_colaborador_reglas valor 
                                        ON (valor.id_regla_evaluacion = r.id_regla_evaluacion_modulo AND c.id_comision = valor.id_comision
                                        AND valor.id_empleado = @id_empleado)
                                    WHERE r.id_comision = @id_comision ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_comision", idComision);
                adp.SelectCommand.Parameters.AddWithValue("@id_empleado", idEmpleado);


                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
                int totalPonderacion = 0;
                int totalPonderacionCompletada = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ValorReglaEvaluacionModulo item = new ValorReglaEvaluacionModulo();
                        item.IdReglaEvaluacionModulo = int.Parse(ds.Tables[0].Rows[i]["id_regla_evaluacion_modulo"].ToString());
                        item.IdComision = int.Parse(ds.Tables[0].Rows[i]["id_comision"].ToString());
                        item.Ponderacion = int.Parse(ds.Tables[0].Rows[i]["ponderacion"].ToString());
                        item.PonderacionStr = item.Ponderacion + "%";
                        item.CalificacionStr = item.Ponderacion + "%";
                        item.Descripcion = (ds.Tables[0].Rows[i]["descripcion"].ToString());
                        item.NombreComision = (ds.Tables[0].Rows[i]["nombre"].ToString());
                        item.Completado = int.Parse(ds.Tables[0].Rows[i]["completado"].ToString());

                        totalPonderacion += item.Ponderacion;

                        if (item.Completado == 1)
                        {
                            totalPonderacionCompletada += item.Ponderacion;
                        }

                        string strChecked = " ";
                        if (item.Completado == 1)
                        {
                            strChecked = " checked = 'checked' ";
                        }

                        html += "<tr>";
                        html += "<td>" + item.Descripcion + "</td>";
                        html += "<td>" + item.PonderacionStr + "</td>";
                        html += "<td>" + item.CalificacionStr + "</td>";
                        html += "<td><input type='checkbox' " + strChecked + " data-idregla=" + item.IdReglaEvaluacionModulo + " data-value=" + item.Ponderacion + " value='" + 1 + "' class='form-checked-control checks sm100'  id='chkChecked" + i + "'></td>";
                        html += "</tr>";

                        data.Array.Add(item);

                    }

                    html += "<tr>";
                    html += "<td>Total</td>";
                    html += "<td id=\"spanTotalPonderacion\">" + totalPonderacion + "%</td>";
                    html += "<td id=\"spanTotalPonderacionObtenida\">" + totalPonderacionCompletada + "%</td>";
                    html += "<td></td>";
                    html += "</tr>";

                }


                EvaluacionModulo evaluacion = new EvaluacionModulo();
                evaluacion.IdEvaluacionModulo = int.Parse(idComision);


                data.Table = html;
                data.evaluacion = evaluacion;
                data.totalEvaluacionCompletada = totalPonderacionCompletada;

                return data;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return data;
            }

            finally
            {
                conn.Close();
            }

        }



    }






}