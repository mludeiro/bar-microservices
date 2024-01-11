using System.Collections.Generic;
using BrandService.DTO;
using MediatR;

namespace BrandService.Infrastucture
{
    public record GetAllBrandsQuery() : IRequest<IEnumerable<Brand>>;
    public record GetBrandByIdQuery(int Id) : IRequest<Brand>;
}
