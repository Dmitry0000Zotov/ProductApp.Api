using MediatR;
using TestTask.Domain;

namespace TestTask.Application.Products.Queries.GetProductDetailsByName
{
    public class GetProductDetailsByNameQuery : IRequest<Product>
    {
        public string Name { get; set; }
    }
}
