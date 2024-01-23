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
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace HINOSystem.Libs
{
    public class DbConnect
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionStirng;
        private readonly string _connectionStirng2;

        public DbConnect(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionStirng = _configuration.GetValue<string>("ConnectionStrings:ServerISSConnection");
        }

        public string GetConncetionString() => _connectionStirng;


        public DataTable ExecuteSQL(string pSQL)
        {
            SqlConnection _cn = null;
            SqlCommand _cmd = null;
            DataTable _dataTable = new DataTable();
            try
            {
                _cn = new SqlConnection(_connectionStirng);
                _cn.Open();

                _cmd = new SqlCommand(pSQL, _cn);
                using (SqlDataReader reader = _cmd.ExecuteReader())
                {
                    _dataTable.Load(reader);
                    _cmd.Dispose();
                    _cn.Close();

                    return _dataTable;
                }

            }catch(Exception ex)
            {
                if(_cmd!=null) _cmd.Dispose();
                _cn.Close();
                return _dataTable;
            }
            //finally
            //{
            //    cmd.Dispose();
            //    cn.Close();
            //    return null;
            //}
            

        }



        public string ExecuteJSON(string pSQL)
        {
            SqlConnection _cn = null;
            SqlCommand _cmd = null;
            DataTable _dataTable = new DataTable();
            try
            {
                _cn = new SqlConnection(_connectionStirng);
                _cn.Open();

                _cmd = new SqlCommand(pSQL, _cn);
                using (SqlDataReader reader = _cmd.ExecuteReader())
                {
                    _dataTable.Load(reader);
                    _cmd.Dispose();
                    _cn.Close();

                    string JSONString = string.Empty;
                    JSONString = JsonConvert.SerializeObject(_dataTable);
                    return JSONString;

                }

            }
            catch(Exception ex)
            {
                if (_cmd != null) _cmd.Dispose();
                _cn.Close();
                return "";
            }
        }

        public void Execute(string pSQL)
        {
            SqlConnection _cn = null;
            SqlCommand _cmd = null;
            try
            {
                 _cn = new SqlConnection(_connectionStirng);
                _cn.Open();

                 _cmd = new SqlCommand(pSQL, _cn);
                _cmd.ExecuteNonQuery();

                _cmd.Dispose();
                _cn.Close();
            }catch(Exception ex)
            {
                if (_cmd != null) _cmd.Dispose();
                _cn.Close();
            }

        }

    }
}
