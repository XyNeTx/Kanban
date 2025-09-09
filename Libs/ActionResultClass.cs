using NPOI.SS.UserModel;
//using Microsoft.Office.Interop.Excel;

namespace HINOSystem.Libs
{
    public class ActionResultClass
    {
        private readonly IConfiguration _config;

        protected IWorkbook workbook;
        protected IFormulaEvaluator formulaEvaluator;
        protected DataFormatter dataFormatter;

        private int FormatRow = 0;

        public ActionResultClass(
            IConfiguration configuration
            )
        {
            _config = configuration;
        }


        public string Failed(string pMessage, dynamic pData = null)
        {
            if(pData == null )
            {
                return @"{
                    ""status"":""200"",
                    ""response"":""FAILED"",
                    ""message"": """ + pMessage + @"""
                }";
            }
            else
            {
                return @"{
                    ""status"":""200"",
                    ""response"":""FAILED"",
                    ""message"": """ + pMessage + @""",
                    ""data"": " + pData + @"
                }";
            }
        }






    }

}