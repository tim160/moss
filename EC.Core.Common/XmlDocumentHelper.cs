using EC.Common.Base;
using EC.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EC.Constants;

namespace EC.Core.Common
{
    [SingletonType]
    [RegisterAsType(typeof(IXmlDocumentHelper))]

    public class XmlDocumentHelper : IXmlDocumentHelper
    {
        /// <summary>
        /// Create XML XPath starting at the root XML document.
        /// </summary>
        /// <param name="xPathElements">XPath elements</param>
        /// <param name="attributeName">Optional: get nodes with a specific attribute</param>
        /// <param name="attributeValue">Optional: get nodes with a specific attribute value</param>
        /// <returns>
        /// Return the XML path (incl. optional <paramref name="attributeName"/>. 
        /// Return <c>null</c> if <paramref name="xPathElements"/> is empty or <c>null</c>.
        /// </returns>
        /// TODO: Remove this implementation in favor of string CreateXPath(IList<Tuple<string, string, string>> xPathElement)

        public string CreateXPath(IList<string> xPathElements, string attributeName = null, string attributeValue = null)
        {
            string result = null;
            if ((xPathElements != null) && (xPathElements.Count > 0))
            {
                result += PathConstants.ElementSeparator;
                result += xPathElements.Aggregate((a, b) => a + PathConstants.ElementSeparator + b);
            }
            if (!string.IsNullOrEmpty(attributeName))
            {
                result += string.Format("[@{0}", attributeName);
                if (!string.IsNullOrEmpty(attributeValue)) { result += string.Format("='{0}'", attributeValue); }
                result += "]";
            }
            return result;
        }

        /// <summary>
        /// Create XML XPath search string from list of element paths
        /// List of Elements represent hierarchy from first item in list & each element may have an optional attribute filter
        ///         Tuple.Item1:  Element Name
        ///         Tuple.Item2:  Attribute Name (Optional)
        ///         Tuple.Item3:  Attribute Value (Optional)
        /// Ex Output:  "/Parent[@attr]/Child[@attr='val']/Child"
        /// </summary>
        /// <param name="xPathElements">list of XPathElement objects in hierarchical order</param>
        /// <returns>
        /// Return the XML path for list of XPathElements or null if list is empty 
        /// </returns>

        public string CreateXPath(IList<Tuple<string, string, string>> xPathElements)
        {
            string result = null;

            foreach (var xPathElement in xPathElements)
            {
                result += PathConstants.ElementSeparator;
                result += xPathElement.Item1;

                if (!string.IsNullOrEmpty(xPathElement.Item2))
                {
                    result += "[@" + xPathElement.Item2;

                    if (!string.IsNullOrEmpty(xPathElement.Item3))
                    {
                        result += string.Format("='{0}'", xPathElement.Item3);
                    }

                    result += "]";
                }
            }

            return result;
        }

        /// <summary>
        /// Create a new XML document with <paramref name="rootElementName"/> as first element.
        /// </summary>
        /// <remarks>
        /// Encoding is 'utf-8'.
        /// 
        /// 'doc.DocumentElement' returns the root element node.
        /// </remarks>
        /// <param name="rootElementName">XML root element name</param>
        /// <returns>Return the new XML document</returns>

