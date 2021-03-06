
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.EntityFrameworkCore;
using mtstatemachine;
using mtstatemachine.BatchConsumers;
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


//MessageDataDefaults.TimeToLive = TimeSpan.FromDays(2);

builder.Services.AddScoped<AllocateInventoryActivity>();
builder.Services.AddScoped(typeof(IMessageValidator<>),typeof(MessageValidator<>));
builder.Services.AddMassTransit(x => {
    var assembly = Assembly.GetExecutingAssembly();
    x.AddConsumers(assembly);
    //x.AddConsumersFromNamespaceContaining<RoutingslipEventConsumer>();
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
        //cfg.UseMessageData();

         cfg.UseDelayedMessageScheduler();
        //cfg.UseMessageScheduler(new Uri("queue:quartz-scheduler"));
        

        //cfg.ReceiveEndpoint(KebabCaseEndpointNameFormatter.Instance.Consumer<RoutingSlipBatchEventConsumer>(), e =>
        //{
        //    e.PrefetchCount = 20;
        //    e.Batch<RoutingSlipCompleted>(b =>
        //    {
        //        b.TimeLimit = TimeSpan.FromSeconds(5);
        //        b.MessageLimit = 10;

        //        b.Consumer<RoutingSlipBatchEventConsumer, RoutingSlipCompleted>(ctx);
        //    });
        //});

        cfg.ServiceInstance(instance => {
            instance.ConfigureJobServiceEndpoints(js => {

                //var dbcontext= new JobServiceSagaDbContextFactory().CreateDbContext(DbContextOptionsBuilder)

                //js.UseEntityFrameworkCoreSagaRepository(() => new MassTransit.EntityFrameworkCoreIntegration.JobServiceSagaDbContext(new Microsoft.EntityFrameworkCore.DbContextOptions<MassTransit.EntityFrameworkCoreIntegration.JobServiceSagaDbContext>()));
            });
            instance.ConfigureEndpoints(ctx);
        });

        //cfg.ConfigureEndpoints(ctx);
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
