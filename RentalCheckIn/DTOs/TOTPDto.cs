namespace RentalCheckIn.DTOs;
public class TOTPDTO
{
    public string Email { get; set; }
    [Required(ErrorMessage = "OTP code required")]
    public string Code { get; set; }
}
