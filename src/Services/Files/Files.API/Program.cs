using Azure.Storage.Blobs;
using Files.Application;
using Files.Infrastructure.Persistence;
using Files.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

// Options
var uploadOptions = builder.Configuration.GetSection(FileUploadOptions.Section).Get<FileUploadOptions>()
    ?? new FileUploadOptions();
builder.Services.AddSingleton(uploadOptions);

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Files.Application.AssemblyMarker).Assembly));

// EF Core — PostgreSQL
builder.Services.AddDbContext<FilesDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IUserFileRepository, UserFileRepository>();

// Storage — elige implementación según configuración
if (uploadOptions.Provider == "AzureBlob")
{
    var azureOpts = builder.Configuration.GetSection(AzureBlobOptions.Section).Get<AzureBlobOptions>()!;
    builder.Services.AddSingleton(azureOpts);
    builder.Services.AddSingleton(new BlobServiceClient(azureOpts.ConnectionString));
    builder.Services.AddScoped<IFileStorageService, AzureBlobStorageService>();
}
else
{
    builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
}

// Auth
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    {
        opt.Authority = builder.Configuration["Auth:Authority"];
        opt.Audience = builder.Configuration["Auth:Audience"];
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Files Service", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });
});

// Límite global para uploads
builder.WebHost.ConfigureKestrel(k =>
    k.Limits.MaxRequestBodySize = uploadOptions.MaxFileSizeBytes);

var app = builder.Build();

// Migrar automáticamente en desarrollo
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<FilesDbContext>().Database.Migrate();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
