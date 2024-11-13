namespace RentalCheckIn.DTOs;
public class CustomResponses
{

    // Use a generic response for some of the identical return types
    public class OperationResult<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
    
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class EmailVerificationResponse
    {
        // Indicates if the email was successfully confirmed
        public bool IsSuccess { get; set; }
        // Indicates if the email verification token is expired
        public bool IsExpired { get; set; }
        // Indicates if the email is already confirmed
        public bool IsAlreadyConfirmed { get; set; }
        // A message for the user
        public string Message { get; set; }
    }

    public class TokenValidateResult
    {
        public bool IsSuccess { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
    }

}
