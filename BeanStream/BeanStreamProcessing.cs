using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Data.SqlClient;
////using System.Web.Configuration;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;

namespace LavaBlast.Util.CreditCards
{
	/// <summary>
	/// Base class for BeanStream Credit Card Processing. Only provides basic features; Verified by Visa is not implemented. 
	/// 
	/// Copyright (c) 2007 Jason Kealey of LavaBlast Software Inc. http://www.lavablast.com
	/// 
	/// Licenced under the MIT licence. 
	/// 
	/// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
	/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
	/// 
	/// </summary>
	public class BeanStreamProcessing
	{
		#region "fields"
		// containers for reference tables.
		protected static Dictionary<RequestFieldNames, int> MaxLengths;
		protected static List<RequestFieldNames> NumericFields;
		protected static List<RequestFieldNames> DollarFields;

		// our merchant id given to us by beanstream
		protected string merchant_id;
		public string MerchantID { get { return merchant_id; } set { merchant_id = value; } }

		// the transaction auth code 
		protected string _AuthCode;
		public string AuthCode { get { return _AuthCode; } }

		// the transaction error message 
		protected string _MessageText;
		public string MessageText { get { return _MessageText; } }

		// used in this class and its children
		protected XmlDocument xmlRequest;
		protected XmlDocument xmlResponse;

		#endregion

		#region "constructors"
		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="merchant_id">the beanstream-assigned merchant id</param>
		public BeanStreamProcessing(string merchant_id)
		{
			this.MerchantID = merchant_id;
		}
		#endregion

