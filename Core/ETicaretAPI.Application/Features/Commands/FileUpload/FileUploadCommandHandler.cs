using ETicaretAPI.Application.Repositories;
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
using T = ETicaretAPI.Domain.Entities;
namespace ETicaretAPI.Application.Features.Commands.FileUpload
{
    public class FileUploadCommandHandler : IRequestHandler<FileUploadCommandRequest, FileUploadCommandResponse>
    {
         
        readonly IFileService _fileService;
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        public FileUploadCommandHandler(IFileService fileService,IProductImageFileWriteRepository productImageFileWriteRepository)
        {
           _productImageFileWriteRepository = productImageFileWriteRepository;
            _fileService = fileService;
        }
        public async Task<FileUploadCommandResponse> Handle(FileUploadCommandRequest request, CancellationToken cancellationToken)
        {
          var datas=  await _fileService.UploadAsync("resource/product-images", request.Files);
           await _productImageFileWriteRepository.AddRangeAsync(datas.Select(datas => new T.ProductImageFile() {
           FileName=datas.fileName,
           Path=datas.path,
           }).ToList());
           await _productImageFileWriteRepository.SaveAsync();
            return new();
        }
    }
}
