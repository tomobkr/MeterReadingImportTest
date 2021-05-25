using CsvHelper;
using MeterReadingImport.Domain.ViewModels.MeterReadingImport.UploadController;
using MeterReadingImport.Repository.DbContexts;
using MeterReadingImport.Service.Interfaces.MeterReadingImport;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace MeterReadingImport.Service.MeterReadingImport
{
    public class UploadService : IUploadService
    {
        public MeterReadingImportDbContext _dbcontext;
        public IConfiguration _configuration;

        public UploadService(MeterReadingImportDbContext dbcontext, IConfiguration configuration)
        {
            _dbcontext = dbcontext;
            _configuration = configuration;
        }

        public async Task<MeterReadingUploadsViewModel> UploadMeterReads(IFormFile file)
        {
            return new MeterReadingUploadsViewModel();
        }

        //Given more time, this should be seeded directly into the DB on the initial migration using the file in MeterReadingImport.Repository.SeedData.Test_Accounts.csv
        //As we can be sure of the data schema as we own the seed file, i have no entered any data integrity validations into this method.
        //To improve this method i would fix a bug with accountId being a string instead of long, and retrieve more field names & types through reflection where neccesary
        public async Task<MeterReadingUploadsViewModel> SeedAccountInformation(IFormFile file)
        {
            var dt = new DataTable();

            using (var textReader = new StreamReader(file.OpenReadStream()))
            {
                using (var csv = new CsvReader(textReader, CultureInfo.CurrentCulture))
                {
                    csv.Read();
                    csv.ReadHeader();

                    dt.Columns.Add("Id", typeof(long));
                    dt.Columns.Add("AccountId", typeof(string));
                    dt.Columns.Add("FirstName", typeof(string));
                    dt.Columns.Add("LastName", typeof(string));

                    while (csv.Read())
                    {
                        AddCSVRowToDTRow(dt, csv);
                    }

                    var bcp = new SqlBulkCopy(_configuration.GetConnectionString("DefaultConnection"))
                    {
                        DestinationTableName = "Accounts"
                    };

                    await bcp.WriteToServerAsync(dt);
                }
            }
            return null;
        }


        private static void AddCSVRowToDTRow(DataTable dt, CsvReader csv)
        {
            var row = dt.NewRow();
            foreach (DataColumn column in dt.Columns)
            {
                if (column.ColumnName == "Id")
                    continue;

                row[column.ColumnName] = csv.GetField(column.DataType, column.ColumnName);
            }
            dt.Rows.Add(row);
        }
    }
}
