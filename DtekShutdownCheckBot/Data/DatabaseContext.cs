using DtekShutdownCheckBot.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace DtekShutdownCheckBot.Data;

public class DatabaseContext : DbContext
{
    private string _connectionString;
    
    public DatabaseContext(string connectionString)
    {
        _connectionString = connectionString;
        Database?.EnsureCreated();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>()
            .Property(m => m.Bio)
            .IsRequired(false);
        modelBuilder.Entity<Chat>()
            .Property(m => m.Description)
            .IsRequired(false);
        modelBuilder.Entity<Chat>()
            .Property(m => m.Title)
            .IsRequired(false);
        modelBuilder.Entity<Chat>()
            .Property(m => m.Username)
            .IsRequired(false);
        modelBuilder.Entity<Chat>()
            .Property(m => m.LastName)
            .IsRequired(false);
        
        modelBuilder.Entity<Chat>().HasMany(p => p.Words).WithOne(w => w.Chat);
        modelBuilder.Entity<Word>().HasOne(w => w.Chat).WithMany(c => c.Words).HasForeignKey(w => w.ChatId)
            .HasConstraintName("Word_chat_fk");
        
        base.OnModelCreating(modelBuilder);
    }


    public DbSet<Chat> Chats { get; init; }

    public DbSet<Shutdown> Shutdowns { get; init; }

    public DbSet<Word> Words { get; init; }

}