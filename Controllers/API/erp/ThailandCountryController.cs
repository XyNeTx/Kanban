using Microsoft.AspNetCore.Http;
using HINOSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data;
using HINOSystem.Libs;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace HINOSystem.Controllers.API.wrt
{
    public class ThailandCountryController : Controller
    {
        private readonly WarrantyClaimConnect _wrtConnect;
        private readonly BearerClass _BearerClass;

        public ThailandCountryController(WarrantyClaimConnect wrtConnect, BearerClass bearerClass)
        {
            _wrtConnect = wrtConnect;
            _BearerClass = bearerClass;
        }


        [HttpGet]  //Tambon
        public IActionResult Tambon(string q = null)
        {
            string SQL = @"
                SELECT TambonID AS value
                    , Tambon +', '+ District + ', ' + Province AS label
                    , TambonTH +', '+ DistrictTH + ', ' + ProvinceTH AS labelTH
                    , *
                FROM [erp].[CountryThailand] 
                WHERE 1=1
                AND isDelete=0 
                " + ((q != null) && (q.ToUpper() != "PHUPHIROM") ? " AND (Tambon LIKE '%" + q + "%' OR TambonTH LIKE '%" + q + "%') " : "") + @"
                
            ";

            if(q != null)
            {
                if (q.ToUpper() == "PHUPHIROM")
                {
                    SQL += @"

                UNION

                SELECT TambonID AS value
                    , TambonTH +', '+ DistrictTH + ', ' + ProvinceTH AS label
                    , Tambon +', '+ District + ', ' + Province AS labelTH
                    , *
                FROM [erp].[CountryThailand] 
                WHERE 1=1
                AND isDelete=0 
                " + ((q != null) && (q.ToUpper() != "PHUPHIROM") ? " AND (Tambon LIKE '%" + q + "%' OR TambonTH LIKE '%" + q + "%') " : "") + @"
                
            ";
                }

            }

            SQL += @"
                ORDER BY Tambon";

            string _jsonData = _wrtConnect.ExecuteJSON(SQL, skipLog: true);
            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Tambon data is found"",
                ""data"":" + _jsonData + @"
            }";

            return Content(_result, "application/json");
        }


        [HttpGet]  //District
        public IActionResult District(string q = null)
        {
            string SQL = @"
                SELECT DistrictID
                    , District
                    , DistrictTH
                    , ProvinceID
                    , Province
                    , ProvinceTH
                    , PostCode
                FROM [erp].[CountryThailand] 
                WHERE 1=1
                AND isDelete=0 
                " + (q != null ? " AND (District LIKE '%" + q + "%' OR DistrictTH LIKE '%" + q + "%') " : "") + @"
                GROUP BY DistrictID
                    , District
                    , DistrictTH
                    , ProvinceID
                    , Province
                    , ProvinceTH
                    , PostCode
                ORDER BY District;
            ";

            string _jsonData = _wrtConnect.ExecuteJSON(SQL, skipLog: true);
            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""District data is found"",
                ""data"":" + _jsonData + @"
            }";

            return Content(_result, "application/json");
        }


        [HttpGet]  //Province
        public IActionResult Province(string q = null)
        {
            string SQL = @"
                SELECT ProvinceID
                    , Province
                    , ProvinceTH
                    , PostCode
                FROM [erp].[CountryThailand] 
                WHERE 1=1
                AND isDelete=0 
                " + (q != null ? " AND (Province LIKE '%" + q + "%' OR ProvinceTH LIKE '%" + q + "%') " : "") + @"

                GROUP BY  ProvinceID
                    , Province
                    , ProvinceTH
                    , PostCode

                ORDER BY Province;
            ";

            string _jsonData = _wrtConnect.ExecuteJSON(SQL, skipLog: true);
            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""Province data is found"",
                ""data"":" + _jsonData + @"
            }";

            return Content(_result, "application/json");
        }


        [HttpGet]  //PostCode
        public IActionResult PostCode(string q = null)
        {
            string SQL = @"
                SELECT *
                FROM [erp].[CountryThailand] 
                WHERE 1=1
                AND isDelete=0 
                " + (q != null ? " AND (Postcode LIKE '%" + q + "%' ) " : "") + @"
                ORDER BY _ID;
            ";

            string _jsonData = _wrtConnect.ExecuteJSON(SQL, skipLog: true);
            string _result = @"{
                ""status"":""200"",
                ""response"":""OK"",
                ""message"": ""PostCode data is found"",
                ""data"":" + _jsonData + @"
            }";

            return Content(_result, "application/json");
        }
        public IActionResult ZipCode(string q = null)
        {
            return PostCode(q);
        }



        [HttpGet]  //Test autocomplete
        public IActionResult autocomplete(string q = null)
        {
            string _result = @"[
                ""Google Cloud Platform"",
                ""Amazon AWS"",
                ""Docker"",
                ""Digital Ocean""
            ]";

            return Content(_result, "application/json");
        }


    }
}
