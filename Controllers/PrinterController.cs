using Microsoft.AspNetCore.Mvc;
using PDFtoPrinter;

namespace printer.Controllers;

[ApiController]
[Route("[controller]")]
public class PrinterController : ControllerBase
{

    private readonly ILogger _logger;

    public PrinterController(ILogger logger)
    {
        _logger = logger;
    }


    [HttpPost(Name = "Print")]
    public async Task<string> Post(IFormFile file, string printerName)
    {

        if (string.IsNullOrEmpty(printerName)) return "No printer name";

        if (file != null && file.Length > 0) // Check if a file was uploaded
        {
            string fileName = Path.GetRandomFileName() + ".pdf"; // Generate a unique file name
            string filePath = Path.Combine(Path.GetTempPath(), fileName); // Get the full path of the temporary file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream); // Save the uploaded file to the temporary file
            }

            _logger.Log(LogLevel.Information, $"File {file.FileName} has been uploaded and saved to {filePath}");

            var printer = new CleanupFilesPrinter(new PDFtoPrinterPrinter());

            printer.Print(new PrintingOptions(printerName, filePath));

            _logger.Log(LogLevel.Information, $"{filePath} is printing");

            return "OK";
        }
        return "No file was uploaded";
    }
}

