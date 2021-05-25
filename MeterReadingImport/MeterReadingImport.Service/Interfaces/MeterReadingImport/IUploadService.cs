using MeterReadingImport.Domain.ViewModels.MeterReadingImport.UploadController;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MeterReadingImport.Service.Interfaces.MeterReadingImport
{
    public interface IUploadService
    {
        Task<MeterReadingUploadsViewModel> UploadMeterReads(IFormFile file);
        Task<MeterReadingUploadsViewModel> SeedAccountInformation(IFormFile file);

    }
}
