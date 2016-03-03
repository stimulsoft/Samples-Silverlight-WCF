#region Copyright (C) 2003-2013 Stimulsoft
/*
{*******************************************************************}
{																	}
{	Stimulsoft Reports.SL											}
{	                         										}
{																	}
{	Copyright (C) 2003-2013 Stimulsoft     							}
{	ALL RIGHTS RESERVED												}
{																	}
{	The entire contents of this file is protected by U.S. and		}
{	International Copyright Laws. Unauthorized reproduction,		}
{	reverse-engineering, and distribution of all or any portion of	}
{	the code contained in this file is strictly prohibited and may	}
{	result in severe civil and criminal penalties and will be		}
{	prosecuted to the maximum extent possible under the law.		}
{																	}
{	RESTRICTIONS													}
{																	}
{	THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES			}
{	ARE CONFIDENTIAL AND PROPRIETARY								}
{	TRADE SECRETS OF Stimulsoft										}
{																	}
{	CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON		}
{	ADDITIONAL RESTRICTIONS.										}
{																	}
{*******************************************************************}
*/
#endregion Copyright (C) 2003-2013 Stimulsoft

using System.Xml;
using Stimulsoft.Cloud.GoogleDocs;

namespace WCFHelper
{
    public static class StiSLGoogleDocsHelper
    {
        #region GetDocs
        public static string[] GetDocs(string xml)
        {
            var result = new string[2];
            var stringReader = new System.IO.StringReader(StiSLEncodingHelper.DecodeString(xml));
            var tr = new XmlTextReader(stringReader);

            tr.Read();
            if (tr.Name == "GoogleDocs")
            {
                while (tr.Read())
                {
                    if (tr.Depth == 0)
                        break;

                    switch (tr.Name)
                    {
                        case "Login":
                            result[0] = tr.ReadString();
                            break;

                        case "Password":
                            result[1] = tr.ReadString();
                            break;
                    }
                }
            }

            tr.Close();
            stringReader.Close();
            stringReader.Dispose();
            tr = null;
            stringReader = null;

            return result;
        }

        public static string GetDocsResult(string error, System.Collections.Generic.List<string> docs)
        {
            var str = new System.IO.StringWriter();
            var writer = new XmlTextWriter(str);
            writer.WriteStartElement("GoogleDocs");

            if (error != null)
            {
                writer.WriteStartElement("Error");
                writer.WriteString(StiSLEncodingHelper.EncodeString(error));
                writer.WriteEndElement();
            }
            else
            {
                if (docs != null)
                {
                    for (int index = 0; index < docs.Count; index++)
                    {
                        writer.WriteStartElement("Value");
                        writer.WriteString(StiSLEncodingHelper.EncodeString(docs[index]));
                        writer.WriteEndElement();
                    }
                }
            }

            writer.WriteEndElement();
            string result = str.ToString();

            writer = null;
            str.Dispose();
            str = null;

            return result;
        }
        #endregion

        #region CreateCollection
        public static StiGoogleDocsCreateCollectionHelper CreateCollection(string xml)
        {
            var stringReader = new System.IO.StringReader(StiSLEncodingHelper.DecodeString(xml));
            var tr = new XmlTextReader(stringReader);

            var helper = new StiGoogleDocsCreateCollectionHelper();
            tr.Read();
            if (tr.Name == "GoogleDocs")
            {
                while (tr.Read())
                {
                    if (tr.Depth == 0)
                        break;

                    switch (tr.Name)
                    {
                        case "Login":
                            helper.Login = tr.ReadString();
                            break;

                        case "Password":
                            helper.Password = tr.ReadString();
                            break;

                        case "ContentURL":
                            helper.ContentURL = tr.ReadString();
                            break;

                        case "title":
                            helper.title = tr.ReadString();
                            break;
                    }
                }
            }

            tr.Close();
            stringReader.Close();
            stringReader.Dispose();
            tr = null;
            stringReader = null;

            return helper;
        }

