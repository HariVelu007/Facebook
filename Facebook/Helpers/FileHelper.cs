using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Helpers
{
    public class FileHelper
    {
        public static async Task<string> UploadFile(string URL,int userid,IFormFile file,int mode=0)
        {
            string fileFullName = "";            
            bool basePathExists = Directory.Exists(URL);
            if (!basePathExists)
                Directory.CreateDirectory(URL);
            //var fileName = Path.GetFileNameWithoutExtension(viewModel.PostImg.FileName);
            var filePath = Path.Combine(URL, file.FileName);
            fileFullName = Path.GetFileName(file.FileName);
            if (!File.Exists(filePath))
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return mode==1? $@"~/ProfImg/{ fileFullName }" : $@"~/PostedImg/{ userid }/{ fileFullName }";
        }
    }
}
