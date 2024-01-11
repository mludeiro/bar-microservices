using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BrandService.Entity;
using Microsoft.AspNetCore.Mvc;

namespace BrandService.Controllers
{
    [Route("/home")]
    public class RootController : ControllerBase
    {
        private BarContext bar;

        public RootController(BarContext bar)
        {
            this.bar = bar;
        }

        [HttpGet]
        [ProducesResponseType(typeof(JsonResult), 200)]
        public IActionResult Get()
        {
            // IEnumerable<Beer> enume = bar.Beers;

            // enume = enume.Where(x => x.BeerId == 2);

            // var b = bar.Beers.ToList();

            // var max = bar.Beers.Count();
            // bar.Beers.Add( new Beer() { BeerId = max + 1, BrandID = 1, Name = $"Cerveza {max+1}"});
            // bar.SaveChanges();
            var beers = bar.Beers.ToList();
            return Ok(new { Success = true, Beers = beers });
        }
    }
}
