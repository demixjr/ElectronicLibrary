using DAL;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
/*
public class LibraryContextFactory : IDesignTimeDbContextFactory<LibraryContext>
{
    public LibraryContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LibraryContext>();

        var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LibraryDb;Trusted_Connection=True;";
        optionsBuilder.UseSqlServer(connectionString);

        return new LibraryContext(optionsBuilder.Options);
    }
}
*/
