using System.Web;
using System.Web.Mvc;

namespace Craft_Beer_Me
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
