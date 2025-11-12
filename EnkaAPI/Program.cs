using EnkaAPI;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

var app = Startup.InicializarApp(args);
app.Run();