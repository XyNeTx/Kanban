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
    public class HRConnect
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionStirng;


        public HttpContext _context;

        public HRConnect(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionStirng = _configuration.GetValue<string>("ConnectionStrings:WorkFlowConnection");
        }

        public string GetConncetionString() => _connectionStirng;


        public void setContext(HttpContext httpContext)
        {
            _context = httpContext;
        }


        public DataTable ExecuteSQL(string SQL, HttpContext httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE")
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

                    return dataTable;
                }

                cmd.Dispose();
                cn.Close();

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public string ExecuteJSON(string SQL, HttpContext httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "LOAD DATA")
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

                    return JSONString;

                }

                cmd.Dispose();
                cn.Close();

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public void Execute(string SQL, HttpContext httpContext = null, bool skipLog = false, BearerClass pUser = null, string pAction = "EXECUTE")
        {
            try
            {
                SqlConnection cn = new SqlConnection(_connectionStirng);
                cn.Open();

                SqlCommand cmd = new SqlCommand(SQL, cn);
                cmd.ExecuteNonQuery();

                cmd.Dispose();
                cn.Close();

            }
            catch (Exception ex)
            {

            }
        }

    }
}
