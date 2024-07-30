using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using System;
using static MedicalInventoryAPI.Models.CreateModels;

namespace MedicalInventoryAPI
{
    public class CommonMethods
    {
        private readonly string _connectionString;

        public CommonMethods(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString1");
        }

        public void APILogInsert(API_Log_Ip ip)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string insertQuery = "INSERT INTO ApiCallLogs VALUES (@Method,@Request,@Response,@Status,@Date)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("@Method", ip.Method);
                    cmd.Parameters.AddWithValue("@Request", ip.Request);
                    cmd.Parameters.AddWithValue("@Response", ip.Response);
                    cmd.Parameters.AddWithValue("@Status", ip.Status);
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                    cmd.ExecuteScalar();
                }                
            }
        }
    }
}
