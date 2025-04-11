using HINOSystem.Context;
using KANBAN.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
        private readonly IHttpContextAccessor _IHttp;

        public FillDataTable(
            KB3Context kB3Context,
            KanbanConnection kBCN,
            IConfiguration configuration,
            PPM3Context pPM3Context,
            ProcDBContext procDB,
            IHttpContextAccessor iHttp
            )
        {
            _KB3Context = kB3Context;
            _KBCN = kBCN;
            _configuration = configuration;
            _PPM3Context = pPM3Context;
            _ProcDB = procDB;
            _IHttp = iHttp;
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
                throw new Exception(SQLex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                        cmd.CommandTimeout = 300;
                        // Add parameters to the SqlCommand
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@p" + i, string.IsNullOrWhiteSpace(parameters[i]?.ToString()) ? DBNull.Value : parameters[i]);
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
                throw new Exception(SQLex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public async Task<DataTable> ExecuteSQLAsync(string sql, params object[] parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(_KB3Context.Database.GetConnectionString()))
                {
                    await con.OpenAsync(); // Open connection asynchronously

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 300;

                        // Add parameters to the SqlCommand
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            cmd.Parameters.AddWithValue("@p" + i, string.IsNullOrWhiteSpace(parameters[i]?.ToString()) ? DBNull.Value : parameters[i]);
                        }

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync()) // ExecuteReaderAsync
                        {
                            dt.Load(reader); // Load DataTable from reader
                        }
                    }
                }
            }
            catch (SqlException SQLex) when (SQLex.Number == -2) // Handle timeout exception
            {
                throw new Exception(SQLex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                throw new Exception(SQLex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                            for (int i = 0; i < parameters.Length; i++)
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
                throw new Exception(SQLex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public DataTable ExecuteStoreSQL(string sql, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(_KB3Context.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(parameters);
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                        }
                    }
                }
            }
            catch (SqlException SQLex) when (SQLex.Number == -2) // Handle timeout exception
            {
                throw new Exception(SQLex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public async Task<DataTable> ExecuteStoreSQLAsync(string sql, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(_KB3Context.Database.GetConnectionString()))
                {
                    await con.OpenAsync(); // Open connection asynchronously

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.Value)
                            {
                                SqlDbType = param.SqlDbType,
                                Direction = param.Direction,
                                Size = param.Size
                            });
                        }

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync()) // ExecuteReaderAsync
                        {
                            dt.Load(reader); // Load DataTable from reader
                        }
                    }
                }
            }
            catch (SqlException SQLex) when (SQLex.Number == -2) // Handle timeout exception
            {
                throw new Exception(SQLex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public DataTable ExecuteSQLProc_Web(string sql, params object[] parameters)
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
                throw new Exception(SQLex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return dt;
        }

        public string ppmConnect()
        {
            string isDev = _IHttp.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "isDev").Value == "1" ? "Dev" : "";
            string plant = _IHttp.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "plantCode").Value;
            string cPlantDev = plant + isDev;

            string ppmConnect = cPlantDev switch
            {
                "1" => "[HMMT-PPM].[PPMDB]",
                "2" => "[HMMT-PPM].[PPMDB]",
                "3" => "[HMMTA-PPM].[PPMDB]",
                "1Dev" => "[HMMT-PPM].[PPMDB]",
                "2Dev" => "[HMMT-PPM].[PPMDB]",
                "3Dev" => "[PPMDB]",
                _ => "[HMMTA-PPM].[PPMDB]"
            };

            return ppmConnect;
        }

        public string QueryStoreCode()
        {
            string QueryStoreCode = _IHttp.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "plantCode").Value switch
            {
                "1" => "1A",
                "2" => "2B",
                "3" => "3C",
                _ => "3C"
            };

            return QueryStoreCode;
        }

        public string procDBConnect()
        {
            string isDev = _IHttp.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "isDev").Value == "1" ? "Dev" : "";
            string plant = _IHttp.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "plantCode").Value;
            string cPlantDev = plant + isDev;

            string procDBConnect = cPlantDev switch
            {
                "1" => "[HMMT-APP03].[Proc_DB]",
                "2" => "[HMMT-APP03].[Proc_DB]",
                "3" => "[HMMT-APP03].[Proc_DB]",
                "1Dev" => "[Proc_DB]",
                "2Dev" => "[Proc_DB]",
                "3Dev" => "[Proc_DB]",
                _ => "[Proc_DB]",
            };

            return procDBConnect;
        }

        public async Task<string> GetUserName(string UserCode)
        {
            var User = await _KB3Context.User
                .FirstOrDefaultAsync(x => x.Code == UserCode);

            string userName = User.Name + " " + User.Surname;

            //userName = User.Title_ID == 1 ? "Ms. " + userName : User.Title_ID == 3 ? "Mr. " + userName : "Mrs. " + userName;

            return userName;
        }

    }
}
