namespace CustomControlApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.ConfigureMauiHandlers(handlers =>
		{
			handlers.AddHandler(typeof(IconView), typeof(IconViewHandler));
		});

		return builder.Build();
	}
}
