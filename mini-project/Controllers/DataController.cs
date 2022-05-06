using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mini_project.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mini_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private DataService _dataService;

        public DataController(DataService dataService)
        {
            _dataService = dataService;
        }

        [EnableCors("MyPolicy")]
        [HttpGet("get-data-by-{granularity}")]
        public IActionResult GetData(string granularity)
        {
            var aggregated = _dataService.GetData(granularity);
            return Ok(aggregated);
        }

        [EnableCors("MyPolicy")]
        [HttpGet("get-data-between")]
        public IActionResult GetDataBetween(string filter, DateTime from, DateTime to)
        {
            var databetween = _dataService.GetDataBetween(filter, from, to);
            return Ok(databetween);
        }

    }



}
