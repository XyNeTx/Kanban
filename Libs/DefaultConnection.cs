using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Xml.Linq;


namespace HINOSystem.Libs
{
    public class DefaultConnection
    {
        private readonly IConfiguration _configuration;
        private string _connectionStirng;

        
        private readonly string _cnKB1Stirng;
        private readonly string _cnKB2Stirng;
        private readonly string _cnKB3Stirng;
        public dynamic Plant = 3;

        public HttpContext _context;

        public DefaultConnection(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionStirng = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");

            _cnKB1Stirng = _configuration.GetValue<string>("ConnectionStrings:KB1Connection");
            _cnKB2Stirng = _configuration.GetValue<string>("ConnectionStrings:KB2Connection");
            _cnKB3Stirng = _configuration.GetValue<string>("ConnectionStrings:KB3Connection");
        }

        public string GetConncetionString() => _connectionStirng;


        public void setContext(HttpContext httpContext)
        {
            _context = httpContext;
        }
        //public void setPlant(int pPlant = 3)
        //{
        //    //this._Plant = pPlant.ToString();
        //}


        public DataTable ExecuteSQL(string SQL, HttpContext httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE", string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            try
            {
                if (this.Plant.ToString() == "1") this._connectionStirng = _cnKB1Stirng;
                if (this.Plant.ToString() == "2") this._connectionStirng = _cnKB2Stirng;
                if (this.Plant.ToString() == "3") this._connectionStirng = _cnKB3Stirng;

                SqlConnection cn = new SqlConnection(this._connectionStirng);
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
            if (this.Plant.ToString() == "1") this._connectionStirng = _cnKB1Stirng;
            if (this.Plant.ToString() == "2") this._connectionStirng = _cnKB2Stirng;
            if (this.Plant.ToString() == "3") this._connectionStirng = _cnKB3Stirng;

            SqlConnection cn = new SqlConnection(this._connectionStirng);
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

                return null;
            }
            finally
            {
                // Make sure to close the connection when you're done with it
                cn.Close();
            }
        }


        public Boolean executeNonQuery(string SQL, HttpContext httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE QUERY", string pControllerName = "", string pActionName = "", string pSystem = "")
        {
            try
            {
                if (this.Plant.ToString() == "1") this._connectionStirng = _cnKB1Stirng;
                if (this.Plant.ToString() == "2") this._connectionStirng = _cnKB2Stirng;
                if (this.Plant.ToString() == "3") this._connectionStirng = _cnKB3Stirng;

                SqlConnection cn = new SqlConnection(this._connectionStirng);
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



            SqlConnection cn = new SqlConnection(_cnKB3Stirng);
            cn.Open();
            SqlCommand cmd = new SqlCommand(_SQL_Log, cn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            cn.Close();



            //SqlCommand cmd = new SqlCommand(SQL, cn);
            //using (SqlDataReader reader = cmd.ExecuteReader())
            //{
            //    var dataTable = new DataTable();
            //    dataTable.Load(reader);
            //    return dataTable;
            //}

            //cmd.Dispose();
            //cn.Close();

            //SqlConnection cn = new SqlConnection(_connectionStirng);
            //cn.Open();

            //SQL = @"INSERT INTO [log].[Action] ([UserCode]
            //          ,[Token]
            //          ,[ActionType]
            //          ,[ActionAt]
            //          ,[SQL]
            //        )VALUES([UserCode]
            //          ,[Token]
            //          ,[ActionType]
            //          ,[ActionAt]
            //          ,[SQL]
            //        )
            //";

            //SqlCommand cmd = new SqlCommand(SQL, cn);
            //using (SqlDataReader reader = cmd.ExecuteReader())
            //{
            //    var dataTable = new DataTable();
            //    dataTable.Load(reader);
            //    return dataTable;
            //}

            //cmd.Dispose();
            //cn.Close();

        }



        //private void ActionLog(string Action = "", string Result = "", string _SQL = null, string Message = "")
        //{
        //    string _SQL_Log = @"INSERT INTO [log].[Action] ([UserCode]
        //                          ,[Token]
        //                          ,[ActionType]
        //                          ,[ActionAt]
        //                          ,[Result]
        //                          ,[Message]
        //                          ,[SQL]
        //                        )VALUES('" + HttpContext.Session.GetString("USER_CODE").ToString() + @"'
        //                          , '" + HttpContext.Session.GetString("TOKEN").ToString() + @"'
        //                          , '" + Action + @"'
        //                          , GETDATE()
        //                          , '" + Result + @"'
        //                          , '" + Message + @"'
        //                          , '" + (_SQL == null ? "" : _SQL.Replace("'", "''").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ")) + @"'
        //                        )";
        //    _wrtConnect.Execute(_SQL_Log);

        //}



    }
}
