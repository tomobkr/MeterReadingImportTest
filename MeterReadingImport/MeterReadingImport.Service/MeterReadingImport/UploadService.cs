using CsvHelper;
using MeterReadingImport.Domain.Entities.MeterReadingImport;
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
using System.Linq;
using System.Text.RegularExpressions;
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
            MeterReadingUploadsViewModel ResultViewModel = new MeterReadingUploadsViewModel();

            using (var textReader = new StreamReader(file.OpenReadStream()))
            {
                using (var csv = new CsvReader(textReader, CultureInfo.CurrentCulture))
                {
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        MeterReading newMeterReading = ValidateAndAddMeterReading(csv);

                        if (newMeterReading != null) 
                        {
                            Account account = _dbcontext.Accounts.FirstOrDefault(x => x.AccountId == newMeterReading.AccountId.ToString());
                            if (account == null)
                            {
                                ResultViewModel.Succesful++;
                                _dbcontext.MeterReadings.Add(newMeterReading);
                                continue;
                            }
                        }

                        ResultViewModel.Failure++;
                    }
                }
            }

            //DB Alteration and Finding should be performed in in the MeterReadingImport.Repository Project, but i am putting it here as it would take too long for this test
            await _dbcontext.SaveChangesAsync();

            return ResultViewModel;
        }

        //Given more time, this should be seeded directly into the DB on the initial migration using the file in MeterReadingImport.Repository.SeedData.Test_Accounts.csv
        //As we can be sure of the data schema as we own the seed file, i have no entered any data integrity validations into this method.
        //To improve this method i would fix a bug with accountId being a string instead of long, and retrieve more field names & types through reflection where neccesary
        public async Task SeedAccountInformation(IFormFile file)
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

                    //DB Alteration should be performed in a seperate Repository in the MeterReadingImport.Repository Project, but i am putting it here as it would take too long for this test
                    var bcp = new SqlBulkCopy(_configuration.GetConnectionString("DefaultConnection"))
                    {
                        DestinationTableName = "Accounts"
                    };

                    await bcp.WriteToServerAsync(dt);
                }
            }

            return;
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

        //I wrote this in about half an hour, As such it has multiple issues such as a redundant Datatable being used to enumerate through the possible column names/types. This should
        //be done using nothing but the CsvReader itself, but for speed it was easier this way.
        private static MeterReading ValidateAndAddMeterReading(CsvReader csv)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(long));
            dt.Columns.Add("AccountId", typeof(string));
            dt.Columns.Add("MeterReadingDateTime", typeof(string));
            dt.Columns.Add("MeterReadValue", typeof(string));

            MeterReading meterReading = new MeterReading();

            foreach (DataColumn column in dt.Columns)
            {
                switch (column.ColumnName)
                {
                    case "AccountId":
                        string fieldValue = csv.GetField(column.DataType, column.ColumnName).ToString();
                        bool AccIdParsed = long.TryParse(fieldValue, out long ParsedAccountId);
                        if (AccIdParsed)
                        {
                            meterReading.AccountId = ParsedAccountId;
                        }
                        else
                        {
                            return null;
                        }

                        break;
                    case "MeterReadingDateTime":
                        string MeterReadingDTfieldValue = csv.GetField(column.DataType, column.ColumnName).ToString();
                        bool DateTimeParsed = DateTime.TryParse(MeterReadingDTfieldValue, out DateTime ParsedMeterReadingDateTime);
                        if (DateTimeParsed)
                        {
                            meterReading.MeterReadingDate = ParsedMeterReadingDateTime;
                        }
                        else
                        {
                            return null;
                        }

                        break;
                    case "MeterReadValue":
                        string MeterReadingfieldValue = csv.GetField(column.DataType, column.ColumnName).ToString();

                        Regex MeterReadRegex = new Regex(@"^(\d){5}$");

                        if (MeterReadRegex.Match(MeterReadingfieldValue).Success)
                        {
                            meterReading.MeterReadValue = MeterReadingfieldValue;
                        }
                        else
                        {
                            return null;
                        }

                        break;
                    case "Id":
                    default:
                        break;
                }
            }

            return meterReading;
        }
    }
}