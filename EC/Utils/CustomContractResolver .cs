using EC.Models.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace EC.Utils
{
    public class CustomContractResolver: DefaultContractResolver
    {
        public new static readonly CustomContractResolver Instance = new CustomContractResolver();

        private static List<string> ignore = new List<string> {
            "user.login_nm",
            "user.password",
            "user.company_disclamer_page",
            "user.company_disclamer_page1",
            "user.company_disclamer_uploads",
            "user.company_disclamer_uploads1",
            "user.report_signoff_mediator",
            "user.report_signoff_mediator1",
            "user.report",
        };

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (ignore.Contains($"{property.DeclaringType.Name}.{property.PropertyName}") || ignore.Contains($"{property.DeclaringType.BaseType.Name}.{property.PropertyName}"))
            {
                property.ShouldSerialize = instance =>
                {
                    return false;
                };
            }

            return property;
        }
    }
}