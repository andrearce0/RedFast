using RedFast.Modules.Notifications;
using RedFast.Modules.Notifications.Infrastructure.Messaging;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<RabbitMqConsumer>();

var host = builder.Build();
host.Run();
