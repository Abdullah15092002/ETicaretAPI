using ETicaretAPI.Application.Services;
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
         
        readonly IFileService _fileService;
        public FileUploadCommandHandler(IFileService fileService)
        {
           
            _fileService = fileService;
        }
        public async Task<FileUploadCommandResponse> Handle(FileUploadCommandRequest request, CancellationToken cancellationToken)
        {
            await _fileService.UploadAsync("resource/product-images", request.Files);
           
            return new();
        }
    }
}
