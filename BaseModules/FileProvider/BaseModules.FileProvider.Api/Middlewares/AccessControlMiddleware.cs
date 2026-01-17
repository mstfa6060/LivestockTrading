namespace BaseModules.FileProvider.Api.Middlewares;

public static class AccessControlMiddleware
{
	public static void UseAccessControl(IApplicationBuilder app)
	{
		app.Use(async (context, next) =>
		{
			var jwt = context.Request.Query["s"];
			var requestPath = context.Request.Path;



			// Do work that doesn't write to the Response.
			await next();
			// Do other work that doesn't write to the Response.

		});
	}

}