using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace EC.Utils
{
    /*
        public ECEntities(): base("name=ECEntities")
        {
            //Change datetime on read
            /*
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized +=
                  (sender, e) => 
                  {
                      EC.Utils.DateTimeKindAttribute.Apply(e.Entity, DateTimeKind.Utc);
                  };
            */

            //Change datetime on save
            /*((IObjectContextAdapter)this).ObjectContext.SavingChanges += 
                (sender, e) =>
                {
                    var ctx = (sender as System.Data.Entity.Core.Objects.ObjectContext);
                    if (ctx != null)
                    {
                        var list = ctx.InterceptionContext.DbContexts
                            .SelectMany(x => x.ChangeTracker.Entries())
                            .Where(x => x.State == EntityState.Modified || x.State == EntityState.Added)
                            .Select(x => x.Entity)
                            .ToList();

                        foreach(var item in list)
                        {
                            DateTimeKindAttribute.Apply(item, DateTimeKind.Utc);
                        }
                    }
                };
        }
     */
    [AttributeUsage(AttributeTargets.Property)]
    public class DateTimeKindAttribute : Attribute
    {
        private readonly DateTimeKind _kind;

        public DateTimeKindAttribute(DateTimeKind kind)
        {
            _kind = kind;
        }

        public DateTimeKind Kind
        {
            get { return _kind; }
        }

        public static void Apply(object entity, DateTimeKind kind)
        {
            if (entity == null)
                return;

            var properties = entity.GetType().GetProperties()
                .Where(x => x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?));

            foreach (var property in properties)
            {
                //var attr = property.GetCustomAttribute<DateTimeKindAttribute>();
                //if (attr == null)
                    //continue;

                var dt = property.PropertyType == typeof(DateTime?)
                    ? (DateTime?)property.GetValue(entity)
                    : (DateTime)property.GetValue(entity);

                if (dt == null)
                    continue;

                //property.SetValue(entity, DateTime.SpecifyKind(dt.Value, kind));
                DateTime.SpecifyKind(dt.Value, DateTimeKind.Unspecified);
                dt = dt.Value.AddHours(3);
                property.SetValue(entity, dt);
            }
        }
    }
}