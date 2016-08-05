using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Common.Util
{
    /// <summary>
    /// Most.Recently.Used Dictionary
    /// </summary>
    /// <typeparam name="T">Key</typeparam>
    /// <typeparam name="X">Value</typeparam>
    public class MruDictionary<T, X> 
    {
        Dictionary<T, X> cache = new Dictionary<T, X>();

        /// <summary>
        /// Keeps up with the most recently read items.
        /// Items at the end of the list were read last. 
        /// Items at the front of the list have been the most idle.
        /// Items at the front are removed if the cache capacity is reached.
        /// </summary>
        List<T> priority = new List<T>();

        public MruDictionary(int capacity)
        {
            Capacity = capacity;
        }

        public bool ContainsKey(T key)
        {
            return cache.ContainsKey(key);
        }

        public X this[T key]
        {
            get
            {
                lock (this)
                {
                    if (!cache.ContainsKey(key)) throw new KeyNotFoundException();
                    //move the item to the end of the list                    
                    priority.Remove(key);
                    priority.Add(key);
                    return cache[key];
                }
            }
            set
            {
                lock (this)
                {
                    if (Capacity > 0 && cache.Count == Capacity)
                    {
                        cache.Remove(priority[0]);
                        priority.RemoveAt(0);
                    }
                    cache[key] = value;
                    priority.Remove(key);
                    priority.Add(key);

                    if (priority.Count != cache.Count)
                        throw new Exception("Capacity mismatch.");
                }
            }
        }
        public int Count { get { return cache.Count; } }
        public int Capacity { get; set; }

        public void Clear()
        {
            lock (this)
            {
                priority.Clear();
                cache.Clear();

            }
        }
        
    }
}
