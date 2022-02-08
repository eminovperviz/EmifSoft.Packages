using EmiSoft.Domain.Abstractions;
using EmiSoft.Domain.Events;
using EmiSoft.Repository.EntityFrameworkCore.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;

namespace EmiSoft.Repository.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public bool AuthenticationEnabled { get; private set; }
    public readonly IMediator? _mediator;
    public readonly IHttpContextAccessor? _httpContextAccessor;
    private static readonly MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(DbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);
    public ApplicationDbContext(DbContextOptions<DbContext> options, IMediator? mediator) : base(options)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        _mediator = mediator;
    }

    public ApplicationDbContext(DbContextOptions<DbContext> options, IHttpContextAccessor httpContextAccessor, IMediator? mediator, bool authenticationEnabled = true) : base(options)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        ArgumentNullException.ThrowIfNull(mediator);
        _httpContextAccessor = httpContextAccessor;
        _mediator = mediator;
        AuthenticationEnabled = authenticationEnabled;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyAllConfigurationsFromCurrentAssembly();
        modelBuilder.ChangeDeleteBehaviorToNoAction();
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureGlobalFiltersMethodInfo
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, new object[] { modelBuilder, entityType });
        }
        base.OnModelCreating(modelBuilder);
    }

    //----------------Global Filter ------------
    protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType) where TEntity : class
    {
        if (entityType.BaseType != null || !ShouldFilterEntity<TEntity>())
            return;
        var filterExpression = CreateFilterExpression<TEntity>();
        if (filterExpression == null)
            return;
        modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
    }

    protected virtual bool ShouldFilterEntity<TEntity>() where TEntity : class
    {
        return typeof(IDictionaryEntity).IsAssignableFrom(typeof(TEntity));
    }

    protected Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>() where TEntity : class
    {
        Expression<Func<TEntity, bool>> expression = default;

        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> removedFilter = e => !((ISoftDelete)e).IsDeleted;
            expression = expression == null ? removedFilter : CombineExpressions(expression, removedFilter);
        }

        return expression;
    }

    protected Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2) => ExpressionCombiner.Combine(expression1, expression2);

    public new async Task<int> SaveChangesAsync(bool isCommit, CancellationToken cancellationToken = new CancellationToken())
    {
        if (!isCommit)
            return 0;

        ChangeTracker.DetectChanges();
        TrackChanges();
        // ignore events if no dispatcher provided
        if (_mediator != null)
        {
            // dispatch events only if save was successful
            await DispathEventsAsync(cancellationToken);
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void TrackChanges()
    {
        // set audit properties value
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                entry.Entity.CreatedBy = AuthenticationEnabled ? UserId() : 1;
                entry.Entity.CreatedDate = DateTime.Now;
                break;
                case EntityState.Modified:
                entry.Entity.LastModifiedBy = AuthenticationEnabled ? UserId() : 1;
                entry.Entity.LastModifiedDate = DateTime.Now;
                break;
            }
        }
        // set soft delete properties true
        foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
        {
            switch (entry.State)
            {
                case EntityState.Deleted:
                entry.State = EntityState.Unchanged;
                entry.Entity.IsDeleted = true;
                break;
            }
        }
        // raise entity data version changed event, if event assigned from IDataVersionTracking interface
        foreach (var entry in ChangeTracker.Entries<IDataVersionTracking>())
        {
            if (entry.State > EntityState.Unchanged)
            {
                var domainEvent = new EntityChangedEvent<IDataVersionTracking>(entry.Entity);
                entry.Entity.Events = entry.Entity.Events ?? new List<BaseDomainEvent>();
                entry.Entity.Events.Add(domainEvent);
            }
        }
    }

    private async Task DispathEventsAsync(CancellationToken cancellationToken)
    {
        // catch entities owned domain events
        var entitiesWithEvents = ChangeTracker.Entries<IDomainEventEntity>()
                    .Select(e => e.Entity)
                    .Where(e => e.Events.Any())
                    .ToList();

        foreach (var entity in entitiesWithEvents)
        {
            foreach (var domainEvent in entity.Events)
            {
                await _mediator!.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private int UserId()
    {
        var userIdentity = _httpContextAccessor!.HttpContext.User.Identity as ClaimsIdentity;
        int.TryParse(userIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int result);
        return result;
    }
}
