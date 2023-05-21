using HOTPExample.Application.HOTP;
using HOTPExample.Contract.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HOTPExample.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class OtpController : Controller
    {   
        private readonly IHOTPGeneratorService _generatorService;
        private readonly IMemoryCache _memoryCache;
        public OtpController(IHOTPGeneratorService generatorService, IMemoryCache memoryCache)
        {
            _generatorService = generatorService;
            _memoryCache = memoryCache;
        }

        [HttpGet("GetOtp")]
        public IActionResult GetOTP(GetOtpRequestModel requestModel)
        {
            string otp = _generatorService.GenerateOTP(requestModel.Username);
            _memoryCache.Set("LoggedUser", requestModel.Username);
            return Ok(otp);
        }

        [HttpPost("VerifyOtp")]
        public IActionResult VerifyOtp(VerifyOtpRequestModel requestModel)
        {
            string loggedUsername = _memoryCache.Get("LoggedUser")?.ToString();
            bool verified = _generatorService.VerifyOTP(requestModel.Otp, loggedUsername);
            if(verified)
                return Ok("OTP Verified");
            return BadRequest("OTP Not Verified");
        }
    }
}
