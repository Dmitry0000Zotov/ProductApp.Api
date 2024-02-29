using AutoMapper;
using TestTask.Application.Common.Mapping;
using TestTask.Domain;

namespace TestTask.Application.Products.Queries.GetProductList
{
    public class ProductLookupDto : IMapWith<Product>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductLookupDto>()
                .ForMember(productDto => productDto.Id, opt => opt.MapFrom(product => product.Id))
                .ForMember(productDto => productDto.Name, opt => opt.MapFrom(product => product.Name));
        }
    }
}
