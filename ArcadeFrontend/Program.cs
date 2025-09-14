using ArcadeFrontend;
using Microsoft.Extensions.DependencyInjection;

var services = DependencyInjection.Build(args);
var host = services.Build();

var client = host.Services.GetRequiredService<AppClient>();

client.Run();
