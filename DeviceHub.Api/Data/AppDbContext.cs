


using DeviceHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DeviceHub.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<Headset> Headsets => Set<Headset>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Headset>().HasIndex(h => h.SerialNumber).IsUnique();
		modelBuilder.Entity<Headset>().Property(h => h.Status).HasConversion<string>();
	}
}