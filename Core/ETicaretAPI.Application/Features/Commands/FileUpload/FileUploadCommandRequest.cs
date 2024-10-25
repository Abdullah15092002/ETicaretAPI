using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.FileUpload
{
    public class FileUploadCommandRequest:IRequest<FileUploadCommandResponse>
    {
        [FromForm]
       public List<IFormFile> Files { get; set; }
    }
}
