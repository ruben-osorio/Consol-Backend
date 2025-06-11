using Microsoft.Extensions.DependencyInjection;
using Consolidado.Services;

namespace Consolidado
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IAsistenciaService, AsistenciaService>();
            services.AddScoped<ITurnoService, TurnoService>();
        }
    }
} 