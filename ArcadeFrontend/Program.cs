using ArcadeFrontend;
using ArcadeFrontend.Interfaces;
using Microsoft.Extensions.DependencyInjection;

var services = DependencyInjection.Build(args);
var host = services.Build();

var client = host.Services.GetRequiredService<IAppClient>();

client.Run();
