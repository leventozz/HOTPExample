using HOTPExample.Application.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;

namespace HOTPExample.Application.HOTP
{
    public class HOTPGeneratorService : IHOTPGeneratorService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private OtpSettings _otpSettings = new();
        private long _counter = 0;
        private readonly string _cacheCounter = "CounterCache";
        public HOTPGeneratorService(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public string GenerateOTP(string username)
        {
            _configuration.GetSection("OtpSettings").Bind(_otpSettings);
            var _secretKey = Encoding.ASCII.GetBytes(_otpSettings.SecretKey);
            _memoryCache.TryGetValue(_cacheCounter, out _counter);

            using var hmac = new HMACSHA1(_secretKey);
            byte[] counterBytes = BitConverter.GetBytes(_counter);
            Array.Reverse(counterBytes);
            byte[] hash = hmac.ComputeHash(counterBytes);

            int offset = hash[hash.Length - 1] & 0x0F;
            int binary =
                ((hash[offset] & 0x7F) << 24) |
                ((hash[offset + 1] & 0xFF) << 16) |
                ((hash[offset + 2] & 0xFF) << 8) |
                (hash[offset + 3] & 0xFF);

            int otp = binary % (int)Math.Pow(10, _otpSettings.Digit);

            var finalOtp = otp.ToString().PadLeft(_otpSettings.Digit, '0');
            _memoryCache.Set(username, finalOtp, TimeSpan.FromSeconds(30));
            return finalOtp;
        }

        public bool VerifyOTP(string otp, string username)
        {
            _memoryCache.TryGetValue(_cacheCounter, out _counter);
            string otpCache;

            if (_memoryCache.TryGetValue(username, out otpCache))
            {
                if (string.Equals(otp, otpCache))
                {
                    long cr = _counter + 1;
                    _memoryCache.Set(_cacheCounter,cr);
                    _memoryCache.Remove(username);
                    return true;
                }
            }
            return false;
        }
    }
}
