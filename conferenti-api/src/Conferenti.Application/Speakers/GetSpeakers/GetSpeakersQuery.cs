using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Domain.Speakers;

namespace Conferenti.Application.Speakers.GetSpeakers;

public record GetSpeakersQuery : IQuery<List<Speaker>>;
