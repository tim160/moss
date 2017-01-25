﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BeanStream.beanstream {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://tempuri.org/wsdl/", ConfigurationName="beanstream.TransClassSoapPort")]
    public interface TransClassSoapPort {
        
        // CODEGEN: Generating message contract since the wrapper namespace (http://tempuri.org/message/) of message TransactionProcessRequest does not match the default value (http://tempuri.org/wsdl/)
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/action/TransClass.TransactionProcess", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        BeanStream.beanstream.TransactionProcessResponse TransactionProcess(BeanStream.beanstream.TransactionProcessRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/action/TransClass.TransactionProcess", ReplyAction="*")]
        System.Threading.Tasks.Task<BeanStream.beanstream.TransactionProcessResponse> TransactionProcessAsync(BeanStream.beanstream.TransactionProcessRequest request);
        
        // CODEGEN: Generating message contract since the wrapper namespace (http://tempuri.org/message/) of message TransactionProcessAuthRequest does not match the default value (http://tempuri.org/wsdl/)
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/action/TransClass.TransactionProcessAuth", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        BeanStream.beanstream.TransactionProcessAuthResponse TransactionProcessAuth(BeanStream.beanstream.TransactionProcessAuthRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/action/TransClass.TransactionProcessAuth", ReplyAction="*")]
        System.Threading.Tasks.Task<BeanStream.beanstream.TransactionProcessAuthResponse> TransactionProcessAuthAsync(BeanStream.beanstream.TransactionProcessAuthRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="TransactionProcess", WrapperNamespace="http://tempuri.org/message/", IsWrapped=true)]
    public partial class TransactionProcessRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string inputTrans;
        
        public TransactionProcessRequest() {
        }
        
        public TransactionProcessRequest(string inputTrans) {
            this.inputTrans = inputTrans;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="TransactionProcessResponse", WrapperNamespace="http://tempuri.org/message/", IsWrapped=true)]
    public partial class TransactionProcessResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string Result;
        
        public TransactionProcessResponse() {
        }
        
        public TransactionProcessResponse(string Result) {
            this.Result = Result;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="TransactionProcessAuth", WrapperNamespace="http://tempuri.org/message/", IsWrapped=true)]
    public partial class TransactionProcessAuthRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string inputTrans;
        
        public TransactionProcessAuthRequest() {
        }
        
        public TransactionProcessAuthRequest(string inputTrans) {
            this.inputTrans = inputTrans;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="TransactionProcessAuthResponse", WrapperNamespace="http://tempuri.org/message/", IsWrapped=true)]
    public partial class TransactionProcessAuthResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public string Result;
        
        public TransactionProcessAuthResponse() {
        }
        
        public TransactionProcessAuthResponse(string Result) {
            this.Result = Result;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface TransClassSoapPortChannel : BeanStream.beanstream.TransClassSoapPort, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TransClassSoapPortClient : System.ServiceModel.ClientBase<BeanStream.beanstream.TransClassSoapPort>, BeanStream.beanstream.TransClassSoapPort {
        
        public TransClassSoapPortClient() {
        }
        
        public TransClassSoapPortClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TransClassSoapPortClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TransClassSoapPortClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TransClassSoapPortClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        BeanStream.beanstream.TransactionProcessResponse BeanStream.beanstream.TransClassSoapPort.TransactionProcess(BeanStream.beanstream.TransactionProcessRequest request) {
            return base.Channel.TransactionProcess(request);
        }
        
        public string TransactionProcess(string inputTrans) {
            BeanStream.beanstream.TransactionProcessRequest inValue = new BeanStream.beanstream.TransactionProcessRequest();
            inValue.inputTrans = inputTrans;
            BeanStream.beanstream.TransactionProcessResponse retVal = ((BeanStream.beanstream.TransClassSoapPort)(this)).TransactionProcess(inValue);
            return retVal.Result;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BeanStream.beanstream.TransactionProcessResponse> BeanStream.beanstream.TransClassSoapPort.TransactionProcessAsync(BeanStream.beanstream.TransactionProcessRequest request) {
            return base.Channel.TransactionProcessAsync(request);
        }
        
        public System.Threading.Tasks.Task<BeanStream.beanstream.TransactionProcessResponse> TransactionProcessAsync(string inputTrans) {
            BeanStream.beanstream.TransactionProcessRequest inValue = new BeanStream.beanstream.TransactionProcessRequest();
            inValue.inputTrans = inputTrans;
            return ((BeanStream.beanstream.TransClassSoapPort)(this)).TransactionProcessAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        BeanStream.beanstream.TransactionProcessAuthResponse BeanStream.beanstream.TransClassSoapPort.TransactionProcessAuth(BeanStream.beanstream.TransactionProcessAuthRequest request) {
            return base.Channel.TransactionProcessAuth(request);
        }
        
        public string TransactionProcessAuth(string inputTrans) {
            BeanStream.beanstream.TransactionProcessAuthRequest inValue = new BeanStream.beanstream.TransactionProcessAuthRequest();
            inValue.inputTrans = inputTrans;
            BeanStream.beanstream.TransactionProcessAuthResponse retVal = ((BeanStream.beanstream.TransClassSoapPort)(this)).TransactionProcessAuth(inValue);
            return retVal.Result;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<BeanStream.beanstream.TransactionProcessAuthResponse> BeanStream.beanstream.TransClassSoapPort.TransactionProcessAuthAsync(BeanStream.beanstream.TransactionProcessAuthRequest request) {
            return base.Channel.TransactionProcessAuthAsync(request);
        }
        
        public System.Threading.Tasks.Task<BeanStream.beanstream.TransactionProcessAuthResponse> TransactionProcessAuthAsync(string inputTrans) {
            BeanStream.beanstream.TransactionProcessAuthRequest inValue = new BeanStream.beanstream.TransactionProcessAuthRequest();
            inValue.inputTrans = inputTrans;
            return ((BeanStream.beanstream.TransClassSoapPort)(this)).TransactionProcessAuthAsync(inValue);
        }
    }
}