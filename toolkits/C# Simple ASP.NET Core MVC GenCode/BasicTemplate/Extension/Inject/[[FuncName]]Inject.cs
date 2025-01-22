using [[ProjName]].DataAccess;
using [[ProjName]].DataAccess.Interface;
using [[ProjName]].Services;
using [[ProjName]].Services.Interface;

namespace [[ProjName]].Extension
{
    public static partial class ServiceInjection
    {
        public static void [[FuncName]]Inject(IServiceCollection services)
        {
            services.AddScoped<I[[FuncName]]Service, [[FuncName]]Service>();
            services.AddScoped<I[[FuncName]]DataAccess, [[FuncName]]DataAccess>();
        }
    }
}
