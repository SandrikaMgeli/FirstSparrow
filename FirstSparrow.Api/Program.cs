using FirstSparrow.Api;
using FirstSparrow.Api.Middlewares;
using FirstSparrow.Application;
using FirstSparrow.Infrastructure;
using FirstSparrow.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi("docs");
    
builder.Services
    .AddApi(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<RequestMetadataMiddleware>();

app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/docs.json", "LionBitcoin.Payments.Service");
});

app.MapControllers();

app.SyncDatabase();

app.Run();