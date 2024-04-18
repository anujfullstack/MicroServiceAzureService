using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





builder.Services.AddAzureClients(clientBuilder =>
{
    // Get the KeyVault URL from configuration
    var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
    //// Get the Storage URL from configuration
    //var storageUrl = builder.Configuration["StorageUrl"];

    // Add a KeyVault client
    clientBuilder.AddSecretClient(new Uri(keyVaultUrl));

    // Add a storage account client
    //clientBuilder.AddBlobServiceClient(storageUrl);

   
    
    clientBuilder.UseCredential(new EnvironmentCredential());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
});



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
