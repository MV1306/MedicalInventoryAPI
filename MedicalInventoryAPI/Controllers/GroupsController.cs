using MedicalInventoryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Reflection.PortableExecutable;
using static MedicalInventoryAPI.Models.CreateModels;

namespace MedicalInventoryAPI.Controllers
{
    [Route("api/Master/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly CommonMethods _commonMethods;

        public GroupsController(IConfiguration configuration, CommonMethods commonMethods)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString1");
            _commonMethods = commonMethods;
        }

        [HttpPost]
        [Route("CreateGroup")]
        public string CreateGroup(CreateModels.Group_Ip ip)
        {
            CreateModels.API_Response response = new CreateModels.API_Response();
            string opjsonstring = "";
            API_Log_Ip apiCallIp = new API_Log_Ip();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string CountQuery = "Select COUNT(*) from Groups Where Grp_Code = @Code";
                    using (SqlCommand cmd = new SqlCommand(CountQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@Code", ip.Code);
                        con.Open();
                        int count = (int)cmd.ExecuteScalar();
                        if(count > 0) 
                        {
                            response.ResCode = "400";
                            response.ResMsg = "Group - " + ip.Code + " already exists";
                            response.Response = null;
                            opjsonstring = JsonConvert.SerializeObject(response);
                            apiCallIp.Method = "Create Group";
                            apiCallIp.Request = JsonConvert.SerializeObject(ip);
                            apiCallIp.Response = JsonConvert.SerializeObject(response);
                            apiCallIp.Status = "400";
                            _commonMethods.APILogInsert(apiCallIp);
                        }
                        else
                        {
                            string insertQuery = "INSERT INTO Groups(Grp_Type,Grp_Code,Grp_Name,Grp_Prefix,Grp_Prefix_PrevNo,IsActive,CreatedUser,CreatedDt) VALUES (@Type,@Code,@Name,@Prefix,@PrevNo,@Active,@CrtdUser,@CrtdDt)";
                            using (SqlCommand cmd1 = new SqlCommand(insertQuery, con))
                            {
                                cmd1.Parameters.AddWithValue("@Type",ip.Type);
                                cmd1.Parameters.AddWithValue("@Code",ip.Code);
                                cmd1.Parameters.AddWithValue("@Name",ip.Name);
                                cmd1.Parameters.AddWithValue("@Prefix",ip.CodePrefix);
                                cmd1.Parameters.AddWithValue("@PrevNo",0);
                                cmd1.Parameters.AddWithValue("@Active",true);
                                cmd1.Parameters.AddWithValue("@CrtdUser",ip.CrtdUser);
                                cmd1.Parameters.AddWithValue("@CrtdDt", DateTime.Now);

                                int rowsAffected = cmd1.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    response.ResCode = "200";
                                    response.ResMsg = "Group - " + ip.Code + " created successfully";
                                    response.Response = null;
                                    opjsonstring = JsonConvert.SerializeObject(response);
                                    apiCallIp.Method = "Create Group";
                                    apiCallIp.Request = JsonConvert.SerializeObject(ip);
                                    apiCallIp.Response = JsonConvert.SerializeObject(response);
                                    apiCallIp.Status = "200";
                                    _commonMethods.APILogInsert(apiCallIp);
                                }
                                else
                                {
                                    response.ResCode = "400";
                                    response.ResMsg = "Group - " + ip.Code + " creation failed";
                                    response.Response = null;
                                    opjsonstring = JsonConvert.SerializeObject(response);
                                    apiCallIp.Method = "Create Group";
                                    apiCallIp.Request = JsonConvert.SerializeObject(ip);
                                    apiCallIp.Response = JsonConvert.SerializeObject(response);
                                    apiCallIp.Status = "400";
                                    _commonMethods.APILogInsert(apiCallIp);
                                }
                            }
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
                apiCallIp.Method = "Create Group";
                apiCallIp.Request = JsonConvert.SerializeObject(ip);
                apiCallIp.Response = JsonConvert.SerializeObject(response);
                apiCallIp.Status = "500";
                _commonMethods.APILogInsert(apiCallIp);
            }
            return opjsonstring;
        }
    }
}
