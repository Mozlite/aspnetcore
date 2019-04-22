using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Storages.Epub
{
    internal class EpubFileEntry : EpubFile, IEpubFile
    {
        public EpubFileEntry(EpubFile file, string directoryName)
        {
            _directoryName = new DirectoryInfo(directoryName).FullName;
            _json = Path.Combine(_directoryName, "epub.json");
            BookId = file.BookId;
            DC = file.DC ?? new DublinCore();
            Metadata = file.Metadata ?? new Dictionary<string, string>();
            Manifest = file.Manifest ?? new List<Manifest>();
        }

        /// <summary>
        /// 文件夹物理路径。
        /// </summary>
        private readonly string _directoryName;
        private readonly string _json;
        private void Create(string fileName, string text)
        {
            var path = Path.Combine(_directoryName, fileName);
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            StorageHelper.SaveText(path, text);
        }

        private async Task CreateAsync(string fileName, string text)
        {
            var path = Path.Combine(_directoryName, fileName);
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            await StorageHelper.SaveTextAsync(path, text);
        }

        /// <summary>
        /// 将实例保存到JSON文件中。
        /// </summary>
        public void Save() => StorageHelper.SaveText(_json, this.ToJsonString());

        /// <summary>
        /// 将实例保存到JSON文件中。
        /// </summary>
        public Task SaveAsync() => StorageHelper.SaveTextAsync(_json, this.ToJsonString());

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
            Create(fileName, content);
            Save();
        }

        /// <summary>
        /// 添加一个文本文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="content">文件内容。</param>
        /// <param name="title">标题，如果是文档文件需要标题。</param>
        /// <param name="isSpine">是否为文件档案。</param>
        public async Task AddContentAsync(string fileName, string content, string title, bool isSpine = true)
        {
            if (isSpine && string.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));

            var manifest = GetOrCreate(fileName);
            manifest.IsSpine = isSpine;
            manifest.Title = title;
            await CreateAsync(fileName, content);
            await SaveAsync();
        }

        /// <summary>
        /// 添加一个文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="path">物理路径。</param>
        public void AddFile(string fileName, string path) => AddFile(fileName, path, null);

        /// <summary>
        /// 添加一个文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="path">物理路径。</param>
        /// <param name="title">标题，如果是文档文件需要标题。</param>
        public void AddFile(string fileName, string path, string title)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("文件不存在！", path);

            var manifest = GetOrCreate(fileName);
            manifest.Title = title;
            manifest.IsSpine = !string.IsNullOrEmpty(title);
            var filePath = Path.Combine(_directoryName, fileName).MakeDirectory();
            File.Copy(path, filePath, true);
        }

        /// <summary>
        /// 保存OPF文件。
        /// </summary>
        private void SaveOPF()
        {
            //生成OPF文件
            var filePath = Path.Combine(_directoryName, "content.opf").DeleteFile();
            using (var writer = new XmlTextWriter(filePath, Encoding.UTF8))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("package", "http://www.idpf.org/2007/opf");
                writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
                writer.WriteAttributeString("unique-identifier", BookId);
                writer.WriteAttributeString("version", "2.0");
                //metadata
                writer.WriteStartElement("metadata");
                writer.WriteAttributeString("xmlns:opf", "http://www.idpf.org/2007/opf");
                writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
                foreach (var property in typeof(DublinCore).GetEntityType().GetProperties())
                {
                    var value = property.Get(DC)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                        writer.WriteElementText($"dc:{property.Name.ToLower()}", value);
                }
                //默认首页和图片
                var coverImage = Manifest.FirstOrDefault(x => x.IsCover && !x.IsSpine);
                if (coverImage != null)
                    Metadata["cover"] = coverImage.Id;
                foreach (var data in Metadata)
                    writer.WriteMetaElement(data.Key, data.Value);
                writer.WriteEndElement();
                //manifest
                writer.WriteStartElement("manifest");
                foreach (var manifest in Manifest)
                {
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("id", manifest.Id);
                    writer.WriteAttributeString("href", manifest.Href);
                    writer.WriteAttributeString("media-type", manifest.MediaType);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                //spine
                writer.WriteStartElement("spine");
                writer.WriteAttributeString("toc", "ncx");//NCX文件Id
                var manifests = LoadSortManifest();
                foreach (var manifest in manifests)
                {
                    writer.WriteStartElement("itemref");
                    writer.WriteAttributeString("idref", manifest.Id);
                    if (manifest.IsCover)
                        writer.WriteAttributeString("linear", "no");
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                //guide
                writer.WriteStartElement("guide");
                //var guide = manifests.FirstOrDefault(x => x.IsCover);
                //writer.WriteReferenceElement(guide, "cover");
                var guide = manifests.FirstOrDefault(x => x.IsToc);
                writer.WriteReferenceElement(guide, "toc");
                writer.WriteEndElement();
                //end
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
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
                Manifest.Add(new Manifest { Href = "toc.ncx", Id = "ncx" });
            var filePath = Path.Combine(_directoryName, "toc.ncx").DeleteFile();
            using (var writer = new XmlTextWriter(filePath, Encoding.UTF8))
            {
                writer.WriteStartDocument();
                writer.WriteDocType("ncx", "-//NISO//DTD ncx 2005-1//EN", "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd", null);
                writer.WriteStartElement("ncx", "http://www.daisy.org/z3986/2005/ncx/");
                writer.WriteAttributeString("version", "2005-1");
                //head
                //uid： 数字图书的惟一 ID。该元素应该和 OPF 文件中的 dc:identifier 对应。
                //depth：反映目录表中层次的深度。该例只有一层，因此是 1。
                //totalPageCount 和 maxPageNumber：仅用于纸质图书，保留 0 即可。
                writer.WriteStartElement("head");
                writer.WriteMetaElement("dtb:uid", BookId);
                writer.WriteMetaElement("dtb:depth", "2");
                writer.WriteMetaElement("dtb:totalPageCount", "0");
                writer.WriteMetaElement("dtb:maxPageNumber", "0");
                writer.WriteEndElement();
                //doc
                writer.WriteSubTextElement("docTitle", DC.Title);
                writer.WriteSubTextElement("docAuthor", DC.Creator);
                //navMap
                writer.WriteStartElement("navMap");
                var i = 1;
                var manifests = Manifest.Where(x => x.IsSpine && !x.IsCover && !x.IsToc).OrderBy(x => x.Id).ToList();
                foreach (var manifest in manifests)
                {
                    writer.WriteStartElement("navPoint");
                    writer.WriteAttributeString("id", $"nav-{i}");
                    writer.WriteAttributeString("playOrder", i.ToString());
                    writer.WriteSubTextElement("navLabel", manifest.Title);
                    writer.WriteStartElement("content");
                    writer.WriteAttributeString("src", manifest.Href);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    i++;
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// 编译成Epub文件，并返回物理路径。
        /// </summary>
        /// <param name="fileName">生成得文件路径。</param>
        public void Compile(string fileName)
        {
            //指向OPF文件
            Create(EpubSettings.ContainerFile, EpubSettings.Container);
            SaveNCX();
            SaveOPF();
            var path = EpubSettings.Compile(_directoryName, "../" + BookId);
            fileName = fileName.MapPath(_directoryName + "/../").DeleteFile().MakeDirectory();
            File.Move(path, fileName);
            Directory.Delete(_directoryName, true);
        }

        /// <summary>
        /// 编译成Epub文件，并返回物理路径。
        /// </summary>
        /// <param name="fileName">生成得文件路径。</param>
        public async Task CompileAsync(string fileName)
        {
            //指向OPF文件
            await CreateAsync(EpubSettings.ContainerFile, EpubSettings.Container);
            SaveNCX();
            SaveOPF();
            var path = EpubSettings.Compile(_directoryName, "../" + BookId);
            fileName = fileName.MapPath(_directoryName + "/../").DeleteFile().MakeDirectory();
            File.Move(path, fileName);
            Directory.Delete(_directoryName, true);
        }

        /// <summary>
        /// 移除文件。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        public void Remove(string fileName)
        {
            var manifest = Manifest.SingleOrDefault(x => x.Href == fileName);
            if (manifest == null)
                return;
            Path.Combine(_directoryName, fileName).DeleteFile();
            Manifest.Remove(manifest);
            Save();
        }

        /// <summary>
        /// 移除文件。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        public async Task RemoveAsync(string fileName)
        {
            var manifest = Manifest.SingleOrDefault(x => x.Href == fileName);
            if (manifest == null)
                return;
            Path.Combine(_directoryName, fileName).DeleteFile();
            Manifest.Remove(manifest);
            await SaveAsync();
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
            Create("stylesheet.css", EpubSettings.DefaultStyle);
            Save();
        }

        /// <summary>
        /// 添加默认样式文件。
        /// </summary>
        public async Task AddDefaultStyleAsync()
        {
            var mani = Manifest.FirstOrDefault(x => x.Href == "stylesheet.css");
            if (mani != null)
                return;
            mani = new Manifest { Href = "stylesheet.css" };
            Manifest.Add(mani);
            await CreateAsync("stylesheet.css", EpubSettings.DefaultStyle);
            await SaveAsync();
        }

        /// <summary>
        /// 添加目录页面。
        /// </summary>
        public void AddToc()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("<h1>目录</h1>");
            builder.Append("<ul>");
            var manifests = Manifest.Where(x => x.IsSpine && !x.IsCover && !x.IsToc).OrderBy(x => x.Id).ToList();
            foreach (var manifest in manifests)
            {
                builder.AppendFormat("<li class=\"chapter\"><a href=\"{0}\">{1}</a></li>", manifest.Href,
                    manifest.Title);
            }
            builder.Append("</ul>");
            AddHtml(EpubSettings.TocFile, builder.ToString(), "目录");
            var current = GetOrCreate(EpubSettings.TocFile);
            current.IsSpine = true;
            current.Title = "目录";
            current.IsToc = true;
            Create($"{current.Href}", EpubSettings.Html(current.Title, builder.ToString()));
            Save();
        }

        /// <summary>
        /// 添加目录页面。
        /// </summary>
        public async Task AddTocAsync()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("<h1>目录</h1>");
            builder.Append("<ul>");
            var manifests = Manifest.Where(x => x.IsSpine && !x.IsCover && !x.IsToc).OrderBy(x => x.Id).ToList();
            foreach (var manifest in manifests)
            {
                builder.AppendFormat("<li class=\"chapter\"><a href=\"{0}\">{1}</a></li>", manifest.Href,
                    manifest.Title);
            }
            builder.Append("</ul>");
            await AddHtmlAsync(EpubSettings.TocFile, builder.ToString(), "目录");
            var current = GetOrCreate(EpubSettings.TocFile);
            current.IsSpine = true;
            current.Title = "目录";
            current.IsToc = true;
            await CreateAsync($"{current.Href}", EpubSettings.Html(current.Title, builder.ToString()));
            await SaveAsync();
        }

        /// <summary>
        /// 添加默认模板内容。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        /// <param name="content">内容。</param>
        /// <param name="title">标题。</param>
        public void AddHtml(string fileName, string content, string title)
        {
            content = $"<h2><span style=\"border-bottom:1px solid\">正文 {title}</span></h2>" + content;
            content = EpubSettings.Html(title, content);
            AddContent(fileName, content, title);
        }

        /// <summary>
        /// 添加默认模板内容。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        /// <param name="content">内容。</param>
        /// <param name="title">标题。</param>
        public Task AddHtmlAsync(string fileName, string content, string title)
        {
            content = $"<h2><span style=\"border-bottom:1px solid\">正文 {title}</span></h2>" + content;
            content = EpubSettings.Html(title, content);
            return AddContentAsync(fileName, content, title);
        }

        /// <summary>
        /// 添加章节内容。
        /// </summary>
        /// <param name="chapters">章节内容。</param>
        public void AddChapters(IDictionary<string, string> chapters)
        {
            if (chapters == null || chapters.Count == 0)
                return;

            var i = 1;
            foreach (var chapter in chapters)
            {
                AddHtml(i.ToString("D6") + ".xhtml", chapter.Value, chapter.Key);
                i++;
            }
        }

        /// <summary>
        /// 添加章节内容。
        /// </summary>
        /// <param name="chapters">章节内容。</param>
        public async Task AddChaptersAsync(IDictionary<string, string> chapters)
        {
            if (chapters == null || chapters.Count == 0)
                return;

            var i = 1;
            foreach (var chapter in chapters)
            {
                await AddHtmlAsync(i.ToString("D6") + ".xhtml", chapter.Value, chapter.Key);
                i++;
            }
        }

        /// <summary>
        /// 添加一个文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="path">物理路径。</param>
        public void AddCover(string fileName, string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("文件不存在！", path);

            var isPicture = fileName.IsPictureFile();
            var stored = isPicture ? Manifest.FirstOrDefault(x => x.IsCover && !x.IsSpine) : Manifest.FirstOrDefault(x => x.IsCover && x.IsSpine);
            if (stored != null)
            {//删除文件
                Manifest.Remove(stored);
                Path.Combine(_directoryName, stored.Href).DeleteFile();
            }

            var manifest = CreateCover(fileName, path, isPicture);
            if (isPicture)
            {//图片默认添加封面
                stored = Manifest.FirstOrDefault(x => x.IsCover && x.IsSpine);
                if (stored == null)
                {
                    stored = CreateCover();
                    Create(stored.Href, $@"<html xmlns=""http://www.w3.org/1999/xhtml""><head><title>""{DC.Title}""</title></head><body><img src=""{manifest.Href}"" alt=""{DC.Title}"" style=""width:100%;height:100%;""/></body></html>");
                }
            }
            Save();
        }

        private Manifest CreateCover(string fileName, string path, bool isPicture)
        {
            fileName = $"cover{Path.GetExtension(fileName)}";
            var manifest = GetOrCreate(fileName);
            manifest.IsSpine = !isPicture;
            manifest.Title = manifest.IsSpine ? "封面" : null;
            manifest.IsCover = true;
            manifest.Id = isPicture ? "cover-image" : "cover";
            var filePath = Path.Combine(_directoryName, fileName).MakeDirectory();
            File.Copy(path, filePath, true);
            return manifest;
        }

        private Manifest CreateCover()
        {
           var cover = GetOrCreate("cover.xhtml");
            cover.IsSpine = true;
            cover.Title = "封面";
            cover.IsCover = true;
            cover.Id = "cover";
            return cover;
        }

        /// <summary>
        /// 添加一个文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="path">物理路径。</param>
        public async Task AddCoverAsync(string fileName, string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("文件不存在！", path);

            var isPicture = fileName.IsPictureFile();
            var stored = isPicture ? Manifest.FirstOrDefault(x => x.IsCover && !x.IsSpine) : Manifest.FirstOrDefault(x => x.IsCover && x.IsSpine);
            if (stored != null)
            {//删除文件
                Manifest.Remove(stored);
                Path.Combine(_directoryName, stored.Href).DeleteFile();
            }

            var manifest = CreateCover(fileName, path, isPicture);
            if (isPicture)
            {//图片默认添加封面
                stored = Manifest.FirstOrDefault(x => x.IsCover && x.IsSpine);
                if (stored == null)
                {
                    stored = CreateCover();
                    await CreateAsync(stored.Href, $@"<html xmlns=""http://www.w3.org/1999/xhtml""><head><title>""{DC.Title}""</title></head><body><img src=""{manifest.Href}"" alt=""{DC.Title}"" style=""width:100%;height:100%;""/></body></html>");
                }
            }
            await SaveAsync();
        }
    }
}