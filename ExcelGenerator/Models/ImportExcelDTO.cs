using Microsoft.AspNetCore.Http;

namespace ExcelGenerator.Models
{
    public class ImportExcelDTO
    {
        public IFormFile File { get; set; }
    }
}
