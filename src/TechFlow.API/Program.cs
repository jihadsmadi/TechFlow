using TechFlow.API;
using TechFlow.Application;
using TechFlow.Infrastructure;
using TechFlow.Infrastructure.Persistence;


var builder = WebApplication.CreateBuilder(args);

// ── Services 
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddAPI(builder.Configuration)
    .AddProblemDetails();

var app = builder.Build();

 //── Middleware 
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

// Migrate + seed in dev only
using var scope = app.Services.CreateScope();
var initialiser = scope.ServiceProvider
    .GetRequiredService<ApplicationDbContextInitialiser>();
await initialiser.InitialiseAsync();
await initialiser.SeedAsync();
}

app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();
app.MapControllers();

app.Run();