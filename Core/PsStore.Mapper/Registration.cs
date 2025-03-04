using Microsoft.Extensions.DependencyInjection;
using PsStore.Application.Interfaces.AutoMapper;

namespace PsStore.Mapper
{
    public static class Registration
    {
        public static void AddCustomMapper(this IServiceCollection services)
        {
            services.AddSingleton<IMapper, AutoMapper.Mapper>();
        }
    }
}
