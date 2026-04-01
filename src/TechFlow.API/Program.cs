using Scalar.AspNetCore;
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
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("TechFlow API");
        options.WithDefaultHttpClient(ScalarTarget.Http, ScalarClient.HttpClient);
    });

    // Migrate + seed in dev only
    using var scope = app.Services.CreateScope();
    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection(); 

app.UseCors("Angular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();