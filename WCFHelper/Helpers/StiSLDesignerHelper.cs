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

using System;
using System.Collections.Generic;
using System.Data;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Cloud.GoogleDocs;

namespace WCFHelper
{
    public static class StiSLDesignerHelper
    {
        #region Methods.Render
        public static string RenderReport(string xml, DataSet previewDataSet)
        {
            string result = null;
            if (!string.IsNullOrEmpty(xml))
            {
                var report = new StiReport();
                report.LoadFromString(StiSLEncodingHelper.DecodeString(xml));

                if (previewDataSet != null)
                    report.RegData("Demo", previewDataSet);

                if (!report.IsRendered)
                {
                    try
                    {
                        report.Compile();
                    }
                    catch
                    {
                    }

                    if (report.CompilerResults.Errors.Count > 0)
                        return StiSLRenderingReportHelper.GetErrorListXml(report);
                }

                bool error = false;
                try
                {
                    report.Render(false);
                }
                catch
                {
                    error = true;
                }

                if (!error)
                    result = StiSLRenderingReportHelper.CheckReportOnInteractions(report, true);
            }

            return result;
        }
        #endregion

        #region Methods.LoadConfiguration
        public static string LoadConfiguration()
        {
            var writer = new StiXmlWriter();
            writer.WriteStartElement("XmlResult");

            #region Databases
            var datas = StiConfig.Services.GetServices(typeof(StiDatabase));
            foreach (StiDatabase data in datas)
            {
                if (data is StiUndefinedDatabase || !data.ServiceEnabled) continue;
                writer.WriteStartElementAndSimpleEndElement(data.GetType().FullName);
            }
            #endregion

            writer.WriteEndElement();

            writer.IsEncodeString = true;
            string result = writer.ToString();
            writer = null;

            return result;
        }
        #endregion

        #region Methods.TestConnection
        public static string TestConnection(string xml)
        {
            var settings = StiDatabaseBuildHelper.Input.ParseTestConnection(xml);
            return StiSLEncodingHelper.EncodeString((settings.Adapter == null) 
                ? "Type is not found" 
                : settings.Adapter.TestConnection(settings.ConnectionString));
        }
        #endregion

        #region Methods.BuildObjects
        public static string BuildObjects(string xml)
        {
            var database = StiDatabaseBuildHelper.Input.ParseBuildObjects(xml);

            if (database != null)
            {
                var info = database.GetDatabaseInformation();
                database = null;
                if (info != null)
                {
                    return StiDatabaseBuildHelper.Output.ParseBuildObjects(info);
                }
            }

            return string.Empty;
        }
        #endregion

        #region Methods.RetrieveColumns
        public static string RetrieveColumns(string xml)
        {
            StiDataColumnsCollection columns = null;
            StiDataParametersCollection parameters = null;
            string result;

            try
            {
                var settingsRetrieveColumns = StiDatabaseBuildHelper.Input.ParseRetrieveColumns(xml);

                settingsRetrieveColumns.connection.ConnectionString = settingsRetrieveColumns.ConnectionString;
                var data = new StiData(settingsRetrieveColumns.Name, settingsRetrieveColumns.connection);

                settingsRetrieveColumns.dataSource.NameInSource = settingsRetrieveColumns.NameInSource;
                settingsRetrieveColumns.dataSource.Name = settingsRetrieveColumns.SqlSourceName;
                settingsRetrieveColumns.dataSource.Alias = settingsRetrieveColumns.SqlSourceAlias;
                settingsRetrieveColumns.dataSource.Type = settingsRetrieveColumns.SqlSourceType;
                settingsRetrieveColumns.dataSource.SqlCommand = settingsRetrieveColumns.SqlCommand;

                columns = settingsRetrieveColumns.adapter.GetColumnsFromData(data, settingsRetrieveColumns.dataSource);
                parameters = settingsRetrieveColumns.dataSource.Parameters;
            }
            finally
            {
                result = StiDatabaseBuildHelper.Output.ParseRetrieveColumns(columns, parameters);
            }

            return result;
        }
        #endregion