        public XmlDocument CreateXmlDocument(string rootElementName)
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement(rootElementName));
            XmlDeclaration xDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            var rootNode = doc.DocumentElement;
            doc.InsertBefore(xDeclaration, rootNode);
            return doc;
        }

        /// <summary>
        /// Load an XML document from the <paramref name="xmlContent"/> string.
        /// </summary>
        /// <param name="xmlContent">String representation of the XML document (usually loaded from a file)</param>
        /// <returns>Return the loaded XML document</returns>
        /// <exception cref="XmlException">If something goes wrong during the loading of <paramref name="xmlContent"/> as XML document</exception>

        public XmlDocument LoadXmlDocument(string xmlContent)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            return doc;
        }

        /// <summary>
        /// Add attributes to specified node.
        /// The <paramref name="xmlPath"/> specifies the single node where to 
        /// add the attributes to.
        /// If there are multiple nodes matching the <paramref name="xmlPath"/> the attributes
        /// are added to the first node.
        /// </summary>
        /// <param name="doc">Xml document to create attributes for</param>
        /// <param name="xmlPath">Xml path to the node to which to add the attributes (i.e. //Page/PageAttributes)</param>
        /// <param name="attributes">Attributes to add to the node</param>

        public void AddAttributes(XmlDocument doc, string xmlPath, IDictionary<string, string> attributes)
        {
            var node = doc.SelectSingleNode(xmlPath);
            AddAttributes(doc, node, attributes);
        }

        /// <summary>
        /// Add XML attributes to the specified node.
        /// </summary>
        /// <remarks>If an attribute with the same name already exists, its value is simply overwritten.</remarks>
        /// <param name="doc">Xml document to create attributes for</param>
        /// <param name="node">Node to add the attributes</param>
        /// <param name="nodeAttributes">Attributes to add to the node</param>

        public void AddAttributes(XmlDocument doc, XmlNode node, IDictionary<string, string> nodeAttributes)
        {
            foreach (var attribute in nodeAttributes)
            {
                var newAttr = doc.CreateAttribute(attribute.Key);
                newAttr.Value = attribute.Value;
                node.Attributes.Append(newAttr);
            }
        }

        /// <summary>
        /// Create an XML attribute that indicates the node value is null.
        /// <param name="doc">Xml document to create a null attribute for</param>
        /// </summary>

        public XmlAttribute CreateNullAttribute(XmlDocument doc)
        {
            var newAttr = doc.CreateAttribute("null");
            newAttr.Value = "true";
            return newAttr;
        }

        /// <summary>
        /// Create a new XML node element with name <paramref name="nodeName" />.
        /// Optionally add node attributes.
        /// </summary>
        /// <param name="doc">XML document to create the node for</param>
        /// <param name="nodeName">Name of the node</param>
        /// <param name="nodeAttributes">
        /// Optional: node attributes to add to the new node. Set <c>null</c> or to an empty dictionary
        /// if no node attributes are needed.
        /// </param>
        /// <returns>
        /// Return the new node element (still has to be attached to the <paramref name="doc" /> somewhere.
        /// </returns>

        public XmlNode CreateXmlNode(XmlDocument doc, string nodeName, IDictionary<string, string> nodeAttributes = null)
        {
            var newNode = doc.CreateElement(nodeName);
            if (nodeAttributes != null && nodeAttributes.Count > 0) { AddAttributes(doc, newNode, nodeAttributes); }
            return newNode;
        }
    
        /// <summary>
        /// Create a new XML node element with name <paramref name="nodeName" /> with one attribute attached to it.
        /// </summary>
        /// <param name="doc">XML document to create the node for</param>
        /// <param name="nodeName">Name of the node</param>
        /// <param name="nodeAttributeKey">Key name of the attribute</param>
        /// <param name="nodeAttributeValue">Value of the attribute</param>
        /// <returns>
        /// Return the new node element (still has to be attached to the <paramref name="doc" /> somewhere.
        /// </returns>

        public XmlNode CreateXmlNodeWithSingleAttribute(XmlDocument doc, string nodeName, string nodeAttributeKey, string nodeAttributeValue)
        {
            var newNode = doc.CreateElement(nodeName);
            var nodeAttr = doc.CreateAttribute(nodeAttributeKey);
            if (nodeAttributeValue != null) { nodeAttr.InnerText = nodeAttributeValue; }
            newNode.Attributes.Append(nodeAttr);
            return newNode;
        }

        /// <summary>
        /// Create a new XML node element with name <paramref name="nodeName"/> and set <paramref name="nodeValue"/>.
        /// </summary>
        /// <param name="doc">XML document to create the node for</param>
        /// <param name="nodeName">Name of the node</param>
        /// <param name="nodeValue">String value of the node.</param>
        /// <param name="nodeAttributes">
        /// Optional: node attributes to add to the new node. Set <c>null</c> or to an empty dictionary
        /// if no node attributes are needed.
        /// </param>
        /// <returns>Return the new node element (still has to be attached to the <paramref name="doc"/> somewhere.</returns>

        public XmlNode CreateXmlNode(XmlDocument doc, string nodeName, string nodeValue, IDictionary<string, string> nodeAttributes = null)
        {
            var newNode = doc.CreateElement(nodeName);
            if (nodeAttributes != null && nodeAttributes.Count > 0) { AddAttributes(doc, newNode, nodeAttributes); }
            if (nodeValue == null)
            {
                newNode.Attributes.Append(CreateNullAttribute(doc));
            }
            else
            {
                newNode.InnerText = nodeValue;
            }
            return newNode;
        }

        /// <summary>
        /// Create a new XML node element with name <paramref name="nodeName" /> containing
        /// CDATA <paramref name="cData" />.
        /// </summary>
        /// <remarks>
        /// If an attribute with the same name already exists, its value is simply overwritten.
        /// </remarks>
        /// <param name="doc">XML document to create the node for</param>
        /// <param name="nodeName">Name of the node</param>
        /// <param name="cData">Encoded data to be added as CData inside <paramref name="nodeName" /></param>
        /// <param name="nodeAttributes">Attributes to add to the node</param>
        /// <returns>
        /// Return the new node element (still has to be attached to the <paramref name="doc" /> somewhere.
        /// </returns>

        public XmlNode CreateCDataSection(XmlDocument doc, string nodeName, string cData, IDictionary<string, string> nodeAttributes = null)
        {
            var newNode = doc.CreateElement(nodeName);
            if (nodeAttributes != null && nodeAttributes.Count > 0) { this.AddAttributes(doc, newNode, nodeAttributes); }
            if (cData == null)
            {
                newNode.Attributes.Append(CreateNullAttribute(doc));
            }
            else
            {
                newNode.AppendChild(doc.CreateCDataSection(cData));
            }
            return newNode;
        }

        /// <summary>
        /// Get string value (inner text) from <paramref name="element"/>.
        /// </summary>
        /// <remarks>The value is trimmed of white spaces</remarks>
        /// <param name="element">XML element to get the value (inner text) from</param>
        /// <param name="allowEmptyValue">Optional: Flag whether to allow an empty or white space value of the element</param></param>
        /// <returns>Return the inner text of the <paramref name="element"/></returns>
        /// <exception cref="XmlException">Throw exception if <paramref name="allowEmptyValue"/> == <c>false</c> and the value is empty or only contains white spaces.</exception>

        public string GetElementValue(XmlElement element, bool allowEmptyValue = true)
        {
            var value = element.HasAttribute("null") ? null : element.InnerText;
            if (string.IsNullOrEmpty(value) && !allowEmptyValue) { throw new XmlException(string.Format("The element '<{0}>' must have an inner text value.", element.Name)); }
            return value;
        }

        #region Get Optional ElementValue (string, bool, int, dateTime)

        /// <summary>
        /// Get optional XML element value by element name. 
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return the trimmed inner text of the XmlElement. Return <c>null</c> if the XmlElement doesn't exist or the values is empty/white spaces.</returns>

        public string GetOptionalElementValueByName(string elementName, IEnumerable<XmlElement> allElements)
        {
            var foundElement = allElements.FirstOrDefault(x => x.Name == elementName);
            if (foundElement == null) return null;
            if (foundElement.HasAttribute("null")) return null;
            return foundElement.InnerText;
        }

        /// <summary>
        /// Get optional XML element bool value by element name. 
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return bool value (<c>true</c>/<c>false</c>). Return <c>false</c> if the XML element doesn't exist.</returns>
        /// <exception cref="XmlException">If the XML element exists, but doesn't have a bool value.</exception>

        public bool GetOptionalBoolElementValueByName(string elementName, IEnumerable<XmlElement> allElements)
        {
            var foundElement = allElements.FirstOrDefault(x => x.Name == elementName);
            if (foundElement == null) { return false; }
           
            bool result = false;
            var boolString =  foundElement.InnerText.TrimOrDefault();

            if ((!string.IsNullOrEmpty(boolString)) && (!bool.TryParse(boolString, out result)))
            {
                throw new XmlException(string.Format("Can't parse bool element '{0}' value '{1}'.", elementName, boolString));
            }
            return result;
        }

        /// <summary>
        /// Get optional XML element int value by element name. 
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return int value if the XML element has been found. Return <c>null</c> if the XML element doesn't exist.</returns>
        /// <exception cref="XmlException">If the XML element exists, but doesn't have an int value.</exception>

        public int? GetOptionalIntElementValueByName(string elementName, IEnumerable<XmlElement> allElements)
        {
            var foundElement = allElements.FirstOrDefault(x => x.Name == elementName);
            if (foundElement == null) { return null; }

            int? result = null;
            var intString = foundElement.InnerText.TrimOrDefault();

            int parsedInt = -1;
            if ((!string.IsNullOrEmpty(intString)) && (!int.TryParse(intString, out parsedInt)))
            {
                throw new XmlException(string.Format("Can't parse int element '{0}' value '{1}'.", elementName, intString));
            }
            if (!string.IsNullOrEmpty(intString)) { result = parsedInt; }
            return result;
        }

        /// <summary>
        /// Get optional XML element DateTime value by element name. 
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return int value if the XML element has been found. Return <c>null</c> if the XML element doesn't exist.</returns>
        /// <exception cref="XmlException">If the XML element exists, but doesn't have a DateTime value.</exception>

        public DateTime? GetOptionalDateTimeElementValueByName(string elementName, IEnumerable<XmlElement> allElements)
        {
            var foundElement = allElements.FirstOrDefault(x => x.Name == elementName);
            if (foundElement == null) { return null; }

            DateTime? result = null;
            var dateTickString = foundElement.InnerText.TrimOrDefault();
            long dateTick = -1;
            if ((!string.IsNullOrEmpty(dateTickString)) && (!long.TryParse(dateTickString, out dateTick)))
            {
                throw new XmlException(string.Format("Can't parse DateTime.Tick attribute '{0}' value '{1}'.", elementName, dateTickString));
            }
            if (dateTick != -1) { result = new DateTime(dateTick, DateTimeKind.Utc); }
            if (result.Value < SQLUtils.MIN_SQL_DATE)
            {
                throw new XmlException(string.Format("The DateTime.Tick element '{0}' value '{1}' (={2}) must be after {3} (min SQL date)",
                    elementName, dateTickString, result.Value.ToShortDateString(), SQLUtils.MIN_SQL_DATE.ToShortDateString()));
            }
            return result;
        }

        #endregion

        #region Get Required ElementValue (string, bool, int, guid, enum)

        /// <summary>
        /// Get the required XML element value by element name.
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allPageInfoElements">List of XmlElements to look for the <paramref name="elementName"/></param>
        /// <param name="allowEmptyValue">Optional: Flag whether to allow an empty or white space value of the required element</param>
        /// <returns>Return the trimmed inner text of the XmlElement</returns>
        /// <exception cref="XmlException">If the XmlElement <paramref name="elementName"/> doesn't exist. Throw exception if <paramref name="allowEmptyValue"/> == <c>false</c>
        /// and the value is empty or only contains white spaces.</exception>

        public string GetRequiredElementValueByName(string elementName, IEnumerable<XmlElement> allPageInfoElements, bool allowEmptyValue = true)
        {
            var foundElement = allPageInfoElements.FirstOrDefault(x => x.Name == elementName);
            if (foundElement == null) { throw new XmlException(string.Format("The mandatory XML element '<{0}>' is missing.", elementName)); }
            var result = foundElement.HasAttribute("null") ? null : foundElement.InnerText;
            if ((string.IsNullOrEmpty(result)) && (!allowEmptyValue)) { throw new XmlException(string.Format("The mandatory element '{0}' must have a value.", elementName)); }
            return result;
        }

        /// <summary>
        /// Get required XML element bool value by element name. 
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return bool value (<c>true</c>/<c>false</c>).</returns>
        /// <exception cref="XmlException">If the XML element exists, but doesn't have a bool value or if the XML element is missing.</exception>

        public bool GetRequiredBoolElementValueByName(string elementName, IEnumerable<XmlElement> allElements)
        {
            var foundElement = allElements.FirstOrDefault(x => x.Name == elementName);
            if (foundElement == null) { throw new XmlException(string.Format("The mandatory XML element '<{0}>' is missing.", elementName)); }
            bool result = false;
            var boolString = foundElement.InnerText.TrimOrDefault();
            if (string.IsNullOrEmpty(boolString)) { throw new XmlException(string.Format("The mandatory element '{0}' must have a value.", elementName)); }
            if (!bool.TryParse(boolString, out result)) { throw new XmlException(string.Format("Can't parse bool element '{0}' value '{1}'.", elementName, boolString)); }
            return result;
        }

        /// <summary>
        /// Get required XML element int value by element name. 
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return int value.</returns>
        /// <exception cref="XmlException">If the XML element exists, but doesn't have an int value or if the XML element is missing.</exception>

        public int GetRequiredIntElementValueByName(string elementName, IEnumerable<XmlElement> allElements)
        {
            var foundElement = allElements.FirstOrDefault(x => x.Name == elementName);
            if (foundElement == null) { throw new XmlException(string.Format("The mandatory XML element '<{0}>' is missing.", elementName)); }
            int result = 0;
            var intString = foundElement.InnerText.TrimOrDefault();
            if (string.IsNullOrEmpty(intString)) { throw new XmlException(string.Format("The mandatory element '{0}' must have a value.", elementName)); }
            if (!int.TryParse(intString, out result)) { throw new XmlException(string.Format("Can't parse int element '{0}' value '{1}'.", elementName, intString)); }
            return result;
        }

        /// <summary>
        /// Get a mandatory Guid value of the element.
        /// </summary>
        /// <param name="element">XML element to get the value from</param>        
        /// <returns>Return the parsed Guid</returns>
        /// <exception cref="XmlException">If the value doesn't exist or is not a Guid</exception>

        public Guid GetRequiredGuidElementValue(XmlElement element)
        {
            var guidString = element.InnerText.TrimOrDefault();
            if (string.IsNullOrEmpty(guidString)) { throw new XmlException(string.Format("Can't parse required Guid value from element '{0}' because it has no value.", element.Name)); }
            Guid parsedGuid = Guid.Empty;
            if ((!string.IsNullOrEmpty(guidString)) && (!Guid.TryParse(guidString, out parsedGuid)))
            {
                throw new XmlException(string.Format("Can't parse required Guid value from element '{0}' value '{1}'.", element.Name, guidString));
            }
            return parsedGuid;
        }

        /// <summary>
        /// Parse a mandatory string element value to an enum of type <c>T</c>. 
        /// </summary>
        /// <typeparam name="T">Type parameter must be an enum</typeparam>
        /// <param name="elementName">Element name</param>
        /// <param name="elements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return the parsed enum</returns>
        /// <exception cref="ArgumentException">If the type <c>T</c> is not an enum</exception>
        /// <exception cref="XmlException">If the value does not exist or value cannot be parsed into enum of type <c>T</c></exception>

        public T GetRequiredEnumElementValue<T>(string elementName, IEnumerable<XmlElement> elements) where T : struct
        {
            if (!typeof(T).IsEnum) { throw new ArgumentException("The type T must be an enumeration."); }
            
            var foundElement = elements.FirstOrDefault(x => x.Name == elementName);
            if (foundElement == null) { throw new XmlException(string.Format("The mandatory XML element '<{0}>' is missing.", elementName)); }
            
            var enumValue = foundElement.InnerText.TrimOrDefault();
            if (string.IsNullOrWhiteSpace(enumValue)) { throw new XmlException(string.Format("Cannot parse required Enum value from element '{0}' because it has no value.", elementName)); }
            
            try
            {
                return (T)Enum.Parse(typeof(T), enumValue, true);
            }
            catch (Exception ex)
            {
                throw new XmlException(string.Format("Cannot parse required Enum value from element '{0}' to enum '{1}' with value '{2}'.", elementName, typeof(T).Name, enumValue), ex);
            }
        }

        #endregion

        #region Get Required Attribute (string, bool, int, long, double, guid, dateTime, dateTimeFormat, enum)

        /// <summary>
        /// Get a mandatory attribute value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <param name="allowEmptyValue">Optional: Flag whether to allow an empty or white space value of the required element</param>
        /// <returns></returns>
        /// <exception cref="XmlException">>If the XmlElement <paramref name="elementName"/> doesn't exist. Throw exception if <paramref name="allowEmptyValue"/> == <c>false</c>
        /// and the value is empty or only contains white spaces.</exception>

        public string GetRequiredAttribute(XmlElement element, string attributeName, bool allowEmptyValue = true)
        {
            if (!element.HasAttribute(attributeName)) { throw new XmlException(string.Format("The mandatory attribute '{0}' is missing.", attributeName)); }
            string result = element.GetAttribute(attributeName).TrimOrDefault();
            if ((string.IsNullOrEmpty(result)) && (!allowEmptyValue)) { throw new XmlException(string.Format("The mandatory attribute '{0}' must have a value.", attributeName)); }
            return result;
        }

        /// <summary>
        /// Get a mandatory attribute bool value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return the parsed bool</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist or is not a bool</exception>

        public bool GetRequiredBoolAttribute(XmlElement element, string attributeName)
        {
            if (!element.HasAttribute(attributeName)) { throw new XmlException(string.Format("The mandatory attribute '{0}' is missing.", attributeName)); }
            bool result = false;
            var boolString = element.GetAttribute(attributeName).TrimOrDefault();
            if ((!string.IsNullOrEmpty(boolString)) && (!bool.TryParse(boolString, out result)))
            {
                throw new XmlException(string.Format("Can't parse bool attribute '{0}' value '{1}'.", attributeName, boolString));
            }
            return result;
        }

        /// <summary>
        /// Get a mandatory attribute int value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return the parsed int</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist or is not an int</exception>

        public int GetRequiredIntAttribute(XmlElement element, string attributeName)
        {
            var parsedInt = this.GetOptionalIntAttribute(element, attributeName);
            if (!parsedInt.HasValue) { throw new XmlException(string.Format("The mandatory attribute '{0}' is missing.", attributeName)); }
            return parsedInt.Value;
        }

        /// <summary>
        /// Get a mandatory attribute long value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return the parsed long</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist or is not a long</exception>

        public long GetRequiredLongAttribute(XmlElement element, string attributeName)
        {
            var parsedLong = this.GetOptionalLongAttribute(element, attributeName);
            if (!parsedLong.HasValue) { throw new XmlException(string.Format("The mandatory attribute '{0}' is missing.", attributeName)); }
            return parsedLong.Value;
        }

        /// <summary>
        /// Get a mandatory attribute double value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return the parsed double</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist or is not a double</exception>

        public double GetRequiredDoubleAttribute(XmlElement element, string attributeName)
        {
            var parsedDouble = this.GetOptionalDoubleAttribute(element, attributeName);
            if (!parsedDouble.HasValue) { throw new XmlException(string.Format("The mandatory attribute '{0}' is missing.", attributeName)); }
            return parsedDouble.Value;
        }

        /// <summary>
        /// Get a mandatory attribute Guid value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return the parsed Guid</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist or is not a Guid</exception>
       
        public Guid GetRequiredGuidAttribute(XmlElement element, string attributeName)
        {
            var parsedGuid = this.GetOptionalGuidAttribute(element, attributeName);
            if (!parsedGuid.HasValue) { throw new XmlException(string.Format("The mandatory attribute '{0}' is missing.", attributeName)); }
            return parsedGuid.Value;
        }

        /// <summary>
        /// Get a mandatory attribute DateTime value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return the parsed DateTime</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist, is not a DateTime.Ticks or before the SQL min date.</exception>

        public DateTime GetRequiredDateTimeAttribute(XmlElement element, string attributeName)
        {
            var parsedDateTime = this.GetOptionalDateTimeAttribute(element, attributeName);
            if (!parsedDateTime.HasValue) { throw new XmlException(string.Format("The mandatory attribute '{0}' is missing.", attributeName)); }
            return parsedDateTime.Value;
        }

        /// <summary>
        /// Get a mandatory attribute DateTime value parsed as <paramref name="dateTimeFormat"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <param name="dateTimeFormat">DateTime format how to parse the attribute</param>
        /// <returns>Return the parsed DateTime</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist, is not of format <paramref name="dateTimeFormat"/> or before the SQL min date.</exception>

        public DateTime GetRequiredDateTimeAttributeByFormat(XmlElement element, string attributeName, string dateTimeFormat)
        {
            var dTimeString= this.GetRequiredAttribute(element, attributeName);
            try
            {
                var result = XmlConvert.ToDateTime(dTimeString, dateTimeFormat);
                result = DateTime.SpecifyKind(result, DateTimeKind.Utc);
                return result;
            }
            catch (Exception ex)
            {
                throw new XmlException(string.Format("Can't parse '{0}' to DateTime because it's not of format '{1}'.", dTimeString, dateTimeFormat), ex);
            }
        }

        /// <summary>
        /// Parse an XML attribute value to an enum of type <c>T</c>. 
        /// </summary>
        /// <typeparam name="T">Type parameter must be an enum</typeparam>
        /// <param name="element">XML element which contains the attribute <paramref name="attributeName"/>.</param>
        /// <param name="attributeName">Attribute name to parse its value.</param>
        /// <returns>Return the parsed enum</returns>
        /// <exception cref="ArgumentException">If the type <c>T</c> is not an enum</exception>
        /// <exception cref="XmlException">If the XML attribute <paramref name="attributeName"/> is missing/empty or the value can't be parsed into the enum of type <c>T</c></exception>

        public T GetRequiredEnumAttribute<T>(XmlElement element, string attributeName) where T : struct
        {
            if (!typeof(T).IsEnum) { throw new ArgumentException("The type T must be an enumeration."); }
            if (!element.HasAttribute(attributeName)) { throw new XmlException(string.Format("Can't parse enum attribute '{0}' because it has no value.", attributeName)); }
            var enumValue = element.GetAttribute(attributeName).TrimOrDefault();
            if (string.IsNullOrWhiteSpace(enumValue)) { throw new XmlException(string.Format("Can't parse enum attribute '{0}' value '{1}' of type '{2}'.", attributeName, enumValue, typeof(T).Name)); }
            try
            {
                return (T)Enum.Parse(typeof(T), enumValue, true);
            }
            catch (Exception ex)
            {
                throw new XmlException(string.Format("Can't parse XML attribute '{0}' to enum '{1}' with value '{2}'.", attributeName, typeof(T).Name, enumValue), ex);
            }
        }

        #endregion

        #region Get Optional Attribute (string, bool, int, long, double, guid, dateTime, enum)

        /// <summary>
        /// Get optional attribute (as string) from the current element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return string value. Return <c>null</c> if the attribute value doesn't exist.</returns>
        /// <exception cref="XmlException">If the attribute has a wrong value.</exception>

        public string GetOptionalAttribute(XmlElement element, string attributeName)
        {
            if (!element.HasAttribute(attributeName)) { return null; }
            return element.GetAttribute(attributeName).TrimOrDefault();
        }

        /// <summary>
        /// Get an optional attribute as DateTime.Ticks from the current element.
        /// </summary>
        /// <param name="reader">Reader is positioned at the element to get the attribute value from.</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Return the DateTime value if it exists. Return <c>null</c> if the attribute doesn't exist.</returns>
        /// <exception cref="XmlException">If the attribute has a wrong value or the DateTime is before the SQL min date (Jan 1, 1753)</exception>

        public DateTime? GetOptionalDateTimeAttribute(XmlElement element, string attributeName)
        {
            if (!element.HasAttribute(attributeName)) { return null; }
            DateTime? result = null;
            var dateTickString = element.GetAttribute(attributeName).TrimOrDefault();
            long dateTick = -1;
            if ((!string.IsNullOrEmpty(dateTickString)) && (!long.TryParse(dateTickString, out dateTick)))
            {
                throw new XmlException(string.Format("Can't parse DateTime.Tick attribute '{0}' value '{1}'.", attributeName, dateTickString));
            }
            if (dateTick != -1) { result = new DateTime(dateTick, DateTimeKind.Utc); }
            if (result.Value < SQLUtils.MIN_SQL_DATE) 
            {
                throw new XmlException(string.Format("The DateTime.Tick attribute '{0}' value '{1}' (={2}) must be after {3} (min SQL date)", 
                    attributeName, dateTickString, result.Value.ToShortDateString(), SQLUtils.MIN_SQL_DATE.ToShortDateString()));
            }
            return result;
        }

        /// <summary>
        /// Get an optional attribute as Guid from the current element.
        /// </summary>
        /// <param name="element">XML element to get the attribute from.</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Return Guid value. Return <c>null</c> if the attribute doesn't exist.</returns>
        /// <exception cref="XmlException">If the attribute has a wrong value</exception>

        public Guid? GetOptionalGuidAttribute(XmlElement element, string attributeName)
        {
            if (!element.HasAttribute(attributeName)) { return null; }
            var guidString = element.GetAttribute(attributeName).TrimOrDefault();
            if (string.IsNullOrEmpty(guidString)) { throw new XmlException(string.Format("Can't parse Guid attribute '{0}' because it has no value.", attributeName)); }
            Guid parsedGuid = Guid.Empty;
            if ((!string.IsNullOrEmpty(guidString)) && (!Guid.TryParse(guidString, out parsedGuid)))
            {
                throw new XmlException(string.Format("Can't parse Guid attribute '{0}' value '{1}'.", attributeName, guidString));
            }
            return parsedGuid;
        }

        /// <summary>
        /// Get optional boolean attribute from the current element.
        /// </summary>
        /// <remarks>
        /// Default value is <c>false</c> if the attribute doesn't exist.
        /// </remarks>
        /// <param name="reader"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return boolean value. Return <c>false</c> if the attribute value is <c>false</c> or doesn't exist.</returns>
        /// <exception cref="XmlException">If the attribute has a wrong value.</exception>

        public bool GetOptionalBoolAttribute(XmlElement element, string attributeName)
        {
            if (!element.HasAttribute(attributeName)) { return false; }

            bool result = false;
            var boolString = element.GetAttribute(attributeName).TrimOrDefault();

            if ((!string.IsNullOrEmpty(boolString)) && (!bool.TryParse(boolString, out result)))
            {
                throw new XmlException(string.Format("Can't parse bool attribute '{0}' value '{1}'.", attributeName, boolString));
            }
            return result;
        }

        /// <summary>
        /// Get optional int attribute from the current element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return int value. Return <c>null</c> if the attribute value doesn't exist.</returns>
        /// <exception cref="Exception">If the attribute has a wrong value.</exception>

        public int? GetOptionalIntAttribute(XmlElement element, string attributeName)
        {
            if (!element.HasAttribute(attributeName)) { return null; }

            int? result = null;
            var intString = element.GetAttribute(attributeName).TrimOrDefault();

            int parsedInt = -1;
            if ((!string.IsNullOrEmpty(intString)) && (!int.TryParse(intString, out parsedInt)))
            {
                throw new XmlException(string.Format("Can't parse int attribute '{0}' value '{1}'.", attributeName, intString));
            }
            if (!string.IsNullOrEmpty(intString)) { result = parsedInt; }
            return result;
        }

        /// <summary>
        /// Get optional double attribute from the current element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return int value. Return <c>null</c> if the attribute value doesn't exist.</returns>
        /// <exception cref="Exception">If the attribute has a wrong value.</exception>

        public double? GetOptionalDoubleAttribute(XmlElement element, string attributeName)
        {
            if (!element.HasAttribute(attributeName)) { return null; }

            double? result = null;
            var doubleString = element.GetAttribute(attributeName).TrimOrDefault();

            double parsedDouble = -1;
            if ((!string.IsNullOrEmpty(doubleString)) && (!double.TryParse(doubleString, out parsedDouble)))
            {
                throw new XmlException(string.Format("Can't parse double attribute '{0}' value '{1}'.", attributeName, doubleString));
            }
            if (!string.IsNullOrEmpty(doubleString)) { result = parsedDouble; }
            return result;
        }

        /// <summary>
        /// Get optional long attribute from the current element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return long value. Return <c>null</c> if the attribute value doesn't exist.</returns>
        /// <exception cref="Exception">If the attribute has a wrong value.</exception>

        public long? GetOptionalLongAttribute(XmlElement element, string attributeName)
        {
            if (!element.HasAttribute(attributeName)) { return null; }

            long? result = null;
            var longString = element.GetAttribute(attributeName).TrimOrDefault();
            
            long parsedLong = -1;
            if ((!string.IsNullOrEmpty(longString)) && (!long.TryParse(longString, out parsedLong)))
            {
                throw new XmlException(string.Format("Can't parse long attribute '{0}' value '{1}'.", attributeName, longString));
            }
            if (!string.IsNullOrEmpty(longString)) { result = parsedLong; }
            return result;
        }

        /// <summary>
        /// Get optional string attribute from the current element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return string value. Return <c>null</c> if the attribute value doesn't exist.</returns>
        /// <exception cref="XmlException">If the attribute has a wrong value.</exception>

        public string GetOptionalStringAttribute(XmlElement element, string attributeName)
        {
            if (!element.HasAttribute(attributeName)) { return null; }
            return element.GetAttribute(attributeName).TrimOrDefault();
        }

        /// <summary>
        /// Parse an XML attribute value to an enum of type <c>T</c>. 
        /// </summary>
        /// <typeparam name="T">Type parameter must be an enum</typeparam>
        /// <param name="element">XML element which contains the attribute <paramref name="attributeName"/>.</param>
        /// <param name="attributeName">Attribute name to parse its value.</param>
        /// <returns>Return the parsed enum. Return null if does not exist.</returns>
        /// <exception cref="ArgumentException">If the type <c>T</c> is not an enum</exception>
        /// 

        public T? GetOptionalEnumAttribute<T>(XmlElement element, string attributeName) where T : struct
        {
            if (!element.HasAttribute(attributeName)) { return null; }

            var enumString = element.GetAttribute(attributeName).TrimOrDefault();
            var result = (T)Enum.Parse(typeof(T), enumString, true);
            if (Enum.IsDefined(typeof(T), result))
            {
                return result;
            }
            else
            {
                return null;
            }

        }

        #endregion
    }
}
