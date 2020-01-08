namespace EC.Models.API.v1.Token
{
	public class TokenRequestModel
	{
		public string CompanyId { get; set; }
		public string SecretKey { get; set; }
	}
}