namespace ForgiveMeCalia.Application.Abstractions;

public interface ICustomAudioService
{
    Task<string> CreateAsync(string inductionPath, string mainHypnosisPath, CancellationToken cancellationToken);
}
