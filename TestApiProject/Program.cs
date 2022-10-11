using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host
	.ConfigureAppConfiguration((hostingContext, config) =>
	{
		var env = hostingContext.HostingEnvironment;
		config.AddJsonFile($"appsettings.json", optional: true);
	})
	.ConfigureServices((hostContext, services) =>
	{
		services.AddHttpClient();
		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();
		services.AddMemoryCache();
		services.AddCors(opt =>
		{
			opt.AddPolicy("MyCorsPolicy", builder =>
			{
				builder.AllowAnyOrigin()
					   .AllowAnyMethod()
					   .AllowAnyHeader();
			});
		});
	})
	.UseSerilog((hostContext, services, loggerConfiguration) =>
	{
		loggerConfiguration
			.ReadFrom.Configuration(hostContext.Configuration)
			.Enrich.WithProperty("Service Name", "TestAPIProject")
			.WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day);
	});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();