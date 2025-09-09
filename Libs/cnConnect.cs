using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;



namespace HINOSystem.Libs
{
    public class cnConnect
    {

        private readonly IConfiguration _config;
        //public string system = "iss";

        private string cnString;


        public cnConnect(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void setDatabase(string system = "iss")
        {

            if (system.ToLower() == "iss")
            {
                cnString = _config.GetValue<string>("ConnectionStrings:ServerISSConnection");
            }
            if (system.ToLower() == "invsm")
            {
                cnString = _config.GetValue<string>("ConnectionStrings:ServerINVSMConnection");
            }
            if (system.ToLower() == "ppm")
            {
                cnString = _config.GetValue<string>("ConnectionStrings:ServerPPMConnection");
            }
            if (system.ToLower() == "procweb")
            {
                cnString = _config.GetValue<string>("ConnectionStrings:ServerProcWebConnection");
            }
            if (system.ToLower() == "kanban")
            {
                cnString = _config.GetValue<string>("ConnectionStrings:ServerKanbanConnection");
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

    }


}