		#region "reference tables"
		/// <summary>
		/// Static reference tables. 
		/// </summary>
		static BeanStreamProcessing()
		{
			MaxLengths = new Dictionary<RequestFieldNames, int>();

			// soap
			MaxLengths.Add(RequestFieldNames.serviceVersion, 6);

			// basis fields
			MaxLengths.Add(RequestFieldNames.merchant_id, 9);
			MaxLengths.Add(RequestFieldNames.trnOrderNumber, 30);
			MaxLengths.Add(RequestFieldNames.trnType, 3);
			MaxLengths.Add(RequestFieldNames.trnLanguage, 3);
			MaxLengths.Add(RequestFieldNames.errorPage, 128);
			MaxLengths.Add(RequestFieldNames.username, 16);
			MaxLengths.Add(RequestFieldNames.password, 16);
			MaxLengths.Add(RequestFieldNames.adjId, 8);
			MaxLengths.Add(RequestFieldNames.approvedPage, 8000);
			MaxLengths.Add(RequestFieldNames.declinedPage, 8000);
			MaxLengths.Add(RequestFieldNames.termURL, 8000);
			MaxLengths.Add(RequestFieldNames.trnComments, 8000);
			MaxLengths.Add(RequestFieldNames.cavEnabled, 1);
			MaxLengths.Add(RequestFieldNames.cavPassCode, 32);
			MaxLengths.Add(RequestFieldNames.vbvEnabled, 1);
			MaxLengths.Add(RequestFieldNames.scEnabled, 1);
			MaxLengths.Add(RequestFieldNames.cavServiceVersion, 3);

			// credit info
			MaxLengths.Add(RequestFieldNames.trnCardOwner, 64);
			MaxLengths.Add(RequestFieldNames.trnCardNumber, 20);
			MaxLengths.Add(RequestFieldNames.trnExpMonth, 2);
			MaxLengths.Add(RequestFieldNames.trnExpYear, 2);
			MaxLengths.Add(RequestFieldNames.trnCardCvd, 4);

			// billing info
			MaxLengths.Add(RequestFieldNames.ordName, 64);
			MaxLengths.Add(RequestFieldNames.ordEmailAddress, 64);
			MaxLengths.Add(RequestFieldNames.ordPhoneNumber, 32);
			MaxLengths.Add(RequestFieldNames.ordAddress1, 64);
			MaxLengths.Add(RequestFieldNames.ordAddress2, 64);
			MaxLengths.Add(RequestFieldNames.ordCity, 32);
			MaxLengths.Add(RequestFieldNames.ordProvince, 2);
			MaxLengths.Add(RequestFieldNames.ordPostalCode, 16);
			MaxLengths.Add(RequestFieldNames.ordCountry, 2);

			// shipping info
			MaxLengths.Add(RequestFieldNames.shipName, 64);
			MaxLengths.Add(RequestFieldNames.shipEmailAddress, 64);
			MaxLengths.Add(RequestFieldNames.shipPhoneNumber, 32);
			MaxLengths.Add(RequestFieldNames.shipAddress1, 64);
			MaxLengths.Add(RequestFieldNames.shipAddress2, 64);
			MaxLengths.Add(RequestFieldNames.shipCity, 32);
			MaxLengths.Add(RequestFieldNames.shipProvince, 2);
			MaxLengths.Add(RequestFieldNames.shipPostalCode, 16);
			MaxLengths.Add(RequestFieldNames.shipCountry, 2);
			MaxLengths.Add(RequestFieldNames.shippingMethod, 64);
			MaxLengths.Add(RequestFieldNames.deliveryEstimate, 9);

			// product info
			MaxLengths.Add(RequestFieldNames.prod_id_n, 32);
			MaxLengths.Add(RequestFieldNames.prod_name_n, 64);
			MaxLengths.Add(RequestFieldNames.prod_quantity_n, 9);
			MaxLengths.Add(RequestFieldNames.prod_shipping_n, 9);
			MaxLengths.Add(RequestFieldNames.prod_cost_n, 9);
			MaxLengths.Add(RequestFieldNames.ordItemPrice, 9);
			MaxLengths.Add(RequestFieldNames.ordShippingPrice, 9);
			MaxLengths.Add(RequestFieldNames.ordTax1Price, 9);
			MaxLengths.Add(RequestFieldNames.ordTax2Price, 9);
			MaxLengths.Add(RequestFieldNames.trnAmount, 9);

			MaxLengths.Add(RequestFieldNames.rbBillingPeriod, 1);
			MaxLengths.Add(RequestFieldNames.trnRecurring, 1);
			MaxLengths.Add(RequestFieldNames.rbBillingIncrement, 64);
			MaxLengths.Add(RequestFieldNames.rbFirstBilling, 8);
			MaxLengths.Add(RequestFieldNames.rbExpiry, 8);

			// the list of numeric fields 
			NumericFields = new List<RequestFieldNames>();
			NumericFields.Add(RequestFieldNames.merchant_id);
			NumericFields.Add(RequestFieldNames.cavEnabled);
			NumericFields.Add(RequestFieldNames.vbvEnabled);
			NumericFields.Add(RequestFieldNames.scEnabled);
			NumericFields.Add(RequestFieldNames.cavServiceVersion);
			NumericFields.Add(RequestFieldNames.trnCardNumber);
			NumericFields.Add(RequestFieldNames.trnExpMonth);
			NumericFields.Add(RequestFieldNames.trnExpYear);
			NumericFields.Add(RequestFieldNames.trnCardCvd);
			NumericFields.Add(RequestFieldNames.deliveryEstimate);
			NumericFields.Add(RequestFieldNames.prod_quantity_n);

			NumericFields.Add(RequestFieldNames.trnRecurring);
			NumericFields.Add(RequestFieldNames.rbBillingIncrement);
			NumericFields.Add(RequestFieldNames.rbFirstBilling);
			NumericFields.Add(RequestFieldNames.rbExpiry);


			// the list of fields in dollar format
			DollarFields = new List<RequestFieldNames>();
			DollarFields.Add(RequestFieldNames.prod_shipping_n);
			DollarFields.Add(RequestFieldNames.prod_cost_n);
			DollarFields.Add(RequestFieldNames.ordItemPrice);
			DollarFields.Add(RequestFieldNames.ordShippingPrice);
			DollarFields.Add(RequestFieldNames.ordTax1Price);
			DollarFields.Add(RequestFieldNames.ordTax2Price);
			DollarFields.Add(RequestFieldNames.trnAmount);
		}
		#endregion

