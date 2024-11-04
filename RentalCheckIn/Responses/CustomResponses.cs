namespace RentalCheckIn.Responses;
public class CustomResponses
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        // Optional, for successful login
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
}
