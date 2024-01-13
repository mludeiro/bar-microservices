using BrandService.Infrastucture;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BrandService.Controllers
{
    [Route("/api/v1/brands")]
    public class BrandController : ControllerBase
    {
        private readonly IMediator mediator;

        public BrandController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(JsonResult), 200)]
        public async Task<IEnumerable<DTO.Brand>> GetAll()
        {
            return await mediator.Send(new GetAllBrandsQuery());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonResult), 200)]
        public async Task<ActionResult<DTO.Brand>> Get(int id)
        {
            var value = await mediator.Send(new GetBrandByIdQuery(id));

            if( value == null ) return NotFound();

            return value;
        }

        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), 200)]
        public async Task<ActionResult<DTO.Brand>> Create(CreateBrandCommand cmd)
        {
            var value = await mediator.Send(cmd);

            return CreatedAtAction(nameof(Get), new{ id = value.Id }, value);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(typeof(JsonResult), 200)]
        public async Task<ActionResult<DTO.Brand>> Update(int id, UpdateBrandCommand cmd)
        {
            if( id != cmd.Id ) return BadRequest();

            var value = await mediator.Send(cmd);

            if( value == null ) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonResult), 200)]
        public async Task<ActionResult<DTO.Brand>> Delete(int id)
        {
            var value = await mediator.Send(new DeleteBrandCommand(id));

            return value ? NoContent() : NotFound();
        }
    }
}
