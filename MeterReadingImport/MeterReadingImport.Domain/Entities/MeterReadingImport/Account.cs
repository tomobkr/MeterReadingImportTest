using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MeterReadingImport.Domain.Entities.MeterReadingImport
{
    public class Account
    {
        [Key]
        public long AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
