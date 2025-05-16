using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.API.Entities;
using System.Reflection;
using System.Security.Claims;

namespace SurveyBasket.API.Persistence;

public class ApplicationDbContext :IdentityDbContext<ApplicationUser>
{ 
    private readonly IHttpContextAccessor _contextAccessor;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor contextAccessor) : base(options)
    {
        _contextAccessor = contextAccessor;
    }

    public DbSet<Poll> Polls { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Question> Questions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var CascadeFks = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(x => x.GetForeignKeys())
            .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

        foreach (var fk in CascadeFks)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }


        base.OnModelCreating(modelBuilder);
    }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {

        var entries = ChangeTracker.Entries<AuditableEntity>();

        var currentUserId = _contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        foreach (var entry in entries) { 
        
            if(entry.State==EntityState.Added)
            {
                entry.Property(x => x.CreatedById).CurrentValue = currentUserId;

            }
            else if(entry.State==EntityState.Modified)
            {
                entry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
                entry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
            }
        
        
        }


        return base.SaveChangesAsync(cancellationToken);
    }
}
