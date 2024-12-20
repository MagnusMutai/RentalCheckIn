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

    public class LocalizationRequest<T>
    {
        public List<T> Data { get; set; }
        public string Culture { get; set; }
    }

    public class OperationRequest
    {
        public  CheckInReservationDTO Model { get; set; }
        public string Culture { get; set; }
    }
    
    public class SendEmailRequest
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public byte[]? PDFByteArray { get; set; }
    }

}
