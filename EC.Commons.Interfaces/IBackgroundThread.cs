using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// The interface is bourne by classes which have a background thread associated with 
    /// them for performing some sort of asynchronous activity. The main entry points on
    /// the interface allow the background thread to be started and stopped.
    /// </summary>
    
    public interface IHaveBackgroundThread
    {
        /// <summary>
        /// Start the background thread within the object.
        /// </summary>
        
        void StartBackgroundThread();
        
        /// <summary>
        /// Stop the background thread within the object synchronously.
        /// </summary>
        
        void StopBackgroundThread();

        /// <summary>
        /// Return the background thread associated with this object.
        /// </summary>

        IUtilityThread GetBackgroundThread();
    }

    /// <summary>
    /// This interface is a special variant of IBackgroundThread which is used for model
    /// classes in MarineLMS.SharedModel. Shared model classes need to have a type
    /// parameter for the UoW implementation specific to the DB context into which they
    /// are going to be bound. If a shared model class also has a background thread,
    /// then the background thread interface also needs to be marked with the UoW 
    /// so that separate background threads can be registered for each DB context within
    /// the application.
    /// </summary>
    /// <typeparam name="U">unit of work interface for DB context</typeparam>
   
    public interface IHaveBoundBackgroundThread<U> : IHaveBackgroundThread
    {
    }
}
