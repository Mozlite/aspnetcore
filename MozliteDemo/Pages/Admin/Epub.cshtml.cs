using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mozlite.Extensions.Storages;
using Mozlite.Extensions.Storages.Epub;
using MozliteDemo.Extensions;

namespace MozliteDemo.Pages.Admin
{
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
            if (file.FileName.IsPictureFile())
            {
                var url = epub.AddCover(info.FullName, Path.GetExtension(file.FileName));
                info.Delete();
                return Success(new { url });
            }

            epub.AddFile(file.FileName, info.FullName);
            return Success();
        }

        public IActionResult OnPost()
        {
            var epub = _epubManager.Create(Input.BookId);
            epub.Compile(false);
            return SuccessPage("成功生成！");
        }
    }
}