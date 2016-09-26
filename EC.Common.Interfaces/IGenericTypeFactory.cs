using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// Interface for generic factory -- for creating and releasing transient objects.
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// This class is intended for use with the RegisterFactorFor attribute. In particular, the implementation
    /// for a class T which has a transient lifestyle can optionally add the attribute 
    /// [RegisterFactoryFor(typeof(IGenericFactory&lt;T&gt;))] which will automatically make a generic factory
    /// for type T available.
    /// </item>
    /// <item>
    /// In addition, the base UoW provides a CreateTransient&lt;T&gt;() call, which will automatically look up
    /// the generic factory for T, then use it to allocate an instance of T, and return that instances.
    /// When the UoW is disposed, it will call ReleaseComponent() on all the generic factories that it 
    /// has allocated, which in turn will cause each factory to release all transients which it has
    /// allocated.
    /// </item>
    /// <item>
    /// NOTE: There is *no* implementation for GenericFactory. It works because of some clever code in
    /// Castle which automatically recognizes factory interfaces, and dynamically builds the factory objects
    /// for them.
    /// </item>
    /// </list>
    /// </remarks>
    /// </summary>
    /// <typeparam name="T">Class of the item you want to create a factory of</typeparam>
    
    public interface IGenericFactory<T> : IDisposable
    {
        /// <summary>
        /// Create a new instances of Type T. ReleaseComponent() will be called on the
        /// instance when ReleaseComponent() is called on the factory.
        /// </summary>
        /// <returns>a new transient instances of type T</returns>
        
        T Create();

        /// <summary>
        /// Call ReleaseComponent() on the given object.
        /// <remarks>
        /// NOTE: Typically instances will not be explicitly released because they are all
        /// released when the factory is released.
        /// </remarks>
        /// </summary>
        /// <param name="value">instance to release</param>
        
        void Release(object value);
    }
}
