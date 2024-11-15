namespace RentalCheckIn.DTOs;
public class OtpDTO
{
    public string Email { get; set; }
    [Required(ErrorMessage = "OTP code required")]
    public string Code { get; set; }
}
