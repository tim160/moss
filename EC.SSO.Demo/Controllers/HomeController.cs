using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using EC.SSO.Demo.Models;
using System.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EC.SSO.Demo.Controllers
{
    public class MyJsonResult
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
    }
    public class HomeController : Controller
    {

        private readonly List<User> _users = new List<User>
        {
            new User
            {
                Username="kmilton",
                Name="Kate Milton",
                Password="1234567",
                CompanyName="Stark Industries",
                User_id = 2
            },
            new User
            {
                Username="4",
                Name="Alex Grace",
                Password="123456",
                CompanyName="Stark Industries",
                User_id = 6
            },
            new User
            {
                Username="6",
                Name="Jeff Wood",
                Password="123456",
                CompanyName="Stark Industries",
                User_id = 8
            },
            new User
            {
                Username="222",
                Name="John McDonald",
                Password="123456",
                CompanyName="Stark Industries",
                User_id = 23
            }
        };

        public ActionResult Index() => View();

        public async Task<ActionResult> GetLink(string username)
        {
            var user_id = _users.Where(t => t.Username.Trim().ToLower() == username.Trim().ToLower()).FirstOrDefault().User_id;
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new StringContent("aaa"), "clientId");
            form.Add(new StringContent("zzz"), "secret");
            form.Add(new StringContent(user_id.ToString()), "userId");

            HttpResponseMessage response = await httpClient.PostAsync("http://localhost:49954/Oauth/Token", form);
            string content = await response.Content.ReadAsStringAsync();
            MyJsonResult jsonResult = JsonConvert.DeserializeObject<MyJsonResult>(content);

            ViewBag.Link = "http://localhost:49954/Oauth/TokenLogin?token=" + jsonResult.access_token;
            return View("BigButton");
        }

        //public ActionResult EmbeddedApp(string username)
        //{
        //    ViewBag.username = username;
        //    var user = _users.Where(t => t.Username.Trim().ToLower() == username.Trim().ToLower()).FirstOrDefault();
        //    string pass = "";
        //    if (user != null)
        //        pass = user.Password;

        //    ViewBag.pass = pass;
        //    ViewBag.token = GenerateToken(user.User_id);
        //    return View();

        //}

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            User user = _users.FirstOrDefault(item =>
                item.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                && item.Password.Equals(password, StringComparison.Ordinal));
            if (user != null)
            {
                return View("BigButton", user);
            }
            else
            {
                return Content("Incorrect user name or password.");
            }
        }

      //  static string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";

      //  private static string GenerateToken(int user_id)
      //  {
      //      var tokenHandler = new JwtSecurityTokenHandler();
      //      //  var tokenHandler = new Microsoft.IdentityModel.Tokens.TokenHandler();
      //      var now = DateTime.UtcNow;
      //      var symmetricKey = Convert.FromBase64String(key);
      //      var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
      //      {
      //          Subject = new ClaimsIdentity(new[]
      //        {
      //  new Claim( "user_id", user_id.ToString() ),
      //  new Claim( "user_guid", "test" ),



      //}),
      //          // Expires = DateTime.UtcNow.AddDays(1),
      //          // Lifetime = new System.IdentityModel.Protocols.WSTrust.Lifetime(now, now.AddMinutes(20)),
      //          //  SigningCredentials = new System.IdentityModel.Tokens.SigningCredentials(new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest),
      //          //   SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.Sha256Digest),
      //          SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(symmetricKey), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256),


      //      };

      //      var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
      //      var tokenString = tokenHandler.WriteToken(token);

      //      /*    var handler = new JwtSecurityTokenHandler();
      //          var jsonToken = handler.ReadToken(tokenString);
      //          var jwtToken = handler.ReadJwtToken(tokenString);

      //          var tokenS = handler.ReadToken(tokenString) as JwtSecurityToken;
      //           var username1 = tokenS.Claims.First(claim => claim.Type == "username").Value;
      //          var email = tokenS.Claims.First(claim => claim.Type == "email").Value;

      //          var temp = jwtToken.SigningKey;
      //          var jwtTokenS = handler.ReadToken(tokenString) as JwtSecurityToken;
      //          //  var username1 = tokenS.Claims.First(claim => claim.Type == "username").Value;
      //          //   var lastName1 = tokenS.Claims.First(claim => claim.Type == "lastName").Value;

      //          Decode(tokenString, key, true);*/
      //      return tokenString;

      //      /*


      //      var securityKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
      //      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest);

      //      var secToken = new JwtSecurityToken(
      //          signingCredentials: credentials,
      //          issuer: "Sample",
      //          audience: "Sample",
      //          claims: new[]
      //          {
      //                new Claim(JwtRegisteredClaimNames.Sub, "meziantou")
      //          },
      //          expires: DateTime.UtcNow.AddDays(1));

      //      var handler = new JwtSecurityTokenHandler();
      //      return handler.WriteToken(secToken);*/
      //  }



      //  public static string Decode(string token, string key, bool verify = true)
      //  {
      //      if (string.IsNullOrEmpty(token))
      //          throw new ArgumentException("Given token is null or empty.");

      //      JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

      //      var tokenHandler = new JwtSecurityTokenHandler();
      //      //byte[] symmetricKey = Convert.FromBase64String(key);
      //      //byte[] symmetricKey = (Encoding.UTF8.GetBytes(key));// Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature
      //      var symmetricKey = Convert.FromBase64String(key);
      //      var tokenValidationParameters = new TokenValidationParameters
      //      {
      //          RequireSignedTokens = true,
      //          ValidateIssuer = false,
      //          ValidateAudience = false,
      //          ValidateLifetime = false,

      //          IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(symmetricKey)
      //      };

      //      //  Microsoft.IdentityModel.Tokens.SecurityToken validatedToken = new 
      //      var tokenSec = tokenHandler.ReadToken(token) as Microsoft.IdentityModel.Tokens.SecurityToken;
      //      try
      //      {






















      //          ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out tokenSec);
      //          return "";
      //      }
      //      catch (Exception ex)
      //      {
      //          throw ex;
      //      }



      //      /*


      //      string[] parts = token.Split('.');
      //      string header = parts[0];
      //      string payload = parts[1];
      //      byte[] crypto = Base64UrlDecode(parts[2]);

      //      string headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
      //      JObject headerData = JObject.Parse(headerJson);

      //      string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
      //      JObject payloadData = JObject.Parse(payloadJson);

      //      if (verify)
      //      {
      //        var keyBytes = Convert.FromBase64String(key); // your key here

      //        AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);
      //        RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
      //        RSAParameters rsaParameters = new RSAParameters();
      //        rsaParameters.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
      //        rsaParameters.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();
      //        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
      //        rsa.ImportParameters(rsaParameters);

      //        SHA256 sha256 = SHA256.Create();
      //        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(parts[0] + '.' + parts[1]));

      //        RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
      //        rsaDeformatter.SetHashAlgorithm("SHA256");
      //        if (!rsaDeformatter.VerifySignature(hash, FromBase64Url(parts[2])))
      //          throw new ApplicationException(string.Format("Invalid signature"));
      //      }

      //      return payloadData.ToString();      */
      //  }




    }
}