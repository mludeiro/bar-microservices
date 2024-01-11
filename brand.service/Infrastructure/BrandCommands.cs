using BrandService.DTO;
using MediatR;

namespace BrandService.Infrastucture
{
    public record CreateBrandCommand(int Id, string Desc) : IRequest<Brand>;
    public record UpdateBrandCommand(int Id, string Desc) : IRequest<Brand>;
    public record DeleteBrandCommand(int Id) : IRequest<bool>;
}