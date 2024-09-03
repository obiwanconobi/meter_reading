using ensek_test.Services;
using Microsoft.AspNetCore.Mvc;

namespace ensek_test.Controllers
{
    [ApiController]
    public class MeterReadingController : ControllerBase
    {
        private readonly IMeterReadingService _meterReadingService;

        public MeterReadingController(IMeterReadingService meterReadingService)
        {
            _meterReadingService = meterReadingService;
        }

        [HttpPost("meter-reading-uploads")]
        public async Task<IActionResult> UploadMeterReadings(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using (var stream = file.OpenReadStream())
            {
                var result = await _meterReadingService.ProcessMeterReadingsAsync(stream);
                return Ok(new { SuccessfulReadings = result.SuccessCount, FailedReadings = result.FailureCount-1 });
            }
        }

    }
}
