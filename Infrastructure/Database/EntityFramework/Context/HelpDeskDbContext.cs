using Infrastructure.Database.EntityFramework.Entities;
using Infrastructure.Database.EntityFramework.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.EntityFramework.Context;

public class HelpDeskDbContext: DbContext
{
        DbSet<UserEntity> Users { get; set; } = null!;
        
        public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<UserEntity>(entity =>
                {
                        entity.ToTable("Users");
                        entity.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                        entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(50).IsRequired();
                        entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").HasMaxLength(20).IsRequired(); 
                        entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
                        entity.Property(e => e.Password).HasColumnName("password").HasMaxLength(72).IsRequired();
                        entity.Property(e => e.DepartmentId).HasColumnName("department_id").IsRequired();
                        entity.Property(e => e.Active).HasColumnName("active").IsRequired().HasDefaultValue(true);
                        
                        entity.Property(e => e.Role)
                                .HasColumnName("role")
                                .HasMaxLength(50) 
                                .HasConversion<string>()
                                .IsRequired();
                        entity.HasIndex(e => e.Email).IsUnique();
                        
                        entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
                        entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
                        entity.Property(e => e.LastModifiedAt).HasColumnName("last_modified_at").IsRequired(); // Nombre corregido
                        entity.Property(e => e.LastModifiedBy).HasColumnName("last_modified_by").IsRequired();
                        
                        // Configurar relaciones con otras entidades (ej. Department) aquí si es necesario
                        // entity.HasOne<DepartmentEntity>().WithMany().HasForeignKey(e => e.DepartmentId);
                });
                // Configurar otras entidades aquí
        }
        
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
                UpdateAuditFields();
                return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
                UpdateAuditFields();
                return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        private void UpdateAuditFields()
        {
                var entries = ChangeTracker.Entries<BaseEntity>();
                var now = DateTime.UtcNow;
                var userId = GetCurrentUserId(); 

                foreach (var entry in entries)
                {
                        if (entry.State == EntityState.Added)
                        {
                                entry.Entity.CreatedAt = now;
                                entry.Entity.CreatedBy = userId;
                                entry.Entity.LastModifiedAt = now; 
                                entry.Entity.LastModifiedBy = userId; 
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                                entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false; 
                                entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false; 

                                entry.Entity.LastModifiedAt = now;
                                entry.Entity.LastModifiedBy = userId;
                        }
                }
        }

        // TODO: Implementar el placeholder
        private int GetCurrentUserId()
        {
                // TODO: Implementar mecanismo para obtener ID del usuario autenticado
                // (ej. inyectando IHttpContextAccessor o un servicio dedicado).
                // Devolver un ID temporal o 0/1 si no hay usuario.
                return 1; 
        }
        
}