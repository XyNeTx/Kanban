using HINOSystem.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;


namespace HINOSystem.Libs
{
    public class ERPConnection
    {
        private readonly KB3Context _KB3Context;

        //public HttpContext _context;

        public ERPConnection(KB3Context kB3Context)
        {
            _KB3Context = kB3Context;
        }

        //public void setContext(HttpContext httpContext)
        //{
        //    _context = httpContext;
        //}

        public DataTable ExecuteSQL(string SQL, HttpContext httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE", string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            try
            {
                SqlConnection cn = new SqlConnection(_KB3Context.Database.GetConnectionString());
                cn.Open();

                SqlCommand cmd = new SqlCommand(SQL, cn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);

                    cmd.Dispose();
                    cn.Close();

                    //if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "OK", "ExecuteSQL", pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem: pSystem);


                    return dataTable;
                }

                cmd.Dispose();
                cn.Close();

            }
            catch (Exception ex)
            {
                //if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "FAILED", ex.Message, pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem: pSystem);

                return null;
            }
        }


        public string ExecuteJSON(string SQL, HttpContext httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE JSON", string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            try
            {


                SqlConnection cn = new SqlConnection(_KB3Context.Database.GetConnectionString());
                cn.Open();

                SqlCommand cmd = new SqlCommand(SQL, cn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    var dataTable = new DataTable();
                    dataTable.Load(reader);

                    string JSONString = string.Empty;
                    JSONString = JsonConvert.SerializeObject(dataTable);

                    cmd.Dispose();
                    cn.Close();

                    //if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "OK", "ExecuteJSON", pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem: pSystem);

                    return JSONString;

                }

                cmd.Dispose();
                cn.Close();

            }
            catch (Exception ex)
            {
                //if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "FAILED", ex.Message, pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem: pSystem);

                return null;
            }
        }


        public void Execute(string SQL, HttpContext httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE QUERY", string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            try
            {
            SqlConnection cn = new SqlConnection(_KB3Context.Database.GetConnectionString());
            cn.Open();

            SqlCommand cmd = new SqlCommand(SQL, cn);
            cmd.ExecuteNonQuery();

            cmd.Dispose();
            cn.Close();

            //if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "OK", "ExecuteNonQuery", pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem:pSystem);

            }
            catch (Exception ex)
            {
                //if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "FAILED", ex.Message , pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem: pSystem);
            }
        }


        //public void executeLog(HttpContext httpContext, string pSQL, string pAction, string pResult, string pMessage, BearerClass pUser = null, string pControllerName = "", string pActionName = "", string pSystem = "")
        //{
        //    string _user = "SYSTEM"
        //        , _token = ""
        //        , _device = ""
        //        , _ipaddress = "";

        //    //if (httpContext != null)
        //    //{
        //    //    _user = User.FindFirst(ClaimTypes.UserData).Value;
        //    //    _token = _BearerClass.Token;
        //    //    //_user = httpContext.Session.GetString("User_Code").ToString();
        //    //    //_token = httpContext.Session.GetString("TOKEN").ToString();
        //    //}
        //    if (pUser != null)
        //    {
        //        _user = pUser.UserCode.ToString();
        //        _token = pUser.Token.ToString();
        //        _device = pUser.Device.ToString();
        //        _ipaddress = pUser.IPAddress.ToString();
        //    }

        //    string _SQL_Log = @"INSERT INTO [log].[Action] ([UserCode]
        //                          ,[Token]
        //                          ,[DeviceName]
        //                          ,[IPAddress]
        //                          ,[ActionType]
        //                          ,[ActionAt]
        //                          ,[SystemName]
        //                          ,[ControllerName]
        //                          ,[ActionName]
        //                          ,[Result]
        //                          ,[Message]
        //                          ,[SQL]
        //                        )VALUES('" + _user + @"'
        //                          , '" + _token + @"'
        //                          , '" + _device + @"'
        //                          , '" + _ipaddress + @"'
        //                          , '" + pAction + @"'
        //                          , GETDATE()
        //                          , '" + pSystem + @"'
        //                          , '" + pControllerName.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + @"'
        //                          , '" + pActionName.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + @"'
        //                          , '" + pResult.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + @"'
        //                          , '" + pMessage.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + @"'
        //                          , '" + (pSQL == null ? "" : pSQL.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()) + @"'
        //                        )";


        //    SqlConnection cn = new SqlConnection(_KB3Context.Database.GetConnectionString());
        //    cn.Open();
        //    SqlCommand cmd = new SqlCommand(_SQL_Log, cn);
        //    cmd.ExecuteNonQuery();
        //    cmd.Dispose();
        //    cn.Close();

        //}

    }
}
