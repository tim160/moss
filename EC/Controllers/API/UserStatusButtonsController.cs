using EC.Constants;
using EC.Models.Database;
using EC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace EC.Controllers.API
{
    public class UserStatusButtonsController : BaseApiController
    {
        [HttpGet]
        public object Get(int user_id)
        {
            user sessionUser = (user)HttpContext.Current.Session[ECGlobalConstants.CurrentUserMarcker];

            UserLevelViewModel buttonValidations = new UserLevelViewModel();
            
            return ResponseObject2Json(new
            {
                UserStatusViewModel = buttonValidations.GetStatusButtonsState(user_id, sessionUser)
            });
        }
    }
}