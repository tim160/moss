using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Common.Util
{
    public enum VehicleStatusEvent
    {
        IgnitionOn,
        IgnitionOff,
        IdleDetected,
        MovementDetected
    }

    public enum VehicleStatusAlert
    {
        Sensor,
        Diagnostic,
        MIL,
        PANIC,
        CANCELPANIC,
        MOTION,
        OFFROUTE,
        ONROUTE,
        OFFLINEACCEL
    }
      
    public enum OBD2Parameters
    {
        Undefined = 0,
        VehicleSpeed = 1,
        EngineRPM = 2,
        ThrottlePosition = 3,
        VIN = 4,
        MIL = 5,
        NumberOfTroubleCodes = 6,
        CalculatedEngineLoad = 7,
        CoolantTemperature = 8,
        FuelRailPressure = 9,
        IntakeManifoldPressure = 10,
        IgnitionTimingAdvanceCylinder1 = 11,
        IntakeAirTemperature = 12,
        MAFAireFlowRate = 13,
        EngineRunTime = 14,
        FuelLevelInput = 15,
        ControlModuleVoltage = 16,
        AbsoluteLoadValue = 17,
        PowerTakeoffStatus = 18,
        DistanceTravelledWithActiveMIL = 19,
        DistanceTravelledSinceCodesCleared = 20,
        MinutesRunWithActiveMIL = 21,
        TimeSinceCodesCleared = 22,
        Odometer = 23,
        HighResolutionOdometer = 24,
        TotalEngineHours = 25,
        PercentLoadAtCurrentRPM = 26,
        EngineTorquePercent = 27,
        FuelTemperature = 28,
        BoostPressure = 29,
        IntakeManifold1Temperature = 30,
        TotalFuelUsed = 31,
        FileDeliverPressure = 32,
        EngineOilPressure = 33,
        FuelRate = 34,
        BatteryPotVoltage = 35,
        AcceleratorPedalPosition = 36,
        DiagnosticCode = 37,
        VIN_J1939 = 38,
        PeekAcceleration = 99, // pa
        PeekDeceleration = 98, //pb
        Idle = 218,
        //id = gateway device id
        PFUEL = 1000,
        PIDLE = 1001,
        PPWR = 1002,
        PODO = 1003,
        PFUELR = 1004
    }
}
