using Microsoft.AspNetCore.Mvc;
using HINOSystem.Libs;

namespace HINOSystem.Controllers
{
    public class VBController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly WarrantyClaimConnect _wrtConnect;

        public VBController()
        {}

        public dynamic IIf(Boolean pComparision = true, dynamic pTrueParse = null, dynamic pFalsePase = null)
        {
            if (pComparision)
            {
                return pTrueParse;
            }
            else
            {
                return pFalsePase;
            }
        }


        public string Mid(string pString = "", int pStart = 0, int pEnd = 0)
        {

            return pString;
        }


        public int InStr(int pStart = 0, string pString = "", string pFind = "")
        {

            return pStart;
        }

        
    }
}
