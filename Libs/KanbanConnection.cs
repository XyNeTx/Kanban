using HINOSystem.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Text;


namespace HINOSystem.Libs
{
    public class KanbanConnection
    {
        private readonly KB3Context _KB3Context;
        private readonly IHttpContextAccessor _httpContext;

        public KanbanConnection(KB3Context kB3Context, IHttpContextAccessor httpContext)
        {
            _KB3Context = kB3Context;
            _httpContext = httpContext;
        }


        public DataTable ExecuteSQL(string SQL, IHttpContextAccessor httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE QUERY", string pControllerName = "", string pActionName = "", string pSystem = "")
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

                    if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "OK", "ExecuteSQL", pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem: pSystem);


                    return dataTable;
                }

                cmd.Dispose();
                cn.Close();

            }
            catch (Exception ex)
            {
                if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "FAILED", ex.Message, pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem: pSystem);

                return null;
            }
        }


        public string ExecuteJSON(string SQL, IHttpContextAccessor httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE JSON", string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            
            SqlConnection cn = new SqlConnection(_KB3Context.Database.GetConnectionString());
            try
            {
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

                    if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "OK", "ExecuteJSON", pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem: pSystem);

                    return JSONString;

                }

                cmd.Dispose();
                cn.Close();

            }
            catch (Exception ex)
            {
                if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "FAILED", ex.Message, pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem: pSystem);

                return "Error " + ex.Message;
            }
            finally
            {
                // Make sure to close the connection when you're done with it
                cn.Close();
            }
        }


        public Boolean Execute(string SQL, IHttpContextAccessor httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE", string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            try
            {
                SqlConnection cn = new SqlConnection(_KB3Context.Database.GetConnectionString());
                cn.Open();

                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.ExecuteNonQuery();

                cmd.Dispose();
                cn.Close();

                if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "OK", "ExecuteNonQuery", pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem: pSystem);

                return true;

            }
            catch (Exception ex)
            {
                if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "FAILED", ex.Message, pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem: pSystem);
                return false;
            }
        }

        public void writeLog(string pSQL = "", string pAction = "", string pResult = "", string pMessage = "", BearerClass pUser = null, string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            this.executeLog(_httpContext, pSQL, pAction, pResult, pMessage, pUser, pControllerName, pActionName, pSystem);
        }



        public void executeLog(IHttpContextAccessor httpContext = null, string pSQL = "", string pAction = "", string pResult = "", string pMessage = "", BearerClass pUser = null, string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            string _user = "SYSTEM"
                , _token = ""
                , _device = ""
                , _ipaddress = "";

            if (httpContext != null)
            {
                _user = httpContext.HttpContext.Session.GetString("USER_CODE").ToString();
                _token = httpContext.HttpContext.Session.GetString("TOKEN").ToString();
            }
            if (pUser != null)
            {
                _user = pUser.UserCode.ToString();
                _token = pUser.Token.ToString();
                _device = pUser.Device.ToString();
                _ipaddress = pUser.IPAddress.ToString();
            }

            string _SQL_Log = @"INSERT INTO [log].[Action] ([UserCode]
                                  ,[Token]
                                  ,[DeviceName]
                                  ,[IPAddress]
                                  ,[ActionType]
                                  ,[ActionAt]
                                  ,[SystemName]
                                  ,[ControllerName]
                                  ,[ActionName]
                                  ,[Result]
                                  ,[Message]
                                  ,[SQL]
                                )VALUES('" + _user + @"'
                                  , '" + _token + @"'
                                  , '" + _device + @"'
                                  , '" + _ipaddress + @"'
                                  , '" + pAction + @"'
                                  , GETDATE()
                                  , '" + (pSystem == "" ? "KB3" : pSystem) + @"'
                                  , '" + pControllerName.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + @"'
                                  , '" + pActionName.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + @"'
                                  , '" + pResult.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + @"'
                                  , '" + pMessage.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + @"'
                                  , '" + (pSQL == null ? "" : pSQL.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()) + @"'
                                )";



            SqlConnection cn = new SqlConnection(_KB3Context.Database.GetConnectionString());
            cn.Open();
            SqlCommand cmd = new SqlCommand(_SQL_Log, cn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();

        }

    }
}
