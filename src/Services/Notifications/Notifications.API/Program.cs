using MassTransit;
using Notifications.Application;
using Notifications.Infrastructure.Email;
using Notifications.Infrastructure.Messaging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Notifications.Application.AssemblyMarker>());

// SMTP options
var smtpOpts = builder.Configuration.GetSection(SmtpOptions.Section).Get<SmtpOptions>()!;
builder.Services.AddSingleton(smtpOpts);

// Email + Template
builder.Services.AddScoped<IEmailService, MailKitEmailService>();
builder.Services.AddSingleton<IEmailTemplateRenderer, ScribanTemplateRenderer>();

// MassTransit — consume eventos del bus
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReportGeneratedConsumer>();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], h =>
        {
            h.Username(builder.Configuration["RabbitMQ:User"]!);
            h.Password(builder.Configuration["RabbitMQ:Password"]!);
        });
        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    {
        opt.Authority = builder.Configuration["Auth:Authority"];
        opt.Audience = builder.Configuration["Auth:Audience"];
    });
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();
