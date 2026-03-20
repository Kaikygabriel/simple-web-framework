using MyServer.Application;

var builder = WebApplication.CreateBuilder();

var app = builder.Build();

app.MapControllers();

await app.Run(); 