using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace Mozlite.Extensions.Storages.Epub
{
    /// <summary>
    /// 定义。
    /// </summary>
    public static class Epubs
    {
        /// <summary>
        /// 默认样式。
        /// </summary>
        internal static readonly string DefaultStyle = @"body {margin: 10px;font-size: 1.0em;word-wrap: break-word;}ul,li {list-style-type: none;margin: 0;padding: 0;}p {text-indent: 2em;line-height: 1.5em;margin-top: 0;margin-bottom: 1.5em;}.chapter {line-height: 3.5em;height: 3.5em;font-size: 0.8em;}li {border-bottom: 1px solid #D5D5D5;}h1 {font-size: 1.6em;font-weight: bold;}h2 {display: block;font-size: 1.2em;font-weight: bold;margin-bottom: 0.83em;margin-left: 0;margin-right: 0;margin-top: 1em;}.break {display: block;margin-bottom: 0;margin-left: 0;margin-right: 0;margin-top: 0}a {color: inherit;text-decoration: none;cursor: default}a[href] {color: blue;text-decoration: none;cursor: pointer}.italic {font-style: italic}";

        /// <summary>
        /// META-INF/container.xml。
        /// </summary>
        internal static readonly string Container = @"<?xml version=""1.0""?>
<container version=""1.0"" xmlns=""urn:oasis:names:tc:opendocument:xmlns:container"">
  <rootfiles>
    <rootfile full-path=""content.opf""
     media-type=""application/oebps-package+xml"" />
  </rootfiles>
</container>";

        /// <summary>
        /// HTML默认模板。
        /// </summary>
        internal static string Html(string title, string body) => $@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""no""?>
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""zh-CN"">
<head>
<title>{title}</title>
<link href=""stylesheet.css"" type=""text/css"" rel=""stylesheet""/><style type=""text/css"">
@page {{ margin-bottom: 5.000000pt; margin-top: 5.000000pt; }}</style>
</head>
<body>{body}<div class=""break""></div></body></html>";

        /// <summary>
        /// 目录文件。
        /// </summary>
        internal const string TocFile = "catalog.xhtml";

        private const string Mimetype = "mimetype";

        /// <summary>
        /// 将文件夹下得文件编译成Epub文件。
        /// </summary>
        /// <param name="directoryName">文件夹物理路径。</param>
        /// <param name="outputFileName">生成得文件路径。</param>
        /// <returns>返回Epub文件得物理路径。</returns>
        public static string Compile(string directoryName, string outputFileName)
        {
            directoryName = new DirectoryInfo(directoryName).FullName;
            var path = Path.Combine(directoryName, Mimetype);
            StorageHelper.SaveText(path, "application/epub+zip");
            if (!outputFileName.IsPhysicalPath())
                outputFileName = Path.Combine(directoryName, "../", outputFileName);
            if (!outputFileName.EndsWith(".epub", StringComparison.OrdinalIgnoreCase))
                outputFileName += ".epub";
            if (File.Exists(outputFileName)) File.Delete(outputFileName);
            using (var zip = ZipFile.Open(outputFileName, ZipArchiveMode.Create, Encoding.UTF8))
            {
                zip.CreateEntryFromFile(path, Mimetype, CompressionLevel.NoCompression);
                foreach (var file in new DirectoryInfo(directoryName).EnumerateFiles("*.*", SearchOption.AllDirectories))
                {
                    var current = file.FullName.Replace(directoryName, string.Empty).Trim('/', '\\');
                    if (current == Mimetype || current == "epub.json")
                        continue;
                    zip.CreateEntryFromFile(file.FullName, current);
                }
            }

            return outputFileName;
        }

        internal static void WriteMetaElement(this XmlTextWriter writer, string name, string content)
        {
            writer.WriteStartElement("meta");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("content", content);
            writer.WriteEndElement();
        }

        internal static void WriteElementText(this XmlTextWriter writer, string nodeName, string content)
        {
            writer.WriteStartElement(nodeName);
            writer.WriteString(content);
            writer.WriteEndElement();
        }

        internal static void WriteSubTextElement(this XmlTextWriter writer, string nodeName, string content)
        {
            if (string.IsNullOrEmpty(content))
                return;
            writer.WriteStartElement(nodeName);
            writer.WriteStartElement("text");
            writer.WriteString(content);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        internal static void WriteReferenceElement(this XmlTextWriter writer, Manifest manifest, string type)
        {
            if (manifest == null)
                return;
            writer.WriteStartElement("reference");
            writer.WriteAttributeString("href", manifest.Href);
            writer.WriteAttributeString("type", type);
            writer.WriteAttributeString("title", manifest.Title);
            writer.WriteEndElement();
        }
    }

}