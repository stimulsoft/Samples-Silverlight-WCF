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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;

using Stimulsoft.Report.Dictionary;
using System.Text;

namespace WCFHelper
{
    internal static class StiDatabaseBuildHelper
    {
        #region class SettingsTestConnection
        public sealed class SettingsTestConnection
        {
            public string ConnectionString;
            public StiSqlAdapterService Adapter;
        }
        #endregion

        #region class SettingsRetrieveColumns
        public sealed class SettingsRetrieveColumns
        {
            public string Name;
            public string Alias;
            public string NameInSource;
            public string ConnectionString;
            public string PromptUserNameAndPassword;
            public string SqlCommand;

            public string SqlSourceName;
            public string SqlSourceAlias;
            public StiSqlSourceType SqlSourceType;

            public StiSqlAdapterService adapter = null;
            public StiSqlSource dataSource;
            public DbConnection connection;
        }
        #endregion

        #region Input
        public static class Input
        {
            public static SettingsTestConnection ParseTestConnection(string xml)
            {
                var settings = new SettingsTestConnection();

                var stringReader = new System.IO.StringReader(StiSLEncodingHelper.DecodeString(xml));
                var tr = new XmlTextReader(stringReader);

                tr.Read();
                if (tr.Name == "XmlResult")
                {
                    while (tr.Read())
                    {
                        if (tr.Depth == 0)
                            break;

                        switch (tr.Name)
                        {
                            case "TypeAdapter":
                                string typeStr = tr.ReadString();
                                if (!string.IsNullOrEmpty(typeStr))
                                    settings.Adapter = Stimulsoft.Base.StiActivator.CreateObject(typeStr) as StiSqlAdapterService;
                                break;

                            case "ConnectionString":
                                settings.ConnectionString = tr.ReadString();
                                break;
                        }
                    }
                }

                tr.Close();
                stringReader.Close();
                stringReader.Dispose();
                tr = null;
                stringReader = null;

                return settings;
            }

            #region BuildObjects
            public static StiDatabase ParseBuildObjects(string xml)
            {
                StiDatabase result = null;
                var stringReader = new System.IO.StringReader(StiSLEncodingHelper.DecodeString(xml));
                var tr = new XmlTextReader(stringReader);

                tr.Read();
                if (tr.Name == "XmlResult")
                {
                    while (tr.Read())
                    {
                        if (tr.Depth == 0)
                            break;

                        result = Stimulsoft.Base.StiActivator.CreateObject(tr.ReadString()) as StiDatabase;
                        if (result == null) break;

                        if (result is StiXmlDatabase)
                        {
                            result = GetXmlDatabase(ref tr);
                        }
                        else if (result is StiSqlDatabase)
                        {
                            result = GetSqlDatabase(ref tr, result as StiSqlDatabase);
                        }

                        break;
                    }
                }

                tr.Close();
                stringReader.Close();
                stringReader.Dispose();
                tr = null;
                stringReader = null;

                return result;
            }

            private static StiDatabase GetXmlDatabase(ref XmlTextReader tr)
            {
                var xmlDatabase = new StiXmlDatabase();

                while (tr.Read())
                {
                    if (tr.Depth != 1)
                        break;

                    switch (tr.Name)
                    {
                        case "Name":
                            xmlDatabase.Name = tr.ReadString();
                            break;

                        case "Alias":
                            xmlDatabase.Alias = tr.ReadString();
                            break;

                        case "PathData":
                            xmlDatabase.PathData = tr.ReadString();
                            break;

                        case "PathSchema":
                            xmlDatabase.PathSchema = tr.ReadString();
                            break;
                    }
                }

                return xmlDatabase;
            }

            private static StiDatabase GetSqlDatabase(ref XmlTextReader tr, StiSqlDatabase database)
            {
                StiSqlDatabase sqlDatabase = database;

                while (tr.Read())
                {
                    if (tr.Depth != 1)
                        break;

                    switch (tr.Name)
                    {
                        case "Name":
                            sqlDatabase.Name = tr.ReadString();
                            break;

                        case "Alias":
                            sqlDatabase.Alias = tr.ReadString();
                            break;

                        case "ConnectionString":
                            sqlDatabase.ConnectionString = tr.ReadString();
                            break;

                        case "PromptUserNameAndPassword":
                            sqlDatabase.PromptUserNameAndPassword = (tr.ReadString() == "1");
                            break;
                    }
                }

                return sqlDatabase;
            }
            #endregion

