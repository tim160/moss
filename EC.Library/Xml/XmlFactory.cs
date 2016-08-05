using System;
using System.Collections.Generic;
using System.Text;
using System.Xml; 

namespace EC.Library.Xml
{
    public class XmlFactory
    {
        #region Attribute
        #region CreateAttribute
        /// <summary>
        /// Create an attribute for a document. 
        /// </summary>
        /// <param name="doc">The document to create the attribute for.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value to set for the attribute.</param>
        /// <returns>The new attribute.</returns>
        public static XmlAttribute CreateAttribute(XmlDocument doc, string name, object value)
        {
            XmlAttribute attribute = doc.CreateAttribute(name);
            attribute.Value = (value == null) ? String.Empty : value.ToString();
            return attribute;
        }
        #endregion

        #region AddAttribute
        /// <summary>
        /// Create an attribute for a document. 
        /// </summary>
        /// <param name="doc">The document to create the attribute for.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value to set for the attribute.</param>
        /// <returns>The new attribute.</returns>
        public static XmlAttribute AddAttribute(XmlDocument doc, XmlElement ele, string name, object value)
        {
            XmlAttribute attribute = CreateAttribute(doc, name, value);
            ele.Attributes.Append(attribute);
            return attribute;
        }
        #endregion
        #endregion

        #region Element
        #region CreateElement
        /// <summary>
        /// Create an element for a document. 
        /// </summary>
        /// <param name="doc">The document to create the element for.</param>
        /// <param name="name">The name of the element.</param>
        /// <returns>The new element.</returns>
        public static XmlElement CreateElement(XmlDocument doc, string name)
        {
            return doc.CreateElement(name);
        }

        /// <summary>
        /// Create an element for a document. 
        /// </summary>
        /// <param name="doc">The document to create the element for.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="value">The value to set for the element.</param>
        /// <returns>The new element.</returns>
        public static XmlElement CreateElement(XmlDocument doc, string name, object value)
        {
            XmlElement element = CreateElement(doc, name);
            if (value != null)
                element.InnerText = value.ToString();
            return element;
        }
        #endregion

        #region AddElement
        /// <summary>
        /// Create an element for a document. 
        /// </summary>
        /// <param name="doc">The document to create the element for.</param>
        /// <param name="name">The name of the element.</param>
        /// <returns>The new element.</returns>
        public static XmlElement AddElement(XmlDocument doc, string name)
        {
            XmlElement element = CreateElement(doc, name);
            doc.AppendChild(element);
            return element;
        }

        /// <summary>
        /// Create an element for a document. 
        /// </summary>
        /// <param name="doc">The document to create the element for.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="value">The value to set for the element.</param>
        /// <returns>The new element.</returns>
        public static XmlElement AddElement(XmlDocument doc, string name, object value)
        {
            XmlElement element = CreateElement(doc, name, value);
            doc.AppendChild(element);
            return element;
        }

        /// <summary>
        /// Create an element for a document. 
        /// </summary>
        /// <param name="doc">The document to create the element for.</param>
        /// <param name="name">The name of the element.</param>
        /// <returns>The new element.</returns>
        public static XmlElement AddElement(XmlDocument doc, XmlElement ele, string name)
        {
            XmlElement element = CreateElement(doc, name);
            ele.AppendChild(element);
            return element;
        }

        /// <summary>
        /// Create an element for a document. 
        /// </summary>
        /// <param name="doc">The document to create the element for.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="value">The value to set for the element.</param>
        /// <returns>The new element.</returns>
        public static XmlElement AddElement(XmlDocument doc, XmlElement ele, string name, object value)
        {
            XmlElement element = CreateElement(doc, name, value);
            ele.AppendChild(element);
            return element;
        }
        #endregion
        #endregion

        #region CDATA
        #region CreateCData
        /// <summary>
        /// Create an element for a document. 
        /// </summary>
        /// <param name="doc">The document to create the element for.</param>
        /// <param name="name">The name of the element.</param>
        /// <returns>The new element.</returns>
        public static XmlCDataSection CreateCDataSection(XmlDocument doc, object data)
        {
            return doc.CreateCDataSection((data == null) ? string.Empty : data.ToString());
        }
        #endregion

        #region AddCData
        /// <summary>
        /// Create an element for a document. 
        /// </summary>
        /// <param name="doc">The document to create the element for.</param>
        /// <param name="name">The name of the element.</param>
        /// <returns>The new element.</returns>
        public static XmlCDataSection AddCDataSection(XmlDocument doc, object data)
        {
            XmlCDataSection cdata = CreateCDataSection(doc, data);
            doc.AppendChild(cdata);
            return cdata;
        }

        /// <summary>
        /// Create an element for a document. 
        /// </summary>
        /// <param name="doc">The document to create the element for.</param>
        /// <param name="name">The name of the element.</param>
        /// <returns>The new element.</returns>
        public static XmlCDataSection AddCDataSection(XmlDocument doc, XmlElement ele, object data)
        {
            XmlCDataSection cdata = CreateCDataSection(doc, data);
            ele.AppendChild(cdata);
            return cdata;
        }
        #endregion
        #endregion
    }
}
