using backend.Controllers;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapNoteControllers();

app.Run();
