﻿using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using EC.Common.Util;
using EC.Common.Util.Models.API;
using EC.Models;
using EC.Models.Database;
using Newtonsoft.Json;

namespace EC.Controllers.API
{
	//[AuthFilterApi]
	public class BaseApiController : ApiController
	{
		private ECEntities db = null;
		internal ECEntities DB
		{
			get
			{
				if (db == null)
				{
					db = new ECEntities();
					db.Configuration.ProxyCreationEnabled = false;
					db.Configuration.LazyLoadingEnabled = false;
				}

				return db;
			}
		}

		internal HttpResponseMessage ResponseObject2Json(object obj)
		{
			var json = JsonConvert.SerializeObject(obj, Formatting.Indented,
				new JsonSerializerSettings
				{
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
					PreserveReferencesHandling = PreserveReferencesHandling.None
				});

			return new HttpResponseMessage() { Content = new StringContent(json, Encoding.UTF8, "application/json") };
		}

		internal bool is_cc
		{


			get
			{
				return DomainUtil.IsCC(HttpContext.Current.Request.Url.AbsoluteUri.ToLower());
			}
		}
		public EmailNotificationModel emailNotificationModel = new EmailNotificationModel();

		#region Response results

		protected ApiResponseResult ApiOk(
			string message = null,
			string status = null,
			string exceptionDetails = null)
		{
			return new ApiResponseResult(HttpStatusCode.OK, message, status, exceptionDetails);
		}

		protected ApiResponseResult<T> ApiOk<T>(
			T data,
			string message = null,
			string status = null,
			string exceptionDetails = null)
		{
			return new ApiResponseResult<T>(HttpStatusCode.OK, data, message, status, exceptionDetails);
		}

		protected ApiResponseResult<T> ApiCreated<T>(
			T data,
			string message = null,
			string status = null,
			string exceptionDetails = null)
		{
			return new ApiResponseResult<T>(HttpStatusCode.Created, data, message, status, exceptionDetails);
		}

		protected ApiBadRequestResult ApiBadRequest(string message) => new ApiBadRequestResult(message);

		protected ApiBadRequestResult ApiBadRequest(ModelStateDictionary modelState) => new ApiBadRequestResult(modelState, this);

		protected ApiResponseResult ApiNotFound(string message = null) => new ApiResponseResult(HttpStatusCode.NotFound, message);

		#endregion
	}
}
