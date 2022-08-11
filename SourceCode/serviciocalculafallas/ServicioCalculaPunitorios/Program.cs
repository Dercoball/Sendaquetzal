using VerifyStatusPaymentsService.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace VerifyStatusPaymentsService
{
    class Program
    {
        static void Main(string[] args)
        {

            PaymentsDao dao = new PaymentsDao();

            Log("Proceso de actualizacion de status de pagos  ");


            DateTime hora = DateTime.Now;
            int hora_ = hora.Hour;
            int min_ = hora.Minute + 1;   //  real
            //int min_ = hora.Minute;         //  test

            Log("Iniciando a las  " + hora_ + " : " + min_ + " hrs ");

            // For Interval in Seconds 
            // This Scheduler will start at 11:10 and call after every 15 Seconds
            // IntervalInSeconds(start_hour, start_minute, seconds)

            Log("Inicio  de actualizacion de status de pagos  ");

            //  fechas
            DateTime endWeekDate = DateTime.Now;
            var numDayOfweek = (int)endWeekDate.DayOfWeek;
            endWeekDate = new DateTime(endWeekDate.Year, endWeekDate.Month, endWeekDate.Day);
            endWeekDate = endWeekDate.AddDays(7);
            endWeekDate = endWeekDate.AddDays(-numDayOfweek);
            Log("Fecha final de la semana: " + endWeekDate);

            DateTime startWeekDate = DateTime.Now;
            var numDayOfweek2 = (int)startWeekDate.DayOfWeek;
            startWeekDate = startWeekDate.AddDays(-numDayOfweek2);
            Log("Fecha inicial de la semana : " + startWeekDate);


            string startDateStr = startWeekDate.Year.ToString() + "-" + startWeekDate.Month.ToString() + "-" + startWeekDate.Day.ToString();
            string endDateStr = endWeekDate.Year.ToString() + "-" + endWeekDate.Month.ToString() + "-" + endWeekDate.Day.ToString();


            string strConexion = Properties.Settings.Default.connection;

            SqlConnection conn = new SqlConnection(strConexion);
            SqlTransaction transaction = null;

            int r = 0;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();

                // 1. Traer los pagos de la semana de los prestamos activos
                List<Pago> payments = dao.GetPayments(Pago.STATUS_PAGO_PENDIENTE.ToString(), startDateStr, endDateStr, conn, transaction);
                Log("Pagos encontrados pendientes para esta semana :" + payments.Count);

                //  Se necesita saber que dia de la semana es
                DateTime today = DateTime.Now;
                var dayOfWeek = (int)today.DayOfWeek;
                Log("dayOfWeek : " + dayOfWeek);

                List<Pago> paymentsFoundUpdate = new List<Pago>();
                List<Pago> paymentsLastWeekFoundUpdate = new List<Pago>();

                foreach (var itemPayment in payments)
                {

                    Log("\n\n\n...........................................Inicio de análisis del pago :" + itemPayment.IdPago);

                    // Obtener el cliente
                    Cliente customer = itemPayment.cliente;
                    Log("IdCliente     : " + customer.IdCliente);
                    Log("Cliente     : " + customer.NombreCompleto);
                    Log("IdTipoCliente : " + customer.IdTipoCliente);
                    Log("IdTipoCliente : " + customer.IdTipoCliente);


                    //  Traer la configuración del tipo de clienta para cada pago
                    TipoCliente configCliente = dao.GetCustomerTypeById(customer.IdTipoCliente.ToString(), conn, transaction);
                    Log("TipoCliente : " + configCliente.NombreTipoCliente);


                    //  Ver cuales son sus días de pago
                    Log("\n\nDias de pago .........: ");
                    Log("FechaPagoLunes : " + configCliente.FechaPagoLunes);
                    Log("FechaPagoMartes : " + configCliente.FechaPagoMartes);
                    Log("FechaPagoMiercoles : " + configCliente.FechaPagoMiercoles);
                    Log("FechaPagoJueves : " + configCliente.FechaPagoJueves);
                    Log("FechaPagoViernes : " + configCliente.FechaPagoViernes);
                    Log("FechaPagoSabado : " + configCliente.FechaPagoSabado);
                    Log("FechaPagoDomingo : " + configCliente.FechaPagoDomingo);
                    Log("\n");

                    int[] dias = new int[7];
                    dias[0] = configCliente.FechaPagoLunes == 1 ? 1 : 0;
                    dias[1] = configCliente.FechaPagoMartes == 1 ? 2 : 0;
                    dias[2] = configCliente.FechaPagoMiercoles == 1 ? 3 : 0;
                    dias[3] = configCliente.FechaPagoJueves == 1 ? 4 : 0;
                    dias[4] = configCliente.FechaPagoViernes == 1 ? 5 : 0;
                    dias[5] = configCliente.FechaPagoSabado == 1 ? 6 : 0;
                    dias[6] = configCliente.FechaPagoDomingo == 1 ? 7 : 0;

                    int maxday = dias[0];
                    for (int i = 1; i < dias.Length; i++)
                    {
                        if (dias[i] > maxday)
                        {
                            maxday = dias[i];
                        }

                    }

                    Log("Día máximo de pago: (1 = lunes, ... 7 = domingo) " + maxday);

                    if (dayOfWeek > maxday)
                    {
                        //  
                        Log("\nEn este punto este pago se encuentra en Falla ... ");

                        int rowsAffected = dao.UpdatePymentStatus(itemPayment.IdPago, Pago.STATUS_PAGO_FALLA, conn, transaction);

                        Log("Filas modificadas en Cambio de status a Falla : " + rowsAffected);

                    }
                    else
                    {
                        Log("El pago " + itemPayment.IdPago + " aún se encuentra con status de pago pendiente...");
                    }



                }


                transaction.Commit();


            }
            catch (Exception ex)
            {

                transaction.Rollback();

                Log("Error ... " + ex.Message);
                Log(ex.StackTrace);
                r = -1;
            }

            finally
            {
                conn.Close();
            }




            //Log("Registros modificados = " + registros);

            Log("\n\nFin de actualizacion de status de pagos  ");



            Console.ReadLine();

        }

        public static void Log(string msg)
        {
            System.Diagnostics.Debug.Print(msg);
            Console.WriteLine(msg);

        }

    }
}
