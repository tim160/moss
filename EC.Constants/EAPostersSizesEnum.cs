using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EC.Constants
{
    public enum EAPostersSizesEnum
    {
        [EnumMember]
        Size1620 = 1,

        [EnumMember]
        Size1824 = 2,

        [EnumMember]
        Size2228 = 3,
        [EnumMember]
        Size2436 = 4,
        [EnumMember]
        Size3648 = 5,

        [EnumMember]
        SizeWallet = 11

    }
}
