namespace EC.Common.Util
{
    public enum OrderStatusType
    {
        Default = 0,
        Unknown = 10,
        Unassigned = 20,
        Unacknowledged = 30,
        Assigned = 40,
        Problematic = 50,
        Enroute = 60,
        OnSite = 70,
        Cancelled = 80,
        Suspended = 90,
        Completed = 100,
        Verified = 110,
        Skipped = 130,
        ProjectUnassigned = 140,
        ProjectAssigned = 150,
        ProjectInProgress = 160,
        ProjectCompleted = 170,
        ProjectClosed = 180,
        ProjectCancelled = 185,
        Scheduled = 190,
        Tentative = 200, 
        InProgress = 210
    }
}
