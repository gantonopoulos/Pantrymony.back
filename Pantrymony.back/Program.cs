using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// }).AddJwtBearer(options =>
// {
//     options.Authority = configuration["Auth0:Authority"];
//     options.Audience = configuration["Auth0:ApiIdentifier"];
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors(policyBuilder =>
{
    policyBuilder
        .AllowAnyOrigin()
        //.WithOrigins("https://localhost:7260", "http://localhost:5123")
        .AllowAnyMethod()
        .WithHeaders(HeaderNames.ContentType);
});
//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();