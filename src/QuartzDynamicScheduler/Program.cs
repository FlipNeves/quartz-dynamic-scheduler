using QuartzDynamicScheduler.Configurations;
using QuartzDynamicScheduler.Consumers;
using QuartzDynamicScheduler.Infrastructure.Broker;
using QuartzDynamicScheduler.Infrastructure.Scheduling;
using QuartzDynamicScheduler.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<BrokerUnavailableExceptionHandler>();

builder.Services
    .AddPersistence(builder.Configuration)
    .AddBroker(builder.Configuration)
    .AddJobScheduling(builder.Configuration)
    .AddQuartzScheduler(builder.Configuration)
    .AddJob<SampleJob>()
    .AddJob<AnotherSampleJob>()
    .AddHostedService<SampleQueueConsumer>();

var app = builder.Build();

await app.Services.EnsureJobsDatabaseAsync();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
