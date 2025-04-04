﻿using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace UniversalParser.NET.Tools;
///<summary>
/// XML is hard to parse, so I made this class to make it easier, but it doesnt work.
/// </summary>
    //For More Info And Example Please Visit https://github.com/HadrielWonda/UniversalParser

    public class XmlParser
    {
        /// <summary>
        /// Get Specified Attribute Value
        /// </summary>
        /// <param name="xmlString">Xml Text</param>
        /// <param name="element">Element Name</param>
        /// <param name="attribute">Attribute Name</param>
        /// <returns></returns>
        public string GetAttributeValue(string xmlString, string element, string attribute)
        {
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlString)))
            {
                reader.ReadToFollowing(element);
                reader.MoveToAttribute(attribute);
                return reader.Value;
            }
        }

        public string[] GetAttributesName(string xmlString, string element)
        {
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlString)))
            {
                Queue<string> res = new Queue<string>();

                reader.ReadToFollowing(element);
                while (reader.MoveToNextAttribute())
                {
                    res.Enqueue(reader.Name);
                }
                return res.ToArray();
            }
        }

        /// <summary>
        /// Get Specified Element Content
        /// </summary>
        /// <param name="xmlString">Xml Text</param>
        /// <param name="element">Element Name</param>
        /// <returns></returns>
        public string[] GetElementContent(string xmlString, string element)
        {
            //using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlString)))
            //{
            //    reader.ReadToFollowing(element);
            //    return reader.ReadElementContentAsString();
            //}

            Regex regex = new Regex(@"<\s*" + element + @".*>.*<\s*/\s*" + element + @"\s*>");
            var match = regex.Matches(xmlString);
            string[] res = new string[match.Count];

            for (int i = 0; i < res.Length; i++)
                res[i] = match[i].Value;

            return res;
        }

        public string GetElementContentByParent(string xmlString, string elementsParent, string element)
        {
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlString)))
            {
                reader.ReadToFollowing(elementsParent);
                reader.ReadToFollowing(element);
                return reader.ReadElementContentAsString();
            }
        }

        public async Task<string> GetElementContentByParentAsync(string xmlString, string elementsParent, string element)
        {
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlString), new XmlReaderSettings() { Async = true }))
            {
                reader.ReadToFollowing(elementsParent);
                reader.ReadToFollowing(element);
                return await reader.ReadElementContentAsStringAsync();
            }
        }

        /// <summary>
        /// Get All Elements In Xml Text
        /// </summary>
        /// <param name="xmlString">Xml Text</param>
        /// <returns></returns>
        public string[] GetAllElements(string xmlString)
        {
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlString)))
            {
                List<string> output = new List<string>();

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                        output.Add(reader.Name);
                }

                return output.ToArray();
            }
        }


        //public string[] GetAllElementsRecords(string xmlString)
        //{
        //    return GetAllElementsRecordsByName(xmlString, null);
        //}

        public string[] GetAllElementsRecordByName(string xmlString, string elementName)
        {
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlString)))
            {
                List<string> output = new List<string>();

                while (reader.Read())
                {
                    if (elementName != null)
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == elementName)
                            output.Add(reader.ReadOuterXml());

                        continue;
                    }

                    if (reader.NodeType == XmlNodeType.Element)
                        output.Add(reader.ReadOuterXml());
                }

                return output.ToArray();
            }
        }

        /// <summary>
        /// Get All Text In Xml Text
        /// </summary>
        /// <param name="xmlString">Xml Text</param>
        /// <returns></returns>
        public string[] GetAllText(string xmlString)
        {
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlString)))
            {
                List<string> output = new List<string>();

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Text)
                        output.Add(reader.Value);
                }

                return output.ToArray();
            }
        }

        public string[] GetElementText(string xmlString, string elementName)
        {
            string pattern = "<\\s*" + elementName + ".*>\\s*.*\\s*<\\s*/\\s*" + elementName + "\\s*>";

            Regex reg = new Regex(pattern);
            var temp = reg.Matches(xmlString);
            Queue<string> res = new Queue<string>();

            for (int i = 0; i < temp.Count; i++)
            {
                res.Enqueue(temp[i].Value.Substring(temp[i].Value.IndexOf('>') + 1, temp[i].Value.IndexOf('<', temp[i].Value.IndexOf('>')) - temp[i].Value.IndexOf('>') - 1));
            }

            return res.ToArray();
        }


        public string SetElementText(string xmlString, string elementName, string value)
        {
            string pattern = "<\\s*" + elementName + ".*>\\s*.*\\s*<\\s*/\\s*" + elementName + "\\s*>";

            Regex reg = new Regex(pattern);
            var temp = reg.Matches(xmlString);
            Queue<string> res = new Queue<string>();

            for (int i = 0; i < temp.Count; i++)
            {
                string text = (temp[i].Value.Substring(temp[i].Value.IndexOf('>') + 1, temp[i].Value.IndexOf('<', temp[i].Value.IndexOf('>')) - temp[i].Value.IndexOf('>') - 1));

                if (string.IsNullOrEmpty(text))
                {
                    xmlString = xmlString.Replace(temp[i].Value, temp[i].Value.Trim().Replace("><", ">" + value + "<"));
                    continue;
                }

                xmlString = xmlString.Replace(temp[i].Value, temp[i].Value.Replace(text, value));
            }

            return xmlString;
        }

        //public void SetAttribute(string xmlString, string parent, string element, string attributeName, string value)
        //{
        //    using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlString)))
        //    {
        //        if (!reader.ReadToFollowing(parent))
        //            return;

        //        SetAttribute(xmlString, element, attributeName, value,false);
        //    }
        //}

        /// <summary>
        /// Set Attribute Value In A Node With The Specified Parent
        /// </summary>
        /// <param name="xmlString">Xml Text</param>
        /// <param name="parent">Parent Name</param>
        /// <param name="element">Element Name</param>
        /// <param name="attributeName">Attribute Name</param>
        /// <param name="value">Attribute Value</param>
        /// <returns></returns>
        public string SetAttribute(string xmlString, string parent, string element, string attributeName, string value)
        {
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlString)))
            {
                using (XmlReader reader2 = XmlReader.Create(new System.IO.StringReader(xmlString)))
                {
                    string total = xmlSubTree(xmlString);
                    while (reader.ReadToFollowing(parent))
                    {
                        reader2.ReadToFollowing(parent);

                        var mainTree = reader.ReadSubtree();
                        var tempTree = reader2.ReadSubtree();

                        string[] orgStr = System.Text.RegularExpressions.Regex.Split(xmlSubTree(tempTree), System.Environment.NewLine);
                        string[] newStr = System.Text.RegularExpressions.Regex.Split(SetAttribute(mainTree, element, attributeName, value, true), System.Environment.NewLine);

                        for (int i = 0; i < orgStr.Length; i++)
                        {
                            total = total.Replace(orgStr[i], newStr[i]);
                        }

                        //if (!allElements)
                        //    return total;
                    }
                    return total;
                }
            }
        }

        /// <summary>
        /// Set Attribute Value In A Node
        /// </summary>
        /// <param name="xmlString">Xml Text</param>
        /// <param name="element">Element Name</param>
        /// <param name="attributeName">Attribute Name</param>
        /// <param name="value">Attribute Value</param>
        /// <param name="allElements">Wethere Set ALL Attribute Value With Specified Name Or Only First Attribute
        /// :True Set All Attribute Value With Specified Name In Xml Text. :False Set Only First Attribute With Specified Name In Xml Text</param>
        /// <returns></returns>
        public string SetAttribute(string xmlString, string element, string attributeName, string value, bool allElements)
        {
            StringBuilder output = new StringBuilder();

            bool loop = true;

            // Create an XmlReader
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlString)))
            {
                XmlWriterSettings ws = new XmlWriterSettings();
                ws.Indent = true;
                using (XmlWriter writer = XmlWriter.Create(output, ws))
                {

                    // Parse the file and display each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                writer.WriteStartElement(reader.Name);

                                if (reader.Name == element && loop)
                                {
                                    UpdateAttribute(reader, writer, attributeName, value);

                                    if (!allElements)
                                        loop = false;

                                    break;
                                }

                                if (reader.HasAttributes)
                                    writer.WriteAttributes(reader, true);

                                break;
                            case XmlNodeType.Text:
                                writer.WriteString(reader.Value);
                                break;
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                break;
                            case XmlNodeType.Comment:
                                writer.WriteComment(reader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                writer.WriteFullEndElement();
                                break;
                        }
                    }

                }
            }
            return output.ToString();
        }

        /// <summary>
        /// Set Attribute Value In A Node
        /// </summary>
        /// <param name="xmlString">Xml Text</param>
        /// <param name="element">Element Name</param>
        /// <param name="attributeName">Attribute Name</param>
        /// <param name="value">Attribute Value</param>
        /// <param name="allElements">Wethere Set ALL Attribute Value With Specified Name Or Only First Attribute
        /// True Set All Attribute Value With Specified Name In Xml Text.
        /// False Set Only First Attribute With Specified Name In Xml Text</param>
        /// <returns></returns>
        public string SetAttribute(XmlReader reader, string element, string attributeName, string value, bool allElements)
        {
            StringBuilder output = new StringBuilder();

            bool loop = true;

            // Create an XmlReader
            {
                XmlWriterSettings ws = new XmlWriterSettings();
                ws.Indent = true;
                using (XmlWriter writer = XmlWriter.Create(output, ws))
                {

                    // Parse the file and display each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                writer.WriteStartElement(reader.Name);

                                if (reader.Name == element && loop)
                                {
                                    UpdateAttribute(reader, writer, attributeName, value);

                                    if (!allElements)
                                        loop = false;

                                    break;
                                }

                                if (reader.HasAttributes)
                                    writer.WriteAttributes(reader, true);

                                break;
                            case XmlNodeType.Text:
                                writer.WriteString(reader.Value);
                                break;
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                break;
                            case XmlNodeType.Comment:
                                writer.WriteComment(reader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                writer.WriteFullEndElement();
                                break;
                        }
                    }

                }
            }
            return output.ToString();
        }

        /// <summary>
        /// Check Wethere Exsist This Attribute Name In Xml Text.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public bool ISExsistAttribute(XmlReader reader, string attributeName)
        {
            if (reader.HasAttributes)
            {

                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == attributeName)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Update Specified Attribute Value
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <param name="attributeName"></param>
        /// <param name="value"></param>
        public void UpdateAttribute(XmlReader reader, XmlWriter writer, string attributeName, string value)
        {
            bool flag = false;

            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == attributeName)
                    {
                        writer.WriteAttributeString(null, attributeName, null, value);
                        flag = true;
                    }
                    else
                        writer.WriteAttributeString(null, reader.Name, null, reader.Value);
                }
            }

            if (!flag)
                writer.WriteAttributeString(null, attributeName, null, value);
        }

        /// <summary>
        /// Get SubTree
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public string xmlSubTree(string xmlString)
        {
            StringBuilder output = new StringBuilder();
            //Create an XmlReader
            using (XmlReader temp = XmlReader.Create(new System.IO.StringReader(xmlString)))
            {
                XmlWriterSettings ws = new XmlWriterSettings();
                ws.Indent = true;

                using (XmlWriter writer = XmlWriter.Create(output, ws))
                {

                    //Parse the file and display each of the nodes.
                    while (temp.Read())
                    {
                        switch (temp.NodeType)
                        {
                            case XmlNodeType.Element:
                                writer.WriteStartElement(temp.Name);

                                if (temp.HasAttributes)
                                    writer.WriteAttributes(temp, true);

                                break;
                            case XmlNodeType.Text:
                                writer.WriteString(temp.Value);
                                break;
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                writer.WriteProcessingInstruction(temp.Name, temp.Value);
                                break;
                            case XmlNodeType.Comment:
                                writer.WriteComment(temp.Value);
                                break;
                            case XmlNodeType.EndElement:
                                writer.WriteFullEndElement();
                                break;
                        }
                    }

                }
            }
            return output.ToString();

        }

        public string xmlSubTree(XmlReader temp)
        {
            StringBuilder output = new StringBuilder();
            // Create an XmlReader
            {
                XmlWriterSettings ws = new XmlWriterSettings();
                ws.Indent = true;

                using (XmlWriter writer = XmlWriter.Create(output, ws))
                {

                    // Parse the file and display each of the nodes.
                    while (temp.Read())
                    {
                        switch (temp.NodeType)
                        {
                            case XmlNodeType.Element:
                                writer.WriteStartElement(temp.Name);

                                if (temp.HasAttributes)
                                    writer.WriteAttributes(temp, true);

                                break;
                            case XmlNodeType.Text:
                                writer.WriteString(temp.Value);
                                break;
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                writer.WriteProcessingInstruction(temp.Name, temp.Value);
                                break;
                            case XmlNodeType.Comment:
                                writer.WriteComment(temp.Value);
                                break;
                            case XmlNodeType.EndElement:
                                writer.WriteFullEndElement();
                                break;
                        }
                    }

                }
            }
            return output.ToString();

        }

        public XmlNode[] GetSubTreeByParent(string data, string parent)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            return GetSubTreeByParent(doc, parent);
        }

        public XmlNode[] GetSubTreeByParent(XmlDocument doc, string parent)
        {
            List<XmlNode> queue = new List<XmlNode>();
            for (int i = 0; i < doc.ChildNodes.Count; i++)
            {
                if (doc.ChildNodes[i].Name == parent)
                {
                    queue.Add(doc.ChildNodes[i]);
                }
                else
                {
                    queue.AddRange(GetSubTreeByParent(doc.ChildNodes[i], parent));
                }
            }

            return queue.ToArray();
        }

        public XmlNode[] GetSubTreeByParent(XmlNode doc, string parent)
        {
            List<XmlNode> queue = new List<XmlNode>();
            for (int i = 0; i < doc.ChildNodes.Count; i++)
            {
                if (doc.ChildNodes[i].Name == parent)
                {
                    queue.Add(doc.ChildNodes[i]);
                }
                else
                {
                    queue.AddRange(GetSubTreeByParent(doc.ChildNodes[i], parent));
                }
            }

            return queue.ToArray();
        }

        public byte[] Serialize<T>(T objetType)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (XmlWriter writer = XmlWriter.Create(ms, settings))
            {
                serializer.Serialize(writer, objetType, null);
            }

            return ms.ToArray();
        }

        public T Deserialize<T>(byte[] data)
        {
            T myObject;

            System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            System.IO.MemoryStream ms = new System.IO.MemoryStream(data);

            myObject = (T)
            mySerializer.Deserialize(ms);

            return myObject;
        }
    }

