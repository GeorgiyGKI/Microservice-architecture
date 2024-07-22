using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
	public static class PrepDb
	{
		public static void PrepPopulation(IApplicationBuilder app, bool isProd)
		{
			using (var serviceScope = app.ApplicationServices.CreateScope())
			{
				SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
			}
		}

		private static void SeedData(AppDbContext context, bool isProd)
		{
			if (isProd)
			{
				Console.WriteLine("--> Attempting to apply migrations...");
				try
				{
					context.Database.Migrate();

				}
				catch (Exception ex)
				{
                    Console.WriteLine($"--> Coulnd't run miragtions: {ex}...");
                    throw;
				}
			}

			if (!context.Platforms.Any())
			{
				Console.WriteLine("--> Seeding data...");

				context.Platforms.AddRange(
					new Platform() { Name = "Dot net", Publisher = "Microsoft", Cost = "Free" }
				);

				context.SaveChanges();
			}
			else
			{
				Console.WriteLine("--> We already have data");
			}
		}
	}
}
