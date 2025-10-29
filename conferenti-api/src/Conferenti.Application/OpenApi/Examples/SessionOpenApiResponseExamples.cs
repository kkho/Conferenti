using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conferenti.Application.OpenApi.Examples;
public static class SessionOpenApiResponseExamples
{
    public const string SessionOkResponse =
        """
        [
          {
            "id": "session-01",
            "title": "Building Scalable Microservices with .NET 9",
            "description": "Learn how to design and implement scalable microservices using the latest features in .NET 9, including improved performance and cloud-native capabilities.",
            "startTime": "2024-03-15T09:00:00Z",
            "endTime": "2024-03-15T10:30:00Z",
            "slug": null,
            "room": "Main Hall A",
            "tags": ["Architecture"],
            "format": "Presentation",
            "level": "Intermediate",
            "language": "English",
            "speakerIds": ["123e4567-e89b-12d3-a456-426614174000"]
          },
          {
            "id": "session-02",
            "title": "Advanced TypeScript Patterns",
            "description": "Explore advanced TypeScript patterns and best practices for building maintainable and type-safe applications.",
            "startTime": "2024-03-15T11:00:00Z",
            "endTime": "2024-03-15T12:00:00Z",
            "room": "Room B",
            "slug": null,
            "format": "Presentation",
            "tags": ["Frontend"],
            "level": "Advanced",
            "language": "English",
            "speakerIds":  ["223e4567-e89b-12d3-a456-426614174001"]
          }
        ]
        """;

    public const string UpsertSessionRequestBody
        = """"
          [
          {
            "id": "session-03",
            "title": "Advanced TypeScript Patterns",
            "description": "Explore advanced TypeScript patterns and best practices for building maintainable and type-safe applications.",
            "startTime": "2024-03-15T11:00:00Z",
            "endTime": "2024-03-15T12:00:00Z",
            "room": "Room B",
            "tags": ["Frontend"],
            "level": "Advanced",
            "language": "English",
            "speakerIds":  ["223e4567-e89b-12d3-a456-426614174001", "323e4567-e89b-12d3-a456-426614174002"]
          },
          ]
          """";
}
