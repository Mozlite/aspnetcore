using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Storages;
using Mozlite.Extensions.Storages.Epub;
using MozliteDemo.Extensions;

namespace MozliteDemo.Pages.Admin
{
    [RequestFormLimits(ValueCountLimit = 5000)]
    public class EpubModel : ModelBase
    {
        private readonly IEpubManager _epubManager;
        private readonly IStorageDirectory _storageDirectory;

        public EpubModel(IEpubManager epubManager, IStorageDirectory storageDirectory)
        {
            _epubManager = epubManager;
            _storageDirectory = storageDirectory;
        }

        [BindProperty]
        public EpubFile Input { get; set; }

        public void OnGet(string id)
        {
            id = id ?? "1f18bd7a-63ca-460e-972f-d81a061f1c99";
            Input = (EpubFile)_epubManager.Create(id);
        }

        public async Task<IActionResult> OnPostUploadAsync(IFormFile file, string bookid)
        {
            var epub = _epubManager.Create(bookid);
            if (Path.GetExtension(file.FileName).ToLower() == ".txt")
            {
                var chapters = await _epubManager.LoadFileAsync(file);
                await epub.AddChaptersAsync(chapters);
                return Success("成功上传了文件！");
            }

            var info = await _storageDirectory.SaveToTempAsync(file);
            if (file.FileName.IsPictureFile())
                await epub.AddCoverAsync(file.FileName, info.FullName);
            else
                epub.AddFile(file.FileName, info.FullName);
            return Success("成功上传了文件！");
        }

        public IActionResult OnPost()
        {
            var epub = _epubManager.Create(Input.BookId);
            var epubFile = (EpubFile)epub;
            epubFile.DC = Input.DC;
            var storeds = epubFile.Manifest.ToDictionary(x => x.Href, StringComparer.OrdinalIgnoreCase);
            if (Input.Manifest != null)
            {
                foreach (var manifest in Input.Manifest)
                {
                    if (storeds.TryGetValue(manifest.Href, out var stored))
                    {
                        //修改
                        stored.IsCover = manifest.IsCover;
                        stored.Title = manifest.Title;
                        stored.IsSpine = manifest.IsSpine;
                        stored.IsToc = manifest.IsToc;
                    }
                    else
                    {
                        epubFile.Manifest.Add(manifest);
                    }
                }
            }
            epubFile.Metadata = Input.Metadata;
            epub.Save();
            Input.Manifest = epubFile.Manifest;
            return SuccessPage("成功保存！");
        }

        public IActionResult OnPostDelete(string bookid, string file)
        {
            var epub = _epubManager.Create(bookid);
            epub.Remove(file);
            return Success("移除成功！");
        }

        public IActionResult OnPostSave()
        {
            var epub = _epubManager.Create(Input.BookId);
            epub.AddToc();
            epub.AddDefaultStyle();
            var path = _storageDirectory.GetTempPath($"{Input.BookId}.epub");
            epub.Compile(path);
            return PhysicalFile(path, ".epub".GetContentType(), $"{Input.DC.Title}.epub");
        }
    }
}