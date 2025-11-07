using Microsoft.EntityFrameworkCore;
using Comprehension.Models;

namespace Comprehension.Data
{
    public class ComprehensionContext : DbContext
    {
        public ComprehensionContext (DbContextOptions<ComprehensionContext> options)
            : base(options)
        {
        }

        public DbSet<Reminder> Reminder { get; set; } = default!;
        public DbSet<Event> Event { get; set; } = default!;
        public DbSet<Note> Note { get; set; } = default!;

        // --- AÑADIR ESTO ---
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<AuthToken> AuthTokens { get; set; } = default!;
        public DbSet<SharedPermission> SharedPermissions { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación: Usuario es dueño de muchos Eventos
            modelBuilder.Entity<User>()
                .HasMany(u => u.OwnedEvents)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación: Usuario es dueño de muchas Notas
            modelBuilder.Entity<User>()
                .HasMany(u => u.OwnedNotes)
                .WithOne(n => n.Owner)
                .HasForeignKey(n => n.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación: Usuario es dueño de muchos Recordatorios
            modelBuilder.Entity<User>()
                .HasMany(u => u.OwnedReminders)
                .WithOne(r => r.Owner)
                .HasForeignKey(r => r.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación: Usuario tiene muchos Tokens
            modelBuilder.Entity<User>()
                .HasMany(u => u.AuthTokens)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación: Usuario tiene muchos Permisos Compartidos
            modelBuilder.Entity<User>()
                .HasMany(u => u.SharedPermissions)
                .WithOne(sp => sp.User)
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para búsquedas rápidas
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<AuthToken>()
                .HasIndex(t => t.Token)
                .IsUnique();
                
            modelBuilder.Entity<SharedPermission>()
                .HasIndex(sp => new { sp.ResourceId, sp.ResourceType, sp.UserId })
                .IsUnique();
        }
        // --------------------
    }
}