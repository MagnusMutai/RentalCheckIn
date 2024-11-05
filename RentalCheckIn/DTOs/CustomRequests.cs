namespace RentalCheckIn.DTOs;
public class CustomRequests
{
    public class TokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
