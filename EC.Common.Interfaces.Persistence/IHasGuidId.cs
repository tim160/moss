using System;

namespace EC.Common.Interfaces.Persistence
{
    public interface IHasGuidId
    {
        Guid Id { get; }
    }
}
