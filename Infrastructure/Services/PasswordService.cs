using System;
using System.Text;
using Core.Application.Common.Services;

namespace Infrastructure.Services
{
    public class PasswordService: IPasswordService
    {
        public string GeneratePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var result = new StringBuilder();
            var random = new Random();
            while (0 < length--)
            {
                result.Append(valid[random.Next(valid.Length)]);
            }
            return result.ToString();
        }
    }
}