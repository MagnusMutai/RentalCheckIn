namespace RentalCheckIn.Responses;
public class CustomResponses
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        // Optional, for successful login
        public Lhost Host { get; set; } 
    }
}
