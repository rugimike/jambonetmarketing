using jamboInterview.Auth;
using jamboInterview.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace jamboInterview.Controllers
{
    public class AccountController : Controller
    {
        Connector conn = new Connector();
        JWTken jwtken = new JWTken();
        //create a user.. ambassador and supporter
        [HttpPost]
        public JToken createUser(Users user)
        {
            JToken returntoken = null;
            try
            {
                string employeesufix = "0";
                var requestHeaders = Request.Headers;
                if (Request.Headers["AUTHTOKEN"] != null && Request.Headers["AUTHKEY"] != null)
                {
                    var token = requestHeaders.GetValues("AUTHTOKEN").First();
                    var secret = requestHeaders.GetValues("AUTHKEY").First();
                    bool validateToken = jwtken.validateToken(token, secret);
                    if (validateToken)
                    {
                        string createusers = "";
                        if (user.UserType.ToString().Trim() == "AMBS")
                        {
                            string employeepref = "A";
                            string getdeviceid = " select case when max(id) is null then '1000000000' else 1000000000+max(id)+1 end as uid from tblAmbassador ";
                            DataTable returnTable = conn.NormalSelect(getdeviceid);
                            foreach (DataRow row in returnTable.Rows)
                            {
                                employeesufix = row["uid"].ToString();
                            }
                            string employeeid = employeepref + employeesufix.Substring(1, 9);

                            createusers = " insert into tblAmbassador values('" + employeeid + "','" + user.FirstName + "','" + user.OtherNames + "','" + user.Email + "', "
                                          + " '" + hashPass(user.Password) + "','" + user.Branch + "','" + user.UserType + "',0,0,'" + user.Status + "',GETDATE()) ";
                        }
                        else if (user.UserType.ToString().Trim() == "SPRT")
                        {
                            string employeepref = "S";
                            string getdeviceid = "select case when max(id) is null then '1000000000' else 1000000000+max(id)+1 end as uid from tblSupporter";
                            DataTable returnTable = conn.NormalSelect(getdeviceid);
                            foreach (DataRow row in returnTable.Rows)
                            {
                                employeesufix = row["uid"].ToString();
                            }
                            string employeeid = employeepref + employeesufix.Substring(1, 9);
                            createusers = " insert into tblSupporter values('" + employeeid + "','" + user.FirstName + "','" + user.OtherNames + "','" + user.Email + "', "
                                          + " '" + hashPass(user.Password) + "','" + user.Branch + "','" + user.UserType + "','" + user.Ambascode + "',0,0,'" + user.Status + "',GETDATE()) ";
                        }
                        else
                        {

                        }
                        string insertstatus = conn.PushDataToDb(createusers);
                        if (insertstatus.Contains("Success"))
                        {
                            returntoken = JToken.Parse("{'success':'" + insertstatus + "'}");
                        }
                        else
                        {
                            returntoken = JToken.Parse("{'error':'" + insertstatus + "'}");
                        }
                    }
                    else
                    {
                        returntoken = JToken.Parse("{'error':'invalidkeyortokenorexpired'}");
                    }
                }
                else
                {
                    returntoken = JToken.Parse("{'error':'provide key and token'}");
                }
            }catch(Exception e)
            {
                returntoken = JToken.Parse("{'error':'There was an error'}");
            }
            return returntoken;
        }


        //view single ambassador
        [HttpPost]
        public JToken getAmbassador(Users users)
        {
            JToken returntoken = null;
            var requestHeaders = Request.Headers;
            if (Request.Headers["AUTHTOKEN"] != null && Request.Headers["AUTHKEY"] != null)
            {
                var token = requestHeaders.GetValues("AUTHTOKEN").First();
                var secret = requestHeaders.GetValues("AUTHKEY").First();
                bool validateToken = jwtken.validateToken(token, secret);
                if (validateToken)
                {
                    string getusers = " select * from tblAmbassador where code ='"+ users.Code+ "' ";
                    string usersdata = conn.GetJsonString(getusers);
                    returntoken = JToken.Parse(usersdata).First;
                }
                else
                {
                    returntoken = JToken.Parse("{'error':'invalidkeyortokenorexpired'}");
                }
            }
            else
            {
                returntoken = JToken.Parse("{'error':'provide key and token'}");
            }
            return returntoken;
        }


        [HttpPost]
        public JToken createtrans(Transaction trans)
        {
            string transuffix = "0";
            JToken returntoken = null;
            try
            {
                var requestHeaders = Request.Headers;
                if (Request.Headers["AUTHTOKEN"] != null && Request.Headers["AUTHKEY"] != null)
                {
                    var token = requestHeaders.GetValues("AUTHTOKEN").First();
                    var secret = requestHeaders.GetValues("AUTHKEY").First();
                    bool validateToken = jwtken.validateToken(token, secret);
                    if (validateToken)
                    {
                        string transref = "T";
                        string getdeviceid = "select case when max(id) is null then '1000000000' else 1000000000+max(id)+1 end as transref from tblTransactions";
                        DataTable returnTable = conn.NormalSelect(getdeviceid);
                        foreach (DataRow row in returnTable.Rows)
                        {
                            transuffix = row["transref"].ToString();
                        }
                        string transactionreference = transref + transuffix.Substring(1, 9); ;
                        string createusers = " insert into tblTransactions values('" + transactionreference + "',GETDATE(),'" + trans.Supporter + "','" + trans.Customer + "', "
                                           + " '" + trans.Transtype + "','" + trans.Modeofpay + "'," + trans.Amount + ",'0') ";
                        string insertstatus = conn.PushDataToDb(createusers);
                        if (insertstatus.Contains("Success"))
                        {
                            string ambassador = getAmbassador(trans.Supporter);
                            string commissionrate = getRate("AMBS", trans.Transtype);
                            double commAmt = 0.01 * Convert.ToDouble(commissionrate) * Convert.ToDouble(trans.Amount);
                            string insertcommission = " insert into tblCommissions values('" + transactionreference + "','" + ambassador + "'," + commAmt + ",GETDATE()) ";
                            string commissionStatus = conn.PushDataToDb(insertcommission);
                            if (commissionStatus.Contains("Success"))
                            {
                                string updatetrans = " update  tblTransactions set assigned = '1' where Reference = '" + transactionreference + "' ";
                                conn.PushDataToDb(updatetrans);

                                string updatecredit = " update  tblAmbassador set totalcredit = totalcredit+" + commAmt + "  where code = '" + ambassador + "' ";
                                conn.PushDataToDb(updatecredit);
                            }
                            //add to the customer.
                            returntoken = JToken.Parse("{'message':'" + insertstatus + "'}");
                        }
                        else
                        {
                            returntoken = JToken.Parse("{'message':'" + insertstatus + "'}");
                        }
                    }
                    else
                    {
                        returntoken = JToken.Parse("{'message':'invalidkeyortokenorexpired'}");
                    }
                }
                else
                {
                    returntoken = JToken.Parse("{'message':'provide key and token'}");
                }
            }
            catch (Exception e)
            {
                returntoken = JToken.Parse("{'message':'there was an error'}");
            }
            return returntoken;
        }

        [HttpPost]
        public JToken getsingletrans(Transaction trans)
        {
            JToken returntoken = null;
            var requestHeaders = Request.Headers;
            if (Request.Headers["AUTHTOKEN"] != null && Request.Headers["AUTHKEY"] != null)
            {
                var token = requestHeaders.GetValues("AUTHTOKEN").First();
                var secret = requestHeaders.GetValues("AUTHKEY").First();
                bool validateToken = jwtken.validateToken(token, secret);
                if (validateToken)
                {
                    string getusers = " select * from tblTransactions where Reference ='" + trans.Reference+ "' ";
                    string usersdata = conn.GetJsonString(getusers);
                    returntoken = JToken.Parse(usersdata).First;
                }
                else
                {
                    returntoken = JToken.Parse("{'error':'invalidkeyortokenorexpired'}");
                }
            }
            else
            {
                returntoken = JToken.Parse("{'error':'provide key and token'}");
            }
            return returntoken;
        }
        


        public string getAmbassador(string supporter)
        {
            string ambassd = "";
            string ratequery = " select Ambassador from tblSupporter where UserType='SPRT' and Code='"+ supporter + "' ";
            DataTable returnTariffName = conn.NormalSelect(ratequery);
            foreach (DataRow row in returnTariffName.Rows)
            {
                ambassd = row["Ambassador"].ToString();
            }
            return ambassd;
        }

        //get the rate
        public string getRate(string usertype,string servicecode)
        {
            string rate = "";
            string ratequery = " select rate from tblRates where UserType='"+ usertype + "' and ServiceCode='"+ servicecode + "' ";
            DataTable returnTariffName = conn.NormalSelect(ratequery);
            foreach (DataRow row in returnTariffName.Rows)
            {
                rate = row["rate"].ToString();
            }
            return rate;
        }
        

        [HttpGet]
        public JToken getAmbasadors()
        {
            JToken returntoken = null;
            var requestHeaders = Request.Headers;
            if (Request.Headers["AUTHTOKEN"] != null  && Request.Headers["AUTHKEY"] != null)
            {
                var token = requestHeaders.GetValues("AUTHTOKEN").First();
                var secret = requestHeaders.GetValues("AUTHKEY").First();
                bool validateToken = jwtken.validateToken(token, secret);
                if (validateToken)
                {
                    string getusers = " select * from tblAmbassador";
                    string usersdata = conn.GetJsonString(getusers);
                    returntoken = JToken.Parse(usersdata);
                }
                else
                {
                    returntoken = JToken.Parse("{'error':'invalidkeyortokenorexpired'}");
                }
            }
            else
            {
                returntoken = JToken.Parse("{'error':'provide key and token'}");
            }
            return returntoken;
        }

        public JToken getservices()
        {
            JToken returntoken = null;
            var requestHeaders = Request.Headers;
            if (Request.Headers["AUTHTOKEN"] != null && Request.Headers["AUTHKEY"] != null)
            {
                var token = requestHeaders.GetValues("AUTHTOKEN").First();
                var secret = requestHeaders.GetValues("AUTHKEY").First();
                bool validateToken = jwtken.validateToken(token, secret);
                if (validateToken)
                {
                    string getusers = " select * from tbllookup where tableid='TRATP'";
                    string usersdata = conn.GetJsonString(getusers);
                    returntoken = JToken.Parse(usersdata);
                }
                else
                {
                    returntoken = JToken.Parse("{'message':'invalidkeyortokenorexpired'}");
                }
            }
            else
            {
                returntoken = JToken.Parse("{'message':'provide key and token'}");
            }
            return returntoken;
        }

        public JToken getpaymenttypes()
        {
            JToken returntoken = null;
            var requestHeaders = Request.Headers;
            if (Request.Headers["AUTHTOKEN"] != null && Request.Headers["AUTHKEY"] != null)
            {
                var token = requestHeaders.GetValues("AUTHTOKEN").First();
                var secret = requestHeaders.GetValues("AUTHKEY").First();
                bool validateToken = jwtken.validateToken(token, secret);
                if (validateToken)
                {
                    string getusers = " select * from tbllookup where tableid='PAYT'";
                    string usersdata = conn.GetJsonString(getusers);
                    returntoken = JToken.Parse(usersdata);
                }
                else
                {
                    returntoken = JToken.Parse("{'message':'invalidkeyortokenorexpired'}");
                }
            }
            else
            {
                returntoken = JToken.Parse("{'message':'provide key and token'}");
            }
            return returntoken;
        }


        public JToken getsupport()
        {  
            JToken returntoken = null;
            var requestHeaders = Request.Headers;
            if (Request.Headers["AUTHTOKEN"] != null && Request.Headers["AUTHKEY"] != null)
            {
                var token = requestHeaders.GetValues("AUTHTOKEN").First();
                var secret = requestHeaders.GetValues("AUTHKEY").First();
                bool validateToken = jwtken.validateToken(token, secret);
                if (validateToken)
                {
                    string getusers = " select code,FirstName+' '+OtherNames as name from tblSupporter where status='A' ";
                    string usersdata = conn.GetJsonString(getusers);
                    returntoken = JToken.Parse(usersdata);
                }
                else
                {
                    returntoken = JToken.Parse("{'message':'invalidkeyortokenorexpired'}");
                }
            }
            else
            {
                returntoken = JToken.Parse("{'message':'provide key and token'}");
            }
            return returntoken;
        }



        [HttpGet]
        public JToken getSupporters()
        {
            JToken returntoken = null;
            var requestHeaders = Request.Headers;
            if (Request.Headers["AUTHTOKEN"] != null && Request.Headers["AUTHKEY"] != null)
            {
                var token = requestHeaders.GetValues("AUTHTOKEN").First();
                var secret = requestHeaders.GetValues("AUTHKEY").First();
                bool validateToken = jwtken.validateToken(token, secret);
                if (validateToken)
                {
                    string getusers = " select * from tblSupporter";
                    string usersdata = conn.GetJsonString(getusers);
                    returntoken = JToken.Parse(usersdata);
                }
                else
                {
                    returntoken = JToken.Parse("{'error':'invalidkeyortokenorexpired'}");
                }
            }
            else
            {
                returntoken = JToken.Parse("{'error':'provide key and token'}");
            }
            return returntoken;
        }


        //get all transactions
        [HttpGet]
        public JToken gettransactions()
        {
            JToken returntoken = null;
            var requestHeaders = Request.Headers;
            if (Request.Headers["AUTHTOKEN"] != null && Request.Headers["AUTHKEY"] != null)
            {
                var token = requestHeaders.GetValues("AUTHTOKEN").First();
                var secret = requestHeaders.GetValues("AUTHKEY").First();
                bool validateToken = jwtken.validateToken(token, secret);
                if (validateToken)
                {
                    string getusers = " select * from tblTransactions";
                    string usersdata = conn.GetJsonString(getusers);
                    returntoken = JToken.Parse(usersdata);
                }
                else
                {
                    returntoken = JToken.Parse("{'error':'invalidkeyortokenorexpired'}");
                }
            }
            else
            {
                returntoken = JToken.Parse("{'error':'provide key and token'}");
            }
            return returntoken;
        }

        //get all transactions
        [HttpGet]
        public JToken getcommissions()
        {
            JToken returntoken = null;
            var requestHeaders = Request.Headers;
            if (Request.Headers["AUTHTOKEN"] != null && Request.Headers["AUTHKEY"] != null)
            {
                var token = requestHeaders.GetValues("AUTHTOKEN").First();
                var secret = requestHeaders.GetValues("AUTHKEY").First();
                bool validateToken = jwtken.validateToken(token, secret);
                if (validateToken)
                {
                    string getusers = " select * from tblCommissions";
                    string usersdata = conn.GetJsonString(getusers);
                    returntoken = JToken.Parse(usersdata);
                }
                else
                {
                    returntoken = JToken.Parse("{'error':'invalidkeyortokenorexpired'}");
                }
            }
            else
            {
                returntoken = JToken.Parse("{'error':'provide key and token'}");
            }
            return returntoken;
        }

        public string hashPass(string rawpass)
        {
            byte[] salt;
            byte[] buffer2;
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(rawpass,0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            string passHash = Convert.ToBase64String(dst);
            return passHash;
        }


    }
}