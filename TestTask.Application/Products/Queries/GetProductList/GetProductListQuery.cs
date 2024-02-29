using MediatR;

namespace TestTask.Application.Products.Queries.GetProductList
{
    public class GetProductListQuery : IRequest<ProductListVm>
    {
        public string? Name { get; set; }
    }
}
