using Microsoft.AspNetCore.Mvc;

namespace TrainingPlanner.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ObjectResult ProblemResponse(
        int statusCode,
        string errorCode,
        string? detail = null,
        string? title = null)
    {
        return Problem(
            statusCode: statusCode,
            title: title,
            detail: detail,
            extensions: new Dictionary<string, object?>
            {
                ["errorCode"] = errorCode
            });
    }
}