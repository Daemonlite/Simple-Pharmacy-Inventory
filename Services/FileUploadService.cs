using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using pharmacy_management.Data;
using pharmacy_management.Models;
using pharmacy_management.Entities;
using pharmacy_management.Services;

namespace pharmacy_management.Services
{
    public class FileUploadService(IWebHostEnvironment _env, IConfiguration configuration)
    {
        public async Task<FinalUploadDto> UploadPatientsFileAsync(Guid patientId, IFormFile file,string uploadLocation)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");


            var uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", $"{uploadLocation}", patientId.ToString());

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Get the base URL from configuration or request
            var baseUrl = configuration["BaseUrl"] ?? "http://localhost:5104";
            
            // Create absolute URL
            var absoluteUrl = $"{baseUrl}/uploads/patients/{patientId}/{fileName}";

            return new FinalUploadDto { AbsoluteUrl = absoluteUrl, ContentType = file.ContentType, FileSize = (int)file.Length, FileName = fileName };
        }
    }
}