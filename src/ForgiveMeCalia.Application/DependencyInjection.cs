using FluentValidation;
using ForgiveMeCalia.Application.Downloads.Commands.DownloadAudio;
using ForgiveMeCalia.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ForgiveMeCalia.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DownloadAudioCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(DownloadAudioCommandValidator).Assembly);
        services.AddTransient<DownloadPlanner>();
        return services;
    }
}
