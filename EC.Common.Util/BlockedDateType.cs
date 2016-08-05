namespace EC.Common.Util
{
    public enum BlockedDateType
    {
        None = -1,

        // Workable set
        NonWorkable = 0,
        Workable = 1,
        Clear = 2,

        // Preferred set
        Preferred = 10,
        ClearPreferred = 11
    }
}
