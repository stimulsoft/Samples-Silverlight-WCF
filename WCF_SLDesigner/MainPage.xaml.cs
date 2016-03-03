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

using System.IO;
using System.Windows;
using System.Windows.Controls;

using Stimulsoft.Base.Localization;
using Stimulsoft.Report;
using Stimulsoft.Report.SLDesign;
using Stimulsoft.Report.Viewer;
using Stimulsoft.Cloud.GoogleDocs;

namespace WCF_SLDesigner
{
    public partial class MainPage : UserControl
    {
        #region Fields
        private IStiTestConnecting testConnecting;
        private StiSelectDataWindow selectDataWindow;
        private StiDataStoreSourceEditWindow dataStoreSourceEditWindow;
        private StiSLViewerControl viewer;
        #endregion

        #region Handlers
        #region RenderReport
        private void WCFService_WCFRenderReport(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            designer.StartProgressInformation("WCF Service", StiLocalization.Get("DesignerFx", "CompilingReport"), Visibility.Visible, true);

            ServiceReference1.DesignerServiceClient service = new ServiceReference1.DesignerServiceClient();
            service.RenderReportCompleted += service_RenderReportCompleted;
            service.RenderReportAsync(e.Xml);
        }

        private void service_RenderReportCompleted(object sender, ServiceReference1.RenderReportCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null && e.Result.Length > 2)
            {
                designer.ApplyRenderedReport(e.Result);
            }

            designer.CloseProgressInformation();
        }
        #endregion

        #region TestConnection
        private void WCFService_WCFTestConnection(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            testConnecting = (IStiTestConnecting)sender;
            testConnecting.ShowProgressBar();

            var service = new ServiceReference1.DesignerServiceClient();
            service.TestConnectionCompleted += service_TestConnectionCompleted;
            service.TestConnectionAsync(e.Xml);
        }

        private void service_TestConnectionCompleted(object sender, ServiceReference1.TestConnectionCompletedEventArgs e)
        {
            testConnecting.HideProgressBar();

            if (e.Error == null && !testConnecting.IsBreakedProgressInformation)
            {
                testConnecting.ApplyResultAfterTestConnection(e.Result);
            }
            testConnecting = null;
        }
        #endregion

        #region BuildObjects
        private void WCFService_WCFBuildObjects(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            selectDataWindow = sender as StiSelectDataWindow;
            selectDataWindow.ShowProgressBar();

            var service = new ServiceReference1.DesignerServiceClient();
            service.BuildObjectsCompleted += service_BuildObjectsCompleted;
            service.BuildObjectsAsync(e.Xml);
        }

        private void service_BuildObjectsCompleted(object sender, ServiceReference1.BuildObjectsCompletedEventArgs e)
        {
            if (e.Error == null && !selectDataWindow.IsBreakedProgressInformation)
            {
                selectDataWindow.ApplyResultAfterBuildObjects(e.Result);
            }

            selectDataWindow.HideProgressBar();
            selectDataWindow = null;
        }
        #endregion

        #region RetrieveColumns
        private void WCFService_WCFRetrieveColumns(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            dataStoreSourceEditWindow = sender as StiDataStoreSourceEditWindow;
            dataStoreSourceEditWindow.ShowProgressBar();

            var service = new ServiceReference1.DesignerServiceClient();
            service.RetrieveColumnsCompleted += service_RetrieveColumnsCompleted;
            service.RetrieveColumnsAsync(e.Xml);
        }

        private void service_RetrieveColumnsCompleted(object sender, ServiceReference1.RetrieveColumnsCompletedEventArgs e)
        {
            if (e.Error == null && !dataStoreSourceEditWindow.IsBreakedProgressInformation)
            {
                dataStoreSourceEditWindow.ApplyResultAfterRetrieveColumns(e.Result);
            }

            dataStoreSourceEditWindow.HideProgressBar();
            dataStoreSourceEditWindow = null;
        }
        #endregion

        #region Opening Report
        private void WCFService_WCFOpeningReportInDesigner(object sender, Stimulsoft.Report.Events.StiWCFOpeningReportEventArgs e)
        {
            e.Handled = true;
            designer.StartProgressInformation(StiLocalization.Get("DesignerFx", "LoadingReport"), Visibility.Collapsed, false);

            var service = new ServiceReference1.DesignerServiceClient();
            service.LoadReportCompleted += service_LoadReportCompleted;
            service.LoadReportAsync();
        }

        private void service_LoadReportCompleted(object sender, ServiceReference1.LoadReportCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                StiReport report = new StiReport();
                report.Load(e.Result);
                designer.Report = report;
            }

