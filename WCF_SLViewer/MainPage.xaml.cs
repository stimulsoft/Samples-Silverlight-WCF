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
using System.IO;
using System.Windows;
using System.Windows.Controls;

using Stimulsoft.Base.Localization;
using Stimulsoft.Report;

namespace WCF_SLViewer
{
    public partial class MainPage : UserControl
    {
        #region Fields
        private StiInteractionType interactionType;
        #endregion

        #region Handlers
        #region ExportDocument
        private string exportFilter;
        private void WCFService_WCFExportDocument(object sender, Stimulsoft.Report.Events.StiWCFExportEventArgs e)
        {
            exportFilter = e.Filter;
            viewer.StartProgressInformation("WCF Service", "Export Report", Visibility.Visible);

            ServiceReference1.ViewerServiceClient service = new ServiceReference1.ViewerServiceClient();
            service.ExportDocumentCompleted += service_ExportDocumentCompleted;
            service.ExportDocumentAsync(e.Xml);
        }

        private void service_ExportDocumentCompleted(object sender, ServiceReference1.ExportDocumentCompletedEventArgs e)
        {
            viewer.CloseProgressInformation();

            if (e.Error == null && e.Result != null)
            {
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
                        var stream = saveFileDialog.OpenFile() as FileStream;
                        stream.Write(e.Result, 0, e.Result.Length);

                        stream.Flush();
                        stream.Close();
                        stream.Dispose();
                    }
                };

                window.Show();
            }
        }
        #endregion

        #region buttonLoad_Click
        private void buttonLoad_Click(object sender, RoutedEventArgs e)
        {
            if (cbReports.SelectedItem != null)
            {
                viewer.progress = new StiProgressInformation();
                viewer.progress.Start("WCF Service", StiLocalization.Get("DesignerFx", "LoadingDocument"), false);

                ServiceReference1.ViewerServiceClient service = new ServiceReference1.ViewerServiceClient();
                service.LoadReportCompleted += service_LoadReportCompleted;
                service.LoadReportAsync(((ComboBoxItem)cbReports.SelectedItem).Content.ToString());
            }
        }

        private void service_LoadReportCompleted(object sender, ServiceReference1.LoadReportCompletedEventArgs e)
        {
            viewer.progress.Start("WCF Service", StiLocalization.Get("DesignerFx", "LoadingDocument"), false);
            if (e.Error == null && e.Result != null && e.Result.Length > 2)
            {
                viewer.ApplyRenderedReport(e.Result);
            }

            viewer.progress.Close();
            viewer.progress = null;
        }
        #endregion

        #region RenderingInteractions
        private void WCFService_WCFRenderingInteractions(object viewer, Stimulsoft.Report.Events.StiWCFRenderingInteractionsEventArgs e)
        {
            this.viewer.progress = new StiProgressInformation();
            this.viewer.progress.Start("WCF Service", StiLocalization.Get("DesignerFx", "CompilingReport"), false);

            interactionType = e.InteractionType;

            ServiceReference1.ViewerServiceClient service = new ServiceReference1.ViewerServiceClient();
            service.RenderingInteractionsCompleted += service_RenderingInteractionsCompleted;
            service.RenderingInteractionsAsync(e.Xml);
        }

        private void service_RenderingInteractionsCompleted(object sender, ServiceReference1.RenderingInteractionsCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null && e.Result.Length > 2)
            {
                switch (interactionType)
                {
                    case StiInteractionType.Collapsing:
                        viewer.ApplyChangesAfterCollapsing(e.Result);
                        break;
                    case StiInteractionType.DrillDownPage:
                        viewer.ApplyChangesAfterDrillDownPage(e.Result);
                        break;
                    case StiInteractionType.Sorting:
                        viewer.ApplyChangesAfterSorting(e.Result);
                        break;
                }
            }

            viewer.progress.Close();
            viewer.progress = null;
        }
        #endregion

        #region RequestFromUserRenderReport
        private void WCFService_WCFRequestFromUserRenderReport(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            this.viewer = sender as Stimulsoft.Report.Viewer.StiSLViewerControl;
            this.viewer.StartProgressInformation("WCF Service",
                StiLocalization.Get("DesignerFx", "CompilingReport"), Visibility.Visible);

            ServiceReference1.ViewerServiceClient service = new ServiceReference1.ViewerServiceClient();
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

        #region WCFPrepareRequestFromUserVariables
        private void WCFService_WCFPrepareRequestFromUserVariables(object sender, Stimulsoft.Report.Events.StiWCFEventArgs e)
        {
            ServiceReference1.ViewerServiceClient service = new ServiceReference1.ViewerServiceClient();
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
            this.viewer.progress = new StiProgressInformation();
            viewer.StartProgressInformation("WCF Service", StiLocalization.Get("DesignerFx", "LoadingReport"), Visibility.Collapsed);

            ServiceReference1.ViewerServiceClient service = new ServiceReference1.ViewerServiceClient();
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

            viewer.CloseProgressInformation();
        }
        #endregion
        #endregion

        public MainPage()
        {
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.UseWCFService = true;
            Stimulsoft.Report.StiOptions.Viewer.Elements.ShowReportSaveButton = false;
            Stimulsoft.Report.StiOptions.Viewer.Elements.ShowReportSaveToServerButton = true;

            // Viewer
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFExportDocument += WCFService_WCFExportDocument;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFRequestFromUserRenderReport += WCFService_WCFRequestFromUserRenderReport;

            // Interactions
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFRenderingInteractions += WCFService_WCFRenderingInteractions;
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFInteractiveDataBandSelection += WCFService_WCFInteractiveDataBandSelection;

            // Prepare RequestFromUser Variables
            Stimulsoft.Report.StiOptions.Silverlight.WCFService.WCFPrepareRequestFromUserVariables += WCFService_WCFPrepareRequestFromUserVariables;

            InitializeComponent();
        }
    }
}