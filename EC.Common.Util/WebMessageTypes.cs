namespace EC.Common.Util
{
    public enum WebMessageType
    {
        Unknown = 0,
        WebService = 1,
        POX = 2,
        REST_JSON = 3,
        SQL = 4,
        REST_XML = 5,
    }

    public enum MessageStatus
    {
        Queued = 0,
        FailRejected = 1,
        FailOffline = 2,
        Success = 3
    }

    public enum SoapVersion
    {
        Soap11 = 1,
        Soap12 = 2
    }

    // 1 has been deprecated - moved elsewhere
    public enum SecurityMode
    {
        None = 0,
        TransportWithMessageCredentials = 2,
        Transport = 3,
        Basic = 4,
    }

    public enum MessageProtocol
    {
        SOAP = 1,
        REST = 2,
    }

    public enum HostLookupStatus
    {
        Unknown = -1,
        FailRejected = 0,
        FailOffline = 1,
        Failed = 2,
        Success = 3,
        Retried = 4
    }

    public enum MethodInvocationStatus
    {
        Queued = 0,
        Failed = 1,
        Success = 2,
        NoHandler = 3
    }

    public enum MethodInvocationPriority
    {
        High = 0,
        Regular = 1
    }

    public enum WebMessageEntityType
    {
        WorkOrder = 0
    }

    public enum MethodInvocationEntityType
    {
        WorkOrder = 0
    }
}
