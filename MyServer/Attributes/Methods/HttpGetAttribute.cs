using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyServer.Abstraction;
using MyServer.Attributes.Parameters;
using MyServer.Jwt.Services;
using MyServer.Model.Abstraction;

namespace MyServer.Attributes.Methods;

[AttributeUsage(AttributeTargets.Method)]
public class HttpGetAttribute : Attribute,IMethod
{
    public HttpGetAttribute(string EndPointName)
    {
        this.EndPointName = EndPointName;
    }
    public string EndPointName { get; private init; }
    
    public static ActionResult? ExecuteAction(string path,List<string> lines,string body = "")
    {
        var methods = GetAllMethod();
        ActionResult? result = null;
        var queryParams = new Dictionary<string, string>();
        if (path.Contains('?'))
        {
            var segregationsParts = path.Split('?');
            queryParams = ContainsParametersFromQuery(path);
            
            path = segregationsParts[0];
        }
        
        foreach (var metodo in methods)
        {
            var atributo = metodo.GetCustomAttribute<HttpGetAttribute>();

            if ( atributo is not null&& IsMethodVerify(path,"/"+atributo.EndPointName))
            {
                var endPointName = "/" + atributo!.EndPointName;

                var attributeAuthorize = metodo.GetCustomAttribute<AuthorizeAttribute>();
                if (attributeAuthorize is not null)
                {
                    var authentication = lines.FirstOrDefault(x => x.Contains("Authorization:"));
                    if(authentication is null )
                        return new ActionResult("", "HTTP/1.1 401 Unauthorized");
                    
                    var authenticationTokenSeparator = authentication.Split(' ');
                    if (authenticationTokenSeparator.Length != 3)
                        return new ActionResult("", "HTTP/1.1 401 Unauthorized");

                    var tokenReceptor = authenticationTokenSeparator.Last();
                    if (!JwtHandler.Verifytoken(tokenReceptor))
                        return new ActionResult("", "HTTP/1.1 401 Unauthorized");
                }
                
                
                var instancia = Activator.CreateInstance(metodo.DeclaringType!);
                var parametros = metodo.GetParameters();

                if (parametros.Length > 0)
                {
                    var argumentos = new object?[parametros.Length];
                    for (var p =0; p < parametros.Length ; p++)
                    {
                        var parametro = parametros[p];
                        var fromQuery = parametro.GetCustomAttribute<FromQueryAttribute>();
                    
                        if (fromQuery != null)
                        {
                            var nome = fromQuery.Value ?? parametro.Name!;
                    
                            if (queryParams.TryGetValue(nome, out var valor))
                                argumentos[p] = Convert.ChangeType(valor, parametro.ParameterType);
                            else
                                argumentos[p] = null;
                        }
                    } 
              
                    var routeParams = ContainsFromRouteParameter(path,endPointName);
                    for (var p =0; p < parametros.Length ; p++)
                    {
                        var parametro = parametros[p];
                        var fromRoute = parametro.GetCustomAttribute<FromRouteAttribute>();
                    
                        if (fromRoute != null)
                        {
                            var nome = fromRoute.Value ?? parametro.Name!;
                    
                            if (routeParams.TryGetValue(nome, out var valor))
                                argumentos[p] = Convert.ChangeType(valor, parametro.ParameterType);
                            else
                                argumentos[p] = null;
                        }
                    } 
                    result = (ActionResult)metodo.Invoke(instancia,argumentos)!;
                }
                else
                    result = (ActionResult)metodo.Invoke(instancia,null)!;
            }
        }
        return result;
    }
    
    private static IEnumerable<MethodInfo> GetAllMethod()
    {
        var assembly = Assembly.GetExecutingAssembly();

        return assembly.GetTypes()
            .SelectMany(t => t.GetMethods())
            .Where(m => m.GetCustomAttribute<HttpGetAttribute>() != null);
    }

    private static Dictionary<string, string> ContainsParametersFromQuery(string path)
    {
        var queryString = "";
        if (path.Contains("?"))
            queryString =  path.Split('?')[1];  
        
        var queryParams = new Dictionary<string, string>();
        foreach (var a in queryString.Split('&'))
        {
            var parameter = a.Split('=');
            if (parameter.Length == 2)
                queryParams[parameter[0]] = parameter[1];
        }
        return queryParams;
    }
    
    private static Dictionary<string, string> ContainsFromRouteParameter(string path,string pathBase)
    {
        var queryString = new Dictionary<string,string>();
        if (pathBase.Contains('{')&&pathBase.Contains('}'))
        {
            var pathArr = path.Split('/');
            var pathBaseArr = pathBase.Split('/');
            if (pathArr.Length == pathBaseArr.Length)
            {
                for (var a = 0; a < pathArr.Length; a++)
                {
                    if (pathBaseArr[a].Contains('{') && pathBaseArr[a].Contains('}'))
                    {
                        var key = pathBaseArr[a].Replace("}", "").Replace("{","");
                        queryString[key] = pathArr[a];
                    }
                }
            }
        }

        return queryString;
    }

    private static bool IsMethodVerify(string path,string pathBase)
    {
        if (pathBase.Contains('{')&&pathBase.Contains('}'))
        {
            var pathArr = path.Split('/');
            var pathBaseArr = pathBase.Split('/');
            if (pathArr.Length == pathBaseArr.Length)
            {
                for (var a = 0; a < pathArr.Length; a++)
                {
                    if (path[a].Equals(pathBase[a]))
                    {
                        
                    }
                    else if (pathBaseArr[a].Contains('{') && pathBaseArr[a].Contains('}'))
                    {
                        
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            
            return false;
        }

        return path.Equals(pathBase);
    }
}