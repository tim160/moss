using System;
using System.Collections;
using System.Text; 

namespace EC.Framework.Data
{
    /// <summary>
    /// PersistentCriteria provides a generic persistence filering mechanism for
    /// business object collections. 
    /// </summary>
    public class PersistentCriteria : ICloneable
    {
        #region Properties
        private OperandType m_OperandType;
        private ArrayList m_Criteria;
        private ArrayList m_Types = new ArrayList();

        /// <summary>
        /// Gets or sets the type of the operand.
        /// </summary>
        /// <value>The type of the operand.</value>
        public OperandType OperandType
        {
            get { return m_OperandType; }
            set { m_OperandType = value; }
        }

        /// <summary>
        /// Gets the criteria.
        /// </summary>
        /// <value>The criteria.</value>
        public ArrayList Criteria
        {
            get { return m_Criteria; }
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <value>The types.</value>
        public ArrayList Types
        {
            get { return m_Types; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentCriteria"/> class.
        /// </summary>
        public PersistentCriteria()
        {
            Initialize(OperandType.None);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentCriteria"/> class.
        /// </summary>
        /// <param name="criterion">The criterion.</param>
        public PersistentCriteria(SelectionCriterion criterion)
        {
            Initialize(OperandType.None);
            AddCriteria(criterion);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentCriteria"/> class.
        /// </summary>
        /// <param name="operandType">Type of the operand.</param>
        /// <param name="criterion">The criterion.</param>
        public PersistentCriteria(OperandType operandType, SelectionCriterion criterion)
        {
            Initialize(operandType);
            AddCriteria(criterion);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentCriteria"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public PersistentCriteria(PersistentCriteria criteria)
        {
            Initialize(OperandType.None);
            AddCriteria(criteria);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentCriteria"/> class.
        /// </summary>
        /// <param name="operandType">Type of the operand.</param>
        /// <param name="criteria">The criteria.</param>
        public PersistentCriteria(OperandType operandType, PersistentCriteria criteria)
        {
            Initialize(operandType);
            AddCriteria(criteria);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the specified operand type.
        /// </summary>
        /// <param name="operandType">Type of the operand.</param>
        private void Initialize(OperandType operandType)
        {
            this.m_Criteria = new ArrayList();
            this.OperandType = operandType;
        }

        /// <summary>
        /// Verifies the criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        private void VerifyCriteria(object criteria)
        {
            if (criteria == null)
            {
                throw new ApplicationException("Could not initialize PersistentCriteria; criteria is null!");
            }
        }

        /// <summary>
        /// Invalids the criteria object.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void InvalidCriteriaObject(object obj)
        {
            throw new ApplicationException("Could not initialize PersistentCriteria; criteria is null!");
        }

        /// <summary>
        /// Adds the criteria.
        /// </summary>
        /// <param name="criterion">The criterion.</param>
        public void AddCriteria(SelectionCriterion criterion)
        {
            // Make sure criterion isn't null
            VerifyCriteria(criterion);

            // Add type to typelist if doesn't exist
            if (Types.IndexOf(criterion.Type) == -1)
            {
                Types.Add(criterion.Type);
            }

            // Add the criteria
            m_Criteria.Add(criterion);
        }

        /// <summary>
        /// Adds the criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public void AddCriteria(PersistentCriteria criteria)
        {
            // Make sure criteria isn't null.
            VerifyCriteria(criteria);

            // Add types to typelist if doesn't exist.
            AddTypes(criteria);

            // Add the criteria.
            m_Criteria.Add(criteria);
        }

        /// <summary>
        /// Adds the types.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        private void AddTypes(PersistentCriteria criteria)
        {
            foreach (object o in criteria.Criteria)
            {
                SelectionCriterion sc = o as SelectionCriterion;
                if (sc != null)
                {
                    if (Types.IndexOf(sc.Type) == -1)
                    {
                        Types.Add(sc.Type);
                    }
                    continue;
                }
                PersistentCriteria pc = o as PersistentCriteria;
                if (pc != null)
                {
                    AddTypes(pc);
                    continue;
                }

                // Neither valid type, throw execption.
                InvalidCriteriaObject(o);
            }
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public PersistentCriteria Clone()
        {
            // Create empty Persistent Criteria
            PersistentCriteria newCriteria = new PersistentCriteria();

            // Set Operand Type
            newCriteria.OperandType = OperandType;

            // Add all the types.
            foreach (Type t in Types)
            {
                newCriteria.Types.Add(t);
            }

            // Add a clone of each Criteria contained
            foreach (object criteria in Criteria)
            {
                // Try as SelectionCriterion
                SelectionCriterion sc = criteria as SelectionCriterion;
                if (sc != null)
                {
                    // Add a copy of the selection criterion
                    newCriteria.AddCriteria((SelectionCriterion)sc.Clone());
                    continue;
                }

                // Try as IPersistentCriteria
                PersistentCriteria pc = criteria as PersistentCriteria;
                if (pc != null)
                {
                    // Add a copy of the persistent criteria.
                    newCriteria.AddCriteria((PersistentCriteria)pc.Clone());
                    continue;
                }

                // Neither valid type, throw execption.
                InvalidCriteriaObject(criteria);

            }

            return newCriteria;
        }
        #endregion

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (SelectionCriterion sc in m_Criteria)
            {
                builder.Append(sc.ToString());
                builder.Append(Environment.NewLine); 
            }
            return builder.ToString(); 
        }
    }
}