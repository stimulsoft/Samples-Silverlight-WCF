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
using WCFHelper.Compression;

namespace WCFHelper
{
    internal static class StiSLEncodingHelper
    {
        #region Encode/Decode
        public static string EncodeString(string xml)
        {
            var stream = new MemoryStream();

            var zipStream = new StiZipOutputStream(stream);
            zipStream.SetLevel(9);
            zipStream.IsStreamOwner = false;

            var entry = new StiZipEntry("1");
            zipStream.PutNextEntry(entry);

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(xml);
            zipStream.Write(buffer, 0, buffer.Length);
            buffer = null;

            zipStream.Finish();
            zipStream.IsStreamOwner = false;
            zipStream.Close();
            zipStream.Dispose();
            zipStream = null;

            string result = Convert.ToBase64String(stream.ToArray());
            stream.Close();
            stream.Dispose();
            stream = null;

            return result;
        }

        public static string DecodeString(string xml)
        {
            string result = string.Empty;
            var stream = new MemoryStream(Convert.FromBase64String(xml));

            var zipStream = new StiZipInputStream(stream)
                {
                    IsStreamOwner = false
                };

            var entry = zipStream.GetNextEntry();
            byte[] buffer = new byte[entry.Size];
            zipStream.Read(buffer, 0, buffer.Length);

            result = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            zipStream.Close();
            zipStream.Dispose();
            zipStream = null;

            stream.Close();
            stream.Dispose();
            stream = null;

            return result;
        }
        #endregion
    }
}