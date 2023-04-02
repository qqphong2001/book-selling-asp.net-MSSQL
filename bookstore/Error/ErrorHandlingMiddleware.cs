namespace bookstore.Error
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

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
            {
                _logger.LogWarning($"Resource not found: {context.Request.Path}");
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "404.html");
                if (File.Exists(filePath))
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(File.ReadAllText(filePath));
                }
            }
        }
    }

}
