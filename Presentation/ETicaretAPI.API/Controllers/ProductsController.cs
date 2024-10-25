using ETicaretAPI.Application.Abstractions;
using ETicaretAPI.Application.Features.Commands.FileUpload;
using ETicaretAPI.Application.Features.Commands.Product.CreateProduct;
using ETicaretAPI.Application.Features.Commands.Product.RemoveProduct;
using ETicaretAPI.Application.Features.Commands.Product.UpdateProduct;
using ETicaretAPI.Application.Features.Queries.Product.GetAllProduct;
using ETicaretAPI.Application.Features.Queries.Product.GetByIdProduct;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameter;
using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        readonly private IProductReadRepository _productReadRepository;
        readonly private IProductWriteRepository _productWriteRepository;
        readonly IMediator _mediator;
        readonly private IWebHostEnvironment _webHostEnvironment;

        public ProductsController(
            IProductReadRepository productReadRepository,
            IProductWriteRepository productWriteRepository,
            IMediator mediator,
            IWebHostEnvironment webHostEnvironment
            )
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
            _mediator = mediator;
            _webHostEnvironment = webHostEnvironment;

        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
        {

            GetAllProductQueryResponse response = await _mediator.Send(getAllProductQueryRequest);
            return Ok(response);

        }

        [HttpGet("{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> Get([FromRoute] GetByIdProductQueryRequest getByIdProductQueryRequest) {
            GetByIdProductQueryResponse response = await _mediator.Send(getByIdProductQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> Post(CreateProductCommandRequest createProductCommandRequest)
        {
            CreateProductCommandResponse response = await _mediator.Send(createProductCommandRequest);
            return StatusCode((int)HttpStatusCode.Accepted);
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> Put([FromBody] UpdateProductCommandRequest updateProductCommandRequest)
        {
            UpdateProductCommandResponse response = await _mediator.Send(updateProductCommandRequest);
            return Ok();
        }
        [HttpDelete("{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] RemoveProductCommandRequest removeProductCommandRequest)
        {
            RemoveProductCommandResponse response = await _mediator.Send(removeProductCommandRequest);
            return Ok(response.isdeleted);
        }
        /*[HttpPost("[action]")]
         public async Task<IActionResult> Upload()
         {


             string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "resource/product-images");
             if (!Directory.Exists(uploadPath))
             {
                 Directory.CreateDirectory(uploadPath);
             }
             Random random = new();
             foreach (IFormFile file in Request.Form.Files)
             {
                 string fullPath = Path.Combine(uploadPath, $"{random.Next()}{Path.GetExtension(file.FileName)}");
                 using FileStream fileStream = new(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);
                 await file.CopyToAsync(fileStream);
                 await fileStream.FlushAsync();
             }
             return Ok();

         }*/
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload([FromForm]FileUploadCommandRequest fileUploadCommandRequest)
        {
            FileUploadCommandResponse response=await _mediator.Send(fileUploadCommandRequest);
            return Ok(response);
        }
    }
}
