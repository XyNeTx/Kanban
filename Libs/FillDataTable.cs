using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.Util.ArrayExtensions;
using System.Data;

namespace HINOSystem.Libs
{
    public class FillDataTable
    {
        private readonly KB3Context _KB3Context;
        private readonly KanbanConnection _KBCN;
        private readonly IConfiguration _configuration;
        private readonly PPM3Context _PPM3Context;
        private readonly ProcDBContext _ProcDB;

        public FillDataTable(
            KB3Context kB3Context, 
            KanbanConnection kBCN, 
            IConfiguration configuration, 
            PPM3Context pPM3Context, 
            ProcDBContext procDB
            )
        {
            _KB3Context = kB3Context;
            _KBCN = kBCN;
            _configuration = configuration;
            _PPM3Context = pPM3Context;
            _ProcDB = procDB;
        }

        public void setConString()
        {
            try
            {
                if (_KBCN.Plant.ToString() == "3")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB3Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPM3Connection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
                else if (_KBCN.Plant.ToString() == "2")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB2Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPMConnection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
                else if (_KBCN.Plant.ToString() == "1")
                {
                    var KBConnectString = _configuration.GetConnectionString("KB1Connection");
                    var PPMConnectString = _configuration.GetConnectionString("PPMConnection");
                    _KB3Context.Database.SetConnectionString(KBConnectString);
                    _PPM3Context.Database.SetConnectionString(PPMConnectString);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public DataTable ExecuteSQLProcDB(string sql)
        {
            setConString();
            SqlConnection con = new SqlConnection(_ProcDB.Database.GetConnectionString());
            DataTable dt = new DataTable();

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.CommandType = CommandType.Text;
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    using (dt)
                    {
                        sda.Fill(dt);
                        return dt;
                    }
                }
            }
        }
        public DataTable ExecuteSQL(string sql,params object[] parameters)
        {
            setConString();
            SqlConnection con = new SqlConnection(_KB3Context.Database.GetConnectionString());
            DataTable dt = new DataTable();
            if (parameters.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    sql = sql.Replace("@p" + i, $"'{parameters[i]}'");
                }
            }

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.CommandType = CommandType.Text;
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    using (dt)
                    {
                        try
                        {
                            sda.Fill(dt);
                            return dt;
                        }
                        catch (SqlException SQLex) when (SQLex.Number == -2) // Handle timeout exception
                        {
                            Console.WriteLine("Timeout occurred. Returning partial results.");
                            return dt; // Return the partially filled DataTable
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message); return dt;
                        }
                    }
                }
            }
        }
        public DataTable ExecuteSQLProc_Web(string sql)
        {
            setConString();
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ProcWebConnection"));
            DataTable dt = new DataTable();

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.CommandType = CommandType.Text;
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    using (dt)
                    {
                        sda.Fill(dt);
                        return dt;
                    }
                }
            }
        }
    }
}
