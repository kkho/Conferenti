namespace Conferenti.Application.Exceptions.Examples;

public static class OpenApiSharedResponseExamples
{
    public const string Unauthorized
    = """
          "HTTP/1.1 401 Unauthorized\nDate: Sun, 12 May 2024 16:36:37 GMT\nContent-Length: 0\nConnection: close\nServer: Kestrel\nWww-Authenticate: Bearer error=\"invalid_token\""
          """;

    public const string Forbidden
        = """
          "HTTP/1.1 403 Forbidden\nDate: Wed, 15 May 2024 17:18:29 GMT\nContent-Length: 0\nConnection: close\nServer: Kestrel"
          """
    ;

    public const string BadRequest
        = """
          {
            "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            "title": "Bad Request",
            "status": 400,
            "detail": "Error speakers",
            "instance": "POST /v1/speakers",
            "errors": [
              {
                "code": "Error code",
                "description": "Error description",
                "type": "Validation"
              }
            ]
          }
          """;


    public const string NotFound
        = """
          {
            "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            "title": "Not Found",
            "status": 404,
            "detail": "Error speakers",
            "instance": "POST /v1/speakers",
            "errors": [
              {
                "code": "Error code",
                "description": "Error description",
                "type": "NotFound"
              }
            ]
          }
          """;

    public const string NoContent = """
        {
          "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
          "title": "No Content",
          "status": 204,
          "detail": "Error speakers",
          "instance": "POST /v1/speakers",
          "errors": [
            {
              "code": "Error code",
              "description": "Error description",
              "type": "NoContent"
            }
          ]
        }
        """;

    public const string InternalServerError
        = """
          {
            "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            "title": "Server Failure",
            "status": 500,
            "instance": "POST /v1/speakers",
            "errors": [
              {
                "code": "Error code",
                "description": "Error description",
                "type": "Failure"
              }
            ]
          }
          """;
}