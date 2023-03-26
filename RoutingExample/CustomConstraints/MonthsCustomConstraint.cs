using System.Text.RegularExpressions;

namespace RoutingExample.CustomConstraints
{
    // Eg: sales-report/2020/apr
    public class MonthsCustomConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            // check whethers the value exists
            if (!values.ContainsKey(routeKey)) // month
            {
                return false; // not a match
            }

            Regex regex = new Regex("^(apr|jul|oct|jan)$");
            string? month = Convert.ToString(values[routeKey]);

            return regex.IsMatch(month);
        }
    }
}
