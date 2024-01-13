using BrandService.Entity;
using BrandService.Infrastucture;
using MediatR;

namespace BrandService.Application
{
    public class CreateBrandHandler : IRequestHandler<CreateBrandCommand, DTO.Brand>
    {
        private readonly BarContext barContext;

        public CreateBrandHandler(BarContext bc)
        {
            this.barContext = bc;
        }

        public async Task<DTO.Brand> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var val = await barContext.Brands.AddAsync(new BrandService.Entity.Brand {
                BrandID = request.Id, Name = request.Desc
            });

            await barContext.SaveChangesAsync(cancellationToken);

            return new DTO.Brand{
                Id = request.Id,
                Name = request.Desc
            };
        }
    }
}