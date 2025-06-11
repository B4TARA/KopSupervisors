namespace KOP.WEB.Middlewares
{
    public class UrlPrefixMiddleware
    {
        private readonly RequestDelegate _next;

        public UrlPrefixMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Проверяем, начинается ли путь с "supervisors/"
            if (context.Request.Path.StartsWithSegments("/supervisors", out var remainingPath))
            {
                // Удаляем префикс "supervisors/"
                context.Request.Path = remainingPath;
            }

            // Вызываем следующий middleware в конвейере
            await _next(context);
        }
    }
}
