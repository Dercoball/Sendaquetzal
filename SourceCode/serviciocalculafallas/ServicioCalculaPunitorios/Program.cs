using VerifyStatusPaymentsService.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Twilio;
using System.Net;
using Twilio.Rest.Api.V2010.Account;
using Newtonsoft.Json.Linq;

namespace VerifyStatusPaymentsService
{
    class Program
    {
        //static void Main(string[] args)
        //{

        //    Log("Enviar mensaje de cumpleaños a empleados.");


        //    using (SQContext db = new SQContext())
        //    {

        //        // Message
        //        var message = db.TemplateMessage.Find(2);// El msg con id 2 es el configurado correctamente por ahora
        //        //  Debe ser texto plano


        //        DateTime date = DateTime.Now;
        //        int month = date.Month;
        //        int day = date.Day;


        //        //  Employees that today is their birthday
        //        var employeesList = db.Employee.SqlQuery(
        //            @" SELECT * FROM empleado 
        //             WHERE month(fecha_nacimiento) = @month AND day(fecha_nacimiento) = @day ",
        //            new SqlParameter("month", month),
        //            new SqlParameter("day", day));

        //        string summary = "";

        //        foreach(var employee in employeesList)
        //        {

        //            string employeeName = employee.nombre + " " + employee.primer_apellido;
        //            string msg = message.contenido;// "Hola {{1}} Feliz cumpleaños a uno de nuestros clientes más importantes de todos los tiempos. No seríamos absolutamente nada sin ti.";
        //            msg = msg.Replace("{{1}}", employeeName);

        //            Log(" Employee : " + employeeName);
        //            Log(" Telefono : " + employee.telefono);

        //            if (employee.telefono.Length == 10) {

        //                Log(" Send whatsapp msg to employee : " + employeeName);

        //                var result = SendMessageWhatsapp(msg, employee.telefono);
        //                Log(result);
        //                summary += "Mensaje de felicitación de cumpleados whatsapp a Empleado : " + employee.id_empleado + " - " + employeeName + " enviado correctamente. ";

        //            }
        //            else
        //            {
        //                summary += "Empleado : " + employee.id_empleado + " - " + employeeName +  " no cuenta con núm celular válido. ";
        //            }

        //        }
        //        //

        //        Log(summary);
        //        resumen_calculo_fallas summaryOperation = new resumen_calculo_fallas();
        //        summaryOperation.fecha = DateTime.Now;
        //        summaryOperation.observaciones= summary;
        //        db.SummaryOperation.Add(summaryOperation);
        //        db.SaveChanges();

        //    }

        //    Log("\n\nFin de actualizacion de status de pagos  ");


        //    Environment.Exit(0);


        //}

        static void Main(string[] args)
        {

            Log("Enviar mensaje de cumpleaños a empleados.");
            string strConexion = Properties.Settings.Default.connection;
            try
            {
                using (SqlConnection conn = new SqlConnection(strConexion))
                {

                    PaymentsDao dao = new PaymentsDao();


                    // Message
                    var messageStr = dao.GetMessageById("2", conn);    // El msg con id 2 es el configurado correctamente por ahora
                                                                       //  Debe ser texto plano


                    DateTime date = DateTime.Now;
                    int month = date.Month;
                    int day = date.Day;


                    var employeesList = dao.GetEmployees(month, day, conn);

                    string summary = "";

                    foreach (var employee in employeesList)
                    {

                        string employeeName = employee.Nombre + " " + employee.PrimerApellido;
                        string msg = messageStr;// "Hola {{1}} Feliz cumpleaños a uno de nuestros clientes más importantes de todos los tiempos. No seríamos absolutamente nada sin ti.";
                                                //    //string msg = message.contenido;// "Hola {{1}} Feliz cumpleaños a uno de nuestros clientes más importantes de todos los tiempos. No seríamos absolutamente nada sin ti.";
                        msg = msg.Replace("{{1}}", employeeName + ".");

                        Log(" Employee : " + employeeName);
                        Log(" Telefono : " + employee.Telefono);

                        if (employee.Telefono.Length == 10)
                        {

                            Log(" Send whatsapp msg to employee : " + employeeName);

                            var result = SendMessageWhatsapp(msg, employee.Telefono);
                            Log(result);
                            summary += "Mensaje de felicitación de cumpleados whatsapp a Empleado : " + employee.IdEmpleado + " - " + employeeName + " enviado correctamente. ";

                        }
                        else
                        {
                            summary += "Empleado : " + employee.IdEmpleado + " - " + employeeName + " no cuenta con núm celular válido. ";
                        }

                    }


                    //dao.InsertLog(summary, conn);

                }

            }
            catch (Exception ex)
            {
                Log(ex.Message);
                Log(ex.StackTrace);


            }

            Log("\n\nFin del proceso de envio de notificaciones.");


            Environment.Exit(0);


        }


        public static string SendMessageWhatsapp(string msg, string celular)
        {
            try
            {
                Log("");
                Log("SendMessageWhatsapp");


                string accountSid = Properties.Settings.Default.TWILIO_ACCOUNT_SID;
                string authToken = Properties.Settings.Default.TWILIO_AUTH_TOKEN;
                string celFromWhatsapp = Properties.Settings.Default.CEL_FROM_WP;

                Log("celFrom Whatsapp " + celFromWhatsapp);

                celular = "whatsapp:+521" + celular;

                msg = msg.Replace("\n", "");
                msg = msg.Trim();

                Log("msg       " + msg);
                Log("to Celular       " + celular);


                TwilioClient.Init(accountSid, authToken);

                System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                // WHATSAPP
                var message = MessageResource.Create(
                  body: msg,
                  from: new Twilio.Types.PhoneNumber(celFromWhatsapp),
              to: new Twilio.Types.PhoneNumber(celular)
              );


                Log("message.Sid " + message.Sid.ToString());
                Log("message.Status " + message.Status.ToString());
                Log("message.ErrorMessage" + message.ErrorMessage);
                Log("");


                return message.Sid;
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                Log(ex.StackTrace);

                return "";

            }

        }

        public static void Log(string msg)
        {
            System.Diagnostics.Debug.Print(msg);
            Console.WriteLine(msg);

        }

    }
}
