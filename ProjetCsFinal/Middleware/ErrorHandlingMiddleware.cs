using System.Net;
using System.Text.Json;

namespace ProjetCsFinal.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                var errorResponse = new
                {
                    Success = false,
                    Message = error.Message
                };

                switch (error)
                {
                    case KeyNotFoundException:
                        // Ressource non trouvée
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    case UnauthorizedAccessException:
                        // Accès non autorisé
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;

                    case ArgumentException:
                        // Requête invalide
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    default:
                        // Erreur non gérée
                        _logger.LogError(error, error.Message);
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        errorResponse = new
                        {
                            Success = false,
                            Message = "Une erreur interne s'est produite."
                        };
                        break;
                }

                var result = JsonSerializer.Serialize(errorResponse);
                await response.WriteAsync(result);
            }
        }
    }
}
