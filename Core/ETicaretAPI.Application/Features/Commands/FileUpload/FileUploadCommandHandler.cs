using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.FileUpload
{
    public class FileUploadCommandHandler : IRequestHandler<FileUploadCommandRequest, FileUploadCommandResponse>
    {
         readonly IWebHostEnvironment _webHostEnvironment;

        public FileUploadCommandHandler(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<FileUploadCommandResponse> Handle(FileUploadCommandRequest request, CancellationToken cancellationToken)
        {
            Random random = new();
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath,"resource/product-images");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            foreach (IFormFile file in request.Files) {
                string fullPath = Path.Combine(uploadPath,$"{random.Next()}{Path.GetExtension(file.FileName)}");
                using FileStream fileStream = new(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024*1024, useAsync: false);
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }
            return new();
        }
    }
}
