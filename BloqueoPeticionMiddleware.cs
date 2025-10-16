namespace BibliotecaAPI
{
    public class BloqueoPeticionMiddleware
    {
        private readonly RequestDelegate next;

        public BloqueoPeticionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext contexto)
        {
            if (contexto.Request.Path == "/bloqueado")
            {
                contexto.Response.StatusCode = 403;
                await contexto.Response.WriteAsync("Acceso denegado");
            }
            else
            {
                await next.Invoke(contexto);
            }
        }
    }
    public static class BlqueaPeticionMiddlewareExtensions
    {
        public static IApplicationBuilder UseBloquePeticion (this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BloqueoPeticionMiddleware>();
        }
    }
}