		/// <summary>
		/// Builds a default request, ready to be augmented with additional fields. Sets merchant_id and service version. 
		/// </summary>
		/// <returns>the default request</returns>
		public Dictionary<RequestFieldNames, string> GetDefaultRequest()
		{
			Dictionary<RequestFieldNames, string> request = new Dictionary<RequestFieldNames, string>();
            /////tim - Add values from WebConfig.
	/*		request.Add(RequestFieldNames.merchant_id, ConfigurationManager.AppSettings["Beanstream_Merchant_id"]);
			request.Add(RequestFieldNames.serviceVersion, ConfigurationManager.AppSettings["Beanstream_Version"]);

			request.Add(RequestFieldNames.username, ConfigurationManager.AppSettings["Beanstream_Login"]);
			request.Add(RequestFieldNames.password, ConfigurationManager.AppSettings["Beanstream_Pass"]);
            */
			return request;
		}

		/// <summary>
		/// Sends a request for processing by beanstream. 
		/// </summary>
		/// <param name="request">the request to be sent</param>
		/// <returns>the response returned by beanstream</returns>
		public ResponseStates ProcessRequest(Dictionary<RequestFieldNames, string> request, out string errMessage, out string authCode)
		{
			try {
				authCode = "";

				if (String.IsNullOrEmpty(merchant_id))
                    throw new Exception("Merchant ID was not configured");

				////	throw new Exception(Resources.Resource.errMsgMerchant_idNotConfig);

				string xml = BuildXmlRequest(request); // build an xml document from the request

				
            /* todelete   BeanStream.beanstream.TransactionProcessRequest tg = new BeanStream.beanstream.TransactionProcessRequest();
                BeanStreamProcessing tt = new BeanStreamProcessing();
                BeanStream.beanstream.TransClassSoapPortClient ff = new BeanStream.beanstream.TransClassSoapPortClient();
                ff.TransactionProcess("");// .TransactionProcessRequest
                */

                // execute the request at beanstream
                /////tim old    WalkerGroup.DKOR.beanstream.ProcessTransaction p = new WalkerGroup.DKOR.beanstream.ProcessTransaction();
                ////tim //new code - check
                BeanStream.beanstream.TransClassSoapPortClient p = new BeanStream.beanstream.TransClassSoapPortClient();

				string resp = p.TransactionProcess(xml); // might throw some kind of network exception. will get caught by our try-catch. 

				// log the actual string returned.. in case it is not well formed
				SafeLogResponse(resp);

				// load the response in an xml document
				xmlResponse = new XmlDocument();
				xmlResponse.LoadXml(resp); // might fail if beanstream doesn't give us good xml - caught by try-catch. 

				//           System.Web.HttpContext.Current.Response.Write(xmlResponse.InnerText.ToString());
				//            System.Web.HttpContext.Current.Response.Flush();

				// ensure this is a transaction
				XmlNode node = xmlResponse.SelectSingleNode("/response/responseType");
				///////			if (node == null || node.InnerText != "T")
				////			throw new ArgumentException(string.Format("Response type ({0}) has not been implemented. Was Verified by Visa enabled?;", node.InnerText));

				node = xmlResponse.SelectSingleNode("/response/trnApproved");
				// if it is not approved
				if (node == null || node.InnerText != "1") {

					// Our system doesn't need detailed messages, but could obtain them by improving this class' API
					//XmlNode errorFields = xmlResponse.SelectSingleNode("/response/errorFields");
					//XmlNode messageId = xmlResponse.SelectSingleNode("/response/messageId");

					// this is sufficient for our purposes. 
					XmlNode messageText = xmlResponse.SelectSingleNode("/response/messageText");
					if (messageText != null) _MessageText = messageText.InnerText;

					// Does beanstream say this is a user error, a system error or is did the transaction just get denied?
					XmlNode errorType = xmlResponse.SelectSingleNode("/response/errorType");
					errMessage = _MessageText;
					if (errorType == null)
						return ResponseStates.CodingError; // node not found in beanstream message
					else if (errorType.InnerText == "N") {
						errMessage = "Not enough funds";
						return ResponseStates.Denied; // not enough funds, etc. 
					} else if (errorType.InnerText == "S") {
						errMessage = "Network problems";
						return ResponseStates.SystemError; // network problems at beanstream, etc. 
					} else if (errorType.InnerText == "U") {
						errMessage = "Problem with your transaction";
						return ResponseStates.UserError; // bad province code, etc. 
					} else {
						errMessage = "Unknown error type";
						return ResponseStates.CodingError; // unknown error type. 
					}
				} else if (node.InnerText == "1") // is approved
				{
					node = xmlResponse.SelectSingleNode("/response/trnAuthCode");
					if (node != null)
						this._AuthCode = node.InnerText; // remember for external use. 
					authCode = this._AuthCode;
					errMessage = "";

					return ResponseStates.Approved;
				}
			} catch (Exception ex) {
				authCode = "";
                ///tim new code - check
                SafeLogException("Unable Process Card", ex);

			////tim	old SafeLogException(Resources.Resource.errMsgUnableProcessCard, ex);
			}
			// if it gets to here, we had an error of some sorts. 
			errMessage = _MessageText;
			return ResponseStates.CodingError;
		}

