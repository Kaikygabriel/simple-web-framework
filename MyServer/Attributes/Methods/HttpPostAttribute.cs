using System.Reflection;
using System.Text.Json;
using MyServer.Abstraction;
using MyServer.Attributes.Parameters;
using MyServer.Model.Abstraction;

namespace MyServer.Attributes.Methods;

[AttributeUsage(AttributeTargets.Method)]
public class HttpPostAttribute :Attribute, IMethod
{
    public HttpPostAttribute(string EndPointName)
    {
        this.EndPointName = EndPointName;
    }
    public string EndPointName { get; private init; }

    public static ActionResult? ExecuteAction(string path, string body = "")
    {
        var methods = GetAllMethod();
        ActionResult? result = null;
        
        foreach (var metodo in methods)
        {
            var atributo = metodo.GetCustomAttribute<HttpPostAttribute>();

            if ( atributo is not null&& IsMethodVerify(path,"/"+atributo.EndPointName))
            {
                var endPointName = "/" + atributo!.EndPointName;
                
                var instancia = Activator.CreateInstance(metodo.DeclaringType!);
                var parametros = metodo.GetParameters();

                if (parametros.Length > 0)
                {
                    var argumentos = new object?[parametros.Length];

                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        argumentos = GetParametersBody(body).ToArray();
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
            .Where(m => m.GetCustomAttribute<HttpPostAttribute>() != null);
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
                        return false;
                }
                return true;
            }
            return false;
        }

        return path.Equals(pathBase);
    }

    private static List<Object> GetParametersBody(string body)
    {
        return JsonSerializer.Deserialize<List<object>>(body) ?? new();
    }
}