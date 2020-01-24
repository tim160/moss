using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityServer.IdentityModels
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            var scopes = new List<Scope>
        {
            new Scope
            {
                Enabled = true,
                Name = "roles",
                Type = ScopeType.Identity,
                IncludeAllClaimsForUser = true,
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim("role")
                }
            },
            new Scope
            {
                Enabled = true,
                DisplayName = "Sample API",
                Name = "sampleApi",
                Description = "Access to a sample API",
                Type = ScopeType.Resource
            }
        };

            scopes.AddRange(StandardScopes.All);

            return scopes;
        }
    }
}