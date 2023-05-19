namespace HOTPExample.Application.HOTP
{
    public interface IHOTPGeneratorService
    {
        string GenerateOTP(string username);
        bool VerifyOTP(string otp, string username);
    }
}
