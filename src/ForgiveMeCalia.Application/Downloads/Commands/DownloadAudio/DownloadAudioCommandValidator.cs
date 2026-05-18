using FluentValidation;
using ForgiveMeCalia.Application.Localization;
using ForgiveMeCalia.Domain.Enums;

namespace ForgiveMeCalia.Application.Downloads.Commands.DownloadAudio;

public sealed class DownloadAudioCommandValidator : AbstractValidator<DownloadAudioCommand>
{
    public DownloadAudioCommandValidator()
    {
        RuleFor(x => x.Scope)
            .Must(scope => scope is DownloadScope.Free or DownloadScope.Paid or DownloadScope.All)
            .WithMessage(AppText.T("download.validatorScope"));
    }
}
