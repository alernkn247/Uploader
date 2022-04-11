using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;

namespace FUploader.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploader : Controller
    {
        private IWebHostEnvironment hostingEnv;

        public FileUploader(IWebHostEnvironment env)
        {
            this.hostingEnv = env;
        }

        [DisableRequestSizeLimit]
        [HttpPost(Name = "UploadFile")]
        public void Save(IList<IFormFile> UploadFiles)
        {
            long size = 0;
            try
            {
                foreach (var file in UploadFiles)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var filename = ContentDispositionHeaderValue
                            .Parse(file.ContentDisposition)
                            .FileName
                            .Trim('"');
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    filename = Path.Combine(hostingEnv.ContentRootPath,filename);
                    size += (int)file.Length;
                    if (!System.IO.File.Exists(filename))
                    {
                        using (FileStream fs = System.IO.File.Create(filename))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File failed to upload";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
        }
    }
}
