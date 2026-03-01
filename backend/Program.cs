using backend.Controllers;
using backend.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<INoteRepository, NoteImplement>();

var app = builder.Build();

app.MapNoteControllers();

app.Run();
