using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using Plataforma.Clases;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace Plataforma.pages
{
    /// <summary>
    /// Summary description
    /// </summary>
    public class FileUploaderCSV: IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            System.Diagnostics.Debug.Print("Respuesta desde FileUploaderCSV ");
            context.Response.ContentType = "text/plain";
            try
            {

                string strNombreArchivo = "";

                foreach (string s in context.Request.Files)
                {
                    HttpPostedFile file = context.Request.Files[s];
                    string fileName = "";
                    string pagina = context.Request.Form[0];//nombre de la pagina que lo invoca
                    string path = context.Request.Form[1];//path server
                    string extension = context.Request.Form[2];//extension del archivo
                    string descrpcion = context.Request.Form[3];//descripcion
                    string tipo = context.Request.Form[4];//tipo
                    string usuario = context.Request.Form[5];//id usuario

                    string uuid = DateTime.Now.ToString("yyyMMdd_HMs");

                   fileName = "csv_" + uuid + descrpcion;

                    //Impuesto impuesto = Plataforma.pages.Impuestos.GetItem(path, idImpuesto);

                    List<DatosCampo> listaCampos = new List<DatosCampo>();


                    DatosCampo campo = new DatosCampo();
                    campo = new DatosCampo();
                    campo.NombreCampo = "clave_empleado";//1
                    campo.NombreTipo = "varchar";
                    campo.Actualizable = true;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "nombre";//2
                    campo.NombreTipo = "varchar";
                    campo.Actualizable = true;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "sn_empleados_ape_paterno";//3
                    campo.NombreTipo = "varchar";
                    campo.Actualizable = false;
                    listaCampos.Add(campo);


                    campo = new DatosCampo();
                    campo.NombreCampo = "sn_empleados_ape_materno";//4
                    campo.NombreTipo = "varchar";
                    campo.Actualizable = false;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "clave_depto";//5
                    campo.NombreTipo = "varchar";
                    campo.Actualizable = false;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "desc_depto";//6
                    campo.NombreTipo = "varchar";
                    campo.Actualizable = false;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "area";//7
                    campo.NombreTipo = "varchar";
                    campo.Actualizable = false;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "desc_area";//8
                    campo.NombreTipo = "varchar";
                    campo.Actualizable = false;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "puesto";//9
                    campo.NombreTipo = "varchar";
                    campo.Actualizable = false;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "descripcion";//10
                    campo.NombreTipo = "varchar";
                    campo.Actualizable = false;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "fecha_alta";//11
                    campo.NombreTipo = "date";
                    campo.Actualizable = true;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "salario_mens";//12
                    campo.NombreTipo = "float";
                    campo.Actualizable = true;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "id_regpat";//13
                    campo.NombreTipo = "int";
                    campo.Actualizable = true;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "desc_reg_pat";//14
                    campo.NombreTipo = "varchar";
                    campo.Actualizable = true;
                    listaCampos.Add(campo);

                    campo = new DatosCampo();
                    campo.NombreCampo = "reg_pat";//15
                    campo.NombreTipo = "int";
                    campo.Actualizable = true;
                    listaCampos.Add(campo);


                    //campo = new DatosCampo();
                    //campo.NombreCampo = "area";//5
                    //campo.NombreTipo = "date";
                    //campo.Actualizable = true;
                    //listaCampos.Add(campo);


                    //campo = new DatosCampo();
                    //campo.NombreCampo = "hora_proceso";//6
                    //campo.NombreTipo = "time";
                    //campo.Actualizable = true;
                    //listaCampos.Add(campo);

                    List<DatosInsertar> listaInserts = null;
                    if (!string.IsNullOrEmpty(fileName))
                    {


                        strNombreArchivo = fileName;

                        string path_folder = HttpContext.Current.Server.MapPath("~/pages/Uploads/");
                        
                        string fullPath = path_folder + strNombreArchivo;
                        file.SaveAs(fullPath);

                        //una vez guardado el archivo
                        listaInserts = ProcesarCSV(fullPath, listaCampos, "tabla", path);

                        

                        if (listaInserts != null)
                        {
                            bool todosOk = true;
                            foreach (var insert in listaInserts)
                            {
                                if (insert.valido == false)
                                {
                                    todosOk = false;

                                }
                            }



                            if (todosOk)
                            {





                                string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
                                SqlConnection conn = new SqlConnection(strConexion);
                                SqlTransaction transaccion = null;


                                try
                                {


                                    conn.Open();
                                    transaccion = conn.BeginTransaction();

                                    SetEmpleadosInactivo(transaccion, conn, strConexion);

                                    foreach (var insert in listaInserts)
                                    {

                                        insert.mensajeError = "<span class=\"claseOk\">Registros ingresados correctamente. Se insertaron todos los registros.</span>";


                                        string nombreTabla = "empleado";




                                        InsertarRegistro(path, nombreTabla,
                                               conn,
                                               transaccion,
                                               insert,
                                               usuario,
                                               " registro ",
                                        strConexion
                                               );

                                        if (insert.valido == false) break;

                                    }

                                    todosOk = true;
                                    foreach (var insert in listaInserts)
                                    {
                                        if (insert.valido == false)
                                        {
                                            todosOk = false;

                                        }
                                    }

                                    if (todosOk == false)
                                    {
                                        try
                                        {

                                            foreach (var insert in listaInserts)
                                            {
                                                insert.mensajeError = "<span class=\"claseError\">No se ha actualizado ningún registro porque existen errores en los datos. Marcados en color rojo.</span>";
                                            }

                                            transaccion.Rollback();
                                        }
                                        catch (Exception exRolBack)
                                        {
                                        }
                                    }
                                    else
                                    {
                                        transaccion.Commit();
                                    }



                                }
                                catch (Exception ex)
                                {
                                    Log("Error " + ex.Message);

                                    foreach (var insert in listaInserts)
                                    {
                                        insert.mensajeError = "<span class=\"claseError\">No se ha actualizado ningún registro porque existen errores en los datos. Marcados en color rojo.</span>";

                                    }


                                }
                               

                            }
                            else
                            {
                                foreach (var insert in listaInserts)
                                {
                                    insert.mensajeError = "<span class=\"claseError\">No se ha insertado ningún registro porque existen errores en los datos. Marcados en color rojo.</span>";

                                }
                            }

                        }



                    }

                    var jsonSerialiser = new JavaScriptSerializer();

                    var valoresRetorno = jsonSerialiser.Serialize(listaInserts);

                    context.Response.Write(valoresRetorno);
                    //}
                }





            }
            catch (Exception ac)
            {
                Log("Error " + ac.Message);
                Log(ac.StackTrace);
            }

        }

        public class DatosInsertar
        {
            public List<DatosCampo> listaCampos;
            public string[] parts;
            public string[] estilosCss;
            public bool valido;
            public string mensajeError;

        }

        public class DataItemInvalid
        {
            public int Row { get; set; }
            public string Line { get; set; }
            public override string ToString()
            {
                return $"[{Row}] '{Line}'";
            }
        }

        List<DatosInsertar> ProcesarCSV(string _inputFileName, List<DatosCampo> listaCampos, string nombreTabla, string path)
        {

            var validRows = new List<String>();
            var invalidRows = new List<DataItemInvalid>();

            int index = 0;
            List<DatosInsertar> listaValoresProcesados = new List<DatosInsertar>();

            try
            {
                using (var readFile = new StreamReader(_inputFileName))
                {
                    string line;
                    string[] parts;
                    string[] estilosCss;

                    Boolean isOk = true;
                    int k = 0;
                    while ((line = readFile.ReadLine()) != null)
                    {
                        //if (k > 0)
                        //{
                            isOk = true;
                            Log("line = " + line);

                            line = line.Replace("URBANISSA,", "URBANISSA");
                            Log("line = " + line);

                            parts = line.Split(';');    //probar separacion con ;

                            if (parts.Length == 1)      //probar separacion con ,
                                parts = line.Split(',');

                            estilosCss = new string[parts.Length];

                            index += 1;

                            if (parts == null)
                            {
                                break;
                            }

                            index += 1;

                            Log("parts.Length = " + parts.Length);
                            Log("listaCampos.Count = " + listaCampos.Count);


                            if ((parts.Length) != listaCampos.Count)//son 15 campos para subir
                            {
                                invalidRows.Add(new DataItemInvalid() { Row = index, Line = string.Join(",", parts) });

                                isOk = false;
                            }

                            Log("\n...");

                            int i = 0;
                            foreach (var valor in parts)
                            {
                                var campo = listaCampos[i];

                                Log("Campo = " + campo.NombreCampo + " Tipo campo = " + campo.NombreTipo + " Valor = " + valor);

                                //intentar parsear una fecha, solo fechas no nulas
                                if (campo.NombreTipo == "date" && valor != "")
                                {
                                    string[] fechas = valor.Split('/');
                                    var nuevaFecha = "";
                                    if (fechas.Length == 3)
                                    {
                                        nuevaFecha = fechas[2] + "-" + fechas[1] + "-" + fechas[0];
                                        estilosCss[i] = "claseOk";
                                    }
                                    else
                                    {
                                        Log("FAlló la conversion a fecha del valor " + valor);
                                        isOk = false;
                                        estilosCss[i] = "claseError";

                                    }
                                }

                                //intentar parsear un numero
                                if (campo.NombreTipo == "float")
                                {
                                    try
                                    {
                                        float valorNumerico = float.Parse(valor);
                                        estilosCss[i] = "claseOk";
                                    }
                                    catch (Exception eex)
                                    {
                                        Log(eex.Message);

                                        Log("Falló la conversion a numero del valor " + valor);
                                        estilosCss[i] = "claseError";
                                        isOk = false;
                                    }
                                }

                                if (campo.NombreTipo == "int")
                                {
                                    try
                                    {
                                        int valorNumerico = int.Parse(valor);
                                        estilosCss[i] = "claseOk";
                                    }
                                    catch (Exception eex)
                                    {
                                        Log(eex.Message);

                                        Log("FAlló la conversion a numero del valor " + valor);
                                        estilosCss[i] = "claseError";
                                        isOk = false;
                                    }
                                }



                                i++;
                            }

                            //Log(line);


                            parts = FormatearValores(parts);

                            DatosInsertar datos = new DatosInsertar();
                            datos.listaCampos = listaCampos;
                            datos.parts = parts;
                            datos.estilosCss = estilosCss;
                            datos.valido = isOk;

                            listaValoresProcesados.Add(datos);

                        //}
                        k++;
                    }//while lines
                }

                return listaValoresProcesados;

            }
            catch (Exception ex)
            {
                Log("Error " + ex.Message);
                Log(ex.StackTrace);
                return null;
            }
        }


        private string[] FormatearValores(string[] parts)
        {
            int j = 0;
            foreach (var valor in parts)
            {

                var fecha = parts[j];
                //12/12/2019
                string[] fechas = fecha.Split('/');
                var nuevaFecha = "";
                if (fechas.Length == 3)
                {
                    nuevaFecha = fechas[2] + "-" + fechas[1] + "-" + fechas[0];
                    parts[j] = nuevaFecha;

                }


                j++;
            }

            return parts;
        }

        public static Empleado GetEmpleadoByClave(SqlTransaction transaccion, SqlConnection conn, string strConexion, string claveEmpleado)
        {
            Empleado item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = " SELECT e.id_empleado, e.nombre, e.apellido_paterno, e.apellido_materno, " +
                    " e.clave, IsNull(e.id_departamento, -1) id_departamento, " +
                    " IsNull(e.id_puesto, -1) id_puesto, " +
                    " IsNull(e.activo, 0) activo " +
                    " FROM empleado e " +
                    " WHERE e.clave = @clave ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.SelectCommand.Parameters.AddWithValue("@clave", claveEmpleado);
                adp.SelectCommand.Transaction = transaccion;

                adp.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        item = new Empleado();
                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[j]["id_empleado"].ToString());
                        item.Nombre = ds.Tables[0].Rows[j]["nombre"].ToString();
                        item.APaterno = ds.Tables[0].Rows[j]["apellido_paterno"].ToString();
                        item.AMaterno = ds.Tables[0].Rows[j]["apellido_materno"].ToString();
                        item.Clave = ds.Tables[0].Rows[j]["clave"].ToString();
                        //item.ClavePuesto = ds.Tables[0].Rows[j]["clave_puesto"].ToString();
                        //item.ClaveDepartamento = ds.Tables[0].Rows[j]["clave_departamento"].ToString();
                        item.IdDepartamento = int.Parse(ds.Tables[0].Rows[j]["id_departamento"].ToString());
                        item.IdPuesto = int.Parse(ds.Tables[0].Rows[j]["id_puesto"].ToString());
                        item.Activo = int.Parse(ds.Tables[0].Rows[j]["activo"].ToString());

                    }
                }


            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return null;
            }

          

            return item;
        }

        public static Puesto GetItemPuestoByClave(SqlTransaction transaccion, SqlConnection conn, string strConexion, string clave)
        {

            Puesto item = null;
            try
            {
                DataSet ds = new DataSet();
                string query = " SELECT id_puesto , nombre, clave, ISNull(activo, 1) activo " +
                    "  FROM  puesto where clave =  @clave ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("clave puesto =  " + clave);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@clave", clave);
                adp.SelectCommand.Transaction = transaccion;
                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Puesto();

                        item.IdPuesto = int.Parse(ds.Tables[0].Rows[i]["id_puesto"].ToString());
                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.Clave = ds.Tables[0].Rows[i]["clave"].ToString();



                    }
                }





                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return null;
            }


        }

        public static Departamento GetItemDepartamentoByClave(SqlTransaction transaccion, SqlConnection conn, string strConexion, string clave)
        {

            Departamento item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = " SELECT id_departamento , nombre, clave, ISNull(activo, 1) activo " +
                    "  FROM  departamento where clave =  @clave ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("clave depto =  " + clave);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@clave", clave);
                adp.SelectCommand.Transaction = transaccion;

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Departamento();

                        item.IdDepartamento = int.Parse(ds.Tables[0].Rows[i]["id_departamento"].ToString());
                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.Clave = ds.Tables[0].Rows[i]["clave"].ToString();



                    }
                }





                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return null;
            }


        }


        public static int ActualizarDepartamento(SqlTransaction transaccion,
            SqlConnection conn, string strConexion, Departamento departamento, 
            string idEmpleado)
        {


            try
            {
                string sql = " UPDATE empleado SET     " +
                      " id_departamento = @id_departamento, activo = 1           " +
                      "    WHERE id_empleado = @id_empleado   ";

            

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");
                Log("id_empleado = " + idEmpleado);
                Log("departamento.IdDepartamento = " + departamento.IdDepartamento);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_departamento", departamento.IdDepartamento);
                cmd.Parameters.AddWithValue("@id_empleado", idEmpleado);
                                
                cmd.Transaction = transaccion;

                int r = cmd.ExecuteNonQuery();

                Log("r (update departamento ) = " + r);



            return r;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1;
            }


        }

        public static int SetEmpleadosInactivo(SqlTransaction transaccion,
            SqlConnection conn, string strConexion)
        {


            try
            {
                string sql = " UPDATE empleado SET     " +
                      " activo = @activo   ";



                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");
                Log("Pasando a inactivo a todos " );

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@activo", 0);


                cmd.Transaction = transaccion;

                int r = cmd.ExecuteNonQuery();

                Log("r (update  ) = " + r);



                return r;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1;
            }


        }


        public static int SetEmpleadoActivo(SqlTransaction transaccion,
            SqlConnection conn, string strConexion, string clateEmpleado)
        {


            try
            {
                string sql = " UPDATE empleado SET     " +
                      " activo = @activo WHERE clave = @clave  ";



                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");
                Log("Pasando a activo  " + clateEmpleado) ;

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@activo", 1);
                cmd.Parameters.AddWithValue("@clave", clateEmpleado);


                cmd.Transaction = transaccion;

                int r = cmd.ExecuteNonQuery();

                Log("r (update  ) = " + r);



                return r;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1;
            }


        }


        public static int ActualizarPuesto(SqlTransaction transaccion,
           SqlConnection conn, string strConexion, Puesto puesto,
           string idEmpleado)
        {

            try
            {
                string sql = " UPDATE empleado SET     " +
                      " id_puesto = @id_puesto, activo = 1 " +
                      "    WHERE id_empleado = @id_empleado   ";



                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");
                
                Log("id_empleado = " + idEmpleado);
                Log("IdPuesto = " + puesto.IdPuesto);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_puesto", puesto.IdPuesto);
                cmd.Parameters.AddWithValue("@id_empleado", idEmpleado);

                cmd.Transaction = transaccion;

                int r = cmd.ExecuteNonQuery();

                Log("r (update puesto ) = " + r);



                return r;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1;
            }


        }


        public static Departamento InsertarDepartamento(SqlTransaction transaccion,
           SqlConnection conn, Departamento depto)
        {

            try
            {
                string sql = " INSERT INTO departamento(nombre, clave, activo) OUTPUT INSERTED.id_departamento    " +
                      " values (@nombre, @clave, 1) ";



                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                Log("insertar nuevo departamento = " + depto.Nombre);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@nombre", depto.Nombre);
                cmd.Parameters.AddWithValue("@clave", depto.Clave);

                cmd.Transaction = transaccion;

                int r = (int) cmd.ExecuteScalar();
                depto.IdDepartamento = r;
                             

                return depto;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return null;
            }


        }

        public static Puesto InsertarPuesto(SqlTransaction transaccion,
         SqlConnection conn, Puesto puesto)
        {

            try
            {
                string sql = " INSERT INTO puesto(nombre, clave, activo) OUTPUT INSERTED.id_puesto " +
                      " values (@nombre, @clave, 1) ";



                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                Log("insertar nuevo puesto = " + puesto.Nombre);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@nombre", puesto.Nombre);
                cmd.Parameters.AddWithValue("@clave", puesto.Clave);

                cmd.Transaction = transaccion;

                int r = (int) cmd.ExecuteScalar();
                puesto.IdPuesto = r;

                return puesto;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return null;
            }


        }

        public static DatosInsertar InsertarRegistro(string path, string nombreTabla,
            
            
            SqlConnection conn,
            SqlTransaction transaction,
            DatosInsertar insert,
            string usuario,
            string nombreTipo,
            string strConexion
            )
        {


            Empleado empleado = null;
            string claveEmppleado = insert.parts[0];

            Log("__________________ START claveEmppleado =  '" + claveEmppleado + "'");

            empleado = GetEmpleadoByClave(transaction, conn, strConexion, claveEmppleado);

            if (empleado != null)
            {
                Log("Nombre empleado encontrado = " + empleado.Nombre.ToString());
                Log("Buscar su departamento = " + insert.parts[4]);
                Log("Buscar su puesto = " + insert.parts[8]);

                Log("Este empleado ya existe");
                Log("");
                //Analizar su puesto y departamento, si cambió reasignarlo con un update
                Puesto puesto = GetItemPuestoByClave(transaction, conn, strConexion, insert.parts[8]);
                Departamento depto = GetItemDepartamentoByClave(transaction, conn, strConexion, insert.parts[4]);//DEPARTAMENTO U OBRA

                Log("Departamento encontrado = " + depto);
                
                if (puesto == null)//   insertar nuevo puesto al sistema
                {
                    Puesto nuevoPueso = new Puesto();
                    nuevoPueso.Clave = insert.parts[8];
                    nuevoPueso.Nombre= insert.parts[9];

                    puesto =  InsertarPuesto(transaction, conn, nuevoPueso);

                    ActualizarPuesto(transaction, conn, strConexion, puesto,
                           empleado.IdEmpleado.ToString());
                }

                if (depto == null)//   insertar nuevo departamento al sistema
                {
                    Departamento nuevoDepartento= new Departamento();
                    nuevoDepartento.Clave = insert.parts[4];
                    nuevoDepartento.Nombre = insert.parts[5];

                    depto = InsertarDepartamento(transaction, conn, nuevoDepartento);

                    ActualizarDepartamento(transaction, conn, strConexion, depto,
                           empleado.IdEmpleado.ToString());

                }


                if (puesto != null && empleado.IdPuesto == puesto.IdPuesto)
                {
                    Log("Sigue teniendo el mismo puesto");

                    ActualizarDepartamento(transaction, conn, strConexion, depto,
                           empleado.IdEmpleado.ToString());


                }
                else
                {
                    if (puesto != null)
                    {
                        Log("Actualizarle el puesto");

                        ActualizarPuesto(transaction, conn, strConexion, puesto,
                            empleado.IdEmpleado.ToString());
                    }
                    else
                    {
                        Log("El puesto no existe, no se le puede actualizar");

                    }

                }

                if (depto != null && empleado.IdDepartamento == depto.IdDepartamento)
                {
                    Log("Sigue teniendo el mismo departamento");

                }
                else
                {
                    if (depto != null)
                    {
                        Log("Actualizarle el departamento");
                        ActualizarDepartamento(transaction, conn, strConexion, depto,
                            empleado.IdEmpleado.ToString());
                    }
                    else
                    {
                        Log("El departamento no existe, no se le puede actualizar");

                    }

                }


                SetEmpleadoActivo(transaction, conn, strConexion, claveEmppleado);


            }
            else
            {
                Log("Este empleado NO existe.......................");
                Log("claveEmppleado =  '" + claveEmppleado + "'");
                Log("Nombre =  '" + insert.parts[1] + "'");

                Log("Insertandolo.......................");
                
                //Armar datos de empleado e insertarlo
                Empleado nuevoEmpleado = new Empleado();
                
                nuevoEmpleado.Clave = insert.parts[0];
                nuevoEmpleado.Nombre = insert.parts[1];
                nuevoEmpleado.APaterno = insert.parts[2];
                nuevoEmpleado.AMaterno = insert.parts[3];

                Puesto puesto = GetItemPuestoByClave(transaction, conn, strConexion, insert.parts[8]);
                Departamento depto = GetItemDepartamentoByClave(transaction, conn, strConexion, insert.parts[4]);//DEPARTAMENTO U OBRA

                if (puesto != null)
                    nuevoEmpleado.IdPuesto = puesto.IdPuesto;
                else
                    nuevoEmpleado.IdPuesto = 0;

                if (depto != null)
                    nuevoEmpleado.IdDepartamento = depto.IdDepartamento;
                else
                    nuevoEmpleado.IdDepartamento = 0;


                nuevoEmpleado.FechaAlta = insert.parts[10];
                nuevoEmpleado.SalarioMensual = insert.parts[11];


                string sql = "INSERT INTO empleado (clave, nombre, apellido_paterno, apellido_materno, " +
                    " id_puesto, id_departamento, fecha_alta, salario_mensual, activo) " +
                        "VALUES (@clave, @nombre, @apellido_paterno, @apellido_materno, " +
                    " @id_puesto, @id_departamento, @fecha_alta, @salario_mensual, @activo) ";

                try
                {

                    Utils.Log("\nMétodo-> " +
                   System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValue("@clave", nuevoEmpleado.Clave);
                    cmd.Parameters.AddWithValue("@nombre", nuevoEmpleado.Nombre);
                    cmd.Parameters.AddWithValue("@apellido_paterno", nuevoEmpleado.APaterno);
                    cmd.Parameters.AddWithValue("@apellido_materno", nuevoEmpleado.AMaterno);
                    cmd.Parameters.AddWithValue("@id_puesto", nuevoEmpleado.IdPuesto);
                    cmd.Parameters.AddWithValue("@id_departamento", nuevoEmpleado.IdDepartamento);
                    cmd.Parameters.AddWithValue("@fecha_alta", nuevoEmpleado.FechaAlta);
                    cmd.Parameters.AddWithValue("@salario_mensual", nuevoEmpleado.SalarioMensual);
                    cmd.Parameters.AddWithValue("@activo", 1);

                    int r = cmd.ExecuteNonQuery();

                    string descripcionLog = "";
                    if (r > 0)
                    {
                        descripcionLog = "Inserción correcta de registro de " + nombreTipo +
                                " SQL =  " + sql;


                        Plataforma.pages.Index.RegistrarLogCambios(path, usuario, descripcionLog, nombreTipo + "  - Subir " + nombreTipo);

                        Utils.Log("Registros insertados " + r);

                    }
                    else
                    {
                        descripcionLog = "Inserción incorrecta de registro de " + nombreTipo +
                               " SQL =  " + sql;


                        Plataforma.pages.Index.RegistrarLogCambios(path, usuario, descripcionLog, nombreTipo + "  - Subir " + nombreTipo);

                    }



                }
                catch (Exception ex)
                {


                    Utils.Log("Error ... " + ex.Message);
                    Utils.Log(ex.StackTrace);


                    insert.valido = false;
                    try
                    {
                        string descripcionLog = "Inserción incorrecta de registro de " + nombreTipo +
                          " SQL =  " + sql + " Error: " + ex.Message;

                        Plataforma.pages.Index.RegistrarLogCambios(path, usuario, descripcionLog, nombreTipo + "  - Subir " + nombreTipo);

                        transaction.Rollback();
                    }
                    catch (Exception exRolBack)
                    {
                        Utils.Log("Error ... " + exRolBack.Message);
                    }
                }
            }

            return insert;


        }


       


        public static void Log(string texto)
        {
            System.Diagnostics.Debug.Print(texto);



        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}