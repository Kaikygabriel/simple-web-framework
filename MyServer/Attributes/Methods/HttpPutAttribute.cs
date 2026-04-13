using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using MyServer.Abstraction;
using MyServer.Attributes.Parameters;
using MyServer.Jwt.Services;
using MyServer.Model.Abstraction;
using MyServer.Services;

namespace MyServer.Attributes.Methods;

[AttributeUsage(AttributeTargets.Method)]
public class HttpPutAttribute : Attribute,IMethod
{
    public HttpPutAttribute(string endPointName)
    {
        EndPointName = endPointName;
    }

    public string EndPointName { get;private init; }
    public static ActionResult? ExecuteAction(string path,List<string> lines, string body = "")
    {
        var methods = RouteService.GetAllMethodByAttribute<HttpPutAttribute>();
        ActionResult? result = null;
        
        foreach (var metodo in methods)
        {
            var atributo = metodo.GetCustomAttribute<HttpPutAttribute>();

            if ( atributo is not null&& RouteService.IsMethodVerify(path,"/"+atributo.EndPointName))
            {
                
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
                
                var endPointName = $"/{atributo!.EndPointName}";
                
                var instancia = Activator.CreateInstance(metodo.DeclaringType!);
                var parametros = metodo.GetParameters();

                if (parametros.Length > 0)
                {
                    var argumentos = new object[parametros.Length];
                    
                    var routeParams = RouteService.ContainsParameterFromRoute(path,endPointName);
                    for (var p =0; p < parametros.Length ; p++)
                    {
                        var parametro = parametros[p];
                        var fromRoute = parametro.GetCustomAttribute<FromRouteAttribute>();
                        var fromBody = parametro.GetCustomAttribute<FromBodyAttribute>();
                        
                        if (fromRoute != null)
                        {
                            var nome = fromRoute.Value ?? parametro.Name!;
                    
                            if (routeParams.TryGetValue(nome, out var valor))
                                argumentos[p] = Convert.ChangeType(valor, parametro.ParameterType);
                            else
                                argumentos[p] = null;
                        }
                        else if (fromBody != null)
                        {
                            argumentos[p] = JsonSerializer.Deserialize(
                                body,
                                parametro.ParameterType,
                                new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true
                                }
                            );
                        }
                        else
                            argumentos[p] = null;
                    }
                    result = (ActionResult)metodo.Invoke(instancia,argumentos.ToArray())!;
                }
                else
                    result = (ActionResult)metodo.Invoke(instancia,null)!;
            }
        }
        return result;
    }
}