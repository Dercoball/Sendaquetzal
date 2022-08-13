
using System;
using System.Data.SqlClient;
using Twilio;
using System.Net;
using Twilio.Rest.Api.V2010.Account;
using ServicioEnviaMsgsNuevoCredito;
using System.Collections.Generic;

namespace VerifyStatusPaymentsService
{
    class Program
    {
        static void Main(string[] args)
        {

            Log("Enviar mensaje de nuevo credito aprobado a clientes.");


            using (SQContext db = new SQContext())
            {

                // Message
                var message = db.TemplateMessage.Find(3);// El msg con id 3
                //  Debe ser texto plano
                //Hola {{1}}
                //Queremos felicitarte por ser un cliente puntual, al mismo tiempo comunicarte que tienes otro préstamo disponible con nosotros.


                //  Customers in inactive status and their last payment is >=30 days
                var customerList =  db.Customer.SqlQuery(
                    @"     SELECT * FROM cliente c
                     WHERE c.id_status_cliente = 1 AND 
                     
                       (
                         select * from  (                                          
                         select datediff(d,
                           (SELECT top 1 fecha  FROM pago
                               WHERE id_prestamo = (
                                          select top 1 id_prestamo from prestamo
                                          where id_cliente = c.id_cliente ORDER BY id_prestamo desc
                           ) ORDER BY id_pago desc ), getdate() ) as diff                          
           	            ) as dias_transcurridos             
                       ) >=30


                    ");

                string summary = "";

                var exists = false;
                foreach (var itemCustomer in customerList)
                {
                    exists = true;

                    string customerName = itemCustomer.nombre + " " + itemCustomer.primer_apellido;
                    string msg = message.contenido;// 
                    msg = msg.Replace("{{1}}", customerName);

                    Log(" Employee : " + customerName);
                    Log(" Telefono : " + itemCustomer.telefono);

                    if (itemCustomer.telefono.Length == 10)
                    {

                        Log(" Send whatsapp msg to employee : " + customerName);

                        var result = SendMessageWhatsapp(msg, itemCustomer.telefono);
                        Log(result);
                        summary += "Mensaje de promoción de nuevo préstamo vía whatsapp a Cliente : " + itemCustomer.id_cliente + " - " + customerName + " enviado correctamente. ";

                    }
                    else
                    {
                        summary += "Cliente : " + itemCustomer.id_cliente + " - " + customerName + " no cuenta con núm celular válido. ";
                    }

                }

                if (exists == false)
                {
                    summary = " No se encontraron clientes con status inactivo y con su último pago hecho hace mas de 30 días.";
                }
                //

                Log(summary);
                resumen_calculo_fallas summaryOperation = new resumen_calculo_fallas();
                summaryOperation.fecha = DateTime.Now;
                summaryOperation.observaciones = summary;
                db.SummaryOperation.Add(summaryOperation);
                db.SaveChanges();

            }

            Log("\n\nFin del proceso.");


            Environment.Exit(0);


        }


        public static string SendMessageWhatsapp(string msg, string celular)
        {
            try
            {
                Log("");
                Log("SendMessageWhatsapp");


                string accountSid = ServicioEnviaMsgsNuevoCredito.Properties.Settings.Default.TWILIO_ACCOUNT_SID;
                string authToken = ServicioEnviaMsgsNuevoCredito.Properties.Settings.Default.TWILIO_AUTH_TOKEN;
                string celFromWhatsapp = ServicioEnviaMsgsNuevoCredito.Properties.Settings.Default.CEL_FROM_WP;

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
