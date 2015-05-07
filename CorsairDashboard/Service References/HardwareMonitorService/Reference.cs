﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CorsairDashboard.HardwareMonitorService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Hardware", Namespace="http://schemas.datacontract.org/2004/07/CorsairDashboard.HardwareMonitoring")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CorsairDashboard.HardwareMonitorService.Hdd))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CorsairDashboard.HardwareMonitorService.Cpu))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CorsairDashboard.HardwareMonitorService.Gpu))]
    public partial class Hardware : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CorsairDashboard.HardwareMonitorService.HardwareKind KindField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CorsairDashboard.HardwareMonitorService.HardwareSensor[] SensorsField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id {
            get {
                return this.IdField;
            }
            set {
                if ((object.ReferenceEquals(this.IdField, value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CorsairDashboard.HardwareMonitorService.HardwareKind Kind {
            get {
                return this.KindField;
            }
            set {
                if ((this.KindField.Equals(value) != true)) {
                    this.KindField = value;
                    this.RaisePropertyChanged("Kind");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CorsairDashboard.HardwareMonitorService.HardwareSensor[] Sensors {
            get {
                return this.SensorsField;
            }
            set {
                if ((object.ReferenceEquals(this.SensorsField, value) != true)) {
                    this.SensorsField = value;
                    this.RaisePropertyChanged("Sensors");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Hdd", Namespace="http://schemas.datacontract.org/2004/07/CorsairDashboard.HardwareMonitoring.Hw")]
    [System.SerializableAttribute()]
    public partial class Hdd : CorsairDashboard.HardwareMonitorService.Hardware {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Cpu", Namespace="http://schemas.datacontract.org/2004/07/CorsairDashboard.HardwareMonitoring.Hw")]
    [System.SerializableAttribute()]
    public partial class Cpu : CorsairDashboard.HardwareMonitorService.Hardware {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Gpu", Namespace="http://schemas.datacontract.org/2004/07/CorsairDashboard.HardwareMonitoring.Hw")]
    [System.SerializableAttribute()]
    public partial class Gpu : CorsairDashboard.HardwareMonitorService.Hardware {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="HardwareKind", Namespace="http://schemas.datacontract.org/2004/07/CorsairDashboard.HardwareMonitoring")]
    public enum HardwareKind : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Unkown = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Cpu = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Mainboard = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Ram = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        GraphicCard = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        HardDisk = 5,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="HardwareSensor", Namespace="http://schemas.datacontract.org/2004/07/CorsairDashboard.HardwareMonitoring")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CorsairDashboard.HardwareMonitorService.UsageSensor))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CorsairDashboard.HardwareMonitorService.TemperatureSensor))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CorsairDashboard.HardwareMonitorService.Hdd))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CorsairDashboard.HardwareMonitorService.Cpu))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CorsairDashboard.HardwareMonitorService.Gpu))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CorsairDashboard.HardwareMonitorService.Hardware))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CorsairDashboard.HardwareMonitorService.HardwareKind))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(CorsairDashboard.HardwareMonitorService.HardwareSensor[]))]
    public partial class HardwareSensor : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private object ValueField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Id {
            get {
                return this.IdField;
            }
            set {
                if ((object.ReferenceEquals(this.IdField, value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public object Value {
            get {
                return this.ValueField;
            }
            set {
                if ((object.ReferenceEquals(this.ValueField, value) != true)) {
                    this.ValueField = value;
                    this.RaisePropertyChanged("Value");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UsageSensor", Namespace="http://schemas.datacontract.org/2004/07/CorsairDashboard.HardwareMonitoring.Hw.Se" +
        "nsors")]
    [System.SerializableAttribute()]
    public partial class UsageSensor : CorsairDashboard.HardwareMonitorService.HardwareSensor {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TemperatureSensor", Namespace="http://schemas.datacontract.org/2004/07/CorsairDashboard.HardwareMonitoring.Hw.Se" +
        "nsors")]
    [System.SerializableAttribute()]
    public partial class TemperatureSensor : CorsairDashboard.HardwareMonitorService.HardwareSensor {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="HardwareMonitorService.IHardwareMonitorService", CallbackContract=typeof(CorsairDashboard.HardwareMonitorService.IHardwareMonitorServiceCallback))]
    public interface IHardwareMonitorService {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IHardwareMonitorService/Subscribe")]
        void Subscribe();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IHardwareMonitorService/Subscribe")]
        System.Threading.Tasks.Task SubscribeAsync();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IHardwareMonitorService/Unsubscribe")]
        void Unsubscribe();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IHardwareMonitorService/Unsubscribe")]
        System.Threading.Tasks.Task UnsubscribeAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IHardwareMonitorServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IHardwareMonitorService/OnHardwareMonitorUpdate")]
        void OnHardwareMonitorUpdate(CorsairDashboard.HardwareMonitorService.Hardware hardware);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IHardwareMonitorServiceChannel : CorsairDashboard.HardwareMonitorService.IHardwareMonitorService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class HardwareMonitorServiceClient : System.ServiceModel.DuplexClientBase<CorsairDashboard.HardwareMonitorService.IHardwareMonitorService>, CorsairDashboard.HardwareMonitorService.IHardwareMonitorService {
        
        public HardwareMonitorServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public HardwareMonitorServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public HardwareMonitorServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public HardwareMonitorServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public HardwareMonitorServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void Subscribe() {
            base.Channel.Subscribe();
        }
        
        public System.Threading.Tasks.Task SubscribeAsync() {
            return base.Channel.SubscribeAsync();
        }
        
        public void Unsubscribe() {
            base.Channel.Unsubscribe();
        }
        
        public System.Threading.Tasks.Task UnsubscribeAsync() {
            return base.Channel.UnsubscribeAsync();
        }
    }
}