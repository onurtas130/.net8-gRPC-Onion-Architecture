using Grpc.Services;
using Persistence;
using Business;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// Injections
builder.Services.AddPersistenceServices();
builder.Services.AddBusinessServices();
builder.Services.AddInfrastructureServices();

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        //Token Producer
        ValidateIssuer = false,
        //Token Consumer
        ValidateAudience = false,
        //Use secretKey to validate
        ValidateIssuerSigningKey = true,
        //Use Expire time to validate
        ValidateLifetime = true,
        //SecretKey
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTToken:SecretKey"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<UserService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


app.UseAuthentication();

app.UseAuthorization();

app.Run();
