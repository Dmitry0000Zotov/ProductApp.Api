using MediatR;
using TestTask.Domain;

namespace TestTask.Application.Products.Queries.GetProductDetails
{
    public class GetProductDetailsQuery : IRequest<Product>
    {
        public Guid Id { get; set; }
    }
}
