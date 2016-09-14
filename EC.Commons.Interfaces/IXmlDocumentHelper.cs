using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EC.Common.Interfaces
{
    public interface IXmlDocumentHelper
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

        string CreateXPath(IList<string> xPathElements, string attributeName = null, string attributeValue = null);

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

        string CreateXPath(IList<Tuple<string, string, string>> xPathElements);

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

        XmlDocument CreateXmlDocument(string rootElementName);

        /// <summary>
        /// Load an XML document from the <paramref name="xmlContent"/> string.
        /// </summary>
        /// <param name="xmlContent">String representation of the XML document (usually loaded from a file)</param>
        /// <returns>Return the loaded XML document</returns>
        /// <exception cref="XmlException">If something goes wrong during the loading of <paramref name="xmlContent"/> as XML document</exception>

        XmlDocument LoadXmlDocument(string xmlContent);

        /// <summary>
        /// Add attributes to specified node.
        /// The <paramref name="xmlPath"/> specifies the single node where to 
        /// add the attributes to.
        /// If there are multiple nodes matching the <paramref name="xmlPath"/> the attributes
        /// are added to the first node.
        /// </summary>
        /// <remarks>
        /// If an attribute with the same name already exists, its value is simply overwritten.
        /// </remarks>
        /// <param name="doc">Xml document to create attributes for</param>
        /// <param name="xmlPath">Xml path to the node to which to add the attributes (i.e. //Page/PageAttributes)</param>
        /// <param name="attributes">Attributes to add to the node</param>

        void AddAttributes(XmlDocument doc, string xmlPath, IDictionary<string, string> attributes);

        /// <summary>
        /// Add XML attributes to the specified node.
        /// </summary>
        /// <remarks>
        /// If an attribute with the same name already exists, its value is simply overwritten.
        /// </remarks>
        /// <param name="doc">Xml document to create attributes for</param>
        /// <param name="node">Node to add the attributes</param>
        /// <param name="attributes">Attributes to add to the node</param>

        void AddAttributes(XmlDocument doc, XmlNode node, IDictionary<string, string> attributes);

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

        XmlNode CreateXmlNode(XmlDocument doc, string nodeName, IDictionary<string, string> nodeAttributes = null);

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

        XmlNode CreateXmlNodeWithSingleAttribute(XmlDocument doc, string nodeName, string nodeAttributeKey, string nodeAttributeValue);

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

        XmlNode CreateXmlNode(XmlDocument doc, string nodeName, string nodeValue, IDictionary<string, string> nodeAttributes = null);

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

        XmlNode CreateCDataSection(XmlDocument doc, string nodeName, string cData, IDictionary<string, string> nodeAttributes = null);

        /// <summary>
        /// Get optional XML element value by element name. 
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return the trimmed inner text of the XmlElement. Return <c>null</c> if the XmlElement doesn't exist.</returns>

        string GetOptionalElementValueByName(string elementName, IEnumerable<XmlElement> allElements);

        /// <summary>
        /// Get optional XML element bool value by element name. 
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return bool value (<c>true</c>/<c>false</c>). Return <c>false</c> if the XML element doesn't exist.</returns>
        /// <exception cref="XmlException">If the XML element exists, but doesn't have a bool value.</exception>

        bool GetOptionalBoolElementValueByName(string elementName, IEnumerable<XmlElement> allElements);

        /// <summary>
        /// Parse an XML attribute value to an enum of type <c>T</c>. 
        /// </summary>
        /// <typeparam name="T">Type parameter must be an enum</typeparam>
        /// <param name="element">XML element which contains the attribute <paramref name="attributeName"/>.</param>
        /// <param name="attributeName">Attribute name to parse its value.</param>
        /// <returns>Return the parsed enum. Return null if does not exist.</returns>
        /// <exception cref="ArgumentException">If the type <c>T</c> is not an enum</exception>
        /// 

        T? GetOptionalEnumAttribute<T>(XmlElement element, string attributeName) where T : struct;

        /// <summary>
        /// Get optional XML element int value by element name. 
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return int value if the XML element has been found. Return <c>null</c> if the XML element doesn't exist.</returns>
        /// <exception cref="XmlException">If the XML element exists, but doesn't have an int value.</exception>

        int? GetOptionalIntElementValueByName(string elementName, IEnumerable<XmlElement> allElements);

        /// <summary>
        /// Get optional XML element DateTime value by element name. 
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return int value if the XML element has been found. Return <c>null</c> if the XML element doesn't exist.</returns>
        /// <exception cref="XmlException">If the XML element exists, but doesn't have a DateTime value.</exception>

        DateTime? GetOptionalDateTimeElementValueByName(string elementName, IEnumerable<XmlElement> allElements);

        /// <summary>
        /// Get the required XML element value by element name.
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/></param>
        /// <param name="allowEmptyValue">Optional: Flag whether to allow an empty or white space value of the required element</param>
        /// <returns>Return the trimmed inner text of the XmlElement</returns>
        /// <exception cref="XmlException">If the XmlElement <paramref name="elementName"/> doesn't exist. Throw exception if <paramref name="allowEmptyValue"/> == <c>false</c>
        /// and the value is empty or only contains white spaces.</exception>

        string GetRequiredElementValueByName(string elementName, IEnumerable<XmlElement> allElements, bool allowEmptyValue = true);

        /// <summary>
        /// Get required XML element bool value by element name. 
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return bool value (<c>true</c>/<c>false</c>).</returns>
        /// <exception cref="XmlException">If the XML element exists, but doesn't have a bool value or if the XML element is missing.</exception>

        bool GetRequiredBoolElementValueByName(string elementName, IEnumerable<XmlElement> allElements);

        /// <summary>
        /// Get required XML element int value by element name. 
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="allElements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return int value.</returns>
        /// <exception cref="XmlException">If the XML element exists, but doesn't have an int value or if the XML element is missing.</exception>

        int GetRequiredIntElementValueByName(string elementName, IEnumerable<XmlElement> allElements);

        /// <summary>
        /// Get a mandatory Guid value of the element.
        /// </summary>
        /// <param name="element">XML element to get the value from</param>        
        /// <returns>Return the parsed Guid</returns>
        /// <exception cref="XmlException">If the value doesn't exist or is not a Guid</exception>

        Guid GetRequiredGuidElementValue(XmlElement element);

        /// <summary>
        /// Parse a mandatory string element value to an enum of type <c>T</c>. 
        /// </summary>
        /// <typeparam name="T">Type parameter must be an enum</typeparam>
        /// <param name="elementName">Element name</param>
        /// <param name="elements">List of XmlElements to look for the <paramref name="elementName"/>.</param>
        /// <returns>Return the parsed enum</returns>
        /// <exception cref="ArgumentException">If the type <c>T</c> is not an enum</exception>
        /// <exception cref="XmlException">If the value does not exist or value cannot be parsed into enum of type <c>T</c></exception>

        T GetRequiredEnumElementValue<T>(string elementName, IEnumerable<XmlElement> elements) where T : struct;

        /// <summary>
        /// Get a mandatory attribute value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <param name="allowEmptyValue">Optional: Flag whether to allow an empty or white space value of the required element</param>
        /// <returns></returns>
        /// <exception cref="XmlException">>If the XmlElement <paramref name="elementName"/> doesn't exist. Throw exception if <paramref name="allowEmptyValue"/> == <c>false</c>
        /// and the value is empty or only contains white spaces.</exception>

        string GetRequiredAttribute(XmlElement element, string attributeName, bool allowEmptyValue = true);

        /// <summary>
        /// Get a mandatory attribute bool value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return the parsed bool</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist or is not a bool</exception>

        bool GetRequiredBoolAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get a mandatory attribute int value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return the parsed int</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist or is not an int</exception>

        int GetRequiredIntAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get a mandatory attribute long value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return the parsed long</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist or is not a long</exception>

        long GetRequiredLongAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get a mandatory attribute double value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return the parsed double</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist or is not a double</exception>

        double GetRequiredDoubleAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get a mandatory attribute Guid value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return the parsed Guid</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist or is not a Guid</exception>

        Guid GetRequiredGuidAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get a mandatory attribute DateTime value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return the parsed DateTime</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist, is not a DateTime.Ticks or before the SQL min date.</exception>

        DateTime GetRequiredDateTimeAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get a mandatory attribute DateTime value parsed as <paramref name="dateTimeFormat"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <param name="dateTimeFormat">DateTime format how to parse the attribute</param>
        /// <returns>Return the parsed DateTime</returns>
        /// <exception cref="XmlException">If the attribute doesn't exist, is not of format <paramref name="dateTimeFormat"/> or before the SQL min date.</exception>

        DateTime GetRequiredDateTimeAttributeByFormat(XmlElement element, string attributeName, string dateTimeFormat);

        /// <summary>
        /// Parse an XML attribute value to an enum of type <c>T</c>. 
        /// </summary>
        /// <typeparam name="T">Type parameter must be an enum</typeparam>
        /// <param name="element">XML element which contains the attribute <paramref name="attributeName"/>.</param>
        /// <param name="attributeName">Attribute name to parse its value.</param>
        /// <returns>Return the parsed enum</returns>
        /// <exception cref="ArgumentException">If the type <c>T</c> is not an enum</exception>
        /// <exception cref="XmlException">If the XML attribute <paramref name="attributeName"/> is missing/empty or the value can't be parsed into the enum of type <c>T</c></exception>

        T GetRequiredEnumAttribute<T>(XmlElement element, string attributeName) where T : struct;

        /// <summary>
        /// Get an optional attribute as DateTime.Ticks from the current element.
        /// </summary>
        /// <param name="element">XML element to get the attribute from.</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Return the DateTime value if it exists. Return <c>null</c> if the attribute doesn't exist.</returns>
        /// <exception cref="XmlException">If the attribute has a wrong value.</exception>

        DateTime? GetOptionalDateTimeAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get an optional attribute as Guid from the current element.
        /// </summary>
        /// <param name="element">XML element to get the attribute from.</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Return Guid value. Return <c>null</c> if the attribute doesn't exist.</returns>
        /// <exception cref="XmlException">If the attribute has a wrong value</exception>

        Guid? GetOptionalGuidAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get optional boolean attribute from the current element.
        /// </summary>
        /// <remarks>
        /// Default value is <c>false</c> if the attribute doesn't exist.
        /// </remarks>
        /// <param name="element">XML element to get the attribute from.</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Return boolean value. Return <c>false</c> if the attribute value is <c>false</c> or doesn't exist.</returns>
        /// <exception cref="XmlException">If the attribute has a wrong value.</exception>

        bool GetOptionalBoolAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get optional int attribute from the current element.
        /// </summary>
        /// <param name="element">XML element to get the attribute from.</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Return int value. Return <c>null</c> if the attribute value doesn't exist.</returns>
        /// <exception cref="XmlException">If the attribute has a wrong value.</exception>

        int? GetOptionalIntAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get optional double attribute from the current element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return int value. Return <c>null</c> if the attribute value doesn't exist.</returns>
        /// <exception cref="Exception">If the attribute has a wrong value.</exception>

        double? GetOptionalDoubleAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get optional long attribute from the current element.
        /// </summary>
        /// <param name="element">XML element to get the attribute from.</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Return long value. Return <c>null</c> if the attribute value doesn't exist.</returns>
        /// <exception cref="XmlException">If the attribute has a wrong value.</exception>

        long? GetOptionalLongAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get optional attribute (as string) from the current element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns>Return string value. Return <c>null</c> if the attribute value doesn't exist.</returns>
        /// <exception cref="XmlException">If the attribute has a wrong value.</exception>

        string GetOptionalAttribute(XmlElement element, string attributeName);

        /// <summary>
        /// Get string value (inner text) from <paramref name="element"/>.
        /// </summary>
        /// <remarks>The value is trimmed of white spaces</remarks>
        /// <param name="element">XML element to get the value (inner text) from</param>
        /// <param name="allowEmptyValue">Optional: Flag whether to allow an empty or white space value of the element</param></param>
        /// <returns>Return the inner text of the <paramref name="element"/></returns>
        /// <exception cref="XmlException">Throw exception if <paramref name="allowEmptyValue"/> == <c>false</c> and the value is empty or only contains white spaces.</exception>

        string GetElementValue(XmlElement element, bool allowEmptyValue = true);

    }
}