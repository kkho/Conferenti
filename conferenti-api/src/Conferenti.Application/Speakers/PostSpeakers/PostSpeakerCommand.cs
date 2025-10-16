using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Domain.Speakers;

namespace Conferenti.Application.Speakers.PostSpeakers;
public record PostSpeakerCommand(List<Speaker> Speakers) : ICommand<List<Speaker>>;
