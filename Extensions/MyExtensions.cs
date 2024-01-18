using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using HINOSystem.Libs;

namespace HINOSystem.Extensions
{
    
    public static class MyExtensions
    {
        private static readonly WarrantyClaimConnect _wrtConnect;


        public static string IsSelected(this IHtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];

            if (String.IsNullOrEmpty(controller)) controller = currentController;
            if (String.IsNullOrEmpty(action) && currentAction != "Home") action = currentAction;
            if (String.IsNullOrEmpty(cssClass)) cssClass = "active";
            string _ret = controller == currentController && action == currentAction ? cssClass : String.Empty;

            if (currentController == "Reports" && controller == currentController && cssClass != "active" ) _ret = cssClass;

            return _ret;
            //return controller == currentController && action == currentAction ? cssClass : String.Empty;
        }

        public static string PageClass(this IHtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        public static string i18nItem(this IHtmlHelper html, string language = null, string item = null)
        {
            string _language = (language != null ? language : (string)html.ViewContext.HttpContext.Session.GetString("USER_UILANGUAGE").ToString());

            dynamic _item = item.Split(".");

            using (StreamReader r = new StreamReader("wwwroot/assets/i18n/" + _language.ToUpper() + "/Layout.json"))
            {
                string json = r.ReadToEnd();

                dynamic array = JsonConvert.DeserializeObject(json);
                //item = JsonConvert.DeserializeObject(array["button"].ToString())[item.ToString().ToLower()];

                if (_item.Length == 1) item = array[_item[0]].ToString();
                if (_item.Length == 2) item = array[_item[0]][_item[1]].ToString();
                if (_item.Length == 3) item = array[_item[0]][_item[1]][_item[2]].ToString();
                if (_item.Length == 4) item = array[_item[0]][_item[1]][_item[2]][_item[3]].ToString();
                if (_item.Length == 5) item = array[_item[0]][_item[1]][_item[2]][_item[3]][_item[4]].ToString();
                if (_item.Length == 6) item = array[_item[0]][_item[1]][_item[2]][_item[3]][_item[4]][_item[5]].ToString();
                if (_item.Length == 7) item = array[_item[0]][_item[1]][_item[2]][_item[3]][_item[4]][_item[5]][_item[6]].ToString();
                if (_item.Length == 8) item = array[_item[0]][_item[1]][_item[2]][_item[3]][_item[4]][_item[5]][_item[6]][_item[7]].ToString();
                if (_item.Length == 9) item = array[_item[0]][_item[1]][_item[2]][_item[3]][_item[4]][_item[5]][_item[6]][_item[7]][_item[8]].ToString();

            }

            return item;
        }


        //public static DataTable getMenu()
        //{
        //    string _sql = "SELECT * FROM [erp].[Menu] ";

            

        //    DataTable _table = _wrtConnect.executeSQL(_sql);

        //    return _table;
        //}




    }
}
