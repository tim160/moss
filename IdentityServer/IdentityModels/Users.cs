using IdentityServer3.Core;
using IdentityServer3.Core.Services.InMemory;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer.IdentityModels
{
    public static class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
        {
            new InMemoryUser
            {
                Username = "kmilton",
                Password = "1234567",
                Subject = "1",

                Claims = new[]
                {
                    new Claim(Constants.ClaimTypes.GivenName, "Kate"),
                    new Claim(Constants.ClaimTypes.FamilyName, "Milton"),
                    new Claim(Constants.ClaimTypes.Role, "Test Role 1"),
                    new Claim(Constants.ClaimTypes.Role, "Test Role 1")
                }
            }
        };
        }
    }
}