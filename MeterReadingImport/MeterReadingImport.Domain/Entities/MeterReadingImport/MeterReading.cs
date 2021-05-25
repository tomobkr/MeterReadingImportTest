using System;
using System.ComponentModel.DataAnnotations;

namespace MeterReadingImport.Domain.Entities.MeterReadingImport
{
    public class MeterReading
    {
        [Key]
        public long Id { get; set; }
        public DateTime MeterReadingDate { get; set; }
        public string MeterReadValue { get; set; }
        public long AccountId { get; set; }
    }
}
