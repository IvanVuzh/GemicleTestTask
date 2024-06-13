using GemicleTestTaskApi.Services;
using GemicleTestTaskApi.Services.Interfaces;
using GemicleTestTaskData;
using GemicleTestTaskData.Repositories;
using GemicleTestTaskData.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CampaignDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("default"));
});

builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerCampaignSchedulerRepository, CustomerCampaignSchedulerRepository>();
builder.Services.AddScoped<ICampaignSchedulerService, CampaignSchedulerService>();
builder.Services.AddScoped<ICustomerProcessingService, CustomerProcessingService>();

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
