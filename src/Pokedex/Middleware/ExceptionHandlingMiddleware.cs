using System.Net;

namespace Pokedex.Middleware
{
	public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
	{
		private readonly RequestDelegate next = next;
		private readonly ILogger<ExceptionHandlingMiddleware> logger = logger;

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			logger.LogError(exception, "An unexpected error occurred.");

			ExceptionResponse response = exception switch
			{
				KeyNotFoundException _ => new ExceptionResponse(HttpStatusCode.NotFound, "The request key was not found."),
				ArgumentNullException _ => new ExceptionResponse(HttpStatusCode.BadRequest, "The request has missing arguments."),
				_ => new ExceptionResponse(HttpStatusCode.InternalServerError, "Internal server error. Please retry later.")
			};

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)response.StatusCode;
			await context.Response.WriteAsJsonAsync(response);
		}
	}

	public record ExceptionResponse(HttpStatusCode StatusCode, string Description);

}
