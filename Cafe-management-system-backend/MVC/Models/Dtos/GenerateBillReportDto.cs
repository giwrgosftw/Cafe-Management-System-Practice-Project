using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Models.Dtos
{
    public class GenerateBillReportDto
    {
        public Bill Bill { get; set; }
        public List<Product> Products { get; set; }
    }
}