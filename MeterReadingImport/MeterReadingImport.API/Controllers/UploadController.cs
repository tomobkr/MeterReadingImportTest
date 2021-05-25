using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeterReadingImport.Domain.ViewModels.MeterReadingImport.UploadController;

namespace MeterReadingImport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [Route("meter-reading-uploads")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MeterReadingUploadsViewModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ImportMeterReadings(IFormFile file)
        {
            return Ok(new MeterReadingUploadsViewModel());
        }
    }
}
