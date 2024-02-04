using BrandService.Entity;
using BrandService.Infrastucture;
using MediatR;

namespace BrandService.Application
{
    public class CreateBrandHandler : IRequestHandler<CreateBrandCommand, DTO.Brand>
    {
        private readonly BarContext BarContext;

        public CreateBrandHandler(BarContext barContext)
        {
            this.BarContext = barContext;
        }

        public async Task<DTO.Brand> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var val = await BarContext.Brands.AddAsync(new BrandService.Entity.Brand {
                BrandID = request.Id, Name = request.Desc
            });

            await BarContext.SaveChangesAsync(cancellationToken);

            return new DTO.Brand{
                Id = request.Id,
                Name = request.Desc
            };
        }
    }
}