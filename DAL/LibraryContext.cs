using Microsoft.EntityFrameworkCore;
using DAL.Models;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {

        }


        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }

        const int maxDecimalNum = 8;
        const int numAfterDot = 2;
        const int nameLength = 32;
        const int passwordLength = 64;
        const int descriptionLength = 256;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(b => b.Title)
                      .IsRequired()
                      .HasMaxLength(nameLength);

                entity.Property(b => b.Description)
                      .HasMaxLength(descriptionLength);

                entity.Property(b => b.Author)
                      .IsRequired()
                      .HasMaxLength(nameLength);

                entity.Property(b => b.Price)
                      .HasPrecision(maxDecimalNum, numAfterDot);

                entity.Property(b => b.BookType)
                      .IsRequired();

                entity.Property(b => b.Genre)
                      .IsRequired();
            });

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(nameLength);

                entity.Property(u => u.Email)
                      .IsRequired();

                entity.Property(u => u.Password)
                      .IsRequired()
                      .HasMaxLength(passwordLength);


                entity.HasOne(u => u.Order)
                      .WithOne(o => o.User)
                      .HasForeignKey<Order>(o => o.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.HasMany(o => o.Books)
                      .WithMany();

                entity.Property(o => o.TotalPrice)
                      .HasPrecision(maxDecimalNum, numAfterDot);
            });
        }
    }
}