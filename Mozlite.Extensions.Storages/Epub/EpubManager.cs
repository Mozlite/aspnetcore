using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Collections.Generic;

namespace Mozlite.Extensions.Storages.Epub
{
    /// <summary>
    /// Epub管理实现类。
    /// </summary>
    public class EpubManager : IEpubManager
    {
        private readonly IStorageDirectory _storageDirectory;
        public EpubManager(IStorageDirectory storageDirectory)
        {
            _storageDirectory = storageDirectory;
        }

        private string GetEpubPath(string bookId)
        {
            var path = _storageDirectory.GetTempPath(bookId);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// 加载电子书解压后得物理文件夹。
        /// </summary>
        /// <param name="bookId">电子书Id。</param>
        /// <returns>返回电子书组成配置文件实例。</returns>
        public IEpubFile Create(string bookId)
        {
            var path = GetEpubPath(bookId);
            var jsonPath = Path.Combine(path, "epub.json");
            if (File.Exists(jsonPath))
            {
                var file = Cores.FromJsonString<EpubFile>(StorageHelper.ReadText(jsonPath));
                if (file != null)
                    return new EpubFileEntry(file, path);
            }
            var instance = new EpubFile { BookId = bookId };
            StorageHelper.SaveText(jsonPath, instance.ToJsonString());
            return new EpubFileEntry(instance, path);
        }

        private class EpubFileEntry : EpubFile, IEpubFile
        {
            public EpubFileEntry(EpubFile file, string directoryName)
            {
                _directoryName = directoryName;
                BookId = file.BookId;
                DC = file.DC;
                Metadata = file.Metadata;
                Manifest = file.Manifest;
            }

            /// <summary>
            /// 文件夹物理路径。
            /// </summary>
            private readonly string _directoryName;
            private void SaveFile(string fileName, string text)
            {
                var path = Path.Combine(_directoryName, fileName);
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                StorageHelper.SaveText(path, text);
            }

            private void Refresh() => StorageHelper.SaveText(Path.Combine(_directoryName, "epub.json"), this.ToJsonString());

            private Manifest GetOrCreate(string fileName)
            {
                if (Manifest == null) Manifest = new List<Manifest>();
                var manifest = Manifest.SingleOrDefault(x => x.Href.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                if (manifest == null)
                {
                    manifest = new Manifest { Href = fileName };
                    Manifest.Add(manifest);
                }

                return manifest;
            }

            /// <summary>
            /// 添加一个文本文件。
            /// </summary>
            /// <param name="fileName">文件名称。</param>
            /// <param name="content">文件内容。</param>
            /// <param name="title">标题，如果是文档文件需要标题。</param>
            /// <param name="isSpine">是否为文件档案。</param>
            public void AddContent(string fileName, string content, string title, bool isSpine = true)
            {
                if (isSpine && string.IsNullOrEmpty(title))
                    throw new ArgumentNullException(nameof(title));

                var manifest = GetOrCreate(fileName);
                manifest.IsSpine = isSpine;
                manifest.Title = title;
                SaveFile($"OEBPS/{fileName}", content);
                Refresh();
            }

            /// <summary>
            /// 添加一个文件。
            /// </summary>
            /// <param name="fileName">文件名称。</param>
            /// <param name="path">物理路径。</param>
            public void AddFile(string fileName, string path)
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException("文件不存在！", path);

                var manifest = GetOrCreate(fileName);
                manifest.IsSpine = false;
                Copy(path, fileName);
            }

            /// <summary>
            /// 添加一个文件。
            /// </summary>
            /// <param name="fileName">文件名称。</param>
            /// <param name="path">物理路径。</param>
            /// <param name="title">标题，如果是文档文件需要标题。</param>
            public void AddFile(string fileName, string path, string title)
            {
                if (string.IsNullOrEmpty(title))
                    throw new ArgumentNullException(nameof(title));
                if (!File.Exists(path))
                    throw new FileNotFoundException("文件不存在！", path);

                var manifest = GetOrCreate(fileName);
                manifest.Title = title;
                manifest.IsSpine = true;
                Copy(path, fileName);
            }

            private void Copy(string path, string fileName)
            {
                var filePath = Path.Combine(_directoryName, "OEBPS", fileName);
                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.Copy(path, filePath, true);
                Refresh();
            }

            /// <summary>
            /// 附加元数据。
            /// </summary>
            /// <param name="parentElement">父级节点。</param>
            private void AppendMetadata(XmlElement parentElement)
            {
                var doc = parentElement.OwnerDocument;
                var metadata = doc.CreateElement("metadata");
                parentElement.AppendChild(metadata);
                foreach (var property in typeof(DublinCore).GetEntityType().GetProperties())
                {
                    var value = property.Get(DC);
                    if (value != null)
                    {
                        var item = doc.CreateElement("dc:" + property.Name.ToLower());
                        item.InnerText = value.ToString();
                        metadata.AppendChild(item);
                    }
                }

                //默认首页和图片
                var coverImage = Manifest.FirstOrDefault(x => x.IsCover && !x.IsSpine);
                var coverHtml = Manifest.FirstOrDefault(x => x.IsCover && x.IsSpine);
                if (coverImage != null)
                    Metadata[coverHtml?.Href ?? "conver.xhtml"] = coverImage.Href;
                foreach (var data in Metadata)
                {
                    var meta = doc.CreateElement("meta");
                    meta.SetAttribute("name", data.Key);
                    meta.SetAttribute("content", data.Value);
                    metadata.AppendChild(meta);
                }
            }

            /// <summary>
            /// 保存OPF文件。
            /// </summary>
            private void SaveOPF()
            {
                //指向OPF文件
                SaveFile("META-INF/container.xml", EpubConstants.Container);
                //生成OPF文件
                var filePath = Path.Combine(_directoryName, "OEBPS/content.opf");
                if (File.Exists(filePath)) File.Delete(filePath);
                var doc = new XmlDocument();
                doc.CreateXmlDeclaration("1.0", "utf-8", null);
                var documentElement = doc.CreateElement("package", "http://www.idpf.org/2007/opf");
                documentElement.SetAttribute("xmlns:dc", "http://purl.org/dc/elements/1.1/");
                documentElement.SetAttribute("unique-identifier", "urn:uuid:" + BookId);
                documentElement.SetAttribute("version", "2.0");
                doc.AppendChild(documentElement);
                AppendMetadata(documentElement);
                //文件
                var manifestElement = doc.CreateElement("manifest");
                documentElement.AppendChild(manifestElement);
                foreach (var manifest in Manifest)
                {
                    var element = doc.CreateElement("item");
                    element.SetAttribute("id", manifest.Id);
                    element.SetAttribute("href", manifest.Href);
                    element.SetAttribute("media-type", manifest.MediaType);
                    manifestElement.AppendChild(element);
                }
                //档案文件
                var spineElement = doc.CreateElement("spine");
                spineElement.SetAttribute("toc", "toc.ncx");//NCX文件Id
                documentElement.AppendChild(spineElement);
                var manifests = LoadSortManifest();
                foreach (var manifest in manifests)
                {
                    var element = doc.CreateElement("itemref");
                    element.SetAttribute("idref", manifest.Id);
                    if (manifest.IsCover)
                        element.SetAttribute("linear", "no");
                    spineElement.AppendChild(element);
                }
                //导航
                var coverFile = Manifest.FirstOrDefault(x => x.IsCover && x.IsSpine);
                if (coverFile != null)
                {
                    var guideElement = doc.CreateElement("guide");
                    documentElement.AppendChild(guideElement);
                    var cover = doc.CreateElement("reference");
                    cover.SetAttribute("href", coverFile.Href);
                    cover.SetAttribute("type", "cover");
                    cover.SetAttribute("title", "封面");
                    guideElement.AppendChild(cover);
                }

                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                doc.Save(filePath);
            }

            private List<Manifest> LoadSortManifest()
            {
                var manifests = Manifest.Where(x => x.IsSpine).OrderBy(x => x.Id).ToList();
                var toc = manifests.FirstOrDefault(x => x.IsToc);
                if (toc != null)
                {
                    manifests.Remove(toc);
                    manifests.Insert(0, toc);
                }
                var cover = manifests.FirstOrDefault(x => x.IsCover);
                if (cover != null)
                {
                    manifests.Remove(cover);
                    manifests.Insert(0, cover);
                }

                return manifests;
            }

            /// <summary>
            /// 保存NCX文件。
            /// </summary>
            private void SaveNCX()
            {
                //生成toc.ncx文件
                var ma = Manifest.FirstOrDefault(x => x.Href == "toc.ncx");
                if (ma == null)
                {
                    Manifest.Add(new Manifest { Href = "toc.ncx" });
                }
                var filePath = Path.Combine(_directoryName, "OEBPS/toc.ncx");
                if (File.Exists(filePath)) File.Delete(filePath);
                var doc = new XmlDocument();
                doc.CreateXmlDeclaration("1.0", "utf-8", null);
                var docType = doc.CreateDocumentType("ncx", "-//NISO//DTD ncx 2005-1//EN",
                    "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd", null);
                doc.AppendChild(docType);
                var documentElement = doc.CreateElement("ncx", "http://www.daisy.org/z3986/2005/ncx/");
                documentElement.SetAttribute("version", "2005-1");
                doc.AppendChild(documentElement);
                //head
                var head = doc.CreateElement("head");
                documentElement.AppendChild(head);
                //uid： 数字图书的惟一 ID。该元素应该和 OPF 文件中的 dc:identifier 对应。
                //depth：反映目录表中层次的深度。该例只有一层，因此是 1。
                //totalPageCount 和 maxPageNumber：仅用于纸质图书，保留 0 即可。
                head.InnerXml = $@"<meta name=""dtb:uid"" content=""urn:uuid:{BookId}""/>
    <meta name=""dtb:depth"" content=""2""/>
    <meta name=""dtb:totalPageCount"" content=""0""/>
    <meta name=""dtb:maxPageNumber"" content=""0""/>";

                //title
                var title = doc.CreateElement("docTitle");
                documentElement.AppendChild(title);
                var text = doc.CreateElement("text");
                text.InnerXml = DC.Title;
                title.AppendChild(text);

                //creator
                if (!string.IsNullOrEmpty(DC.Creator))
                {
                    var author = doc.CreateElement("docAuthor");
                    documentElement.AppendChild(author);
                    text = doc.CreateElement("text");
                    text.InnerXml = DC.Creator;
                    author.AppendChild(text);
                }

                //导航
                var navMap = doc.CreateElement("navMap");
                documentElement.AppendChild(navMap);

                var i = 1;
                var manifests = LoadSortManifest();
                foreach (var manifest in manifests)
                {
                    var navPoint = doc.CreateElement("navPoint");
                    var navLabel = doc.CreateElement("navLabel");
                    navPoint.AppendChild(navLabel);
                    var textNode = doc.CreateElement("text");
                    textNode.InnerXml = manifest.Title;
                    navLabel.AppendChild(textNode);
                    //内容文件
                    var content = doc.CreateElement("content");
                    navPoint.AppendChild(content);
                    content.SetAttribute("src", manifest.Href);
                    navMap.AppendChild(navPoint);
                    navPoint.SetAttribute("id", $"navpoint-{i}");
                    navPoint.SetAttribute("playOrder", i.ToString());
                    i++;
                }

                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                doc.Save(filePath);
            }

            /// <summary>
            /// 编译成Epub文件，并返回物理路径。
            /// </summary>
            /// <param name="fileName">生成得文件路径。</param>
            public void Save(string fileName)
            {
                if (DC == null)
                    DC = new DublinCore();
                if (Metadata == null)
                    Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (Manifest == null)
                    Manifest = new List<Manifest>();
                SaveCover();
                SaveNCX();
                SaveOPF();
                if (File.Exists(fileName))
                    File.Delete(fileName);
                var path = "../" + BookId;
                Compile(path);
                File.Move(path, fileName);
            }

            /// <summary>
            /// 编译成Epub文件，并返回物理路径。
            /// </summary>
            /// <param name="fileName">生成得文件路径。</param>
            public void Compile(string fileName)
            {
                SaveFile("mimetype", "application/epub+zip");
                if (fileName.IndexOf(":\\") == -1)
                    fileName = Path.Combine(_directoryName, "../", fileName);
                if (!fileName.EndsWith(".epub", StringComparison.OrdinalIgnoreCase))
                    fileName += ".epub";
                if (File.Exists(fileName)) File.Delete(fileName);
                using (var zip = ZipFile.Open(fileName, ZipArchiveMode.Create, Encoding.UTF8))
                {
                    zip.CreateEntryFromFile(Path.Combine(_directoryName, "mimetype"), "mimetype", CompressionLevel.NoCompression);
                    foreach (var file in new DirectoryInfo(_directoryName).EnumerateFiles("*.*", SearchOption.AllDirectories))
                    {
                        var current = GetFileName(file.FullName);
                        if (current == "mimetype" || current == "epub.json")
                            continue;
                        zip.CreateEntryFromFile(file.FullName, current);
                    }
                }
            }

            /// <summary>
            /// 移除文件。
            /// </summary>
            /// <param name="fileName">文件名。</param>
            public void RemoveFile(string fileName)
            {
                var manifest = Manifest.SingleOrDefault(x => x.Href == fileName);
                if (manifest == null)
                    return;
                Manifest.Remove(manifest);
                Refresh();
            }

            /// <summary>
            /// 添加默认样式文件。
            /// </summary>
            public void AddDefaultStyle()
            {
                var mani = Manifest.FirstOrDefault(x => x.Href == "stylesheet.css");
                if (mani != null)
                    return;
                mani = new Manifest { Href = "stylesheet.css" };
                Manifest.Add(mani);
                SaveFile("OEBPS/stylesheet.css", EpubConstants.DefaultStyle);
                Refresh();
            }

            /// <summary>
            /// 添加默认模板内容。
            /// </summary>
            /// <param name="fileName">文件名。</param>
            /// <param name="content">内容。</param>
            /// <param name="title">标题。</param>
            public void AddHtml(string fileName, string content, string title)
            {
                content = EpubConstants.Html(title, content);
                AddContent(fileName, content, title);
            }

            private string GetFileName(string fullName)
            {
                var index = fullName.IndexOf(BookId, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                    return null;
                return fullName.Substring(index + BookId.Length + 1);
            }

            private void SaveCover()
            {
                //默认首页和图片
                var coverImage = Manifest.FirstOrDefault(x => x.IsCover && !x.IsSpine);
                var coverHtml = Manifest.FirstOrDefault(x => x.IsCover && x.IsSpine);
                if (coverImage == null)
                    return;
                AddContent(coverHtml?.Href ?? "cover.xhtml", $@"<html xmlns=""http://www.w3.org/1999/xhtml"">
  <head>
    <title>""{DC.Title}""</title>
  </head>
  <body>
    <img src=""{coverImage.Href}"" alt=""{DC.Title}"" style=""width:100%;height:100%;""/>
  </body>
</html>", "封面");
            }
        }
    }
}