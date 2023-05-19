using HOTPExample.Application.HOTP;
using Microsoft.AspNetCore.Mvc;

namespace HOTPExample.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class OtpController : Controller
    {   
        private readonly IHOTPGeneratorService _generatorService;

        public OtpController(IHOTPGeneratorService generatorService)
        {
            _generatorService = generatorService;
        }

        [HttpGet("GetOtp")]
        public IActionResult GetOTP()
        {
            string otp = _generatorService.GenerateOTP();
            return Ok(otp);
        }
    }
}
