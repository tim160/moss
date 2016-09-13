using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EC.Errors.LMSExceptions
{
    /// <summary>
    /// Exception thrown if a nonce is claimed more than once.
    /// </summary>

    public class NonceAlreadyClaimedException : FaultableException<NonceAlreadyClaimedFault>
    {
        public override NonceAlreadyClaimedFault ToFault(string reqPath, CurrentUserInfo userInfo)
        {
            var f = new NonceAlreadyClaimedFault(Message, reqPath, userInfo);
            f.Nonce = Nonce;
            f.ClaimDate = ClaimDate;
            return f;
        }

        public NonceAlreadyClaimedException(Guid nonce, DateTime claimDate, Exception innerException = null) : base(string.Format("Nonce has already been claimed on {0} [{1}]", claimDate.ToShortDateString(), nonce.ToString()), innerException)
        {
            Nonce = nonce;
            ClaimDate = claimDate;
        }

        public Guid Nonce { get; set; }
        public DateTime ClaimDate { get; set; }
    }

    /// <summary>
    /// Fault thrown if a nonce is claimed more than once.
    /// </summary>
    
    [DataContract]
    public class NonceAlreadyClaimedFault : BasicFault
    {
        public NonceAlreadyClaimedFault(string msg, string path, CurrentUserInfo userInfo) : base(msg, path, userInfo)
        {
        }

        [DataMember]
        public Guid Nonce { get; set; }

        [DataMember]
        public DateTime ClaimDate { get; set; }
    }
}
