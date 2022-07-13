using MassTransit;
using mtstatemachine;
using mtstatemachine.Consumers;
using mtstatemachine.CourierActivities;
using mtstatemachine.StateMachines;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<AllocateInventoryActivity>();

builder.Services.AddMassTransit(x => {
    var assembly = Assembly.GetExecutingAssembly();
    x.AddConsumers(assembly);
    x.AddConsumersFromNamespaceContaining<RoutingslipEventConsumer>();
    x.AddActivities(assembly);
    x.SetKebabCaseEndpointNameFormatter();
    x.AddRequestClient<AllocationStatusRequest>();
    x.AddSagaStateMachine<AllocateStateMachine, AllocationState>(typeof(AllocationStateDefinition)).MongoDbRepository(m => {
        m.DatabaseName = "alloction";
        m.Connection = "mongodb://127.0.0.1";
    });
    x.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition)).MongoDbRepository(m => {
        m.DatabaseName = "orderstate";
        m.Connection = "mongodb://127.0.0.1";
    });
    x.AddDelayedMessageScheduler();

    x.AddRequestClient<SubmitOrder>();
    x.AddRequestClient<CheckOrderRequest>();
    x.AddRequestClient<AllocateInventory>();

    x.UsingRabbitMq((ctx, cfg) => {
        // cfg.UseDelayedMessageScheduler();
        cfg.UseMessageScheduler(new Uri("queue:quartz-scheduler"));
        cfg.ConfigureEndpoints(ctx);
    });
})
    .AddScoped<OrderAcceptedActivity>()
    .AddScoped<IServiceSvc,ServiceSvc>();

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
