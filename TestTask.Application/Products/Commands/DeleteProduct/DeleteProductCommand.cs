using MediatR;

namespace TestTask.Application.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
    }
}