            public static SettingsRetrieveColumns ParseRetrieveColumns(string xml)
            {
                var settings = new SettingsRetrieveColumns();
                var stringReader = new System.IO.StringReader(StiSLEncodingHelper.DecodeString(xml));
                var tr = new XmlTextReader(stringReader);

                tr.Read();
                if (tr.Name == "XmlResult")
                {
                    while (tr.Read())
                    {
                        if (tr.Depth == 0)
                            break;

                        switch (tr.Name)
                        {
                            case "DataAdapterType":
                                {
                                    string typeStr = tr.ReadString();
                                    settings.adapter =
                                        Stimulsoft.Base.StiActivator.CreateObject(typeStr) as StiSqlAdapterService;
                                    settings.dataSource =
                                        Stimulsoft.Base.StiActivator.CreateObject(settings.adapter.GetDataSourceType())
                                        as StiSqlSource;

                                    switch (settings.adapter.GetType().Name)
                                    {
                                        case "StiSqlAdapterService":
                                            settings.connection = new SqlConnection();
                                            break;
                                        case "StiOleDbAdapterService":
                                            settings.connection = new System.Data.OleDb.OleDbConnection();
                                            break;
                                        case "StiOdbcAdapterService":
                                            settings.connection = new System.Data.Odbc.OdbcConnection();
                                            break;
                                        case "StiMSAccessAdapterService":
                                            settings.connection = new System.Data.OleDb.OleDbConnection();
                                            break;
                                        case "StiDB2AdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject("IBM.Data.DB2.DB2Connection")
                                                as DbConnection;
                                            break;
                                        case "StiDotConnectUniversalAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "Devart.Data.Universal.UniConnection") as DbConnection;
                                            break;
                                        case "StiEffiProzAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "System.Data.EffiProz.EfzConnection") as DbConnection;
                                            break;
                                        case "StiFirebirdAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "FirebirdSql.Data.FirebirdClient.FbConnection") as DbConnection;
                                            break;
                                        case "StiInformixAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "IBM.Data.Informix.IfxConnection") as DbConnection;
                                            break;
                                        case "StiMySqlAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "MySql.Data.MySqlClient.MySqlConnection") as DbConnection;
                                            break;
                                        case "StiOracleAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "System.Data.OracleClient.OracleConnection") as DbConnection;
                                            break;
                                        case "StiOracleODPAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "Oracle.DataAccess.Client.OracleConnection") as DbConnection;
                                            break;
                                        case "StiPostgreSQLAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject("Npgsql.NpgsqlConnection") as
                                                DbConnection;
                                            break;
                                        case "StiSqlCeAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "System.Data.SqlServerCe.SqlCeConnection") as DbConnection;
                                            break;
                                        case "StiSQLiteAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "System.Data.SQLite.SQLiteConnection") as DbConnection;
                                            break;
                                        case "StiSybaseAdsAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "Advantage.Data.Provider.AdsConnection") as DbConnection;
                                            break;
                                        case "StiSybaseAseAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "Sybase.Data.AseClient.AseConnection") as DbConnection;
                                            break;
                                        case "StiUniDirectAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "CoreLab.UniDirect.UniConnection") as DbConnection;
                                            break;
                                        case "StiVistaDBAdapterService":
                                            settings.connection =
                                                Stimulsoft.Base.StiActivator.CreateObject(
                                                    "VistaDB.Provider.VistaDBConnection") as DbConnection;
                                            break;
                                    }
                                }
                                break;

                            case "Name":
                                settings.Name = tr.ReadString();
                                break;

                            case "Alias":
                                settings.Alias = tr.ReadString();
                                break;

                            case "NameInSource":
                                settings.NameInSource = tr.ReadString();
                                break;

                            case "ConnectionString":
                                settings.ConnectionString = tr.ReadString();
                                break;

                            case "PromptUserNameAndPassword":
                                settings.PromptUserNameAndPassword = tr.ReadString();
                                break;

                            case "SqlCommand":
                                settings.SqlCommand = tr.ReadString();
                                break;

                            case "SqlSourceName":
                                settings.SqlSourceName = tr.ReadString();
                                break;

                            case "SqlSourceAlias":
                                settings.SqlSourceAlias = tr.ReadString();
                                break;

                            case "SqlSourceType":
                                settings.SqlSourceType = (tr.ReadString() == "0") 
                                    ? StiSqlSourceType.Table
                                    : StiSqlSourceType.StoredProcedure;
                                break;
                        }
                    }
                }

                tr.Close();
                stringReader.Close();
                stringReader.Dispose();
                tr = null;
                stringReader = null;

                return settings;
            }
        }
        #endregion

