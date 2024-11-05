namespace RentalCheckIn.DTOs;
public class CustomResponses
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public LHost Host { get; set; }
    }

    public class EmailVerificationResult
    {
        // Indicates if the email was successfully confirmed
        public bool IsSuccess { get; set; }
        // Indicates if the token is expired
        public bool IsExpired { get; set; }
        // Indicates if the email is already confirmed
        public bool IsAlreadyConfirmed { get; set; }
        // A message for the user
        public string Message { get; set; }
    }

    public class RefreshTokenResult
    {
        public bool Success { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class LHostResult
    {
        public bool Success { get; set; }
        public LHost LHost { get; set; }
        public string ErrorMessage { get; set; }
    }

}
