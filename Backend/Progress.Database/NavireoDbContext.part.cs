using Microsoft.EntityFrameworkCore;
using Progress.Database.Custom;

namespace Progress.Database
{
	public partial class NavireoDbContext 
    {
        public virtual DbSet<TwSearchResult> TwSearchResult { get; set; }

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TwSearchResult>().HasNoKey(); // Important: Mark as no key
		}
	}
}
