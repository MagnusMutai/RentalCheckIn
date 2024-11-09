﻿namespace RentalCheckIn.DTOs;
public class CustomResponses
{

    // Use a generic response for some of the identical return types
    public class AuthenticationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public LHost Host { get; set; }
    }

    public class EmailVerificationResponse
    {
        // Indicates if the email was successfully confirmed
        public bool IsSuccess { get; set; }
        // Indicates if the token is expired
        public bool IsExpired { get; set; }
        // Indicates if the email is already confirmed
        public bool IsAlreadyConfirmed { get; set; }
        // A message for the user
        public string Message { get; set; }
    }

    public class RefreshTokenResponse
    {
        public bool Success { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public string ErrorMessage { get; set; }
    }

    // Not being used for now
    public class LHostResponse
    {
        public bool Success { get; set; }
        public LHost LHost { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class TokenValidateResult
    {
        public bool IsSuccess { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
    }

    public class ResetPasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

}