		/// <summary>
		/// Builds an xml request from our dictionary. Also re-initializes the object and logs the request. 
		/// </summary>
		/// <param name="request">the request</param>
		/// <returns>an xml equivalent of the request, ready to be sent to beanstream</returns>
		protected string BuildXmlRequest(Dictionary<RequestFieldNames, string> request)
		{
			try {
				xmlRequest = new XmlDocument();
				xmlResponse = null; // clear anything cached. 
				_AuthCode = string.Empty; // clear anything cached. 
				_MessageText = string.Empty; // clear anything cached. 

				// new transaction. 
				XmlNode transaction = xmlRequest.CreateNode(XmlNodeType.Element, "transaction", string.Empty);
				xmlRequest.AppendChild(transaction);

				// add all the keys we have. 
				foreach (RequestFieldNames key in request.Keys) {
					AddChild(xmlRequest, transaction, key, request[key]);
				}
			} catch (Exception ex) {
				ex = ex;
			}
			string str = xmlRequest.OuterXml;
			SafeLogRequest(str);

			return str;
		}

		/// <summary>
		/// Adds a new child xml node to the transaction. 
		/// </summary>
		/// <param name="xml">the xml request</param>
		/// <param name="transaction">the transaction in it</param>
		/// <param name="field">the field name to be added</param>
		/// <param name="value">the value to be added</param>
		protected static void AddChild(XmlDocument xml, XmlNode transaction, RequestFieldNames field, string value)
		{
			XmlNode n = xml.CreateNode(XmlNodeType.Element, GetField(field), string.Empty);

			value = CleanValue(field, value);

			n.InnerText = value;
			transaction.AppendChild(n);
		}

		/// <summary>
		/// Cleans the value according to the field's data format and maximum length. 
		/// </summary>
		/// <param name="field">the field</param>
		/// <param name="value">the value</param>
		/// <returns>the sanitized field</returns>
		protected static string CleanValue(RequestFieldNames field, string value)
		{
			value = value ?? string.Empty; // get rid of null
			value = value.Trim();

			// could throw exception but we don't care about cropped addresses in their system because our system remembers the original string, except for the CC-related things.
			if (value.Length > MaxLengths[field]) value = value.Substring(0, MaxLengths[field]);

			if (NumericFields.Contains(field)) {
				if (!Regex.Match(value, @"^[0-9]{0," + MaxLengths[field] + "}$").Success)
					throw new ArgumentException(String.Format("Field {0} was not a number of the proper length (value={1}).", GetField(field), value));
			}
			if (DollarFields.Contains(field)) {
				if (!("0" == value || Regex.Match(value, @"^[0-9]{0,6}\.[0-9]{0,2}$").Success))
					throw new ArgumentException(String.Format("Field {0} was not a dollar amount (value={1}).", GetField(field), value));
			}
			return value;
		}

		#region "logging"
		/// <summary>
		/// Logs the request via #LogRequest(msg) but catches any errors to prevent any exceptions from bubbling up
		/// </summary>
		/// <param name="msg">the xml request</param>
		protected void SafeLogRequest(string msg)
		{
			try {
				// should always replace the credit card number, validation code, and expiry date before outputting to disk. 
				LogRequest(msg);
			} catch (Exception ex) {
				SafeLogException("Unable to log request.", ex);
			}
		}

