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
using System.Text;
using Stimulsoft.Report;
using System.Xml;
using Stimulsoft.Report.Export;
using System.Drawing.Imaging;
using System.Globalization;

namespace WCFHelper
{
    public static class StiSLExportHelper
    {
        #region Methods
        public static byte[] StartExport(string xml)
        {
            string exportSettings = null;
            StiSLExportType? exportFormat = null;
            var report = ParseExportSettings(xml, ref exportSettings, ref exportFormat);
            if (report == null || exportFormat == null || exportSettings == null) return null;

            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            StiPageRangeExportSettings settings;
            var stream = new MemoryStream();

            switch (exportFormat)
            {
                case StiSLExportType.Csv:
                    settings = GetCsvExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Csv, stream, settings);
                    break;

                case StiSLExportType.Dbf:
                    settings = GetDbfExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Dbf, stream, settings);
                    break;

                case StiSLExportType.Dif:
                    settings = GetDifExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Dif, stream, settings);
                    break;

                case StiSLExportType.Excel:
                    settings = GetExcelExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Excel, stream, settings);
                    break;

                case StiSLExportType.Excel2007:
                    settings = GetExcel2007ExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Excel2007, stream, settings);
                    break;

                case StiSLExportType.ExcelXml:
                    settings = GetExcelXmlExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.ExcelXml, stream, settings);
                    break;

                case StiSLExportType.Html:
                    report.ExportDocument(StiExportFormat.Html, stream);
                    break;

                case StiSLExportType.Mht:
                    settings = GetMhtExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Mht, stream, settings);
                    break;

                case StiSLExportType.Bmp:
                    settings = CopyBitmapSettings(new StiBmpExportSettings(), GetImageExportSettings(exportSettings));
                    report.ExportDocument(StiExportFormat.ImageBmp, stream, settings);
                    break;

                case StiSLExportType.Gif:
                    settings = CopyBitmapSettings(new StiGifExportSettings(), GetImageExportSettings(exportSettings));
                    report.ExportDocument(StiExportFormat.ImageGif, stream, settings);
                    break;

                case StiSLExportType.Jpeg:
                    settings = CopyBitmapSettings(new StiJpegExportSettings(), GetImageExportSettings(exportSettings));
                    report.ExportDocument(StiExportFormat.ImageJpeg, stream, settings);
                    break;

                case StiSLExportType.Emf:
                    settings = CopyBitmapSettings(new StiEmfExportSettings(), GetImageExportSettings(exportSettings));
                    report.ExportDocument(StiExportFormat.ImageEmf, stream, settings);
                    break;

                case StiSLExportType.Pcx:
                    settings = CopyBitmapSettings(new StiPcxExportSettings(), GetImageExportSettings(exportSettings));
                    report.ExportDocument(StiExportFormat.ImagePcx, stream, settings);
                    break;

                case StiSLExportType.Png:
                    settings = CopyBitmapSettings(new StiPngExportSettings(), GetImageExportSettings(exportSettings));
                    report.ExportDocument(StiExportFormat.ImagePng, stream, settings);
                    break;

                case StiSLExportType.Svg:
                    settings = CopyBitmapSettings(new StiSvgExportSettings(), GetImageExportSettings(exportSettings));
                    report.ExportDocument(StiExportFormat.ImageSvg, stream, settings);
                    break;

                case StiSLExportType.Svgz:
                    settings = CopyBitmapSettings(new StiSvgzExportSettings(), GetImageExportSettings(exportSettings));
                    report.ExportDocument(StiExportFormat.ImageSvgz, stream, settings);
                    break;

                case StiSLExportType.Tiff:
                    settings = CopyBitmapSettings(new StiTiffExportSettings(), GetImageExportSettings(exportSettings));
                    report.ExportDocument(StiExportFormat.ImageTiff, stream, settings);
                    break;

                case StiSLExportType.Ods:
                    settings = GetOdsExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Ods, stream, settings);
                    break;

                case StiSLExportType.Odt:
                    settings = GetOdtExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Odt, stream, settings);
                    break;

                case StiSLExportType.Pdf:
                    StiPdfExportSettings pdfExportSettings = GetPdfExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Pdf, stream, pdfExportSettings);
                    break;

                case StiSLExportType.Rtf:
                    settings = GetRtfExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Rtf, stream, settings);
                    break;

                case StiSLExportType.Sylk:
                    settings = GetSylkExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Sylk, stream, settings);
                    break;

                case StiSLExportType.Text:
                    settings = GetTextExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Text, stream, settings);
                    break;

                case StiSLExportType.Word2007:
                    settings = GetWord2007ExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Word2007, stream, settings);
                    break;

                case StiSLExportType.Xps:
                    settings = GetXpsExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Xps, stream, settings);
                    break;

                case StiSLExportType.Ppt2007:
                    settings = GetPpt2007ExportSettings(exportSettings);
                    report.ExportDocument(StiExportFormat.Ppt2007, stream, settings);
                    break;

                case StiSLExportType.Xml:
                    report.ExportDocument(StiExportFormat.Xml, stream);
                    break;
            }

            byte[] bufferResult = null;
            if (stream != null)
            {
                bufferResult = new byte[(int)stream.Length];

                stream.Position = 0;
                stream.Read(bufferResult, 0, bufferResult.Length);
                stream.Close();
                stream.Dispose();
                stream = null;
            }

            System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;

            return bufferResult;
        }

        private static StiReport ParseExportSettings(string xml, ref string exportSettings, ref StiSLExportType? exportFormat)
        {
            var report = new StiReport();
            xml = StiSLEncodingHelper.DecodeString(xml);

            #region Read Format and Size
            int lastPos = 0;
            int index = 0;
            int reportLength;
            while (true)
            {
                if (xml[index] == ',')
                {
                    if (exportFormat == null)
                    {
                        exportFormat = (StiSLExportType)int.Parse(xml.Substring(0, index++));
                        lastPos = index;
                    }
                    else
                    {
                        reportLength = int.Parse(xml.Substring(lastPos, index - lastPos));
                        lastPos = index + 1;
                        break;
                    }
                }

                index++;
            }
            #endregion

            report.LoadDocumentFromString(xml.Substring(lastPos, reportLength));
            lastPos += reportLength;
            exportSettings = xml.Substring(lastPos, xml.Length - lastPos);

            return report;
        }

        private static StiImageExportSettings CopyBitmapSettings(StiImageExportSettings settings1, StiImageExportSettings settings2)
        {
            var settings = settings1 as StiBitmapExportSettings;
            if (settings != null)
            {
                settings.CutEdges = settings2.CutEdges;
                settings.DitheringType = settings2.DitheringType;
                settings.ImageFormat = settings2.ImageFormat;
                settings.ImageResolution = settings2.ImageResolution;
                settings.ImageZoom = settings2.ImageZoom;
                settings.MultipleFiles = settings2.MultipleFiles;
                settings.PageRange = settings2.PageRange;
                settings.TiffCompressionScheme = settings2.TiffCompressionScheme;
            }
            else
            {
                settings1.PageRange = settings2.PageRange;
            }

            return settings1;
        }
        #endregion

        #region Methods.ParseExportSettings
        private static StiCsvExportSettings GetCsvExportSettings(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiCsvExportSettings
                {
                    Encoding = ParseEncoding(rootNode.ChildNodes[0].FirstChild.Value),
                    PageRange = GetPagesRange(rootNode.ChildNodes[1]),
                    Separator = (rootNode.ChildNodes[2].FirstChild == null) ? string.Empty : rootNode.ChildNodes[2].FirstChild.Value,
                    SkipColumnHeaders = rootNode.ChildNodes[3].FirstChild.Value == "1"
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiDbfExportSettings GetDbfExportSettings(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiDbfExportSettings
                {
                    CodePage = (StiDbfCodePages) int.Parse(rootNode.ChildNodes[0].FirstChild.Value),
                    PageRange = GetPagesRange(rootNode.ChildNodes[1])
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiDifExportSettings GetDifExportSettings(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiDifExportSettings
                {
                    Encoding = Encoding.GetEncoding(int.Parse(rootNode.ChildNodes[0].FirstChild.Value)),
                    ExportDataOnly = rootNode.ChildNodes[1].FirstChild.Value == "1",
                    PageRange = GetPagesRange(rootNode.ChildNodes[2]),
                    UseDefaultSystemEncoding = rootNode.ChildNodes[3].FirstChild.Value == "1"
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiExcelExportSettings GetExcelExportSettings(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiExcelExportSettings
                {
                    ExportDataOnly = rootNode.ChildNodes[0].FirstChild.Value == "1",
                    ExportEachPageToSheet = rootNode.ChildNodes[1].FirstChild.Value == "1",
                    ExportObjectFormatting = rootNode.ChildNodes[2].FirstChild.Value == "1",
                    ExportPageBreaks = rootNode.ChildNodes[3].FirstChild.Value == "1",
                    ImageQuality = float.Parse(rootNode.ChildNodes[4].FirstChild.Value),
                    ImageResolution = float.Parse(rootNode.ChildNodes[5].FirstChild.Value),
                    PageRange = GetPagesRange(rootNode.ChildNodes[6]),
                    UseOnePageHeaderAndFooter = rootNode.ChildNodes[7].FirstChild.Value == "1"
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiExcel2007ExportSettings GetExcel2007ExportSettings(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiExcel2007ExportSettings
                {
                    ExportDataOnly = rootNode.ChildNodes[0].FirstChild.Value == "1",
                    ExportEachPageToSheet = rootNode.ChildNodes[1].FirstChild.Value == "1",
                    ExportObjectFormatting = rootNode.ChildNodes[2].FirstChild.Value == "1",
                    ExportPageBreaks = rootNode.ChildNodes[3].FirstChild.Value == "1",
                    ImageQuality = float.Parse(rootNode.ChildNodes[4].FirstChild.Value),
                    ImageResolution = float.Parse(rootNode.ChildNodes[5].FirstChild.Value),
                    PageRange = GetPagesRange(rootNode.ChildNodes[6]),
                    UseOnePageHeaderAndFooter = rootNode.ChildNodes[7].FirstChild.Value == "1"
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiExcelXmlExportSettings GetExcelXmlExportSettings(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiExcelXmlExportSettings
                {
                    PageRange = GetPagesRange(rootNode.ChildNodes[0])
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiHtmlExportSettings GetHtmlExportSettings(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiHtmlExportSettings
                {
                    AddPageBreaks = rootNode.ChildNodes[0].FirstChild.Value == "1",
                    ExportMode = (StiHtmlExportMode) int.Parse(rootNode.ChildNodes[1].FirstChild.Value),
                    ExportQuality = (StiHtmlExportQuality) int.Parse(rootNode.ChildNodes[2].FirstChild.Value),
                    ImageFormat = new ImageFormat(new Guid(rootNode.ChildNodes[3].FirstChild.Value)),
                    PageRange = GetPagesRange(rootNode.ChildNodes[4]),
                    Zoom = double.Parse(rootNode.ChildNodes[5].FirstChild.Value),
                    CompressToArchive = rootNode.ChildNodes[6].FirstChild.Value == "1",
                    UseEmbeddedImages = rootNode.ChildNodes[7].FirstChild.Value == "1"
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiHtml5ExportSettings GetHtml5ExportSettings(string xmlSettings)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiHtml5ExportSettings
                {
                    ImageQuality = float.Parse(rootNode.ChildNodes[0].FirstChild.Value),
                    ImageResolution = float.Parse(rootNode.ChildNodes[1].FirstChild.Value),
                    ImageFormat = new ImageFormat(new Guid(rootNode.ChildNodes[2].FirstChild.Value)),
                    ContinuousPages = rootNode.ChildNodes[3].FirstChild.Value == "1",
                    PageRange = GetPagesRange(rootNode.ChildNodes[4])
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiMhtExportSettings GetMhtExportSettings(string xmlSettings)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiMhtExportSettings();
            settings.AddPageBreaks = rootNode.ChildNodes[0].FirstChild.Value == "1";
            settings.Encoding = Encoding.UTF8;
            settings.ExportMode = (StiHtmlExportMode)int.Parse(rootNode.ChildNodes[2].FirstChild.Value);
            settings.ExportQuality = (StiHtmlExportQuality)int.Parse(rootNode.ChildNodes[3].FirstChild.Value);
            settings.ImageFormat = new ImageFormat(new Guid(rootNode.ChildNodes[4].FirstChild.Value));
            settings.PageRange = GetPagesRange(rootNode.ChildNodes[5]);
            settings.Zoom = double.Parse(rootNode.ChildNodes[6].FirstChild.Value);

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiImageExportSettings GetImageExportSettings(string xmlSettings)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiImageExportSettings
                {
                    CutEdges = rootNode.ChildNodes[0].FirstChild.Value == "1",
                    DitheringType = (StiMonochromeDitheringType) int.Parse(rootNode.ChildNodes[1].FirstChild.Value),
                    ImageFormat = (StiImageFormat) int.Parse(rootNode.ChildNodes[2].FirstChild.Value),
                    ImageResolution = int.Parse(rootNode.ChildNodes[3].FirstChild.Value),
                    ImageZoom = double.Parse(rootNode.ChildNodes[4].FirstChild.Value),
                    MultipleFiles = rootNode.ChildNodes[5].FirstChild.Value == "1",
                    PageRange = GetPagesRange(rootNode.ChildNodes[6]),
                    TiffCompressionScheme = (StiTiffCompressionScheme) int.Parse(rootNode.ChildNodes[7].FirstChild.Value)
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiOdsExportSettings GetOdsExportSettings(string xmlSettings)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiOdsExportSettings
                {
                    ImageQuality = float.Parse(rootNode.ChildNodes[0].FirstChild.Value),
                    ImageResolution = float.Parse(rootNode.ChildNodes[1].FirstChild.Value),
                    PageRange = GetPagesRange(rootNode.ChildNodes[2])
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiOdtExportSettings GetOdtExportSettings(string xmlSettings)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiOdtExportSettings
                {
                    ImageQuality = float.Parse(rootNode.ChildNodes[0].FirstChild.Value),
                    ImageResolution = float.Parse(rootNode.ChildNodes[1].FirstChild.Value),
                    PageRange = GetPagesRange(rootNode.ChildNodes[2]),
                    UsePageHeadersAndFooters = rootNode.ChildNodes[3].FirstChild.Value == "1",
                    RemoveEmptySpaceAtBottom = rootNode.ChildNodes[4].FirstChild.Value == "1"
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiPdfExportSettings GetPdfExportSettings(string xmlSettings)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiPdfExportSettings();
            settings.PageRange = GetPagesRange(rootNode.ChildNodes[0]);
            settings.ImageQuality = float.Parse(rootNode.ChildNodes[1].FirstChild.Value);
            settings.ImageCompressionMethod = (StiPdfImageCompressionMethod)int.Parse(rootNode.ChildNodes[2].FirstChild.Value);
            settings.ImageResolution = float.Parse(rootNode.ChildNodes[3].FirstChild.Value);
            settings.EmbeddedFonts = rootNode.ChildNodes[4].FirstChild.Value == "1";
            settings.StandardPdfFonts = rootNode.ChildNodes[5].FirstChild.Value == "1";
            settings.Compressed = rootNode.ChildNodes[6].FirstChild.Value == "1";
            settings.ExportRtfTextAsImage = rootNode.ChildNodes[7].FirstChild.Value == "1";
            if (rootNode.ChildNodes[8].FirstChild != null)
                settings.PasswordInputUser = rootNode.ChildNodes[8].FirstChild.Value;
            if (rootNode.ChildNodes[9].FirstChild != null)
                settings.PasswordInputOwner = rootNode.ChildNodes[9].FirstChild.Value;
            settings.UserAccessPrivileges = (StiUserAccessPrivileges)int.Parse(rootNode.ChildNodes[10].FirstChild.Value);
            settings.KeyLength = (StiPdfEncryptionKeyLength)int.Parse(rootNode.ChildNodes[11].FirstChild.Value);
            settings.UseUnicode = rootNode.ChildNodes[12].FirstChild.Value == "1";
            settings.GetCertificateFromCryptoUI = rootNode.ChildNodes[13].FirstChild.Value == "1";
            settings.UseDigitalSignature = rootNode.ChildNodes[14].FirstChild.Value == "1";
            if (rootNode.ChildNodes[15].FirstChild != null)
                settings.SubjectNameString = rootNode.ChildNodes[15].FirstChild.Value;
            settings.PdfACompliance = rootNode.ChildNodes[16].FirstChild.Value == "1";
            settings.ImageFormat = (StiImageFormat)int.Parse(rootNode.ChildNodes[17].FirstChild.Value);
            settings.DitheringType = (StiMonochromeDitheringType)int.Parse(rootNode.ChildNodes[18].FirstChild.Value);

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiRtfExportSettings GetRtfExportSettings(string xmlSettings)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiRtfExportSettings
                {
                    ExportMode = (StiRtfExportMode) int.Parse(rootNode.ChildNodes[0].FirstChild.Value),
                    ImageQuality = float.Parse(rootNode.ChildNodes[1].FirstChild.Value),
                    ImageResolution = float.Parse(rootNode.ChildNodes[2].FirstChild.Value),
                    PageRange = GetPagesRange(rootNode.ChildNodes[3]),
                    RemoveEmptySpaceAtBottom = rootNode.ChildNodes[4].FirstChild.Value == "1"
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiSylkExportSettings GetSylkExportSettings(string xmlSettings)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiSylkExportSettings
                {
                    Encoding = Encoding.GetEncoding(rootNode.ChildNodes[0].FirstChild.Value),
                    ExportDataOnly = rootNode.ChildNodes[1].FirstChild.Value == "1",
                    PageRange = GetPagesRange(rootNode.ChildNodes[2]),
                    UseDefaultSystemEncoding = rootNode.ChildNodes[3].FirstChild.Value == "1"
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiTxtExportSettings GetTextExportSettings(string xmlSettings)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiTxtExportSettings
                {
                    BorderType = (StiTxtBorderType) int.Parse(rootNode.ChildNodes[0].FirstChild.Value),
                    CutLongLines = rootNode.ChildNodes[1].FirstChild.Value == "1",
                    Encoding = ParseEncoding(rootNode.ChildNodes[2].FirstChild.Value),
                    PageRange = GetPagesRange(rootNode.ChildNodes[3]),
                    ZoomX = float.Parse(rootNode.ChildNodes[4].FirstChild.Value),
                    ZoomY = float.Parse(rootNode.ChildNodes[5].FirstChild.Value)
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiWord2007ExportSettings GetWord2007ExportSettings(string xmlSettings)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiWord2007ExportSettings
                {
                    PageRange = GetPagesRange(rootNode.ChildNodes[0]),
                    UsePageHeadersAndFooters = rootNode.ChildNodes[1].FirstChild.Value == "1",
                    ImageQuality = float.Parse(rootNode.ChildNodes[2].FirstChild.Value),
                    ImageResolution = float.Parse(rootNode.ChildNodes[3].FirstChild.Value),
                    RemoveEmptySpaceAtBottom = rootNode.ChildNodes[4].FirstChild.Value == "1"
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiXpsExportSettings GetXpsExportSettings(string xmlSettings)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiXpsExportSettings
                {
                    ExportRtfTextAsImage = rootNode.ChildNodes[0].FirstChild.Value == "1",
                    ImageQuality = float.Parse(rootNode.ChildNodes[1].FirstChild.Value),
                    ImageResolution = float.Parse(rootNode.ChildNodes[2].FirstChild.Value),
                    PageRange = GetPagesRange(rootNode.ChildNodes[3])
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }

        private static StiPpt2007ExportSettings GetPpt2007ExportSettings(string xmlSettings)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSettings);
            XmlNode rootNode = xmlDoc.DocumentElement;

            var settings = new StiPpt2007ExportSettings
                {
                    ImageQuality = float.Parse(rootNode.ChildNodes[0].FirstChild.Value),
                    ImageResolution = float.Parse(rootNode.ChildNodes[1].FirstChild.Value),
                    PageRange = GetPagesRange(rootNode.ChildNodes[2])
                };

            xmlDoc = null;
            rootNode = null;

            return settings;
        }
        #endregion

        #region Methods.ParseProperties
        private static StiPagesRange GetPagesRange(XmlNode pageRange)
        {
            var rangeType = (StiRangeType)int.Parse(pageRange.ChildNodes[2].FirstChild.Value);
            return new StiPagesRange(rangeType, (pageRange.ChildNodes[1].FirstChild == null) 
                ? string.Empty 
                : pageRange.ChildNodes[1].FirstChild.Value, int.Parse(pageRange.ChildNodes[0].FirstChild.Value));
        }

        private static Encoding ParseEncoding(string value)
        {
            Encoding result = null;
            switch (value)
            {
                case "Default":
                    result = Encoding.Default;
                    break;

                case "ASCII":
                    result = Encoding.ASCII;
                    break;

                case "BigEndianUnicode":
                    result = Encoding.BigEndianUnicode;
                    break;

                case "Unicode":
                    result = Encoding.Unicode;
                    break;

                case "UTF7":
                    result = Encoding.UTF7;
                    break;

                case "UTF8":
                    result = Encoding.UTF8;
                    break;
            }

            return result;
        }
        #endregion
    }
}