        #region Output
        public static class Output
        {
            public static string ParseBuildObjects(StiDatabaseInformation info)
            {
                var str = new System.IO.StringWriter();
                var writer = new XmlTextWriter(str);
                writer.WriteStartElement("Result");

                #region Tables
                if (info.Tables.Count > 0)
                {
                    writer.WriteStartElement("Tables");
                    foreach (var table in info.Tables)
                    {
                        string tableName = CheckName(table.TableName);
                        writer.WriteStartElement(tableName);

                        foreach (DataColumn column in table.Columns)
                        {
                            writer.WriteStartElement(CheckName(column.ColumnName));
                            writer.WriteValue(column.DataType.ToString());
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                #endregion

                #region Views
                if (info.Views.Count > 0)
                {
                    writer.WriteStartElement("Views");
                    foreach (DataTable table in info.Views)
                    {
                        string tableName = CheckName(table.TableName);
                        writer.WriteStartElement(tableName);

                        foreach (DataColumn column in table.Columns)
                        {
                            writer.WriteStartElement(CheckName(column.ColumnName));
                            writer.WriteValue(column.DataType.ToString());
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                #endregion

                #region StoredProcedures
                if (info.StoredProcedures.Count > 0)
                {
                    writer.WriteStartElement("StoredProcedures");
                    foreach (var table in info.StoredProcedures)
                    {
                        if (table.TableName.IndexOfAny(new char[] { '~', '(', ')' }) != -1) continue;

                        string tableName = CheckName(table.TableName);
                        writer.WriteStartElement(tableName);

                        foreach (DataColumn column in table.Columns)
                        {
                            writer.WriteStartElement(CheckName(column.ColumnName));
                            writer.WriteValue(column.DataType.ToString());
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                #endregion

                writer.WriteEndElement();
                string result = StiSLEncodingHelper.EncodeString(str.ToString());

                writer = null;
                str.Dispose();
                str = null;

                return result;
            }

            public static string ParseRetrieveColumns(StiDataColumnsCollection columns, StiDataParametersCollection parameters)
            {
                var str = new System.IO.StringWriter();
                var writer = new XmlTextWriter(str);
                writer.WriteStartElement("Result");

                #region Columns

                if (columns != null && columns.Count > 0)
                {
                    writer.WriteStartElement("Columns");
                    foreach (StiDataColumn column in columns)
                    {
                        string columnName = CheckName(column.Name);

                        writer.WriteStartElement(columnName);
                        writer.WriteValue(column.Type.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                #endregion

                #region Parameters

                if (parameters != null && parameters.Count > 0)
                {
                    writer.WriteStartElement("Parameters");
                    foreach (StiDataParameter parameter in parameters)
                    {
                        string parameterName = CheckName(parameter.Name);

                        writer.WriteStartElement(parameterName);

                        writer.WriteStartElement("Type");
                        writer.WriteValue(parameter.Type.ToString());
                        writer.WriteEndElement();

                        writer.WriteStartElement("Size");
                        writer.WriteValue(parameter.Size.ToString());
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                #endregion

                writer.WriteEndElement();
                string result = StiSLEncodingHelper.EncodeString(str.ToString());

                writer = null;
                str.Dispose();
                str = null;

                return result;
            }
        }
        #endregion

        #region Methods.Helpers
        public static string CheckName(string name)
        {
            var builder = new StringBuilder(name);
            builder.Replace(" ", "_x0020_");
            builder.Replace("@", "_x0040_");
            builder.Replace("~", "_x007e_");
            builder.Replace("$", "_x0024_");
            builder.Replace("#", "_x0023_");
            builder.Replace("%", "_x0025_");
            builder.Replace("&", "_x0026_");
            builder.Replace("*", "_x002A_");
            builder.Replace("^", "_x005E_");
            builder.Replace("(", "_x0028_");
            builder.Replace(")", "_x0029_");
            builder.Replace("!", "_x0021_");

            return builder.ToString();
        }
        #endregion
    }
}