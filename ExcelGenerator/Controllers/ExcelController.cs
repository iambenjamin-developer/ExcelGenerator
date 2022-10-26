using ExcelGenerator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using static System.Net.WebRequestMethods;

namespace ExcelGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {

        [Route("import")]
        [HttpPost]
        public ActionResult Post([FromForm] ImportExcelDTO request)
        {
            if (Path.GetExtension(request.File.FileName) != ".xls" && Path.GetExtension(request.File.FileName) != ".xlsx")
            {
                return BadRequest("Solo se admiten archivos excel");
            }

            var result = ReadExcelFile(request.File);

            return Ok(result);
        }

        private List<Employee> ReadExcelFile(IFormFile file)
        {
            var employees = new List<Employee>();
            int numberOfRecords = 0;

            IWorkbook workbook = null;

            if (Path.GetExtension(file.FileName) == ".xlsx")
            {
                workbook = new XSSFWorkbook(file.OpenReadStream());
            }
            else if (Path.GetExtension(file.FileName) == ".xls")
            {
                workbook = new HSSFWorkbook(file.OpenReadStream());
            }

            //Obtener la primera hoja del libro de excel
            ISheet sheet = workbook.GetSheetAt(0);

            int nameColumnIndex = 0;
            int lastNameColumnIndex = 1;
            int salaryColumnIndex = 2;

            //Nombre de Cabeceras - Fila 1
            string nameHeaderName = "Nombre";
            string lastNameHeaderName = "Apellido";
            string salaryHeaderName = "Salario";

            for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                //Verifica si la fila tiene datos
                if (sheet.GetRow(rowIndex) != null && !string.IsNullOrEmpty(sheet.GetRow(rowIndex).GetCell(nameColumnIndex).StringCellValue))
                {
                    numberOfRecords++;

                    string nameCellValue = sheet.GetRow(rowIndex).GetCell(nameColumnIndex).StringCellValue.Trim().ToUpperInvariant();
                    string lastNameCellValue = sheet.GetRow(rowIndex).GetCell(lastNameColumnIndex).StringCellValue;
                    decimal salaryCellValue = (decimal)sheet.GetRow(rowIndex).GetCell(salaryColumnIndex).NumericCellValue;

                    employees.Add(new Employee
                    {
                        Name = nameCellValue,
                        LastName = lastNameCellValue,
                        Salary = salaryCellValue
                    });

                }
            }

            return employees;
        }


    }
}
