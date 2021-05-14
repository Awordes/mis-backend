namespace Core.Application.Common.Services
{
    public interface IPasswordService
    {
        string GeneratePassword(int length);
    }
}