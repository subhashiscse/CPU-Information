using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace CPUTracking.Models.Create
{
    public class CPUAdvisor
    {
        public int ItemId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Session { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? Position { get; set; }
    }
}

