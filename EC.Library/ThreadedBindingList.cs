using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;

namespace EC.Library
{
    public class ThreadedBindingList<T> : BindingList<T>
    {
        public delegate void SortedHandler(ListSortDirection sortDirection, string sortProperty);
        public event SortedHandler Sorted;
        private bool m_IsSorted;

        private ListSortDirection m_SortDirection = ListSortDirection.Ascending;

        public ListSortDirection ListSortDirection
        {
            get
            {
                return m_SortDirection;
            }
        }

        [NonSerialized()]
        private PropertyDescriptor m_SortProperty = null;

        public string SortProperty
        {
            get
            {
                return m_SortProperty.Name;
            }
        }

        #region BindingList<T> Public Sorting API
        public void Sort()
        {
            ApplySortCore(m_SortProperty, m_SortDirection);
        }

        public void Sort(string property)
        {
            /* Get the PD */
            m_SortProperty = FindPropertyDescriptor(property);

            /* Sort */
            ApplySortCore(m_SortProperty, m_SortDirection);
        }

        public void Sort(string property, ListSortDirection direction)
        {
            /* Get the sort property */
            m_SortProperty = FindPropertyDescriptor(property);
            m_SortDirection = direction;

            /* Sort */
            ApplySortCore(m_SortProperty, m_SortDirection);
        }
        #endregion

        #region BindingList<T> Sorting Overrides
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<T> items = this.Items as List<T>;

            if ((null != items) && (null != property))
            {
                PropertyComparer<T> pc = new PropertyComparer<T>(property, direction);
                items.Sort(pc);

                /* Set sorted */
                m_IsSorted = true;
                if (Sorted != null)
                    Sorted(direction, property.Name);
            }
            else
            {
                /* Set sorted */
                m_IsSorted = false;
            }
        }

        protected override bool IsSortedCore
        {
            get { return m_IsSorted; }
        }

        protected override void RemoveSortCore()
        {
            m_IsSorted = false;
        }
        #endregion

        #region BindingList<T> Private Sorting API
        private PropertyDescriptor FindPropertyDescriptor(string property)
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(T));
            PropertyDescriptor prop = null;

            if (null != pdc)
            {
                prop = pdc.Find(property, true);
            }

            return prop;
        }
        #endregion

        #region PropertyComparer<TKey>
        internal class PropertyComparer<TKey> : System.Collections.Generic.IComparer<TKey>
        {
            /*
            * The following code contains code implemented by Rockford Lhotka:
            * //msdn.microsoft.com/library/default.asp?url=/library/en-us/dnadvnet/html/vbnet01272004.asp" href="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnadvnet/html/vbnet01272004.asp">http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnadvnet/html/vbnet01272004.asp
            */

            private PropertyDescriptor m_PropertyDescriptor;
            private ListSortDirection m_SortDirection;

            public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
            {
                m_PropertyDescriptor = property;
                m_SortDirection = direction;
            }

            public int Compare(TKey xVal, TKey yVal)
            {
                /* Get property values */
                object xValue = GetPropertyValue(xVal, m_PropertyDescriptor.Name);
                object yValue = GetPropertyValue(yVal, m_PropertyDescriptor.Name);

                /* Determine sort order */
                if (m_SortDirection == ListSortDirection.Ascending)
                {
                    return CompareAscending(xValue, yValue);
                }
                else
                {
                    return CompareDescending(xValue, yValue);
                }
            }

            public bool Equals(TKey xVal, TKey yVal)
            {
                return xVal.Equals(yVal);
            }

            public int GetHashCode(TKey obj)
            {
                return obj.GetHashCode();
            }

            /* Compare two property values of any type */
            private int CompareAscending(object xValue, object yValue)
            {
                int result;
                if (xValue == null)
                    xValue = string.Empty;
                if (yValue == null)
                    yValue = string.Empty;
                /* If values implement IComparer */
                if (xValue is IComparable)
                {
                    result = ((IComparable)xValue).CompareTo(yValue);
                }
                /* If values don't implement IComparer but are equivalent */
                else if (xValue.Equals(yValue))
                {
                    result = 0;
                }
                /* Values don't implement IComparer and are not equivalent, so compare as string values */
                else result = xValue.ToString().CompareTo(yValue.ToString());

                /* Return result */
                return result;
            }

            private int CompareDescending(object xValue, object yValue)
            {
                /* Return result adjusted for ascending or descending sort order ie
                   multiplied by 1 for ascending or -1 for descending */
                return CompareAscending(xValue, yValue) * -1;
            }

            private object GetPropertyValue(TKey value, string property)
            {
                /* Get property */
                PropertyInfo propertyInfo = value.GetType().GetProperty(property);

                /* Return value */
                return propertyInfo.GetValue(value, null);
            }
        }
        #endregion

        SynchronizationContext ctx = SynchronizationContext.Current;

        protected override void OnAddingNew(AddingNewEventArgs e)
        {
            if (ctx == null)
            {
                BaseAddingNew(e); 
            }
            else
            { 
                ctx.Send(delegate { BaseAddingNew(e); }, null); 
            }
        }

        void BaseAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e); 
        }

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (ctx == null)
            {
                BaseListChanged(e); 
            }
            else
            {
                try { ctx.Send(delegate { BaseListChanged(e); }, null); }
                catch { }
            }
        }

        void BaseListChanged(ListChangedEventArgs e)
        { 
            base.OnListChanged(e); 
        }
    }
}
