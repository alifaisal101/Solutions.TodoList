namespace Solutions.TodoList.WebApi.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseCommonMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("DefaultCorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
