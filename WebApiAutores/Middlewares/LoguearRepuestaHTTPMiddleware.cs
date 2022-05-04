namespace WebApiAutores.Middlewares
{
    public static class LoguearRepuestaHTTPMiddlewareEstensions
    {
        public static IApplicationBuilder UseLoguearRepuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRepuestaHTTPMiddleware>();
        }
    }

    public class LoguearRepuestaHTTPMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoguearRepuestaHTTPMiddleware> logger;

        public LoguearRepuestaHTTPMiddleware(RequestDelegate siguiente, ILogger<LoguearRepuestaHTTPMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

      

        // Invoke o invokeAsync
        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRespuesta = context.Response.Body;
                context.Response.Body = ms;

                await siguiente(context);

                ms.Seek(0, SeekOrigin.Begin);
                string repuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                context.Response.Body = cuerpoOriginalRespuesta;

                 logger.LogInformation(repuesta);
            }
        }
    }
}
