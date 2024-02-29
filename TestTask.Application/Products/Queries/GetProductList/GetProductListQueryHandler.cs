using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TestTask.Application.Interfaces;

namespace TestTask.Application.Products.Queries.GetProductList
{
    public class GetProductListQueryHandler : IRequestHandler<GetProductListQuery, ProductListVm>
    {
        private readonly ITestDbContext _context;
        private readonly IMapper _mapper;

        public GetProductListQueryHandler(ITestDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductListVm> Handle(GetProductListQuery request, CancellationToken cancellationToken)
        {
            var productQuery = _context.Products.ProjectTo<ProductLookupDto>(_mapper.ConfigurationProvider);
            if (!string.IsNullOrEmpty(request.Name))
            {
                productQuery = productQuery.Where(p => p.Name.Contains(request.Name));
            }

            return new ProductListVm { Products = productQuery.ToList() };
        }
    }
}
