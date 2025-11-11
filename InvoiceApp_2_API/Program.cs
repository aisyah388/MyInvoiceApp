using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyInvoiceApp.Shared.Model;
using MyInvoiceApp_API.Controller;
using MyInvoiceApp_API.Data;
using MyInvoiceApp_API.Services.Implementation;
using MyInvoiceApp_API.Services.Repository;
using MyInvoiceApp_API.Validators;
using MyInvoiceApp_Shared.Model;
using System;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Register interface and services
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IValidator<Invoice>, InvoiceValidator>();
builder.Services.AddScoped<IValidator<Invoice_Item>, Invoice_ItemValidator>();
builder.Services.AddScoped<IValidator<Client>, ClientValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
