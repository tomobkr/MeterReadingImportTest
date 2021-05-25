using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MeterReadingImport.Domain.Entities.MeterReadingImport
{
    public class MeterReading
    {
        [Key]
        public long Id { get; set; }
        public DateTime MeterReadingDate { get; set; }
        public float MeterReadValue { get; set; }
        public long AccountId { get; set; }
    }
}