		/// <summary>
		/// Logs the response via #LogResponse(msg) but catches any errors to prevent any exceptions from bubbling up
		/// </summary>
		/// <param name="msg">the xml response</param>
		protected void SafeLogResponse(string msg)
		{
			try {
				LogResponse(msg);
			} catch (Exception ex) {
				SafeLogException("Unable to log response.", ex);
			}

		}

		/// <summary>
		/// Logs the exception via #LogException(msg,ex) but catches any errors to prevent any exceptions from bubbling up
		/// </summary>
		/// <param name="msg">the error message</param>
		/// <param name="ex">the exception that occurred</param>
		protected void SafeLogException(string msg, Exception ex)
		{
			try {
				_MessageText = msg;
				LogException(msg, ex);
			} catch {
				// no where left to log!
			}
		}

		/// <summary>
		/// Logs the request. To be implemented by subclasses. 
		/// </summary>
		/// <param name="msg">the xml request</param>
		protected virtual void LogRequest(string msg)
		{
			// used in the testing phase. don't uncomment as would be a security hole. 
			//string str = xmlRequest.OuterXml;
			//System.Diagnostics.Debug.WriteLine(str);
			//System.Diagnostics.Debug.WriteLine("****");
			// should always replace the credit card number, validation code, and expiry date before outputting to disk. 

		}

		/// <summary>
		/// Logs the response. To be implemented by subclasses. 
		/// </summary>
		/// <param name="msg">the xml response</param>
		protected virtual void LogResponse(string msg)
		{
			//System.Diagnostics.Debug.WriteLine(msg);
			//System.Diagnostics.Debug.WriteLine("****");
		}

		/// <summary>
		/// Logs the exception. To be implemented by subclasses. 
		/// </summary>
		/// <param name="msg">the error message</param>
		/// <param name="ex">the exception</param>
		protected virtual void LogException(string msg, Exception ex)
		{
			//System.Diagnostics.Debug.Writeline(msg);
			//System.Diagnostics.Debug.WriteLine("****");
		}
		#endregion

		#region "enums"

		/// <summary>
		/// All the field names accepted in a request to beanstream. 
		/// </summary>
		public enum RequestFieldNames
		{
			serviceVersion, merchant_id, trnOrderNumber, trnType, trnLanguage, errorPage, username, password, adjId, approvedPage, declinedPage, termURL, trnComments, cavEnabled, cavPassCode, vbvEnabled, scEnabled, cavServiceVersion, trnCardOwner, trnCardNumber, trnExpMonth, trnExpYear, trnCardCvd, ordName, ordEmailAddress, ordPhoneNumber, ordAddress1, ordAddress2, ordCity, ordProvince, ordPostalCode, ordCountry, shipName, shipEmailAddress, shipPhoneNumber, shipAddress1, shipAddress2, shipCity, shipProvince, shipPostalCode, shipCountry, shippingMethod, deliveryEstimate, prod_id_n, prod_name_n, prod_quantity_n, prod_shipping_n, prod_cost_n, ordItemPrice, ordShippingPrice, ordTax1Price, ordTax2Price, trnAmount, trnRecurring, rbBillingPeriod, rbBillingIncrement, rbFirstBilling, rbExpiry
		}

		/// <summary>
		/// The possible states after execution. CodingError is any exception that we catch here. Approved, Denied, UserError and SystemError are all defined by beanstream. 
		/// </summary>
		public enum ResponseStates
		{
			Approved, Denied, SystemError, UserError, CodingError
		}

		#endregion

		#region "utility methods"
		/// <summary>
		/// Given a field name, return the equivalent RequestFieldNames value. 
		/// </summary>
		/// <param name="s">the field name</param>
		/// <returns>the field</returns>
		public static RequestFieldNames GetField(string s)
		{
			return (RequestFieldNames)Enum.Parse(typeof(RequestFieldNames), s);
		}

		/// <summary>
		/// Given a field, return the field name. 
		/// </summary>
		/// <param name="f">the field</param>
		/// <returns>the field name</returns>
		public static string GetField(RequestFieldNames f)
		{
			return Enum.GetName(typeof(RequestFieldNames), f);
		}
		#endregion
	}
}
