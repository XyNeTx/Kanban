using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;


namespace HINOSystem.Libs
{
    public class DBConnection
    {

        private readonly IConfiguration _configuration;
        //public string system = "iss";

        private string cnString;


        public DBConnection(IConfiguration configuration, string system = "iss")
        {
            _configuration = configuration;

            if (system.ToLower() == "iss")
            {
                cnString = _configuration.GetValue<string>("ConnectionStrings:ServerISSConnection");
            }
            if (system.ToLower() == "invsm")
            {
                cnString = _configuration.GetValue<string>("ConnectionStrings:ServerINVSMConnection");
            }
            if (system.ToLower() == "ppm")
            {
                cnString = _configuration.GetValue<string>("ConnectionStrings:ServerPPMConnection");
            }
            if (system.ToLower() == "procweb")
            {
                cnString = _configuration.GetValue<string>("ConnectionStrings:ServerProcWebConnection");
            }
            if (system.ToLower() == "kanban")
            {
                cnString = _configuration.GetValue<string>("ConnectionStrings:ServerKanbanConnection");
            }
            if (system.ToLower() == "pse")
            {
                cnString = _configuration.GetValue<string>("ConnectionStrings:ServerPSEConnection");
            }

        }
        


        public DataTable ExecuteSQL(string pSQL)
        {
            SqlConnection _cn = null;
            SqlCommand _cmd = null;
            DataTable _dataTable = new DataTable();
            try
            {
                _cn = new SqlConnection(cnString);
                _cn.Open();

                _cmd = new SqlCommand(pSQL, _cn);
                using (SqlDataReader reader = _cmd.ExecuteReader())
                {
                    _dataTable.Load(reader);
                    _cmd.Dispose();
                    _cn.Close();

                    return _dataTable;
                }

            }
            catch (Exception ex)
            {
                if (_cmd != null) _cmd.Dispose();
                _cn.Close();
                return _dataTable;
            }
        }



        public string ExecuteJSON(string pSQL)
        {
            SqlConnection _cn = null;
            SqlCommand _cmd = null;
            DataTable _dataTable = new DataTable();
            try
            {
                _cn = new SqlConnection(cnString);
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
            catch (Exception ex)
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
                _cn = new SqlConnection(cnString);
                _cn.Open();

                _cmd = new SqlCommand(pSQL, _cn);
                _cmd.ExecuteNonQuery();

                _cmd.Dispose();
                _cn.Close();
            }
            catch (Exception ex)
            {
                if (_cmd != null) _cmd.Dispose();
                _cn.Close();
            }

        }


        //public IEnumerable ExecuteJSON(string SQL)
        //{
        //    SqlConnection cn = new SqlConnection(cnString);
        //    cn.Open();

        //    SqlCommand cmd = new SqlCommand(SQL, cn);
        //    using (SqlDataReader reader = cmd.ExecuteReader())
        //    {

        //        var dataTable = new DataTable();
        //        dataTable.Load(reader);

        //        string JSONString = string.Empty;
        //        JSONString = JsonConvert.SerializeObject(dataTable);
        //        return JSONString;

        //    }

        //    cmd.Dispose();
        //    cn.Close();

        //}


        //public DataTable ExecuteSQL(string SQL)
        //{
        //    SqlConnection cn = new SqlConnection(cnString);
        //    cn.Open();

        //    SqlCommand cmd = new SqlCommand(SQL, cn);
        //    using (SqlDataReader reader = cmd.ExecuteReader())
        //    {
        //        var dataTable = new DataTable();
        //        dataTable.Load(reader);
        //        return dataTable;
        //    }

        //    cmd.Dispose();
        //    cn.Close();

        //}


    }


}
