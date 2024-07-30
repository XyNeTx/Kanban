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

        public DataTable ExecuteSQLProcDB(string sql, params object[] parameters)
        {

            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(_ProcDB.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;

                        // Add parameters to the SqlCommand
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@p" + i, parameters[i] ?? DBNull.Value);
                        }

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                        }
                    }
                }
            }
            catch (SqlException SQLex) when (SQLex.Number == -2) // Handle timeout exception
            {
                Console.WriteLine("Timeout occurred. Returning partial results.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dt;
        }

        public DataTable ExecuteSQL(string sql, params object[] parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(_KB3Context.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        // Add parameters to the SqlCommand
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@p" + i, parameters[i] != null ? parameters[i] : DBNull.Value);
                        }

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                        }
                    }
                }
            }
            catch (SqlException SQLex) when (SQLex.Number == -2) // Handle timeout exception
            {
                Console.WriteLine("Timeout occurred. Returning partial results.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dt;
        }
        
        public DataTable ExecuteSQLPPMDB(string sql, params object[] parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(_PPM3Context.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        // Add parameters to the SqlCommand
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@p" + i, parameters[i] != null ? parameters[i] : DBNull.Value);
                        }

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                        }
                    }
                }
            }
            catch (SqlException SQLex) when (SQLex.Number == -2) // Handle timeout exception
            {
                Console.WriteLine("Timeout occurred. Returning partial results.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dt;
        }

        public DataTable ExecuteSQL_Param(string sql, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(_KB3Context.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        if (sql.Contains("@p0"))
                        {
                            // Add parameters to the SqlCommand
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                cmd.Parameters.AddWithValue("@p" + i, parameters[i] != null ? parameters[i] : DBNull.Value);
                            }
                        }
                        else
                        {
                            for(int i = 0;i < parameters.Length; i++)
                            {
                                sql = sql.Replace(parameters[i].ParameterName, $"{parameters[i].ParameterName}='{parameters[i].Value}'");
                            }
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                        }
                    }
                }
            }
            catch (SqlException SQLex) when (SQLex.Number == -2) // Handle timeout exception
            {
                Console.WriteLine("Timeout occurred. Returning partial results.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dt;
        }

        public DataTable ExecuteSQLProc_Web(string sql, params object[] parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ProcWebConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;

                        // Add parameters to the SqlCommand
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@p" + i, parameters[i] ?? DBNull.Value);
                        }

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                        }
                    }
                }
            }
            catch (SqlException SQLex) when (SQLex.Number == -2) // Handle timeout exception
            {
                Console.WriteLine("Timeout occurred. Returning partial results.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dt;
        }
    }
}
