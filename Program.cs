// var builder = WebApplication.CreateBuilder(args);
// var app = builder.Build();

// // A simple in-memory list to simulate a database
// var todos = new List<string> { "Learn ASP.NET", "Build an API", "Ship it!" };

// // GET: Retrieve all items
// app.MapGet("/todos", () => todos);

// // POST: Add a new item
// //http://localhost:5041/todos?task=Drink%20Coffee
// app.MapPost("/todos", (string task) => {
//     todos.Add(task);
//     return Results.Created($"/todos/{todos.Count - 1}", task);
// });

// app.Run();



var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var todos = new List<string> { "Learn ASP.NET", "Build an API", "Ship it!" };

// GET: All items
app.MapGet("/todos", () => todos);

// GET: A specific item by index (e.g., /todos/1)
app.MapGet("/todos/{id:int}", (int id) => 
    id >= 0 && id < todos.Count ? Results.Ok(todos[id]) : Results.NotFound("Todo not found."));

// POST: Add new
//curl -i -X POST "http://localhost:5041/todos?task=Drink%20Coffee"
app.MapPost("/todos", (string task) => {
    todos.Add(task);
    return Results.Created($"/todos/{todos.Count - 1}", task);
});

// PUT: Update existing
app.MapPut("/todos/{id:int}", (int id, string updatedTask) => {
    if (id < 0 || id >= todos.Count) return Results.NotFound("Todo not found.");
    todos[id] = updatedTask;
    return Results.Ok(todos[id]);
});

// DELETE: Remove item
app.MapDelete("/todos/{id:int}", (int id) => {
    if (id < 0 || id >= todos.Count) return Results.NotFound("Todo not found.");
    todos.RemoveAt(id);
    return Results.NoContent();
});

app.Run();
