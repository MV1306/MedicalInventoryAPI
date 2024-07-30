using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MedicalInventoryAPI.Models;
using Microsoft.Data.SqlClient;

namespace MedicalInventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private readonly string _connectionString;

        public MasterController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString1");
        }

        [HttpPost]
        [Route("CreateUsers")]
        public string CreateUser(CreateModels.Users ip)
        {
            try
            {
                if (ip != null)
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        string InsertQuery = "Insert into Users (Username,Password,Email,Phone,IsActive,CreatedDt) VALUES (@Username,@Password,@Email,@Phone,@Active,@CrtdDt); SELECT SCOPE_IDENTITY()";
                        using (SqlCommand cmd = new SqlCommand(InsertQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@Username", ip.Username);
                            cmd.Parameters.AddWithValue("@Password", ip.Password);
                            cmd.Parameters.AddWithValue("@Email", ip.Email);
                            cmd.Parameters.AddWithValue("@Phone", ip.Phone);
                            cmd.Parameters.AddWithValue("@Active", true);
                            cmd.Parameters.AddWithValue("@CrtdDt", DateTime.Now);

                            con.Open();

                            int PK_ID = Convert.ToInt32(cmd.ExecuteScalar());

                            con.Close();

                            if(PK_ID != 0)
                            {
                                return "User created successfully. UserID - " + PK_ID;
                            }
                            else
                            {
                                return "Unable to create user";
                            }
                        }
                    }
                }
                else
                {
                    return "Input cannot be null";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
