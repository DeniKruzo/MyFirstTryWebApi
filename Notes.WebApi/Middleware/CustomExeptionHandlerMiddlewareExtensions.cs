namespace Notes.WebApi.Middleware
{
    /// <summary>
    /// Нужно чтобы добавить в конвейр
    /// </summary>
    public static class CustomExeptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this
           IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}
