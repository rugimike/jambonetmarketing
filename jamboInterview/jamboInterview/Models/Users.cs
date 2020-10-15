using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jamboInterview.Models
{
    //pojo
    public class Users
    {  
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Branch { get; set; }
        public string UserType { get; set; }
        public string Status { get; set; }
        public string Ambascode { get; set; }
    }

    public class Transaction
    {
        public string Reference { get; set; }
        public string Supporter { get; set; }
        public string Customer { get; set; }
        public string Transtype { get; set; }
        public string Modeofpay { get; set; }
        public string Amount { get; set; }
    }




    public class UserToken
    {
        public string utoken { get; set; }
    }

    public class AUser
    {
        public string userid { get; set; }
        public string secret { get; set; }
    }
}