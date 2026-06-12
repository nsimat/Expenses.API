using System.Reflection;
using Asp.Versioning;
using AutoInject;
using Expenses.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

// This is the entry point of the "Expenses Tracker Web API" application.
// Create a WebApplicationBuilder builder, which is used to configure services and the web application itself.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register PasswordHasher as a scoped service
builder.Services.AddScoped<PasswordHasher<UserAccount>>();

builder.Services.AddControllers();

// Add OpenApi support to the service collection, enabling automatic generation of API documentation and interactive
// testing capabilities for the REST API endpoints.   
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;

    options.AddDocumentTransformer((doc, ctx, ct) =>
    {
        doc.Info.Title = "Income & Expenses Service - REST API";

        doc.Info.Summary = "Expense Tracker Web API for managing income and expenses of registered user accounts";

        doc.Info.Description =
            "Documentation of Income & Expenses Service (IES) REST API providing resources for income and expenses management.";

        doc.Info.Version = "v1";

        doc.Info.Contact = new OpenApiContact()
        {
            Name = "Support",
            Email = "ies-support@ies.com",
            Url = new Uri("https://www.ies.com/support")
        };

        doc.Info.License = new OpenApiLicense()
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        };
        
        return Task.CompletedTask;
    });
});
// Generate OpenAPI at /openapi/v1.json in Development

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

builder.Services.UseAutoInjection();

// Add SwaggerGen configuration to the service collection, enabling the generation of Swagger documentation for the API
// endpoints for developers and consumers of the API to have comprehensive and well-structured documentation for better
// understanding and usage of the API endpoints. 
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Income & Expenses Service - REST API",
        Version = "v1",
        Description =
            "Documentation of Income & Expenses Service (IES) REST API providing resources for income and expenses management.",
        Contact = new OpenApiContact()
        {
            Name = "Support",
            Email = "ies-support@ies.com",
            Url = new Uri("https://www.ies.com/support")
        },
        License = new OpenApiLicense()
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddCors(options =>
    options.AddPolicy(name: "AngularExpensesPolicy",
        cfg =>
        {
            cfg.AllowAnyHeader();
            cfg.AllowAnyMethod();
            cfg.WithOrigins(builder.Configuration["AllowedCORS"] ?? "https://localhost:4200");
        }));

// 1) Authentication: validate incoming bearer tokens by properly setting up the JwtBearerMiddleware 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            builder.Configuration["JwtSettings:SecurityKey"] ??
            throw new InvalidOperationException("JWT Key not found in configuration."))),
        ClockSkew = TimeSpan.Zero
    };
});

// Add API versioning to the service collection, enabling support for multiple versions of the API and allowing clients
// to specify which version they want to use when making requests. This configuration includes reporting API versions
// in responses, assuming a default version when none is specified, and defining the default API version as 1. 
// To be reviewed!!!!
/*builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.ReportApiVersions = true;
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new ApiVersion(1, 0);
}).AddMvc()
    .AddApiExplorer(setupAction =>
    {
        setupAction.SubstituteApiVersionInUrl = true;
        setupAction.GroupNameFormat = "'v'V";
    });*/

// Register IHttpContextAccessor for accessing HTTP context in services
builder.Services.AddHttpContextAccessor();

// Build the WebApplication, which will be used to configure the HTTP request pipeline and routes.
var app = builder.Build();

#region Configure the HTTP request pipeline and routes.

// Configure the HTTP request pipeline for DEVELOPMENT only
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.MapOpenApi("/openapi/{documentName}.yaml"); // Serves /openapi/v1.yaml in Development

    app.MapSwagger("/swagger/{documentName}.json"); // Serves /swagger/v1.json in Development

    app.MapScalarApiReference("/api-docs", options =>
    {
        options.WithTitle("Income & Expenses Service (IES) - REST API")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .ShowOperationId()
            .ExpandAllTags()
            .SortTagsAlphabetically()
            .SortOperationsByMethod()
            .PreserveSchemaPropertyOrder()
            .WithOpenApiRoutePattern("/openapi/{documentName}.yaml")
            .AddDocument("v1", "Version 1.0");
    }); // Serves UI at /api-docs in Development

    // Adding and enabling the middleware to generate the swagger documentation
    app.UseSwagger();
    app.UseSwaggerUI(setupAction =>
    {
        setupAction.DocumentTitle = "Income & Expenses Service (IES) - REST API";
        setupAction.SwaggerEndpoint("/swagger/v1/swagger.json", "Income & Expenses Service (IES) - Swagger UI V.1");
        // Set Swagger UI at the app's root (e.g., http://localhost:5000/)
        setupAction.RoutePrefix = string.Empty;
        // Add this custom CSS style to Swagger.ui
        setupAction.InjectStylesheet("/swagger-ui/custom.css");
    });
}

// Enabling CORS middleware to allow cross-origin requests from the Angular frontend application.
app.UseCors("AngularExpensesPolicy");

// Configure the middleware to redirect HTTP requests to HTTPS, ensuring secure communication.
app.UseHttpsRedirection();

// Enabling the use of static files
app.UseStaticFiles(); // For serving custom CSS for Swagger UI

// Enabling authentication middleware to validate incoming requests based on the configured authentication scheme
// (e.g., JWT Bearer tokens).   
app.UseAuthentication();

// Enabling authorization middleware to enforce access control policies based on authenticated users and their roles
// or claims, ensuring that only authorized users can access protected resources and perform specific actions within
// the API.
app.UseAuthorization();

// Enabling routing for handling incoming HTTP requests and mapping them to appropriate controllers and actions.
app.MapControllers();

#endregion

// Start the web server, host the web API, and listen for incoming HTTP requests.
app.Run(); // This is a thread-blocking call

// Exit point of the Web API application
Console.WriteLine("The Expenses Tracker Web API has been stopped! Program will exit automatically...");