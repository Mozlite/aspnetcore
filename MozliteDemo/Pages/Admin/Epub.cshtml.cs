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

        /// <summary>
        /// 电子书模型。
        /// </summary>
        public class InputModel : DublinCore
        {
            /// <summary>
            /// 封面图片地址。
            /// </summary>
            public string CoverUrl { get; set; }

            /// <summary>
            /// 电子书Id。
            /// </summary>
            public string BookId { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IEpubFile EpubFile { get; private set; }

        public void OnGet(string id)
        {
            id = id ?? "1f18bd7a-63ca-460e-972f-d81a061f1c99";
            Input = new InputModel { BookId = id };
            EpubFile = _epubManager.Create(id);
        }

        public async Task<IActionResult> OnPostUpload(IFormFile file, string bookid)
        {
            EpubFile = _epubManager.Create(bookid);
            var info = await _storageDirectory.SaveToTempAsync(file);
            if (file.FileName.IsPictureFile())
            {
                var url = EpubFile.AddCover(info.FullName, Path.GetExtension(file.FileName));
                info.Delete();
                return Success(new { url });
            }

            EpubFile.AddFile(file.FileName, info.FullName);
            return Success();
        }
    }
}