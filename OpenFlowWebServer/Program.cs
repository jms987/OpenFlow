using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenFlowWebServer.Components;
using OpenFlowWebServer.Components.Account;
using OpenFlowWebServer.Data;

using AspNetCore.Identity.Mongo;
using OpenFlowWebServer.Data.Repositories;
using OpenFlowWebServer.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.  
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
   options.UseMongoDB(builder.Configuration["Databases:CosmosDB:ConnectionString"], builder.Configuration["Databases:CosmosDB:DatabaseName"]));
builder.Services.AddLogging();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IHyperparameterRepository, HyperparameterRepository>();
builder.Services.AddScoped<IDatasetRepository, DatasetRepository>();
builder.Services.AddScoped<IModelRepository, ModelRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();

builder.Services.AddSingleton<BlobServiceClient>(new BlobServiceClient(builder.Configuration["Databases:BlobStorage:ConnectionString"]));
builder.Services.AddScoped<IBlobRepository<string>, BlobRepository>();
builder.Services.AddScoped<IBlobRepository<byte[]>, BlobRepository>();
builder.Services.AddScoped<IBlobRepository<Stream>, BlobRepository>();

builder.Services.AddScoped<IBrowserFileService, BrowserFileService>();

builder.Services.AddIdentityMongoDbProvider<ApplicationUser>(

    mongoOptions => {
        mongoOptions.ConnectionString = builder.Configuration["Databases:CosmosDB:ConnectionString"]+'/'+ builder.Configuration["Databases:CosmosDB:DatabaseName"];
    });

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.  
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.Run();
