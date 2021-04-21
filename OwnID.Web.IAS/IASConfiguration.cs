using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OwnID.Web.IAS
{
    public class IASConfiguration
    {
        public RSA jwtSigningCredentials { get; set;}
    }
}
