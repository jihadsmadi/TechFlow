using Microsoft.AspNetCore.Mvc;
using TechFlow.Domain.Common.Results;

namespace TechFlow.API.Extensions;

public static class ResultExtensions
{
    /// <summary>
    /// Maps Result<T> to an IActionResult using ProblemDetails for errors.
    /// Handles all ErrorKind values — one place for all HTTP status mapping.
    /// </summary>
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        ControllerBase controller)
    {
        if (!result.IsFailure)
            return controller.Ok(result.Value);

        return result.TopError.Type switch
        {
            ErrorKind.Validation => controller.BadRequest(BuildValidationProblem(result.Errors)),
            ErrorKind.NotFound => controller.NotFound(BuildProblem(result.TopError, 404)),
            ErrorKind.Conflict => controller.Conflict(BuildProblem(result.TopError, 409)),
            ErrorKind.Unauthorized => controller.Unauthorized(BuildProblem(result.TopError, 401)),
            ErrorKind.Forbidden => controller.StatusCode(403, BuildProblem(result.TopError, 403)),
            ErrorKind.Failure => controller.BadRequest(BuildProblem(result.TopError, 400)),
            _ => controller.StatusCode(500, BuildProblem(result.TopError, 500))
        };
    }

    /// <summary>
    /// Overload for Result<Updated>, Result<Deleted> etc.
    /// Returns 204 No Content on success.
    /// </summary>
    public static IActionResult ToNoContentResult<T>(
        this Result<T> result,
        ControllerBase controller)
    {
        if (result.IsSuccess)
            return controller.NoContent();

        return result.ToActionResult(controller);
    }

    public static IActionResult ToCreatedResult<T>(
    this Result<T> result,
    ControllerBase controller,
    string actionName,
    object routeValues)
    {
        if (result.IsSuccess)
            return controller.CreatedAtAction(actionName, routeValues, result.Value);

        return result.ToActionResult(controller);
    }
    // ── Private Builders 

    private static ProblemDetails BuildProblem(Error error, int status) => new()
    {
        Title = error.Code,
        Detail = error.Description,
        Status = status
    };

    private static ValidationProblemDetails BuildValidationProblem(List<Error> errors)
    {
        var errorDict = errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description).ToArray());

        return new ValidationProblemDetails(errorDict)
        {
            Status = StatusCodes.Status400BadRequest
        };
    }
}