using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RedFast.API.Middlewares;

public class GlobalValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is UnauthorizedAccessException unauthorizedEx)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Acesso Negado",
                Detail = unauthorizedEx.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }

        if (exception is InvalidOperationException invalidOpEx)
        {
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Operação Inválida",
                Detail = invalidOpEx.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }

        if (exception is BadHttpRequestException badRequestException)
        {
            var jsonProblem = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Erro de Formatação no JSON",
                Detail = "Um ou mais campos enviados possuem tipos de dados inválidos.",
                Instance = httpContext.Request.Path
            };

            // Coloca a mensagem técnica exata para facilitar o debug do front-end
            jsonProblem.Extensions.Add("technical_error", badRequestException.Message);

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(jsonProblem, cancellationToken);
            return true;
        }

        var validationException = exception as ValidationException
                               ?? exception.InnerException as ValidationException;

        if (validationException == null)
        {
            return false;
        }

        var errors = validationException.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Um ou mais erros de validação ocorreram.",
            Detail = "Verifique a propriedade 'errors' para mais detalhes.",
            Instance = httpContext.Request.Path
        };

        // 4. Adiciona o dicionário de erros de forma segura
        problemDetails.Extensions.Add("errors", errors);

        // 5. Configura a resposta HTTP
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/problem+json";

        // 6. Escreve a resposta no Swagger/Postman
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // 7. Avisa o ASP.NET que nós resolvemos o problema
        return true;
    }
}
