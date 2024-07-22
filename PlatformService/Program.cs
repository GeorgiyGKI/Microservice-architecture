using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Environment - based configuration for database
if (builder.Environment.IsProduction())
{
	Console.WriteLine("--> Using SqlServer Db");
builder.Services.AddDbContext<AppDbContext>(opt =>
	opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
	Console.WriteLine("--> Using InMem Db");
	builder.Services.AddDbContext<AppDbContext>(opt =>
		opt.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(options =>
{
	options.ListenLocalhost(7000, listenOptions =>
	{
		listenOptions.UseHttps();
	}); // HTTPS
});

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();

app.MapGet("/protos/platforms.proto", async context =>
{
	await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();