        public static string CreateCollectionResult(string error, StiDocumentEntry entry)
        {
            var str = new System.IO.StringWriter();
            var writer = new XmlTextWriter(str);
            writer.WriteStartElement("GoogleDocs");

            if (error != null)
            {
                writer.WriteStartElement("Error");
                writer.WriteString(error);
                writer.WriteEndElement();
            }
            else
            {
                if (entry != null && entry.ResourceId != null)
                {
                    writer.WriteStartElement("ResourceId");
                    writer.WriteString(entry.ResourceId);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            string result = StiSLEncodingHelper.EncodeString(str.ToString());

            writer = null;
            str.Dispose();
            str = null;

            return result;
        }
        #endregion

        #region Delete
        public static StiGoogleDocsDeleteHelper Delete(string xml)
        {
            var stringReader = new System.IO.StringReader(StiSLEncodingHelper.DecodeString(xml));
            var tr = new XmlTextReader(stringReader);

            var helper = new StiGoogleDocsDeleteHelper();
            tr.Read();
            if (tr.Name == "GoogleDocs")
            {
                while (tr.Read())
                {
                    if (tr.Depth == 0)
                        break;

                    switch (tr.Name)
                    {
                        case "Login":
                            helper.Login = tr.ReadString();
                            break;

                        case "Password":
                            helper.Password = tr.ReadString();
                            break;

                        case "Title":
                            helper.doc.Title = tr.ReadString();
                            break;

                        case "EditURL":
                            helper.doc.EditURL = tr.ReadString();
                            break;

                        case "SelfURL":
                            helper.doc.SelfURL = tr.ReadString();
                            break;
                    }
                }
            }

            tr.Close();
            stringReader.Close();
            stringReader.Dispose();
            tr = null;
            stringReader = null;

            return helper;
        }

        public static string DeleteResult(string error)
        {
            var str = new System.IO.StringWriter();
            var writer = new XmlTextWriter(str);
            writer.WriteStartElement("GoogleDocs");

            if (error != null)
            {
                writer.WriteStartElement("Error");
                writer.WriteString(error);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            string result = StiSLEncodingHelper.EncodeString(str.ToString());

            writer = null;
            str.Dispose();
            str = null;

            return result;
        }
        #endregion

        #region Download
        public static StiGoogleDocsDeleteHelper Download(string xml)
        {
            var stringReader = new System.IO.StringReader(StiSLEncodingHelper.DecodeString(xml));
            var tr = new XmlTextReader(stringReader);

            var helper = new StiGoogleDocsDeleteHelper();
            tr.Read();
            if (tr.Name == "GoogleDocs")
            {
                while (tr.Read())
                {
                    if (tr.Depth == 0)
                        break;

                    switch (tr.Name)
                    {
                        case "Login":
                            helper.Login = tr.ReadString();
                            break;

                        case "Password":
                            helper.Password = tr.ReadString();
                            break;

                        case "Title":
                            helper.doc.Title = tr.ReadString();
                            break;

                        case "ContentType":
                            helper.doc.ContentType = tr.ReadString();
                            break;

                        case "ContentURL":
                            helper.doc.ContentURL = tr.ReadString();
                            break;
                    }
                }
            }

            tr.Close();
            stringReader.Close();
            stringReader.Dispose();
            tr = null;
            stringReader = null;

            return helper;
        }

        public static string DownloadResult(string error, string content)
        {
            var str = new System.IO.StringWriter();
            var writer = new XmlTextWriter(str);
            writer.WriteStartElement("GoogleDocs");

            if (error != null)
            {
                writer.WriteStartElement("Error");
                writer.WriteString(error);
                writer.WriteEndElement();
            }
            else
            {
                if (content != null)
                {
                    writer.WriteStartElement("Content");
                    writer.WriteString(content);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            string result = StiSLEncodingHelper.EncodeString(str.ToString());

            writer.Close();
            str.Close();
            str.Dispose();
            str = null;
            writer = null;

            return result;
        }
        #endregion

        #region Upload
        public static StiGoogleDocsUploadHelper Upload(string xml)
        {
            var stringReader = new System.IO.StringReader(StiSLEncodingHelper.DecodeString(xml));
            var tr = new XmlTextReader(stringReader);

            var helper = new StiGoogleDocsUploadHelper();
            tr.Read();
            if (tr.Name == "GoogleDocs")
            {
                while (tr.Read())
                {
                    if (tr.Depth == 0)
                        break;

                    switch (tr.Name)
                    {
                        case "Login":
                            helper.Login = tr.ReadString();
                            break;

                        case "Password":
                            helper.Password = tr.ReadString();
                            break;

                        case "Title":
                            CheckDocument(helper);
                            helper.document.Title = tr.ReadString();
                            break;

                        case "ResourceId":
                            CheckDocument(helper);
                            helper.document.ResourceId = tr.ReadString();
                            break;

                        case "EditURL":
                            CheckDocument(helper);
                            helper.document.EditURL = tr.ReadString();
                            break;

                        case "SelfURL":
                            CheckDocument(helper);
                            helper.document.SelfURL = tr.ReadString();
                            break;

                        case "EditMediaURL":
                            CheckDocument(helper);
                            helper.document.EditMediaURL = tr.ReadString();
                            break;

                        case "ContentURL":
                            CheckCollection(helper);
                            helper.collection.ContentURL = tr.ReadString();
                            break;

                        case "title":
                            helper.title = tr.ReadString();
                            break;

                        case "content":
                            helper.content = tr.ReadString();
                            break;
                    }
                }
            }

            tr.Close();
            stringReader.Close();
            stringReader.Dispose();
            tr = null;
            stringReader = null;

            return helper;
        }

        private static void CheckDocument(StiGoogleDocsUploadHelper helper)
        {
            if (helper.document == null)
                helper.document = new StiDocumentEntry();
        }

        private static void CheckCollection(StiGoogleDocsUploadHelper helper)
        {
            if (helper.collection == null)
                helper.collection = new StiDocumentEntry();
        }
        #endregion

        #region Additional classes
        public sealed class StiGoogleDocsCreateCollectionHelper
        {
            public string Login;
            public string Password;
            public string ContentURL;
            public string title;
        }

        public sealed class StiGoogleDocsDeleteHelper
        {
            public string Login;
            public string Password;
            public StiDocumentEntry doc = new StiDocumentEntry();
        }

        public sealed class StiGoogleDocsUploadHelper
        {
            public string Login;
            public string Password;
            public StiDocumentEntry document;
            public StiDocumentEntry collection;
            public string content;
            public string title;
        }
        #endregion
    }
}