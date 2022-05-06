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
    public class AggregatorController : ControllerBase
    {
        public AggregatorService _aggregatorService;

        public AggregatorController(AggregatorService aggregatorService)
        {
            _aggregatorService = aggregatorService;
        }
        [HttpPost("aggregate-data")]
        public IActionResult AggregateData()
        {
            _aggregatorService.AggregateData();
            return Ok();
        }
    }
}
