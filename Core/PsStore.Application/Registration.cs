using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PsStore.Application.Bases;
using PsStore.Application.Behaviors;
using PsStore.Application.Exceptions;
using PsStore.Application.Features.Game.Commands.CreateGame;
using System.Reflection;

namespace PsStore.Application
{
    public static class Registration
    {
        public static void AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddTransient<ExceptionMiddleware>();
            services.AddRulesFromAssemblyContaining(assembly, typeof(BaseRules));

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));


            services.AddValidatorsFromAssemblyContaining<CreateGameCommandValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));
        }

        private static IServiceCollection AddRulesFromAssemblyContaining(
            this IServiceCollection services,
            Assembly assembly,
            Type type)
        {
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
            foreach (var item in types)
                services.AddTransient(item);

            return services;
        }
    }
}
