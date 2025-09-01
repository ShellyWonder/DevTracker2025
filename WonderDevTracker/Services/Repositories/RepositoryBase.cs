using Microsoft.EntityFrameworkCore;
using WonderDevTracker.Data;

namespace WonderDevTracker.Services.Repositories
{
    /// <summary>
    /// Base class for repositories that use EF Core via IDbContextFactory.
    /// Provides helper methods to create and dispose a short-lived DbContext per operation.
    /// </summary>
    public class RepositoryBase(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));

        /// <summary>
        /// Creates a DbContext, runs the provided async delegate, and disposes the DbContext.
        /// Use for commands/queries that return a value.
        /// </summary>
        protected async Task<TResult> WithContextAsync<TResult>(
            Func<ApplicationDbContext, Task<TResult>> action,
            CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            return await action(context);
        }
        /// <summary>
        /// Creates a DbContext, runs the provided async delegate, and disposes the DbContext.
        /// Use for commands/queries that do not return a value.
        /// </summary>
        protected async Task WithContextAsync(
            Func<ApplicationDbContext, Task> action,
            CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            await action(context);
        }

        /// <summary>
        /// Convenience wrapper for read-only operations.
        /// Sets QueryTrackingBehavior.NoTracking for the lifetime of this context instance.
        /// </summary>
        protected async Task<TResult> WithReadOnlyContextAsync<TResult>(
            Func<ApplicationDbContext, Task<TResult>> action,
            CancellationToken cancellationToken = default)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var previous = context.ChangeTracker.QueryTrackingBehavior;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            try
            {
                return await action(context);
            }
            finally
            {
                context.ChangeTracker.QueryTrackingBehavior = previous;
            }
        }
    }
}

