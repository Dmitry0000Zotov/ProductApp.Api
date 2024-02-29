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

namespace TestTask.Application.Products.Queries.GetProductDetailsByName
{
    public class GetProductDetailsByNameQueryHandler : IRequestHandler<GetProductDetailsByNameQuery, Product>
    {
        private readonly ITestDbContext _context;

        public GetProductDetailsByNameQueryHandler(ITestDbContext context) => _context = context;

        public async Task<Product> Handle(GetProductDetailsByNameQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Products.FirstOrDefaultAsync(product => product.Name == request.Name, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Product), request.Name);
            }

            return entity;
        }
    }
}
