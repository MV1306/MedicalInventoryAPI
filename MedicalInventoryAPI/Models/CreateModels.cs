namespace MedicalInventoryAPI.Models
{
    public class CreateModels
    {
        public class Users
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }

        public class LoginIp
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class API_Response
        {
            public string ResCode { get; set; }
            public string ResMsg { get; set; }
            public dynamic Response { get; set; }
        }

        public class API_Log_Ip
        {
            public string Method { get; set;}
            public string Request { get; set;}
            public string Response { get; set;}
            public string Status { get; set;}
        }

        public class Group_Ip
        {
            public string Type { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string CodePrefix { get; set; }
            public string CrtdUser { get; set; }
        }
    }
}
