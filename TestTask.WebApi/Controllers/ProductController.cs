using Microsoft.AspNetCore.Mvc;
using TestTask.Application.Products.Commands.CreateProduct;
using TestTask.Application.Products.Commands.DeleteProduct;
using TestTask.Application.Products.Commands.UpdateProduct;
using TestTask.Application.Products.Queries.GetProductDetails;
using TestTask.Application.Products.Queries.GetProductList;
using TestTask.Domain;

namespace TestTask.WebApi.Controllers
{
    [Route("products/[controller]/[action]")]
    public class ProductController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductLookupDto>>> GetProductList(string? name)
        {
            var query = new GetProductListQuery 
            { 
                Name = name
            };
            var vm = await Mediator.Send(query);
            List<ProductLookupDto> products = new List<ProductLookupDto>();
            foreach (var product in vm.Products)
            {
                products.Add(product);
            }
            return Ok(products);
        }

        [HttpGet]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var query = new GetProductDetailsQuery
            {
                Id = id
            };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] CreateProductCommand createProductCommand)
        {
            if(!string.IsNullOrEmpty(createProductCommand.Name) && !string.Equals(createProductCommand.Name, "null"))
            {
                var product = await Mediator.Send(createProductCommand);
                return Ok(product);
            }
            return BadRequest();
            
        }

        [HttpPut]
        public async Task<ActionResult<Product>> UpdateProduct([FromBody] UpdateProductCommand updateProductCommand)
        {
            if (!string.IsNullOrEmpty(updateProductCommand.Name) && !string.Equals(updateProductCommand.Name, "null"))
            {
                var product = await Mediator.Send(updateProductCommand);
                return Ok(product);
            }
            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var command = new DeleteProductCommand
            {
                Id = id
            };
            var productId = await Mediator.Send(command);
            return Ok(productId);
        }
    }
}
