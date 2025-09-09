using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;


namespace HINOSystem.Libs
{
    public class WarrantyClaimConnect
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionStirng;


        public HttpContext _context;

        public WarrantyClaimConnect(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionStirng = _configuration.GetValue<string>("ConnectionStrings:WRTConnection");
        }

        public string GetConncetionString() => _connectionStirng;


        public void setContext(HttpContext httpContext)
        {
            _context = httpContext;
        }


        public DataTable ExecuteSQL(string SQL, HttpContext httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE", string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            try
            {
                SqlConnection cn = new SqlConnection(_connectionStirng);
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


        public string ExecuteJSON(string SQL, HttpContext httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE JSON", string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            try
            {


                SqlConnection cn = new SqlConnection(_connectionStirng);
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

                return null;
            }
        }


        public void Execute(string SQL, HttpContext httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE QUERY", string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            try
            {
            SqlConnection cn = new SqlConnection(_connectionStirng);
            cn.Open();

            SqlCommand cmd = new SqlCommand(SQL, cn);
            cmd.ExecuteNonQuery();

            cmd.Dispose();
            cn.Close();

            if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "OK", "ExecuteNonQuery", pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem:pSystem);

            }
            catch (Exception ex)
            {
                if (skipLog != true) this.executeLog(httpContext, SQL, pAction, "FAILED", ex.Message , pUser: pUser, pControllerName: pControllerName, pActionName: pActionName, pSystem: pSystem);
            }
        }





        public void executeLog(HttpContext httpContext, string pSQL, string pAction, string pResult, string pMessage, BearerClass pUser = null, string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            string _user = "SYSTEM";
            string _token = "";

            if (httpContext != null)
            {
                _user = httpContext.Session.GetString("USER_CODE").ToString();
                _token = httpContext.Session.GetString("TOKEN").ToString();
            }
            if (pUser != null)
            {
                _user = pUser.UserCode.ToString();
                _token = pUser.Token.ToString();
            }

            string _SQL_Log = @"INSERT INTO [log].[Action] ([UserCode]
                                  ,[Token]
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
                                  , '" + pAction + @"'
                                  , GETDATE()
                                  , '" + pSystem + @"'
                                  , '" + pControllerName.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + @"'
                                  , '" + pActionName.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + @"'
                                  , '" + pResult.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + @"'
                                  , '" + pMessage.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ") + @"'
                                  , '" + (pSQL == null ? "" : pSQL.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim()) + @"'
                                )";


            SqlConnection cn = new SqlConnection(_connectionStirng);
            cn.Open();
            SqlCommand cmd = new SqlCommand(_SQL_Log, cn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();

        }

    }
}
