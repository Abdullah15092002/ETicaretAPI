using ETicaretAPI.Application.Abstractions.Storage;
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
         
        readonly IStorageService _storageService;
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        public FileUploadCommandHandler(
            IProductImageFileWriteRepository productImageFileWriteRepository,
            IStorageService storageService
            )
        {
           _productImageFileWriteRepository = productImageFileWriteRepository;
            _storageService = storageService;
        }
        public async Task<FileUploadCommandResponse> Handle(FileUploadCommandRequest request, CancellationToken cancellationToken)
        {
          var datas=  await _storageService.UploadAsync("resource/product-images", request.Files);
           await _productImageFileWriteRepository.AddRangeAsync(datas.Select(datas => new T.ProductImageFile() {
           FileName=datas.fileName,
           Path=datas.pathOrContainerName,
           Storage=_storageService.StorageName,
           }).ToList());
           await _productImageFileWriteRepository.SaveAsync();
            return new();
        }
    }
}
