using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeterReadingImport.Domain.ViewModels.MeterReadingImport.UploadController;
using MeterReadingImport.Service.Interfaces.MeterReadingImport;

namespace MeterReadingImport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        IUploadService _uploadService;

        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [Route("meter-reading-uploads")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MeterReadingUploadsViewModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportMeterReadings(IFormFile file)
        {
            var ViewModel = await _uploadService.UploadMeterReads(file);

            return Ok(new MeterReadingUploadsViewModel());
        }

        [Route("seed-accounts")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OkResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SeedAccountData(IFormFile file)
        {
            var ViewModel = await _uploadService.SeedAccountInformation(file);

            return Ok();
        }
    }
}
