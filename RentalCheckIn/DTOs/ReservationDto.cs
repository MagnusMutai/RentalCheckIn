namespace RentalCheckIn.DTOs;
public class ReservationDTO
{
    public uint Id { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public string? CheckInTime { get; set; }
    public string? CheckOutTime { get; set; }
    public string QuestName { get; set; }
    public string? PhoneNumber { get; set; }
    public int NumberOfQuests { get; set; }
    public int NumberOfNights { get; set; }
    public decimal? Price { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public decimal? TotalPrice { get; set; }
    public string ApartmentName { get; set; }
    public string? ChannelName { get; set; }
    public string StatusLabel { get; set; }
    public string CurrencySymbol { get; set; }
}
