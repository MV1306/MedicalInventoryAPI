using MedicalInventoryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;
using static MedicalInventoryAPI.Models.CreateModels;

namespace MedicalInventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly CommonMethods _commonMethods;

        public LoginController(IConfiguration configuration, CommonMethods commonMethods)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString1");
            _commonMethods = commonMethods;
        }

        [HttpPost]
        [Route("ValidateLogin")]
        public string ValidateLogin(CreateModels.LoginIp ip)
        {
            CreateModels.API_Response response = new CreateModels.API_Response();
            string opjsonstring = "", user = "", email = "", pass = "", mobile = "";
            int id = 0;
            API_Log_Ip apiCallIp = new API_Log_Ip();
            DateTime LastLogin = DateTime.Now;
            try
            {
                using(SqlConnection con = new SqlConnection(_connectionString))
                {
                    string selectQuery = "SELECT * FROM Users WHERE Username = @User";
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(selectQuery,con))
                    {
                        cmd.Parameters.AddWithValue("@User", ip.Username);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                id = reader.GetInt32(reader.GetOrdinal("UserID"));
                                user = reader.GetString(reader.GetOrdinal("Username"));
                                email = reader.GetString(reader.GetOrdinal("Email"));
                                mobile = reader.GetString(reader.GetOrdinal("Phone"));
                                pass = reader.GetString(reader.GetOrdinal("Password"));
                            }

                            reader.Close();

                            if (ip.Password != pass)
                            {
                                response.ResCode = "400";
                                response.ResMsg = "Invalid Password";
                                response.Response = null;
                                opjsonstring = JsonConvert.SerializeObject(response);
                                apiCallIp.Method = "Validate Login";
                                apiCallIp.Request = JsonConvert.SerializeObject(ip);
                                apiCallIp.Response = JsonConvert.SerializeObject(response);
                                apiCallIp.Status = "400";
                                _commonMethods.APILogInsert(apiCallIp);
                            }
                            else
                            {
                                string updateQuery = "UPDATE Users SET LastLoginDt = @Date WHERE Username = @Username";

                                LastLogin = DateTime.Now;

                                using (SqlCommand cmd1 = new SqlCommand(updateQuery,con))
                                {
                                    cmd1.Parameters.AddWithValue("@Date", LastLogin);
                                    cmd1.Parameters.AddWithValue("@Username", ip.Username);
                                    cmd1.ExecuteNonQuery();
                                }

                                var finalRes = new
                                {
                                    UserId = id,
                                    Username = user,
                                    Email = email,
                                    Mobile = mobile,
                                    LastLoginDt = LastLogin
                                };

                                response.ResCode = "200";
                                response.ResMsg = ip.Username + " - User logged in successfully";
                                response.Response = finalRes;
                                opjsonstring = JsonConvert.SerializeObject(response);
                                apiCallIp.Method = "Validate Login";
                                apiCallIp.Request = JsonConvert.SerializeObject(ip);
                                apiCallIp.Response = JsonConvert.SerializeObject(response);
                                apiCallIp.Status = "200";
                                _commonMethods.APILogInsert(apiCallIp);
                            }
                        }
                        else
                        {
                            response.ResCode = "400";
                            response.ResMsg = ip.Username + " - User does not exists";
                            response.Response = null;
                            opjsonstring = JsonConvert.SerializeObject(response);
                            apiCallIp.Method = "Validate Login";
                            apiCallIp.Request = JsonConvert.SerializeObject(ip);
                            apiCallIp.Response = JsonConvert.SerializeObject(response);
                            apiCallIp.Status = "400";
                            _commonMethods.APILogInsert(apiCallIp);
                        }
                    }    
                }    
            }
            catch (Exception ex) 
            {
                response.ResCode = "500";
                response.ResMsg = ex.Message;
                response.Response = null;
                opjsonstring = JsonConvert.SerializeObject(response);
                apiCallIp.Method = "Validate Login";
                apiCallIp.Request = JsonConvert.SerializeObject(ip);
                apiCallIp.Response = JsonConvert.SerializeObject(response);
                apiCallIp.Status = "500";
                _commonMethods.APILogInsert(apiCallIp);
            }
            return opjsonstring;
        }
    }
}
