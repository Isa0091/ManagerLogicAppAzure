using ManagerLogicAppAzure.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerLogicAppAzure.Injection
{
    public static class ManagerLogicAppAzureExtension
    { 
    /// <summary>
    /// Agrega la injeccion de dependencias
    /// </summary>
    /// <param name="services"></param>
    public static void AddManagerLogicAppAzure(this IServiceCollection services)
    {
        services.AddScoped<ILogicAppProvider>(x => new LogicAppProvider(x.GetRequiredService<ILoggerFactory>().CreateLogger<LogicAppProvider>()));
    }
}
}
