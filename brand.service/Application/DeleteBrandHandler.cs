using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using BrandService.Entity;
using BrandService.DTO;
using BrandService.Infrastucture;
using MediatR;

namespace BrandService.Application
{
    public class DeleteBrandIdHandler : IRequestHandler<DeleteBrandCommand, bool>
    {
        private BarContext barContext;

        public DeleteBrandIdHandler(BarContext bc)
        {
            this.barContext = bc;
        }

        public async Task<bool> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var val = await barContext.Brands.FindAsync( new object[] { request.Id  }, cancellationToken );

            if( val == null )
            {
                return false;
            }

            barContext.Brands.Remove(val);
            await barContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}