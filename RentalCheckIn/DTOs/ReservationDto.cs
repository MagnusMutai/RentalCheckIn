namespace RentalCheckIn.DTOs
{
    public class ReservationDto
    {
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public string QuestName { get; set; }
        public int NumberOfQuests { get; set; }
        public int NumberOfNights { get; set; }
        public decimal? TotalPrice { get; set; }
        public string ApartmentName { get; set; }
        public string ChannelName { get; set; }
        public string StatusLebel { get; set; }
    }
}
