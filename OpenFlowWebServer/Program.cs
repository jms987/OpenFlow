using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AspNetCore.Identity.Mongo;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenFlowWebServer.Components;
using OpenFlowWebServer.Components.Account;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Repository;
using OpenFlowWebServer.Repository.DbRepository;
using OpenFlowWebServer.Services;
using Swashbuckle.AspNetCore.Swagger;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using OpenFlowWebServer.Services.SecurityMethodsServices;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddRazorPages();
        // Add services to the container.  
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "Project API v1", Version = "v1" });
            options.SwaggerDoc("v2", new() { Title = "Project API v2", Version = "v2" });
        });
        builder.Services.AddControllers();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMongoDB(builder.Configuration["Databases:CosmosDB:ConnectionString"],
                builder.Configuration["Databases:CosmosDB:DatabaseName"]));
        builder.Services.AddLogging();
        builder.Services.AddScoped<ILogRepository, LogRepository>();
        builder.Services.AddScoped<IHyperparameterRepository, HyperparameterRepository>();
        builder.Services.AddScoped<IDatasetRepository, DatasetRepository>();
        builder.Services.AddScoped<IModelRepository, ModelRepository>();
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
        builder.Services.AddScoped<IFileRepository, FileRepository>();
        builder.Services.AddScoped<ISecurityRepository, SecurityRepository>();

        builder.Services.AddSingleton<BlobServiceClient>(
            new BlobServiceClient(builder.Configuration["Databases:BlobStorage:ConnectionString"]));
        builder.Services.AddScoped<IBlobRepository<string>, BlobRepository>();
        builder.Services.AddScoped<IBlobRepository<byte[]>, BlobRepository>();
        builder.Services.AddScoped<IBlobRepository<Stream>, BlobRepository>();

        builder.Services.AddSingleton<QueueServiceClient>(
            new QueueServiceClient(builder.Configuration["Databases:QueueStorage:ConnectionString"]));
        builder.Services.AddScoped<IQueueRepository, QueueRepository>();

        builder.Services.AddScoped<IBrowserFileService, BrowserFileService>();
        builder.Services.AddScoped<IDatasetServices, DatasetServices>();
        builder.Services.AddScoped<IParametersListServices, ParametersListServices>();
        builder.Services.AddScoped<IDeployProjectServices, DeployProjectServices>();
        builder.Services.AddScoped<IJWTService, JWTService>();
        builder.Services.AddScoped<ILoginServices, LoginServices>();


        //security services
        builder.Services.AddScoped<ISecurityMethodService, PasswordSecurityMethodService>();
        builder.Services.AddScoped<ISecurityMethodService, SecretSecurityMethodService>();


        builder.Services.AddIdentityMongoDbProvider<ApplicationUser>(mongoOptions =>
        {
            mongoOptions.ConnectionString = builder.Configuration["Databases:CosmosDB:ConnectionString"] + '/' +
                                            builder.Configuration["Databases:CosmosDB:DatabaseName"];
        });

        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        // --- AUTH: użyj schematu Identity jako domyślnego (IdentityConstants.ApplicationScheme)
        // Zarejestruj JwtBearer tylko jako dodatkowy schemat (nie zmieniamy domyślnego)
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.Cookie.Name = "AspNetCore.Identity.Application";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // wymaga HTTPS w prod
            options.Cookie.SameSite = SameSiteMode.Lax; // jeśli cross-site → None
        });

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "dev_secret_please_change"))
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            // policy akceptująca cookie (Identity) lub JWT
            options.AddPolicy("CookieOrJwt", policy =>
            {
                policy.AddAuthenticationSchemes(IdentityConstants.ApplicationScheme, JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });

            // jeśli chcesz endpointy tylko dla JWT, zdefiniuj:
            options.AddPolicy("JwtOnly", policy =>
            {
                policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });
        });

        var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });

        apiVersioningBuilder.AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.  
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        $"OpenFlowWebServer {description.GroupName.ToUpperInvariant()}");
                }
                c.RoutePrefix = "swagger";
            });
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        // KLASYCZNA KOLEJNOŚĆ MIDDLEWARE:
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // <-- WAŻNE: authentication before authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // jeśli masz jakieś dodatkowe middleware anty-forgery itp. możesz trzymać je po auth/authz
        app.UseAntiforgery();

        // Map endpoints (controllers + Blazor)
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorComponents<App>().AddInteractiveServerRenderMode();
          //  endpoints.MapFallbackToPage("/home"); // jeśli potrzebujesz fallback
        });

        app.MapAdditionalIdentityEndpoints();

        app.Run();
    }
}
