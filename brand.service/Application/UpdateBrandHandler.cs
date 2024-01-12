using BrandService.Entity;
using BrandService.Infrastucture;
using MediatR;

namespace BrandService.Application
{
    public class UpdateBrandIdHandler : IRequestHandler<UpdateBrandCommand, DTO.Brand?>
    {
        private readonly BarContext barContext;

        public UpdateBrandIdHandler(BarContext bc)
        {
            this.barContext = bc;
        }

        public async Task<DTO.Brand?> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var val = await barContext.Brands.FindAsync( new object[] { request.Id  }, cancellationToken );

            if( val == null )
            {
                return null;
            }

            val.Name = request.Desc;

            await barContext.SaveChangesAsync(cancellationToken);

            return new DTO.Brand{
                Id = request.Id,
                Name = request.Desc
            };
        }
    }
}