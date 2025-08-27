namespace Solutions.TodoList.WebApi.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseCommonMiddleware(this WebApplication app)
    {
        if (!app.Environment.IsProduction())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });            
        }
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("DefaultCorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
