using SkyStore.Data;
using SkyStore.Interfaces;
using SkyStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace SkyStore.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileStorageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const long StorageLimit = 5L * 1024 * 1024 * 1024; // 5 GB in bytes
        private readonly IStorageFactory _storageFactory;
        private readonly List<string> _storageOrder = new() { "Azure","AWS" };

        public FileStorageController(IStorageFactory storageFactory, ApplicationDbContext context)
        {
            _storageFactory = storageFactory;
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("User must be logged in to upload files.");

            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            User? user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound("User not found.");

            var monthStorageUsed = _context.Files
                .Where(f => f.UserId == userId &&
                            f.UploadAt.Month == DateTime.Now.Month &&
                            f.UploadAt.Year == DateTime.Now.Year)
                .Sum(f => f.SizeInBytes);

            if (monthStorageUsed + file.Length > StorageLimit)
                return BadRequest("Storage limit exceeded.");

            var storeFile = new StoreFile
            {
                Id = file.FileName,
                SizeInBytes = file.Length,
                UploadAt = DateTime.UtcNow,
                UserId = userId
            };

            string filePath = string.Empty;

            foreach (var providerName in _storageOrder)
            {
                var storageProvider = _storageFactory.CreateStorage(providerName);
                try
                {
                    filePath = await storageProvider.UploadFileAsync(file,userId);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{providerName} upload failed: {ex.Message}");
                }
            }

            if (string.IsNullOrEmpty(filePath))
                return StatusCode(500, "Could not upload the file.");

            storeFile.Path = filePath;
            user.StorageUsed += storeFile.SizeInBytes;
            _context.Files.Add(storeFile);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "File uploaded successfully", Url = filePath });
        }
    }
}


