using System.Reflection;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Notes.Application.Common.Mappings;
using Notes.Application.Interfaces;
using Notes.Application;
using Notes.Persistence;
using Notes.WebApi.Middleware;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Notes.WebApi;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Notes.WebApi.Services;
using Serilog.Events;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.File("NotesWebAppLog-.txt", rollingInterval:
                    RollingInterval.Day)
                .CreateLogger();

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
    config.AddProfile(new AssemblyMappingProfile(typeof(INotesDbContext).Assembly));
});

builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddPersistence(builder.Configuration);

// � �������� ������� ��� �� ������ - ��� ���� ������ ������ ������ � ������.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = 
                JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer("Bearer", options =>
        {
            options.Authority = "https://localhost:7081/";
            options.Audience = "NotesWebAPI";
            options.RequireHttpsMetadata = false;
        });

builder.Services.AddEndpointsApiExplorer();

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context = serviceProvider.GetRequiredService<NotesDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception exception)
    {
        Log.Fatal(exception, "An error occurred while app initialization");
    }

}
builder.Services.AddVersionedApiExplorer(options =>
                options.GroupNameFormat = "'v'VVV");
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>
                                ,ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning();

//logging
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();
builder.Host.UseSerilog();

var app = builder.Build();

//��� ��� ����� ������� � 5 �� 6 ������, ��� ������ Configure
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger();
app.UseSwaggerUI(config =>
{
    foreach (var description in provider.ApiVersionDescriptions)
    {
        config.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
        config.RoutePrefix = string.Empty;
    }
});

if (app.Environment.IsDevelopment())
{
    
}

//app.UseSerilogRequestLogging();
app.UseCustomExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseApiVersioning();
app.MapControllers();

app.Run();
