namespace RentalCheckIn.DTOs;
public class CustomRequests
{
    public class TokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class PasswordResetRequest
    { 
        public string ResetToken { get; set; }
        public string NewPassword { get; set; }
    }
}
