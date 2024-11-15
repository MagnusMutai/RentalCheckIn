﻿namespace RentalCheckIn.DTOs
{
    public class ReservationDTO
    {
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public string QuestName { get; set; }
        public int NumberOfQuests { get; set; }
        public int NumberOfNights { get; set; }
        public decimal? TotalPrice { get; set; }
        public string ApartmentName { get; set; }
        public string ChannelName { get; set; }
        public string StatusLabel { get; set; }
        public string CurrencySymbol { get; set; }
    }
}
