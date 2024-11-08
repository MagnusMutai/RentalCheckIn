namespace RentalCheckIn.DTOs;
public class OtpDto
{
    public string Email { get; set; }
    [Required(ErrorMessage = "OTP code required")]
    public string Code { get; set; }
}
