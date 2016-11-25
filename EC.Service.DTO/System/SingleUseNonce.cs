using System;
using System.Runtime.Serialization;

namespace EC.Service.DTO
{
    [DataContract]
    public class SingleUseNonce
    {
        /// <summary>
        /// The nonce.
        /// </summary>
        [DataMember]
        public Guid Nonce { get; set; }

        /// <summary>
        /// When the nonce is created initially, the caller can provide a value that
        /// can be looked up again when the nonce is supplied.
        /// </summary>
        [DataMember]
        public Guid? StoredValue { get; set; }

        /// <summary>
        /// Records when the nonce was created.
        /// </summary>
        [DataMember]
        public DateTime Start { get; set; }

        /// <summary>
        /// Records when the nonce was used. Null means that the nonce has
        /// not been "claimed" yet.
        /// </summary>
        [DataMember]
        public DateTime? Used { get; set; }
    }
}
