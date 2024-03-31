using System.Net;

namespace Pokedex.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate next;
		private readonly ILogger<ExceptionHandlingMiddleware> logger;

		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
		{
			this.next = next;
			this.logger = logger;
		}

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
				_ => new ExceptionResponse(HttpStatusCode.InternalServerError, "Internal server error. Please retry later.")
			};

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)response.StatusCode;
			await context.Response.WriteAsJsonAsync(response);
		}
	}

	public record ExceptionResponse(HttpStatusCode StatusCode, string Description);

}
