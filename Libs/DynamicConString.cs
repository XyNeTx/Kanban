using HINOSystem.Context;
using Microsoft.EntityFrameworkCore;

namespace KANBAN.Libs
{
    public class DynamicConString : IDynamicConString
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration configuration;
        private readonly KB3Context _KB3Context;
        private string _KB = "";

        public DynamicConString(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, KB3Context kB3Context)
        {
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            _KB3Context = kB3Context;
        }

        public string GetConString()
        {
            string KanbanConString = "";
            try
            {
                if(httpContextAccessor.HttpContext.Session.GetString("USER_PLANT") == "1")
                {
                    KanbanConString = configuration.GetConnectionString("KB1Connection");
                }
                else if (httpContextAccessor.HttpContext.Session.GetString("USER_PLANT") == "2")
                {
                    KanbanConString = configuration.GetConnectionString("KB2Connection");
                }
                else if (httpContextAccessor.HttpContext.Session.GetString("USER_PLANT") == "3")
                {
                    KanbanConString = configuration.GetConnectionString("KB3Connection");
                }
            }
            catch (Exception ex)
            {
                KanbanConString = "";
            }
            return KanbanConString;
        }
        public void SetConString(string KanbanConString)
        {
            _KB = KanbanConString;
            _KB3Context.Database.SetConnectionString(KanbanConString);
        }
    }
}
