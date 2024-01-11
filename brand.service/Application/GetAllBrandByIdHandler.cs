using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BrandService.Entity;
using BrandService.DTO;
using BrandService.Infrastucture;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BrandService.Application
{
    public class GetAllBrandHandler : IRequestHandler<GetAllBrandsQuery, IEnumerable<BrandService.DTO.Brand>>
    {
        private BarContext barContext;

        public GetAllBrandHandler(BarContext bc)
        {
            this.barContext = bc;
        }

        public async Task<IEnumerable<BrandService.DTO.Brand>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var val = await barContext.Brands.ToListAsync();

            return val.Select(value => new BrandService.DTO.Brand
            {
                Id = value.BrandID,
                Name = value.Name
            });
        }
    }
}