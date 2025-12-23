using System.Reflection;
using Expenses.API.Data;
using Expenses.API.Data.Services;
using Expenses.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container, especially the DbContext and SQL Server support.
builder.Services.AddDbContext<ExpensesDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ExpensesConnection"));
});
// Register TransactionService as a scoped service
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
// Register PasswordHasher as a scoped service
builder.Services.AddScoped<PasswordHasher<User>>();
// Register JwtHandler as a scoped service
builder.Services.AddScoped<JwtHandler>();
// Register AccountService as a scoped service
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v2", new OpenApiInfo()
    {
        Title = "Income & Expenses Service - REST API",
        Version = "Version .Net 10",
        Description = "Documentation of Income & Expenses Service (IES) REST API providing resources for income and expenses management.",
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

// Properly set up the JwtBearerMiddleware 
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

var app = builder.Build();

#region Configure the HTTP request pipeline and routes.
    
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    // Adding and enabling the middleware to generate the swagger documentation
    app.UseSwagger();
    app.UseSwaggerUI(setupAction =>
    {
        setupAction.DocumentTitle = "Income & Expenses Service (IES) - REST API";
        setupAction.SwaggerEndpoint("/swagger/v2/swagger.json", "Income & Expenses Service (IES) - Swagger UI V.2");
        // Add this custom CSS styles to Swagger.ui
        setupAction.InjectStylesheet("/swagger-ui/custom.css");
    });
}

app.UseCors("AngularExpensesPolicy");

app.UseHttpsRedirection();

// Enabling the use of static files
app.UseStaticFiles(); // For serving custom CSS for Swagger UI

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

// Start the web server, host the web API, and listen for incoming HTTP requests.
await app.RunAsync(); // This is a thread-blocking call

// Exit point of the Web API application
Console.WriteLine("The Expenses Tracker Web API has been stopped! Program will exit automatically...");