        #region Methods.GoogleDocs
        public static string GoogleDocsGetDocuments(string xml)
        {
            var result = StiSLGoogleDocsHelper.GetDocs(xml);

            List<string> docs = null;
            string error = null;
            var provider = new StiGoogleDocsProvider();

            try
            {
                provider.Login(result[0], result[1]);
                if (provider.IsLogged)
                {
                    docs = provider.GetDocsForSilverlight();
                    provider.Logout();
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            return StiSLGoogleDocsHelper.GetDocsResult(error, docs);
        }

        public static string GoogleDocsCreateCollection(string xml)
        {
            var helper = StiSLGoogleDocsHelper.CreateCollection(xml);
            var provider = new StiGoogleDocsProvider();
            StiDocumentEntry entry = null;
            string error = null;

            try
            {
                provider.Login(helper.Login, helper.Password);

                if (provider.IsLogged)
                {
                    StiDocumentEntry folder = null;
                    if (!string.IsNullOrEmpty(helper.ContentURL))
                    {
                        folder = new StiDocumentEntry
                                     {
                                         ContentURL = helper.ContentURL
                                     };
                    }

                    entry = provider.Create(folder, helper.title, true);
                    provider.Logout();
                }
            }
            catch (Exception ee)
            {
                error = ee.Message;
            }

            return StiSLGoogleDocsHelper.CreateCollectionResult(error, entry);
        }

        public static string GoogleDocsDelete(string xml)
        {
            var helper = StiSLGoogleDocsHelper.Delete(xml);
            var provider = new StiGoogleDocsProvider();
            string error = null;

            try
            {
                provider.Login(helper.Login, helper.Password);

                if (provider.IsLogged)
                {
                    provider.Delete(helper.doc);
                    provider.Logout();
                }
            }
            catch (Exception ee)
            {
                error = ee.Message;
            }

            return StiSLGoogleDocsHelper.DeleteResult(error);
        }

        public static string GoogleDocsOpen(string xml)
        {
            var helper = StiSLGoogleDocsHelper.Download(xml);
            var provider = new StiGoogleDocsProvider();
            string error = null;
            string content = null;

            try
            {
                provider.Login(helper.Login, helper.Password);

                if (provider.IsLogged)
                {
                    content = provider.Download(helper.doc);
                    provider.Logout();
                }
            }
            catch (Exception ee)
            {
                error = ee.Message;
            }

            return StiSLGoogleDocsHelper.DownloadResult(error, content);
        }

        public static string GoogleDocsSave(string xml)
        {
            var helper = StiSLGoogleDocsHelper.Upload(xml);
            var provider = new StiGoogleDocsProvider();
            string error = null;

            try
            {
                provider.Login(helper.Login, helper.Password);

                if (provider.IsLogged)
                {
                    provider.Upload(helper.document, helper.collection, helper.content, helper.title);
                    provider.Logout();
                }
            }
            catch (Exception ee)
            {
                error = ee.Message;
            }

            return StiSLGoogleDocsHelper.DeleteResult(error);
        }
        #endregion

        #region Methods.ReportScript
        public static string OpenReportScript(string xml)
        {
            var report = new StiReport();
            report.LoadFromString(StiSLEncodingHelper.DecodeString(xml));

            report.ScriptUnpack();
            string result = StiSLEncodingHelper.EncodeString(report.Script);
            report.Dispose();
            report = null;

            return result;
        }

        public static string SaveReportScript(string xml)
        {
            var report = StiSLRenderingReportHelper.ParseXmlSaveReportScript(xml);

            report.ScriptPack();
            string result = StiSLEncodingHelper.EncodeString(report.Script);
            report.Dispose();
            report = null;

            return result;
        }
        #endregion
    }
}