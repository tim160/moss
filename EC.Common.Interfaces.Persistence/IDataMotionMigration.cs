using System.Collections.Generic;
using Castle.Core.Logging;

namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// A migration needs to implement this if there are data motions which need access to
    /// the current context and database for easier data manipulation.
    /// The methods BeforeMigration and AfterMigration are called in the following order:
    /// 1. BeforeMigration
    /// 2. execute migration itself
    /// 3. AfterMigration
    ///
    /// <example>
    /// We have 3 migrations:
    /// - M1 implements IDataMotionMigration<CoreDbContext>
    /// - M2
    /// - M3 implements IDataMotionMigration<CoreDbContext>
    /// 
    /// 1. M1.BeforeMigrationUp()
    /// 2. M1 migration itself
    /// 3. M1.AfterMigrationUp()
    /// 4. M2 migration itself
    /// 5. M3.BeforeMigrationUp()
    /// 6. M3 migration itself
    /// 7. M3.AfterMigrationUp()
    /// </example>
    /// </summary>
    /// <typeparam name="TContext">Use 'CoreDbContext' for the core service data motions</typeparam>
    
    public interface IDataMotionMigration<TContext>
    {
        /// <summary>
        /// Data motion to be done before the migration which implements this interface needs to be done.
        /// For example, gather data and store them for <see cref="AfterMigrationUp"/>.
        /// It is possible to return a list of SQL statements to be executed before the actual migration
        /// is applied.
        /// </summary>
        /// <param name="context">Context of the migration e.g. CoreDbContext)</param>
        /// <param name="logger">Logger</param>
        /// <returns>
        /// List of SQL commands to be executed before the migration is applied
        /// or <c>null</c> if no command needs to be executed.
        /// </returns>

        IList<string> BeforeMigrationUp(TContext context, ILogger logger);

        /// <summary>
        /// Data motion to be done after the migration which implements this interface needs to be done.
        /// It is possible to return a list of SQL statements to be executed after the actual migration
        /// has been applied.
        /// </summary>
        /// <param name="context">Context of the migration e.g. CoreDbContext)</param>
        /// <param name="logger">Logger</param>
        /// <returns>
        /// List of SQL commands to be executed after the migration has been applied 
        /// or <c>null</c> if no command needs to be executed.
        /// </returns>

        IList<string> AfterMigrationUp(TContext context, ILogger logger);
    }
}
