using MyServer.Application;

var builder = WebApplication.CreateBuilder();

var app = builder.Build();

app.MapRouting();

app.MapControllers();

await app.Run(); 