using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Demo.CommonFramework.ExceptionHandler;

namespace Demo.CommonFramework.Helpers
{
    public class BaseXmlHelper
    {
        public const string TESTING_ROOT_NAME = @"ROOT_TESTING";
        public const string ROOT_TEMPLATE = @"<" + TESTING_ROOT_NAME + ">%XML%</" + TESTING_ROOT_NAME + ">";
        private const string LOCAL_NAME_IGNORE_CASE = "translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')";

        /// <summary>
        /// Pretties the XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static string PrettyXml(string xml)
        {
            var stringBuilder = new StringBuilder();
            XmlDocument element = new XmlDocument();
            element.LoadXml(xml);

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                NewLineOnAttributes = true
            };

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }


        /// <summary>
        /// Fills the properties from XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="type">The type.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="parentName">Name of the parent.</param>
        /// <param name="elementName">Name of the element.</param>
        public static void FillPropertiesFromXml<T>(ref T obj, Type type, XmlDocument xml, string parentName = null, string elementName = null)
        {
            XmlNode node = SelectSingleNodeIgnoreCase(xml, type.Name, parentName, elementName);

            if (node == null) return;

            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                XmlNode nodeProp = SelectSingleNodeIgnoreCase(xml, node.LocalName, parentName, property.Name);
                if (nodeProp != null && nodeProp.HasChildNodes
                    && !nodeProp.FirstChild.NodeType.Equals(XmlNodeType.Text)
                    && !nodeProp.FirstChild.NodeType.Equals(XmlNodeType.CDATA))
                {
                    object instance = Activator.CreateInstance(property.PropertyType);
                    GetInstanceValue(ref instance, nodeProp, property.PropertyType, xml);
                    property.SetValue(obj, instance);
                    continue;
                }

                string nodeValue = nodeProp?.InnerXml;

                if (string.IsNullOrEmpty(nodeValue)) continue;

                SetPropertyValue(ref obj, property, nodeValue);
            }
        }

        /// <summary>
        /// Selects the single node ignore case.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="name">The name.</param>
        /// <param name="parentName">Name of the parent.</param>
        /// <param name="childName">Name of the child.</param>
        /// <returns></returns>
        private static XmlNode SelectSingleNodeIgnoreCase(XmlDocument xml, string name, string parentName = null, string childName = null)
        {
            name = BaseUtils.RemoveNumbersAtTheEnd(name);
            if (parentName == null && childName == null)
            {
                return xml.SelectSingleNode(@"//*[" + LOCAL_NAME_IGNORE_CASE + "='" + name.ToLower() + "']");
            }
            else if (parentName == null)
            {
                return xml.SelectSingleNode(@"//*[" + LOCAL_NAME_IGNORE_CASE + "='" + name.ToLower()
                                            + "']/*[" + LOCAL_NAME_IGNORE_CASE + "='" + childName.ToLower() + "']");
            }
            else
            {
                return xml.SelectSingleNode(@"//*[" + LOCAL_NAME_IGNORE_CASE + "='" + parentName.ToLower()
                                            + "']/*[" + LOCAL_NAME_IGNORE_CASE + "='" + name.ToLower()
                                            + "']/*[" + LOCAL_NAME_IGNORE_CASE + "='" + childName.ToLower() + "']");
            }
        }

        /// <summary>
        /// Fills the list elements from XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="type">The type.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="parentName">Name of the parent.</param>
        /// <param name="elementName">Name of the element.</param>
        public static void FillListElementsFromXml<T>(ref T obj, Type type, XmlDocument xml, string parentName = null, string elementName = null, string grandParentName = null, int index = 0)
        {
            PropertyInfo[] properties = type.GetProperties();

            if (type.BaseType.Name == typeof(List<>).Name)
            {
                XmlNodeList nodeList = null;

                if (parentName == null)
                {
                    string typeName = type.Name.EndsWith("Type") ? type.Name.ToLower().Replace("type", "") : type.Name.ToLower();
                    nodeList = xml.SelectNodes(@"//*[" + LOCAL_NAME_IGNORE_CASE + "='" + typeName + "']");
                }
                else if (grandParentName == null)
                {
                    nodeList = xml.SelectNodes(@"//*[" + LOCAL_NAME_IGNORE_CASE + "='" + parentName.ToLower() + "']");
                }
                else
                {
                    nodeList = xml.SelectNodes(@"//*[" + LOCAL_NAME_IGNORE_CASE + "='" + grandParentName.ToLower() + "'][" + index + "]/*["
                                                    + LOCAL_NAME_IGNORE_CASE + "='" + parentName.ToLower() + "']");
                }

                IList nList = (IList)obj;
                foreach (XmlNode node in nodeList)
                {
                    if (node == null || !node.HasChildNodes) continue;

                    foreach (PropertyInfo property in properties)
                    {
                        if (property.PropertyType.IsPrimitive)
                        {
                            SetPrimitivePropertyValue(ref obj, xml, property);
                            continue;
                        }

                        nList = FillElementsOfAList(obj, property, xml, parentName, elementName, grandParentName, index);
                    }
                }
                obj = (T)nList;
            }
            else if (type.BaseType == typeof(object))
            {
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name.Contains("ExtensionData") || property.Name.Contains("Any")) continue;

                    if (property.PropertyType.IsPrimitive
                        || property.PropertyType.BaseType == typeof(object))
                    {
                        SetPrimitivePropertyValue(ref obj, xml, property);
                        continue;
                    }

                    if (property.PropertyType.BaseType == typeof(Array))
                    {
                        FillArrayPropertyFromXml(ref obj, property, xml);
                        continue;
                    }

                    object instance = Activator.CreateInstance(property.PropertyType);
                    FillPropertiesFromXml(ref instance, property.PropertyType, xml, parentName, elementName);
                    property.SetValue(obj, instance);
                }
            }
        }

        /// <summary>
        /// Fills the elements of a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="parentName">Name of the parent.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        internal static IList FillElementsOfAList<T>(T obj, PropertyInfo property, XmlDocument xml, string parentName, string elementName, string grandParentName, int index)
        {
            IList nList = (IList)obj;

            XmlNodeList nodeList = null;

            if (parentName == null)
            {
                nodeList = xml.SelectNodes(@"//*[" + LOCAL_NAME_IGNORE_CASE + "='" + property.PropertyType.Name.ToLower() + "']");
            }
            else if (grandParentName == null)
            {
                nodeList = xml.SelectNodes(@"//*[" + LOCAL_NAME_IGNORE_CASE + "='" + parentName.ToLower() + "']/*["
                                                + LOCAL_NAME_IGNORE_CASE + "='" + elementName.ToLower() + "']");
            }
            else
            {
                nodeList = xml.SelectNodes(@"//*[" + LOCAL_NAME_IGNORE_CASE + "='" + grandParentName.ToLower() + "'][" + (index % 100) + "]/*["
                                                + LOCAL_NAME_IGNORE_CASE + "='" + parentName.ToLower() + "']/*[" + LOCAL_NAME_IGNORE_CASE + "='" + elementName.ToLower() + "']");
            }

            if (nodeList.Count == 0) return nList;

            int count = index * 100;

            PropertyInfo[] properties = property.PropertyType.GetProperties();
            foreach (XmlNode node in nodeList)
            {
                object instance = Activator.CreateInstance(property.PropertyType);
                XmlNodeList nodeChildren = node.ChildNodes;
                count++;
                foreach (XmlNode nodeChild in nodeChildren)
                {
                    List<PropertyInfo> list = properties.Where(x => (x.PropertyType.Name.Equals(nodeChild.LocalName) || x.Name.Equals(nodeChild.LocalName))).ToList();

                    if (list.Count > 0 && list.First().PropertyType.BaseType.Name == typeof(List<>).Name && nodeChild.ChildNodes.Count >= 1)
                    {
                        object instanceList = Activator.CreateInstance(list.First().PropertyType);
                        FillListElementsFromXml(ref instanceList, list.First().PropertyType, xml, nodeChild.LocalName, nodeChild?.FirstChild.LocalName, nodeChild.ParentNode.LocalName, count);
                        list.First().SetValue(instance, instanceList);
                    }
                    else
                    {
                        string nodeValue = nodeChild.InnerText;
                        if (string.IsNullOrEmpty(nodeValue) || list.Count == 0) continue;
                        SetPropertyValue(ref instance, list.First(), nodeValue);
                    }
                }

                nList.Add(instance);
            }

            return nList;
        }

        /// <summary>
        /// Sets the primitive property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="property">The property.</param>
        private static void SetPrimitivePropertyValue<T>(ref T obj, XmlDocument xml, PropertyInfo property)
        {
            string nodeValue = SelectSingleNodeIgnoreCase(xml, property.Name)?.InnerXml;
            if (string.IsNullOrEmpty(nodeValue)) return;
            SetPropertyValue(ref obj, property, nodeValue);
        }

        /// <summary>
        /// Fills the array property from XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="xml">The XML.</param>
        private static void FillArrayPropertyFromXml<T>(ref T obj, PropertyInfo property, XmlDocument xml)
        {
            var instance = Activator.CreateInstance(property.PropertyType.GetElementType());
            Type type = property.PropertyType.GetElementType();

            XmlNodeList parentNodeList = xml.SelectNodes(@"//*[" + LOCAL_NAME_IGNORE_CASE + "='" + type.Name.ToLower() + "']");
            var baseType = typeof(List<>);
            var genericType = baseType.MakeGenericType(type);
            IList list = (IList)Activator.CreateInstance(genericType);

            foreach (XmlNode node in parentNodeList)
            {
                GetInstanceValue(ref instance, node, type, xml);
                list.Add(instance);
            }

            Array newArray = Array.CreateInstance(type, list.Count);
            list.CopyTo(newArray, 0);
            property.SetValue(obj, newArray);
        }

        /// <summary>
        /// Gets the element value from XML by name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elementValue">The element value.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="soapXml">The SOAP XML.</param>
        private static void GetElementValueFromXmlByName<T>(ref T elementValue, string nodeName, XmlDocument soapXml)
        {
            string nodeValue = SelectSingleNodeIgnoreCase(soapXml, nodeName)?.InnerText;

            if (string.IsNullOrEmpty(nodeValue)) return;

            Type t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            elementValue = (T)Convert.ChangeType(nodeValue, t);
        }

        /// <summary>
        /// Gets the instance value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="node">The node.</param>
        /// <param name="type">The type.</param>
        /// <param name="xml">The XML.</param>
        private static void GetInstanceValue<T>(ref T instance, XmlNode node, Type type, XmlDocument xml)
        {
            if (node != null
                && node.HasChildNodes // it can be a list of elements
                && node.FirstChild.HasChildNodes) // if it has two levels of children, it is a list of elements
            {
                FillListElementsFromXml(ref instance, type, xml);
            }
            else
            {
                FillPropertiesFromXml(ref instance, type, xml);
            }
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="nodeValue">The node value.</param>
        private static void SetPropertyValue<T>(ref T obj, PropertyInfo property, string nodeValue)
        {
            if (property.SetMethod == null) return;

            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(Nullable<Int32>))
            {
                property.SetValue(obj, Int32.Parse(nodeValue));
            }
            else if (property.PropertyType == typeof(long) || property.PropertyType == typeof(Nullable<Int64>))
            {
                property.SetValue(obj, Int64.Parse(nodeValue));
            }
            else if (property.PropertyType == typeof(byte[]))
            {
                property.SetValue(obj, Encoding.ASCII.GetBytes(nodeValue));
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(obj, nodeValue);
            }
            else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(Nullable<bool>))
            {
                property.SetValue(obj, (nodeValue == "true"));
            }
            // .. skipping Nullable<DateTime> and other special types
        }

        /// <summary>
        /// Creates the XML Document.
        /// </summary>
        /// <param name="pathFile">The path file.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">The current file is not found in the indicated directory: '" + pathToFile + "', please copy it in the correct directory</exception>
        public static XmlDocument GetXMLDocumentFromFile(string pathFile)
        {
            string pathToFile = BaseFileUtils.CURRENT_PATH + Path.DirectorySeparatorChar + pathFile;
            if (!File.Exists(pathToFile))
            {
                throw new FrameworkException("The current file is not found in the indicated directory: '" + pathToFile + "', please copy it in the correct directory");
            }

            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.Load(pathToFile);
            return soapEnvelopeDocument;
        }

        /// <summary>
        /// Gets the string element value.
        /// </summary>
        /// <param name="resultXml">The result XML.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public static string GetStringElementValueByName(XmlDocument resultXml, string elementName)
        {
            string value = string.Empty;
            GetElementValueFromXmlByName(ref value, elementName, resultXml);
            return value;
        }

        /// <summary>
        /// Gets the name of the list string element value by name.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="parentName">Name of the parent.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public static List<string> GetListStringElementValueByName(XmlDocument xml, string parentName, string elementName)
        {
            List<string> list = new List<string>();
            XmlNodeList nodes = xml.SelectNodes("//*[" + LOCAL_NAME_IGNORE_CASE + "='" + parentName.ToLower() + "']/*["
                                                        + LOCAL_NAME_IGNORE_CASE + "='" + elementName.ToLower() + "']");
            foreach (XmlNode node in nodes)
            {
                list.Add(node.InnerText);
            }

            return list;
        }
    }
}
