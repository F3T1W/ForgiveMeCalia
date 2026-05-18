using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Domain.Enums;
using MediatR;

namespace ForgiveMeCalia.Application.Downloads.Commands.DownloadAudio;

public sealed record DownloadAudioCommand(DownloadScope Scope) : IRequest<DownloadSummary>;
