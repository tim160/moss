using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Common.Util
{
    public enum VehicleStatusType
    {
        Normal = 0,
        NotConnected = 1,
        NotReporting = 2,
        EngineIdle = 4,
        EngineOff = 8,
        Moving = 16,
        BucketUp = 32
    }

    public enum VisualDisplayStatus
    {
        Unknown = unchecked((int)0xFF808080),   // gray
        Idle = unchecked((int)0xFFFBCF38),      // yellow
        Moving = unchecked((int)0xFF80FF9E),    // green
        Speeding = unchecked((int)0xFFEF0101),  // red
        Stopped = unchecked((int)0xFF3B3662),   // purple ver 2
        Busy = unchecked((int)0xFF67ADDA)       // light blue
    }

    public enum VehicleRouteStatusType
    {
        Assigned = 0,
        Unassigned = 1,
        AssignedProcessed = 2,
        UnassignedProcessed = 3
    }
}
