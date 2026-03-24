using System.Reflection;
using MyServer.Attributes.Parameters;
using MyServer.Model.Abstraction;

namespace MyServer.Attributes.Methods;

[AttributeUsage(AttributeTargets.Method)]
public class HttpGetAttribute : Attribute
{
    public HttpGetAttribute(string EndPointName)
    {
        this.EndPointName = EndPointName;
    }
    public string EndPointName { get; private init; }
    
     public static ActionResult? ExecuteAction(string path)
    {
        var methods = GetAllMethod();
        ActionResult? result = null;
        var queryParams = new Dictionary<string, string>();
        if (path.Contains('?'))
        {
            var segregationsParts = path.Split('?');
            queryParams = ContainsParameters(path);
            
            path = segregationsParts[0];
        }
        
        foreach (var metodo in methods)
        {
            var atributo = metodo.GetCustomAttribute<HttpGetAttribute>();
            if ( atributo is not null&&path.Equals("/" + atributo.EndPointName))
            {
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

    private static Dictionary<string, string> ContainsParameters(string path)
    {
        var queryString = "";
        if (path.Contains("?"))
        {
            var split = path.Split('?');
            path = split[0];         // "/hello"
            queryString = split[1];  // "name=kaiky&age=18"
        }
        
        var queryParams = new Dictionary<string, string>();
        foreach (var a in queryString.Split('&'))
        {
            var parameter = a.Split('=');
            if (parameter.Length == 2)
            {
                queryParams[parameter[0]] = parameter[1];
            }
        }

        return queryParams;
    }
}