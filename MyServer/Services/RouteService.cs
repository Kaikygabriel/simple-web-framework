using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyServer.Services;

public class RouteService
{
    public static IEnumerable<MethodInfo> GetAllMethodByAttribute<T>() where T: Attribute  
        =>  Assembly.GetExecutingAssembly()
            .GetTypes()
            .SelectMany(t => t.GetMethods())
            .Where(m => m.GetCustomAttribute<T>() != null);
    public static bool IsMethodVerify(string path,string pathBase)
    {
        if (pathBase.Contains('{')&&pathBase.Contains('}'))
        {
            var pathArr = path.Split('/');
            var pathBaseArr = pathBase.Split('/');
            
            if (pathArr.Length != pathBaseArr.Length)
                return false;

            for (var a = 0; a < pathArr.Length; a++)
                if (!pathArr[a].Equals(pathBaseArr[a]) && !(pathBaseArr[a].Contains('{') && pathBaseArr[a].Contains('}')))
                    return false;

            return true;
        }

        return path.Equals(pathBase);
    } 
    
    public static Dictionary<string, string> ContainsParameterFromRoute(string path,string pathBase)
    {
        var queryString = new Dictionary<string,string>();
        if (pathBase.Contains('{')&&pathBase.Contains('}'))
        {
            var pathArr = path.Split('/');
            var pathBaseArr = pathBase.Split('/');
            if (pathArr.Length == pathBaseArr.Length)
            {
                for (var a = 0; a < pathArr.Length; a++)
                    if (pathBaseArr[a].Contains( '{') && pathBaseArr[a].Contains('}'))
                    {
                        var key = pathBaseArr[a].Replace("}", "").Replace("{","");
                        queryString[key] = pathArr[a];
                    }
            }
        }
        return queryString;
    }
}