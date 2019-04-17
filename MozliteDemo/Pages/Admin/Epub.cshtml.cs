using System;
using System.IO;
using System.Text;
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

        public async Task<IActionResult> OnPostUpload(IFormFile file, string bookid)
        {
            var epub = _epubManager.Create(bookid);
            var info = await _storageDirectory.SaveToTempAsync(file);
            epub.AddFile(file.FileName, info.FullName);
            return Success("成功上传了文件！");
        }

        public async Task<IActionResult> OnPostLoadAsync(string bookid, string file, string encoding)
        {
            var epub = await _epubManager.LoadFileAsync(bookid, file ?? "8b7ff96e-90a5-4c00-9635-52dc0d6043a1", Encoding.GetEncoding(encoding ?? "gb2312"));
            epub.AddDefaultStyle();
            epub.AddToc();
            return Success("成功加载文件！");
        }

        public IActionResult OnPost()
        {
            var epub = _epubManager.Create(Input.BookId);
            var epubFile = (EpubFile)epub;
            epubFile.DC = Input.DC;
            epubFile.Manifest = Input.Manifest;
            epubFile.Metadata = Input.Metadata;
            epub.Save();
            return SuccessPage("成功保存！");
        }

        public IActionResult OnPostDelete(string bookid, string file)
        {
            var epub = _epubManager.Create(bookid);
            epub.RemoveFile(file);
            return Success("移除成功！");
        }

        public IActionResult OnPostSave(string bookid, string file)
        {
            var epub = _epubManager.Create(bookid);
            epub.Compile("xx.epub");
            Epubs.Compile(_storageDirectory.GetTempPath("1"), "1.epub");
            return Success("生成成功！");
        }
    }
}