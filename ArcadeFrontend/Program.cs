using ArcadeFrontend;
using Serilog;
using Autofac;

var builder = DependencyInjection.Build();
using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder
    .RegisterInstance(log)
    .AsSelf()
    .AsImplementedInterfaces();

var container = builder
    //.BuildGameClient()
    //.BuildClientMediatr()
    .Build()
    ;

var client = container.Resolve<AppClient>();

client.Run();
