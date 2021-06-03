using System.Data.Entity;

public class Gabs2021DbContext : DbContext
{
	public Gabs2021DbContext() : base("gabs2021db")
	{
		
	}
	
	 // Map model classes to database tables
	 public DbSet<Song> Songs { get; set; }
	 
	
}