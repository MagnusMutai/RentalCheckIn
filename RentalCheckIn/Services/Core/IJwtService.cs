namespace RentalCheckIn.Services.Core;
public interface IJWTService 
{
    string GenerateToken(LHost lHost);
}
