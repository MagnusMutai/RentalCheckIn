namespace RentalCheckIn.Services.Core;
public interface IJwtService 
{
    string GenerateToken(LHost lHost);
}
