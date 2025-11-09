using Microsoft.AspNetCore.Routing;

namespace Conferenti.Application.Endpoints;

public interface IEndpoint
{
    void AddEndpoints(IEndpointRouteBuilder app);
}
