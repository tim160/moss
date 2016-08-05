using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EC.Business
{
    public class AdvancedQuery
    {
        /// <summary>
        /// Optional for individual field use - Value comparator
        /// </summary>
        public string Comparison { get; set; }

        /// <summary>
        /// Optional for individual field use - FieldName of field 
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Optional if this AdvancedQuery represents Brackets
        /// </summary>
        public AdvancedQuery[] Queries { get; set; }

        /// <summary>
        /// Optional for individual field use - OrderType for field
        /// </summary>
        public string QueryType { get; set; }

        public string FilterType { get; set; }

      
        /// <summary>
        /// Display value
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Type of AdvancedQuery (1 - Named Filter, 2 - Brackets, 3 - Individual Field
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Value for type (1 - ObjectKey, 2 - not used, 3 - Individual Field Value)
        /// </summary>
        public object Value { get; set; }

        public AdvancedQuery DeepCopy()
        {
            AdvancedQuery other = (AdvancedQuery)MemberwiseClone();
            if (Queries != null)
            {
                List<AdvancedQuery> otherQueries = new List<AdvancedQuery>();
                foreach (AdvancedQuery q in Queries)
                {
                    otherQueries.Add(q.DeepCopy());
                }
                other.Queries = otherQueries.ToArray();
            }
            return other;
        }
    }
}