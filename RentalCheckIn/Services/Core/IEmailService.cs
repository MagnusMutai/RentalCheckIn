﻿namespace RentalCheckIn.Services.Core;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}
