using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Common.Extensions
{
    public static class IdentityResultExtension
    {
        public static void CheckResult(this IdentityResult identityResult)
        {
            try
            {
                if (!identityResult.Succeeded)
                {
                    var errors = identityResult.Errors.Select(err => $"{err.Code}: {err.Description}").ToArray();
                    throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}