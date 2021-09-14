using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ContosoUni.Areas.JWT.Models
{
    public class MyJwt
    {
        public const string Issuer = "chong";
        public const string Audience = "ApiUser";
        public const string Key = "Chong@SecurityKey";

        public const string Schemes = "Identity.Application" + "," + JwtBearerDefaults.AuthenticationScheme;

    }
}
