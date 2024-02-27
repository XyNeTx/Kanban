using HINOSystem.Context;
using HINOSystem.Libs;
using KANBAN.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using System.Data;

namespace HINOSystem.Libs
{
    public class FillDataTable
    {
        private readonly KB3Context _KB3Context;
        private readonly KanbanConnection _KBCN;
        private readonly IConfiguration _configuration;
        private readonly PPM3Context _PPM3Context;

        public FillDataTable(KB3Context kB3Context, KanbanConnection kBCN, IConfiguration configuration, PPM3Context pPM3Context)
        {
            _KB3Context = kB3Context;
            _KBCN = kBCN;
            _configuration = configuration;
            _PPM3Context = pPM3Context;
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

        public DataTable ExecuteSQL(string sql)
        {
            setConString();
            SqlConnection con = new SqlConnection(_KB3Context.Database.GetConnectionString());
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
