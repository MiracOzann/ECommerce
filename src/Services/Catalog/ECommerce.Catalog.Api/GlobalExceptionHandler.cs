namespace ECommerce.Catalog.Api;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetails,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
    {
        var (status, title) = ex switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Kaynak bulunamadı"),
            ConflictException => (StatusCodes.Status409Conflict, "Çakışma"),
            _ => (StatusCodes.Status500InternalServerError, "Beklenmeyen bir hata oluştu")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(ex, "Yakalanmayan hata");

        ctx.Response.StatusCode = status;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = ctx,
            Exception = ex,
            ProblemDetails =
            {
                Status = status,
                Title = title,
                Detail = status == StatusCodes.Status500InternalServerError ? null : ex.Message
            }
        });
    }
}