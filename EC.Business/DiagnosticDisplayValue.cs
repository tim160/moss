using System;
using System.Collections.Generic;
using System.Text;
using EC.Business.Entities;

namespace EC.Business
{
    public class TelemetryDisplayValue
    {
        string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }


        string _Value;
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        string _Unit;
        public string Unit
        {
            get
            {
                return _Unit;
            }
            set
            {
                _Unit = value;
            }
        }

        private DateTime? _Timestamp;
        public DateTime? Timestamp
        {
            get
            {
                return _Timestamp;
            }
            set
            {
                _Timestamp = value;
            }
        }

        public long ParameterTypeId
        {
            get;
            set;
        }

        public long VehicleId
        {
            get;
            set; 
        }

        private int _Ordinal = 0;
        public int Ordinal
        {
            get
            {
                return _Ordinal;
            }
            set
            {
                _Ordinal = value;
            }
        }

        public long VehicleTypeId
        {
            get; set;
        }

        public long LogId
        {
            get;
            set;
        }

        private string _AlertMessage;
        public string AlertMessage
        {
            get { return _AlertMessage; }
            set { _AlertMessage = value; }
        }

        public TelemetryDisplayValue(long vehicleTypeId, int ordinal, string name, long vehicleId, string displayValue, string unit, DateTime logDatetime, long parameterId, long logId)
        {
            Name = name;
            Ordinal = ordinal;
            VehicleId = vehicleId;
            VehicleTypeId = vehicleTypeId;
            Value = displayValue;
            Unit = unit;
            Timestamp = logDatetime;
            ParameterTypeId = parameterId;
            LogId = logId;
        }

        public TelemetryDisplayValue(long vehicleTypeId, int ordinal, string name, long vehicleId, string unit, long parameterId)
        {
            Name = name;
            Ordinal = ordinal;
            VehicleTypeId = vehicleTypeId;
            Value = string.Empty;
            Unit = unit;
            Timestamp = null;
            ParameterTypeId = parameterId;
        }

        public TelemetryDisplayValue()
        {
        }


    }
}
