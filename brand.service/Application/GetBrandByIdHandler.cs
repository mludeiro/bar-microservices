using System.Threading;
using System.Threading.Tasks;
using BrandService.Entity;
using BrandService.DTO;
using BrandService.Infrastucture;
using MediatR;

namespace BrandService.Application
{
    public class GetBrandByIdHandler : IRequestHandler<GetBrandByIdQuery, BrandService.DTO.Brand?>
    {
        private BarContext barContext;

        public GetBrandByIdHandler(BarContext bc)
        {
            this.barContext = bc;
        }

        public async Task<BrandService.DTO.Brand?> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            var val = await barContext.Brands.FindAsync( new object[] { request.Id  }, cancellationToken );

            if( val == null ) return null;

            return new DTO.Brand {
                Id = val.BrandID,
                Name = val.Name
            };
        }
    }
}