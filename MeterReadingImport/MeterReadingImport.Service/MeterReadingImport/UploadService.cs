using MeterReadingImport.Domain.ViewModels.MeterReadingImport.UploadController;
using MeterReadingImport.Repository.DbContexts;
using MeterReadingImport.Service.Interfaces.MeterReadingImport;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeterReadingImport.Service.MeterReadingImport
{
    public class UploadService : IUploadService
    {
        public MeterReadingImportDbContext _dbcontext;

        public UploadService(MeterReadingImportDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public MeterReadingUploadsViewModel UploadMeterReads()
        {
            return null;
        }
    }
}
