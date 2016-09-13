using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace EC.Common.Interfaces.Persistence
{
    /// <summary>
    /// This interface is exposed by classes that need to be called to
    /// register object model(s) with Entity Framework.
    /// <remarks>
    /// <para>
    /// An application might potentially use multiple DbContext implementations -- this would 
    /// occur if the application needed to access more than one database. For example, the current
    /// plan with the extension service is that each 'extension' would have it's own database.
    /// Each DbContext would therefore be associated with a specific set of model objects 
    /// corresponding to the tables in that database. 
    /// </para>
    /// <para>
    /// Each DbContext therefore needs a way to bind to its specific model objects. The mechanism 
    /// that we have chosen for this is that BaseMLSDbContext calls GetSchemeConfigurators() and
    /// then calls ConfigureSchema() on each. It is assumed that ConfigureSchema() registers a set
    /// of model objects into the context via the model builder passed as a parameter.
    /// </para>
    /// <para>
    /// Each concrete implementation of IMLSDbContext must provide an implementation for 
    /// GetSchemaConfigurators(), and the implementation of this function must be able to bind
    /// its specific set of model objects. To achieve this, the schema configuration immplementations
    /// inherit from a 'marker' interface which itself inherits from ISchemaConfigurator. 
    /// </para>
    /// <para>
    /// For example, suppose that there were two 'extensions' residing in the extension service and
    /// two corresponding concrete IMLSDbContext implementations. Then there would also be two marker
    /// interfaces for schema configurators, e.g. ICSMARTSchemaConfigurator and IChevronSchemaConfigurator.
    /// Each concrete Db context implementation would implement GetSchemaConfigurators() by doing a
    /// ResolveAll() on its corresponding schema configurator type. At the model level, the schema 
    /// configurator for each model class is tagged with the schema configurator interface corresponding
    /// to the Db context that model should be assoicated with.This ensures that the model
    /// objects for each extension were correctly bound into the corresponding Db context.
    /// </para>
    /// </remarks>
    /// </summary>

    public interface ISchemaConfigurator
    {
        /// <summary>
        /// This is called when EF called the OnModelCreating() method of
        /// the DbContext.
        /// </summary>
        /// <param name="modelBuilder">EF model builder</param>

        void ConfigureSchema(DbModelBuilder modelBuilder);
    }

    /// <summary>
    /// This interface is used to tag EF schema configrurators for entity types that are intended
    /// to be shared across projects.
    /// </summary>
    /// <remarks>
    /// As noted above, we use sub-typing on the ISchemaConfigurator interface to control which model
    /// classes are bound into which DB Contexts. However, there are some model classes that provide
    /// relatively general purpose features (e.g. the TimerManager) and so to make it simpler to use 
    /// these classes in multiple Db contexts we have the ISharedSchemaConfigurator. Re-usasble model
    /// classes reside in the MarineLMS.SharedModel namespaces and are all tagged with the shared
    /// schema configurator. Db contexts wishing to use model classes from MarineLMS.SharedModel
    /// need to ensure that they include all classes tagged with ISharedSchemeConfigurator in their
    /// GetSchemaConfigurators() implementation.
    /// </remarks>

    public interface ISharedSchemaConfigurator : ISchemaConfigurator
    {
    }
}
