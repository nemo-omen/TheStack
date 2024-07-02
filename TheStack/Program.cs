using System.Text.Json;
using System.Text.Json.Serialization;
using TheStack.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
    {
        var stack = new Node("Your Stack", "This is the root stack", null, null);
        stack.AddChild(new Node("Task 1", "This is the first task", null, stack.Id));
        stack.AddChild(new Node("Task 2", "This is the second task", null, stack.Id));
        
        var taskStack = new Node("Task Stack", "This is a stack of tasks", null, stack.Id);
        stack.AddChild(taskStack);
        taskStack.AddChild(new Node("Task 3", "This is the third task", null, taskStack.Id));
        taskStack.AddChild(new Node("Task 4", "This is the fourth task", null, taskStack.Id));

        return Results.Json(stack, new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
        });
    })
    .WithName("GetStack")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}