using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conferenti.Application.Abstractions.Messaging;
using Conferenti.Domain.Sessions;

namespace Conferenti.Application.Sessions.PostSessions;

public record PostSessionCommand(List<Session> Sessions) : ICommand<List<Session>>;
