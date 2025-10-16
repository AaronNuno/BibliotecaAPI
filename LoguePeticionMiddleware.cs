namespace BibliotecaAPI
{
    public class LoguePeticionMiddleware
    {
        private readonly RequestDelegate next;

        public LoguePeticionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext contexto)
        {
            // Viene petición
            var logger = contexto.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation($"Peitición: {contexto.Request.Method} {contexto.Request.Path}");

            await next.Invoke(contexto);

            // Se va la petición

            logger.LogInformation($"Respuesta: {contexto.Response.StatusCode}");
        }
    }
    public static class LogueaPeticionMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoguePeticion (this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoguePeticionMiddleware>();
        }
    }
}
