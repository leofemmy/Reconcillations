using Serilog;
using System.Data.SqlClient;
using System.Data;
using System;
using Microsoft.Extensions.Configuration;

namespace Reconcillations.HangFireServices
{
    public class ServiceScheduler
    {
        private readonly IConfiguration _iconfiguration;
        public ServiceScheduler(IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
        }
        public void DoReemsPushAsync()
        {
            //var connectionString = this.GetConnection();

            string connectionString = _iconfiguration.GetConnectionString("DefaultConnection");
            try
            {
                SqlDataAdapter _adp;

                DataSet response = new ();

                using (SqlConnection con = new (connectionString))
                {
                    if (con.State != ConnectionState.Closed)
                    {
                        con.Close();
                    }

                    SqlCommand cmd = new ("spdoGetRecordtoReems", con);

                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    cmd.CommandTimeout = 0;

                    response.Clear();
                    _adp = new SqlDataAdapter(cmd);
                    _adp.Fill(response);


                    con.Close();

                    var loggers = new LoggerConfiguration()
                        .WriteTo.MSSqlServer(connectionString, "Logs")
                        .CreateLogger();
                    loggers.Information($"Reems Push To Successfully ");

                }
            }
            catch (Exception e)
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.MSSqlServer(connectionString, "Logs")
                    .CreateLogger();

                logger.Fatal($"Do Reems Push thrown an error - {e.Message}");
            }
        }
    }
}
