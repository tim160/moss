using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Common.Util
{
    public class Constants1
    {
        public static readonly Guid Password = new Guid("50A1D150-F52A-4F9B-BEBA-B0246548A55A");

        public static readonly int ClientNotification = 1;

        public static readonly int AssignOrderEventType = 10;
        public static readonly int ReleaseOrderEventType = 20;
        public static readonly int ReassignOrderEventType = 30;
        public static readonly int UnassignOrderEventType = 40;
        public static readonly int CancelOrderEventType = 50;
        public static readonly int ResolveOrderEventType = 60;
        public static readonly int ResolveOrderToUnassignedEventType = 70;
        public static readonly int ScheduleOrderEventType = 80;
        public static readonly int TentativeOrderEventType = 90;
        public static readonly int SentBackToHostEventType = 100;
        public static readonly int UndoSentBackToHostEventType = 110;
        public static readonly int ModifyByHistoryOrderEventType = 120;
        public static readonly int GeoCodeOrderEventType = 130;
        public static readonly int ReverseGeoCodeOrderEventType = 140;
        public static readonly int SetOrderCategoryEventType = 150;
        public static readonly int EnableOrderIndexEventType = 160;
        public static readonly int UpdateOrderEventType = 170;
        public static readonly int ResequenceOrderEventType = 180;
        public static readonly int AddEntityEventType = 190;
        public static readonly int AddEntityByApiEventType = 200;
        public static readonly int UpdateEntityEventType = 210;
        public static readonly int UpdateEntityByApiEventType = 220;
        public static readonly int DeleteEntityEventType = 230;
        public static readonly int AVLVehiclePositionEventType = 240;
        public static readonly int VehicleStatusUpdateEventType = 250;
        public static readonly int AddOrderFormEventType = 260;
        public static readonly int UpdateOrderFormEventType = 270;
        public static readonly int MobileStatusChangeEventType = 280;
        public static readonly int MobileDownloadOrderEventType = 290;
        public static readonly int MobileUploadOrderEventType = 300;
        public static readonly int MobileUploadAttachmentEventType = 310;
        public static readonly int MobileOrderCountEventType = 320;
        public static readonly int RequestTimerEventType = 330;
        public static readonly int StartTimerEventType = 340;
        public static readonly int CancelTimerEventType = 350;
        public static readonly int ExpireTimerEventType = 360;
        public static readonly int AcknowledgeTimerEventType = 370;
        public static readonly int WorkerSignOnEventType = 380;
        public static readonly int WorkerSignOffEventType = 390;
        public static readonly int WorkerAvailableEventType = 400;
        public static readonly int WorkerUnavailableEventType = 410;
        public static readonly int WorkerEmergencyEventType = 420;
        public static readonly int WorkerUnsetEmergencyEventType = 430;
        public static readonly int WorkerOnlineEventType = 440;
        public static readonly int WorkerOfflineEventType = 450;
        public static readonly int WorkerGPSEventType = 460;
        public static readonly int WorkerSetVehicleEventType = 470;
        public static readonly int WorkerUnsetVehicleEventType = 480;
        public static readonly int AcknowledgeOrderEventType = 490;
        public static readonly int EnrouteOrderEventType = 500;
        public static readonly int OnSiteOrderEventType = 510;
        public static readonly int SuspendOrderEventType = 520;
        public static readonly int ReferOrderEventType = 530;
        public static readonly int CompleteOrderEventType = 540;
        public static readonly int ProblematicOrderEventType = 550;
        public static readonly int VerifiedOrderEventType = 560;
        public static readonly int SkipOrderEventType = 570;
        public static readonly int SaveOrderEventType = 580;
        public static readonly int AttachOrderToProjectEventType = 590;
        public static readonly int TakeOwnershipOfProjectEventType = 600;
        public static readonly int SetOwnershipEventType = 610;
        public static readonly int UpdateAttributeAssociationsType = 620;
        public static readonly int SetEtaOrderEventType = 630;
        public static readonly int LoginEventType = 640;
        public static readonly int LogoutEventType = 650;
        public static readonly int SessionEventType = 660;
        public static readonly int SessionInvalidatedEventType = 670;
        public static readonly int SystemSetSafePeriodEventType = 680;
        public static readonly int SystemBlackoutExpiryEventType = 690;
        public static readonly int VehicleBucketUpEvent = 700;
        public static readonly int VehicleBucketDownEvent = 710;
        public static readonly int VehicleIgnitionOnEvent = 720;
        public static readonly int VehicleIgnitionOffEvent = 730;
        public static readonly int VehicleConnectedEvent = 740;
        public static readonly int VehicleDisconnectedEvent = 750;
        public static readonly int VehicleGpsLockLostEvent = 760;
        public static readonly int VehicleGpsLockObtainedEvent = 770;
        public static readonly int VehicleIdleStartEvent = 780;
        public static readonly int VehicleIdleStopEvent = 790;
        public static readonly int VehicleStopEvent = 800;
        public static readonly int VehicleStartEvent = 810;
        public static readonly int VehicleOdometerUpdatedEvent = 820;
        public static readonly int VehicleEnteredFenceEvent = 830;
        public static readonly int VehicleLeftFenceEvent = 840;
        public static readonly int VehicleMILUpdatedEvent = 850;
        public static readonly int VehicleWeightEvent = 860;
        public static readonly int VehicleSetRouteEventType = 870;
        public static readonly int GarminConnectedEvent = 880;
        public static readonly int GarminDisconnectedEvent = 890;
        public static readonly int MessageEventType = 900;
        //public static readonly Guid LISCheckoutEventType = new Guid("1C93D29F-74E7-416F-8C16-C77E9125B6D1");
        public static readonly int LISCheckoutEventType = 910;
        //public static readonly Guid PingRemoteModem = new Guid("EF49F1FD-F5E2-48e6-A698-9C18150C1CD4");
        public static readonly int PingRemoteModem = 920;
        public static readonly int ChangeBusinessGroupEventType = 930;
        public static readonly int QueuedMessageUpdateEventType = 940;
        public static readonly int DispatchCompleteOrderEventType = 950;
        public static readonly int WorkOrderProblematicEventType = 960;

        public static Dictionary<Int64, string> VehicleEventNames = new Dictionary<Int64, string>();
        // can't do static dictionary with default values PDA/Vernon is not happy
        static Constants1()
        {
            VehicleEventNames.Add(VehicleBucketUpEvent, "VehicleBucketUpEvent");
            VehicleEventNames.Add(VehicleBucketDownEvent, "VehicleBucketDownEvent");
            VehicleEventNames.Add(VehicleIgnitionOnEvent, "VehicleIgnitionOnEvent");
            VehicleEventNames.Add(VehicleIgnitionOffEvent, "VehicleIgnitionOffEvent");
            VehicleEventNames.Add(VehicleConnectedEvent, "VehicleConnectedEvent");
            VehicleEventNames.Add(VehicleDisconnectedEvent, "VehicleDisconnectedEvent");
            VehicleEventNames.Add(VehicleGpsLockLostEvent, "VehicleGpsLockLostEvent");
            VehicleEventNames.Add(VehicleGpsLockObtainedEvent, "VehicleGpsLockObtainedEvent");
            VehicleEventNames.Add(VehicleIdleStartEvent, "VehicleIdleStartEvent");
            VehicleEventNames.Add(VehicleIdleStopEvent, "VehicleIdleStopEvent");
            VehicleEventNames.Add(VehicleStopEvent, "VehicleStopEvent");
            VehicleEventNames.Add(VehicleStartEvent, "VehicleStartEvent");
            VehicleEventNames.Add(VehicleOdometerUpdatedEvent, "VehicleOdometerUpdatedEvent");
            VehicleEventNames.Add(VehicleEnteredFenceEvent, "VehicleEnteredFenceEvent");
            VehicleEventNames.Add(VehicleLeftFenceEvent, "VehicleLeftFenceEvent");
            VehicleEventNames.Add(VehicleMILUpdatedEvent, "VehicleMILUpdatedEvent");
            VehicleEventNames.Add(VehicleWeightEvent, "VehicleWeightEvent");
            VehicleEventNames.Add(VehicleSetRouteEventType, "VehicleSetRouteEventType");
        }




        public static readonly Guid LISFittedWarehouse = new Guid("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE");
        public static readonly Guid LISFittedStorageLocation = new Guid("11111111-2222-3333-4444-555555555555");
        
    }

    public class GridConstants
    {
        public const int SessionGridMaxRowPerPage = 100;
    }

    public enum TimerType
    {
        Default = 0,
        Safety = 10
    }

    public enum TimerStatusType
    {
        Request = 0,
        Expire = 10,
        Active = 20,
        Cancel = 30,
        Close = 40
    }

    public enum FromHostMessageType
    {
        Unknown = -1,

        Handler = 1000,

        OrderCreate = 1010,
        OrderModify = 1020,
        OrderCancel = 1030,
        OrderAssign = 1040,
        OrderReassign = 1050,
        OrderUnassign = 1060,
        OrderReschedule = 1070,
        ProjectOrderCreate = 1080,
        OrderCreateOrModify = 1090,
        OrderSetToProblematic = 1100,

        HostEnquiryReply = 2010,

        OrderFromHostApi = 2210,
        ProcessStateChangeApi = 2220,
        OrderFormCreate = 2230,
        //OrderFormModify = 2240
    }

    public enum ToHostMessageType
    {
        Unknown = -1,

        Handler = 3000,

        OrderCreate = 3010,
        OrderModify = 3020,
        OrderCancel = 3030,
        OrderAssign = 3040,
        OrderReassign = 3050,
        OrderUnassign = 3060,
        OrderReschedule = 3070,
        OrderAcknowledge = 3080,
        OrderEnroute = 3090,
        OrderOnSite = 3100,
        OrderSuspend = 3110,
        OrderRefer = 3120,
        OrderCompletion = 3130,
        OrderFinalCompletion = 3140,
        OrderSkip = 3150,
        OrderWorkNotification = 3160,
        OrderLog = 3170,
        OrderSchedule = 3180,
        OrderBookAppt = 3190,
        OrderCancelAppt = 3200,
 
        WorkerCreate = 4010,
        WorkerModify = 4020,
        WorkerSignOn = 4030,
        WorkerSignOff = 4040,
        WorkerAvailable = 4050,
        WorkerUnavailable = 4060,
        WorkerEmergency = 4070,
        WorkerUnsetEmergency = 4080,
        WorkerOnline = 4090,
        WorkerOffline = 4100,
        WorkerGPS = 4110,

        HostEnquiry = 5010,

        OrderToHostApi = 5210,
        OrderReply = 5220,
        OrderFormCreate = 6010,
        OrderFormModify = 6020
    }

    public enum FromHostReturnCode
    {
        Unknown = -1,                   // Not set
        Success = 0,                    // No error
        WebServiceUnavailable = 1,      // Web Service temporarily unavailable
        AuthenticationError = 2,        // Authentication error
        DuplicateOrderId = 3,           // Notification number already defined
        InvalidData = 4,                // Creation failure: Invalid data
        InvalidXMLTag = 5,              // Creation failure: Invalid XML tag
        MissingData = 6,                // Creation failure: Mandatory data are required
        InconsistentData = 7,           // Creation failure: Data inconsistent
        FailCreateOrder = 8,            // Create failure
        FailModifyOrder = 9,            // Modify failure
        WrongOrderState = 10,           // Order state transition is not allowed - wrong state
        OrderNotExist = 11,             // Failure: Order cannot be found
        WorkerNotExist = 12,            // Assign Failure: No such worker
        OrderTypeNotExist = 13,         // Failure: Order Type not defined
        TxCodeNotExist = 14,            // Failure: Transaction code not defined or invalid
        OrderTypeUnsupported = 15,      // Failure: This Order Type version is not supported
        ConfigNotExist = 16,            // Failure: Host interface config not defined
        FailCancelOrder = 17,           // Cancel failure
        ProjectHostOrderMustBeUnique = 18,  // Failure: Project Host Order Number must be unique
        FailCreateProject = 19,         // Create project failure
        OrderExistAlready = 20,         // Order with same HostOrderNumber exists already
        FailProcessStateChange = 21,    // Failure: Error processing state change
        FailOrderAssignmentChange = 22, // Failure: Error change order assignment (assign,reassign,unassign)
        FailOrderSchedule = 23,         // Failure: Order schedule error
        FailChangeOrderStatus = 24      // Failure: Change order status failure
    }

    public enum ToHostReturnCode
    {
        Unknown = -1,
        Success = 0,
        SkipTxCode = 11,
        ErrorMissingData = 21,
        ErrorParseData = 22,
        ErrorDuplicate = 23
    }

    public enum OrderEvent
    {
        None = -1,
        BeforeStatusChange = 4,
        StatusChange = 5,
        BeforeCreate = 10,
        Create = 15,
        BeforeModify = 20,
        Modify = 25,
        BeforeProcessStatus = 30,
        ProcessStatus = 35,
        BeforeComplete = 40,
        Complete = 45,
        BeforeResolve = 50,
        Resolve = 55,
        BeforeBooking = 60,
        AfterBooking = 65,
        BeforeBookingCancel = 70,
        BookingCancel = 75,
        BeforeAssign = 80,
        Assign = 85,
        BeforeUnassign = 90,
        Unassign = 95,
        BeforeSchedule = 100,
        Schedule = 105,
        ScheduleTentative = 115,
        BeforeAcknowledged = 120,
        Acknowledged = 125,
        BeforeCancel = 130,
        Cancel = 135,
        BeforeSkip = 140,
        Skip = 145,
        BeforeSuspend = 150,
        Suspend = 155,
        BeforeMobileSave = 160,
        MobileSave = 165,
        AttachmentSaved = 175,
        Attach = 185,
        PrefillNewOrder = 195,
        AfterOwnershipChange = 205,
        BeforeSetProblematic = 210,
        SetProblematic = 215,
        BeforeSetEta = 220,
        SetEta = 225,
        OnDemand = 230,

    }

    public enum ClientOrderEvent
    {
        None = -1,
        Create = 10,
        Modify = 20,
    }

    public enum ClientType
    {
        Unknown = 0,
        WorkBook = 1,
        Laptop = 2,
        PDA = 3,
        Dispatch = 4,
        WorkSpace = 5,
        WebAVL = 6,
        Configuration = 7, // Genesis
        Admin = 8, // WebAdmin, etc
        WebCSM = 9,
        HostInterface = 10,
        AVLGateway = 11,
    }

}