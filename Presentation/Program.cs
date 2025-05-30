using Data.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using Service.Models;
using Service.Services;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
#region || Configure Services
var connectionString = builder.Configuration.GetConnectionString("accountConnection") ?? throw new NullReferenceException("DB Connectionstring is not present");
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? throw new NullReferenceException("JWT Issuer is not present");
var jwtAudience = builder.Configuration["JWT:Audience"] ?? throw new NullReferenceException("JWT Audience is not present");
var jwtKeyBase64 = builder.Configuration["JWT:PublicKey"] ?? throw new NullReferenceException("JWT Public key is not present");


builder.Services.AddOpenApi();
builder.Services.AddDbContext<SqlServerDbContext>(opt =>
{
    opt.UseSqlServer(connectionString);
});
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddHostedService<ESBService>();

builder.Services.AddCors();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        RSA rsa = RSA.Create();
        byte[] publicKeyBytes = Convert.FromBase64String(jwtKeyBase64);
        rsa.ImportRSAPublicKey(publicKeyBytes, out _);

        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
    });
#endregion

var app = builder.Build();
#region || App Configurations
app.MapOpenApi();
app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.UseAuthentication();
app.UseAuthorization();
#endregion

#region || Endpoints

app.MapPost("/account", async (AccountDto dto, IAccountService service) =>
{
    if (dto is null || dto.Id == Guid.Empty)
        return Results.BadRequest("Bad Payload");

    var result = await service.CreateAccountAsync(dto);

    return result.Success
        ? Results.Ok(result)
        : Results.BadRequest(result);
});


app.MapGet("/account/{id}", async (Guid id, IAccountService service) =>
{
    var result = await service.GetAccountAsync(id);

    return result.Success
        ? Results.Ok(result)
        : Results.BadRequest(result);
});


// For later
// Add endpoints for update / delete
//
#endregion


app.Run();


