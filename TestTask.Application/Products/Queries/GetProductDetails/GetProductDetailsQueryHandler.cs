using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask.Application.Common.Exceptions;
using TestTask.Application.Interfaces;
using TestTask.Domain;

namespace TestTask.Application.Products.Queries.GetProductDetails
{
    public class GetProductDetailsQueryHandler : IRequestHandler<GetProductDetailsQuery, Product>
    {
        private readonly ITestDbContext _context;

        public GetProductDetailsQueryHandler(ITestDbContext context) => _context = context;

        public async Task<Product> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Products.FirstOrDefaultAsync(product => product.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Product), request.Id);
            }

            return entity;
        }
    }
}
