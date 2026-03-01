using System.ComponentModel.DataAnnotations;
using backend.Dtos;
using backend.Model;
using backend.Repository;

namespace backend.Controllers;

public static class NoteEndpoints
{
    public static void MapNoteControllers(this WebApplication app)
    {
        var notes = app.MapGroup("/api/v1/notes");

        notes.MapGet("/", async (string? title, int? type, INoteRepository repo, int page = 1, int pageSize = 15) =>
        {
            if (pageSize > 100) pageSize = 100;
            var data = await repo.GetPagedAsync(title, type, page, pageSize);
            return Results.Ok(data);
        });

        notes.MapGet("/{id:int}", async (int id, INoteRepository repo) =>
        {
            var note = await repo.GetByIdAsync(id);
            return note is not null ? Results.Ok(note) : Results.NotFound();
        });

        notes.MapPost("/",
            async (CreateNoteRequest request, INoteRepository repo) =>
            {
                var created = await repo.CreateAsync(request);
                return Results.Created($"/api/v1/notes/{created.Id}", created);
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var body = context.GetArgument<CreateNoteRequest>(0);
                var errors = Validate(body);
                if (errors.Count > 0)
                    return Results.ValidationProblem(errors);
                return await next(context);
            });

        notes.MapPut("/{id:int}",
            async (int id, UpdateNoteRequest request, INoteRepository repo) =>
            {
                var updated = await repo.UpdateAsync(id, request);
                return updated is not null ? Results.Ok(updated) : Results.NotFound();
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var body = context.GetArgument<UpdateNoteRequest>(1);
                var errors = Validate(body);
                if (errors.Count > 0)
                    return Results.ValidationProblem(errors);
                return await next(context);
            });

        notes.MapDelete("/{id:int}", async (int id, INoteRepository repo) =>
        {
            var deleted = await repo.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }

    private static Dictionary<string, string[]> Validate(object instance)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(instance);
        if (Validator.TryValidateObject(instance, context, results, validateAllProperties: true))
            return new Dictionary<string, string[]>();

        var errors = new Dictionary<string, string[]>();
        foreach (var r in results)
        {
            var key = r.MemberNames.FirstOrDefault() ?? "Request";
            var message = r.ErrorMessage ?? "Invalid value.";
            if (errors.TryGetValue(key, out var list))
            {
                var newList = list.ToList();
                newList.Add(message);
                errors[key] = newList.ToArray();
            }
            else
            {
                errors[key] = [message];
            }
        }
        return errors;
    }
}
