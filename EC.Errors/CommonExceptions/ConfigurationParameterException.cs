using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.CommonExceptions
{
    /// <summary>
    /// Used when the configuration attributes for the system are incorrect.
    /// </summary>

    public class ConfigurationParameterException : FaultableException<ConfigurationParameterFault>
    {
        public override ConfigurationParameterFault ToFault(string path, CurrentUserInfo userInfo)
        {
            var f = new ConfigurationParameterFault(Message, path, userInfo);
            f.ElementId = ElementId;
            f.ElementName = ElementName;
            f.ElementType = ElementType;
            return f;
        }

        public ConfigurationParameterException(string message, string elementType, string elementName, Guid? elementId, Exception innerException = null) : base(message, innerException)
        {
            this.ElementId = elementId;
            this.ElementType = elementType;
            this.ElementName = elementName;
        }

        /// <summary>
        /// Optional Id of the element which is misconfigured.
        /// </summary>

        public Guid? ElementId { get; set; }

        /// <summary>
        /// Optional element name which is misconfigured
        /// </summary>

        public string ElementName { get; set; }

        /// <summary>
        /// Element type if available (e.g. NavPageLink).
        /// </summary>

        public string ElementType { get; set; }
    }

    /// <summary>
    /// If the system is misconfigured. For example if a mandatory attribute is missing.
    /// </summary>

    [DataContract]
    public class ConfigurationParameterFault : BasicFault
    {
        public ConfigurationParameterFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        /// <summary>
        /// Optional Id of the element which is misconfigured.
        /// </summary>

        [DataMember]
        public Guid? ElementId { get; set; }

        /// <summary>
        /// Optional element name which is misconfigured
        /// </summary>

        [DataMember]
        public string ElementName { get; set; }

        /// <summary>
        /// Element type if available (e.g. NavPageLink).
        /// </summary>

        [DataMember]
        public string ElementType { get; set; }
    }
}
