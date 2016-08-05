using System;
using System.Collections;

namespace EC.Framework.Data
{
    /// <summary>
    /// OrderCriteria provides a generic ordering mechanism for
    /// business object collections. 
    /// </summary>
    public class OrderCriteria
    {
        #region Properties
        private ArrayList m_OrderCriteriaList = new ArrayList();

        /// <summary>
        /// Gets the order criteria list.
        /// </summary>
        /// <value>The order criteria list.</value>
        public ArrayList OrderCriteriaList
        {
            get { return m_OrderCriteriaList; }
        }

        public bool SortValueListOnDisplay
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCriteria"/> class.
        /// </summary>
        public OrderCriteria()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCriteria"/> class.
        /// </summary>
        /// <param name="orderCriterionOriginal">The order criterion original.</param>
        public OrderCriteria(OrderCriteria orderCriterionOriginal)
        {
            foreach (OrderCriterion orderCriterion in orderCriterionOriginal.OrderCriteriaList)
                m_OrderCriteriaList.Add(orderCriterion.Clone());
        }
        #endregion

        #region Methods
        public void AddOrder(Type type, string propertyName, string sortOrder)
        {
            OrderCriterion orderCriterion = new OrderCriterion(type, propertyName, sortOrder, AggregateFunction.None);
            m_OrderCriteriaList.Add(orderCriterion);
        }

        public void AddOrder(Type type, string propertyName, string sortOrder, AggregateFunction aggregateFunction)
        {
            OrderCriterion orderCriterion = new OrderCriterion(type, propertyName, sortOrder, aggregateFunction);
            m_OrderCriteriaList.Add(orderCriterion);
        }

        public void AddXmlOrder(Type type, Type dataType, string xmlPath, string propertyName, string sortOrder)
        {
            AddXmlOrder(type, dataType, 0, xmlPath, propertyName, sortOrder);
        }

        public void AddXmlOrder(Type type, Type dataType, Int32 length, string xmlPath, string propertyName, string sortOrder)
        {
            AddXmlOrder(type, dataType, length, xmlPath, propertyName, null, sortOrder);
        }

        public void AddXmlOrder(Type type, Type dataType, Int32 length, string xmlPath, string propertyName, string wtName, string sortOrder)
        {
            OrderCriterion orderCriterion = new OrderCriterion(type, dataType, length, xmlPath, propertyName, wtName, sortOrder, AggregateFunction.None);
            m_OrderCriteriaList.Add(orderCriterion);
        }

        /// <summary>
        /// Clears the orders.
        /// </summary>
        public void ClearOrders()
        {
            m_OrderCriteriaList.Clear();
        }
        #endregion
    }
}