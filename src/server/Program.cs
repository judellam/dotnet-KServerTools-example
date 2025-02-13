using KServerTools.Common;
using server;
using server.Components;
using server.Configs;
using server.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
services.AddControllers();
services.AddSwaggerGen();
services
    .AddOpenApi()
    .AddExceptionHandler<ExceptionHandler>()

    // App functionality
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    .AddSingleton<IUserComponent, UserComponent>()
    .AddSingleton<IUserRepository, UserSqlRepository>()

    // KST Add-ons
    .KSTAddRequestContext<RequestContext>()
    .KSTAddCommon()
    .KSTAddLogger()
    .KSTAddSqlServiceConnectionString<UserDatabaseSqlServerConfiguration>()

    // Configs
    .AddSingleton(static impl=> {
        var configHelper = impl.GetService<ConfigurationHelper>() ?? throw new InvalidOperationException("ConfigurationHelper service is not available.");
        var config = configHelper.TryGet<UserDatabaseSqlServerConfiguration>() ?? throw new InvalidOperationException("UserDatabaseSqlServerConfiguration could not be retrieved.");

        // For more complicated configurations, you can use the secret resolver. The secret resolver will require
        // and AKV. Below is the example code.
        // You must add .KSTAddSecretResolver() and add an AKV configuration.
        //ISecretResolver secretResolver = impl.GetService<ISecretResolver>() ?? throw new InvalidOperationException("ISecretResolver service is not available.");
        //config.SecretResolver = secretResolver;
        return config;
    })

    // Authorization
    .AddAuthorization()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(static options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false,
            ClockSkew = TimeSpan.FromMinutes(1),
            ValidIssuer = Constants.Jwt.Issuer,
            ValidAudience = Constants.Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.Jwt.Secret))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.TryAdd("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
        };
    });

WebApplication app = builder.Build();
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // Register 1st
app.UseAuthorization();  // Register 2nd
app.MapControllers();
app.UseExceptionHandler(_ => {});

app.Run();