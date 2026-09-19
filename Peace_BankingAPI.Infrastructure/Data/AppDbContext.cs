using Microsoft.EntityFrameworkCore;
using Peace_BankingAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peace_BankingAPI.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSets represent tables in our database
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Account entity
            modelBuilder.Entity<Account>(entity =>
            {
                // Primary Key
                entity.HasKey(e => e.Id);

                // Properties
                entity.Property(e => e.AccountNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(e => e.AccountNumber)
                    .IsUnique(); 

                entity.Property(e => e.AccountName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Balance)
                    .HasPrecision(18, 2); 

                entity.Property(e => e.Currency)
                    .HasConversion<string>(); 

                entity.Property(e => e.Type)
                    .HasConversion<string>();

                entity.Property(e => e.Status)
                    .HasConversion<string>();

                entity.HasOne<User>()
                    .WithMany(u => u.Accounts)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Soft delete filter - only show active records
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Configure the Transaction entity
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Reference)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(e => e.Reference)
                    .IsUnique();

                entity.Property(e => e.Amount)
                    .HasPrecision(18, 2);

                entity.HasOne(e => e.SourceAccount)
                    .WithMany(a => a.Transactions)
                    .HasForeignKey(e => e.SourceAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure the USer entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(e => e.PhoneNumber)
                    .IsUnique();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Role)
                    .HasConversion<string>();

                // Ignore the computed property (it has no setter and is derived)
                entity.Ignore(e => e.FullName);

                // Soft-delete query filter
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }
    }
}
