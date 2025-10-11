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
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "My Income & Expenses API",
        Version = "v1",
        Description = "Backend service that provides resources for managing income and expenses.",
        Contact = new OpenApiContact()
        {
            Name = "Mike Matondo",
            Email = "mike.matondo@gmail.com"
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

builder.Services.AddScoped<PasswordHasher<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(setupAction =>
    {
        setupAction.DocumentTitle = "My Income & Expenses API";
        setupAction.SwaggerEndpoint("/swagger/v1/swagger.json", "My Income & Expenses API V1");
    });
}

app.UseCors("AngularExpensesPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();