            designer.CloseProgressInformation();
        }
        #endregion

        #region Saving Report
        private void GlobalEvents_SavingReportInDesigner(object sender, Stimulsoft.Report.Design.StiSavingObjectEventArgs e)
        {
            designer.StartProgressInformation(StiLocalization.Get("Report", "SavingReport"), Visibility.Collapsed, false);
            var service = new ServiceReference1.DesignerServiceClient();
            service.SaveReportCompleted += service_SaveReportCompleted;
            service.SaveReportAsync(designer.Report.SaveToByteArray());
        }

        private void service_SaveReportCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            designer.CloseProgressInformation();
        }
        #endregion 

        #region Export Document
        private string exportFilter;
        private void WCFService_WCFExportDocument(object sender, Stimulsoft.Report.Events.StiWCFExportEventArgs e)
        {
            viewer = sender as StiSLViewerControl;
            exportFilter = e.Filter;
            viewer.StartProgressInformation("WCF Service", "Export Report", Visibility.Visible);

            var service = new ServiceReference1.DesignerServiceClient();
            service.ExportDocumentCompleted += service_ExportDocumentCompleted;
            service.ExportDocumentAsync(e.Xml);
        }

        private void service_ExportDocumentCompleted(object sender, ServiceReference1.ExportDocumentCompletedEventArgs e)
        {
            viewer.CloseProgressInformation();

            if (e.Error != null || e.Result == null) return;

            var window = new SaveWindow();
            window.OKButton.Click += delegate
                {
                    window.Close();

                    var saveFileDialog = new SaveFileDialog
                        {
                            Filter = string.Format("Export Document (*.{0})|*.{0}", exportFilter)
                        };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        var stream = (FileStream)saveFileDialog.OpenFile();
                        stream.Write(e.Result, 0, e.Result.Length);

                        stream.Flush();
                        stream.Close();
                        stream.Dispose();
                        stream = null;
                    }
                    
                    exportFilter = null;
                };

            window.Show();
        }
        #endregion

        #region RenderingInteractions
        private void WCFService_WCFRenderingInteractions(object viewer, Stimulsoft.Report.Events.StiWCFRenderingInteractionsEventArgs e)
        {
            this.viewer = viewer as StiSLViewerControl;
            this.viewer.StartProgressInformation("WCF Service", StiLocalization.Get("DesignerFx", "CompilingReport"), Visibility.Visible);

            var service = new ServiceReference1.DesignerServiceClient();
            service.RenderingInteractionsCompleted += service_RenderingInteractionsCompleted;
            service.RenderingInteractionsAsync(e.Xml);
        }

        private void service_RenderingInteractionsCompleted(object sender, ServiceReference1.RenderingInteractionsCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null && e.Result.Length > 2)
            {
                viewer.ApplyChangesAfterSorting(e.Result);
            }

            viewer.CloseProgressInformation();
        }
        #endregion

        #region RequestFromUserRenderReport
        private void WCFService_WCFRequestFromUserRenderReport(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            this.viewer = sender as StiSLViewerControl;
            this.viewer.StartProgressInformation("WCF Service", StiLocalization.Get("DesignerFx", "CompilingReport"), Visibility.Visible);

            var service = new ServiceReference1.DesignerServiceClient();
            service.RequestFromUserRenderReportCompleted += service_RequestFromUserRenderReportCompleted;
            service.RequestFromUserRenderReportAsync(e.Xml);
        }

        private void service_RequestFromUserRenderReportCompleted(object sender, ServiceReference1.RequestFromUserRenderReportCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null && e.Result.Length > 2)
            {
                viewer.ApplyRenderedReport(e.Result, true);
            }

            viewer.CloseProgressInformation();
        }
        #endregion

        #region LoadConfiguration
        private void service_LoadConfigurationCompleted(object sender, ServiceReference1.LoadConfigurationCompletedEventArgs e)
        {
            if (e.Error != null || string.IsNullOrEmpty(e.Result)) return;

            var databases = new System.Collections.Generic.List<string>();

            var stringReader = new StringReader(Stimulsoft.Report.SL.Helpers.StiParseResultsHelper.DecodeString(e.Result));
            var tr = new System.NetXml.XmlTextReader(stringReader);

            tr.Read();
            if (tr.Name == "XmlResult")
            {
                while (tr.Read())
                {
                    if (tr.Depth == 1)
                    {
                        databases.Add(tr.Name);
                    }
                }
            }

            tr.Close();
            stringReader.Close();
            stringReader.Dispose();
            tr = null;
            stringReader = null;

            var services = StiConfig.GetServices(StiLoadServiceType.StiDatabase);

            #region Disable all services
            foreach (Stimulsoft.Report.Dictionary.StiDatabase service in services)
            {
                if (service is Stimulsoft.Report.Dictionary.StiUndefinedDatabase) continue;
                service.ServiceEnabled = false;
            }
            #endregion

            #region Check all services
            foreach (string database in databases)
            {
                foreach (Stimulsoft.Report.Dictionary.StiDatabase service in services)
                {
                    if (service.GetType().FullName != database) continue;

                    service.ServiceEnabled = true;
                    break;
                }
            }
            #endregion
        }
        #endregion

        #region GoogleDocs
        private StiCloudDialogWindow cloudDialog;

        #region WCFGoogleDocsGetDocuments
        private void WCFService_WCFGoogleDocsGetDocuments(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            this.cloudDialog = sender as StiCloudDialogWindow;

            var service = new ServiceReference1.DesignerServiceClient();
            service.GoogleDocsGetDocumentsCompleted += service_GoogleDocsGetDocumentsCompleted;
            service.GoogleDocsGetDocumentsAsync(e.Xml);
        }

        private void service_GoogleDocsGetDocumentsCompleted(object sender, ServiceReference1.GoogleDocsGetDocumentsCompletedEventArgs e)
        {
            this.cloudDialog.GetDocumentsFinish(e.Error, e.Result);
        }
        #endregion

        #region WCFGoogleDocsCreateCollection
        private void WCFService_WCFGoogleDocsCreateCollection(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            this.cloudDialog = sender as StiCloudDialogWindow;

            var service = new ServiceReference1.DesignerServiceClient();
            service.GoogleDocsCreateCollectionCompleted += service_GoogleDocsCreateCollectionCompleted;
            service.GoogleDocsCreateCollectionAsync(e.Xml);
        }

        private void service_GoogleDocsCreateCollectionCompleted(object sender, ServiceReference1.GoogleDocsCreateCollectionCompletedEventArgs e)
        {
            this.cloudDialog.CreateCollectionFinish(e.Error, e.Result);
        }
        #endregion

        #region WCFGoogleDocsDelete
        private void WCFService_WCFGoogleDocsDelete(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            this.cloudDialog = sender as StiCloudDialogWindow;

            var service = new ServiceReference1.DesignerServiceClient();
            service.GoogleDocsDeleteCompleted += service_GoogleDocsDeleteCompleted;
            service.GoogleDocsDeleteAsync(e.Xml);
        }

        private void service_GoogleDocsDeleteCompleted(object sender, ServiceReference1.GoogleDocsDeleteCompletedEventArgs e)
        {
            this.cloudDialog.DeleteFinish(e.Error, e.Result);
        }
        #endregion

        #region WCFGoogleDocsOpen
        private void WCFService_WCFGoogleDocsOpen(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            this.cloudDialog = sender as StiCloudDialogWindow;

            var service = new ServiceReference1.DesignerServiceClient();
            service.GoogleDocsOpenCompleted += service_GoogleDocsOpenCompleted;
            service.GoogleDocsOpenAsync(e.Xml);
        }

        private void service_GoogleDocsOpenCompleted(object sender, ServiceReference1.GoogleDocsOpenCompletedEventArgs e)
        {
            this.cloudDialog.OpenFinish(e.Error, e.Result);
        }
        #endregion

        #region WCFGoogleDocsSave
        private void WCFService_WCFGoogleDocsSave(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            this.cloudDialog = sender as StiCloudDialogWindow;

            var service = new ServiceReference1.DesignerServiceClient();
            service.GoogleDocsSaveCompleted += service_GoogleDocsSaveCompleted;
            service.GoogleDocsSaveAsync(e.Xml);
        }

        private void service_GoogleDocsSaveCompleted(object sender, ServiceReference1.GoogleDocsSaveCompletedEventArgs e)
        {
            this.cloudDialog.SaveFinish(e.Error, e.Result);
        }
        #endregion
        #endregion

        #region WCFPrepareRequestFromUserVariables
        private void WCFService_WCFPrepareRequestFromUserVariables(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            this.viewer = sender as StiSLViewerControl;

            var service = new ServiceReference1.DesignerServiceClient();
            service.PrepareRequestFromUserVariablesCompleted += service_PrepareRequestFromUserVariablesCompleted;
            service.PrepareRequestFromUserVariablesAsync(e.Xml);
        }

        private void service_PrepareRequestFromUserVariablesCompleted(object sender, ServiceReference1.PrepareRequestFromUserVariablesCompletedEventArgs e)
        {
            this.viewer.ApplyResultAfterPrepareRequestFromUserVariables(e.Error, e.Result);
        }
        #endregion

        #region WCFInteractiveDataBandSelection
        private void WCFService_WCFInteractiveDataBandSelection(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            this.viewer = sender as StiSLViewerControl;
            this.viewer.progress = new StiProgressInformation();
            this.viewer.StartProgressInformation("WCF Service", StiLocalization.Get("DesignerFx", "CompilingReport"), System.Windows.Visibility.Collapsed);

            var service = new ServiceReference1.DesignerServiceClient();
            service.InteractiveDataBandSelectionCompleted += service_InteractiveDataBandSelectionCompleted;
            service.InteractiveDataBandSelectionAsync(e.Xml);
        }

        private void service_InteractiveDataBandSelectionCompleted(object sender, ServiceReference1.InteractiveDataBandSelectionCompletedEventArgs e)
        {
            if (e.Error == null && !this.viewer.IsBreakedProgressInformation)
            {
                this.viewer.ApplyChangesAfterInteractiveDataBandSelection(e.Result);
            }

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButton.OK);
            }

            this.viewer.CloseProgressInformation();
        }
        #endregion

        #region ReportScript (12 February 2012)
        private IStiReportCodeEditor iReportCodeEditor;

        #region WCFOpenReportScript
        private void WCFService_WCFOpenReportScript(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            this.iReportCodeEditor = sender as IStiReportCodeEditor;

            var service = new ServiceReference1.DesignerServiceClient();
            service.OpenReportScriptCompleted += service_OpenReportScriptCompleted;
            service.OpenReportScriptAsync(e.Xml);
        }

        private void service_OpenReportScriptCompleted(object sender, ServiceReference1.OpenReportScriptCompletedEventArgs e)
        {
            this.iReportCodeEditor.ApplyResultAfterOpenReportScript(e.Error, e.Result);
        }
        #endregion

        #region WCFSaveReportScript
        private void WCFService_WCFSaveReportScript(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            this.iReportCodeEditor = sender as IStiReportCodeEditor;
            var service = new ServiceReference1.DesignerServiceClient();
            service.SaveReportScriptCompleted += service_SaveReportScriptCompleted;
            service.SaveReportScriptAsync(e.Xml);
        }

        private void service_SaveReportScriptCompleted(object sender, ServiceReference1.SaveReportScriptCompletedEventArgs e)
        {
            this.iReportCodeEditor.ApplyResultAfterSaveReportScript(e.Error, e.Result);
        }
        #endregion
        #endregion
        #endregion

        public MainPage()
        {
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.UseWCFService = true;

            #region Configuration
            var service = new ServiceReference1.DesignerServiceClient();
            service.LoadConfigurationCompleted += service_LoadConfigurationCompleted;
            service.LoadConfigurationAsync();
            #endregion

            // Designer
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFRenderReport += WCFService_WCFRenderReport;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFTestConnection += WCFService_WCFTestConnection;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFBuildObjects += WCFService_WCFBuildObjects;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFRetrieveColumns += WCFService_WCFRetrieveColumns;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFOpeningReportInDesigner += WCFService_WCFOpeningReportInDesigner;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFRequestFromUserRenderReport += WCFService_WCFRequestFromUserRenderReport;
            Stimulsoft.Report.StiOptions.Engine.GlobalEvents.SavingReportInDesigner += GlobalEvents_SavingReportInDesigner;

            // Interactions
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFRenderingInteractions += WCFService_WCFRenderingInteractions;

            // Viewer
            Stimulsoft.Report.StiOptions.Viewer.Elements.ShowReportSaveButton = false;
            Stimulsoft.Report.StiOptions.Viewer.Elements.ShowReportSaveToServerButton = true;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFExportDocument += WCFService_WCFExportDocument;

            // GoogleDocs
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFGoogleDocsGetDocuments += WCFService_WCFGoogleDocsGetDocuments;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFGoogleDocsCreateCollection += WCFService_WCFGoogleDocsCreateCollection;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFGoogleDocsDelete += WCFService_WCFGoogleDocsDelete;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFGoogleDocsOpen += WCFService_WCFGoogleDocsOpen;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFGoogleDocsSave += WCFService_WCFGoogleDocsSave;

            // Prepare RequestFromUser Variables
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFPrepareRequestFromUserVariables += WCFService_WCFPrepareRequestFromUserVariables;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFInteractiveDataBandSelection += WCFService_WCFInteractiveDataBandSelection;

            // Report Script
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFOpenReportScript += WCFService_WCFOpenReportScript;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFSaveReportScript += WCFService_WCFSaveReportScript;

            InitializeComponent();

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var sr = new StreamReader(assembly.GetManifestResourceStream("WCF_SLDesigner.Reports.SimpleList.mrt"));

            var report = new StiReport();
            report.Load(sr.BaseStream);

            sr.BaseStream.Dispose();
            sr.Dispose();
            sr = null;

            designer.Report = report;
        }
    }
}