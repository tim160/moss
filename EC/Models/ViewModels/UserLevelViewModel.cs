using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Constants;
using EC.Models.Database;

namespace EC.Models.ViewModels
{
    public class UserLevelViewModel
    {
        public UserStatusViewModel GetStatusButtonsState(int user_id, user sessionUser)
        {
            UserStatusViewModel buttons_status = new UserStatusViewModel();
            UserModel um = null;
            if (sessionUser == null || sessionUser.id == 0)
                return buttons_status;
            if (user_id == 0)
            {
                buttons_status.current_user_status = 0;
                um = new UserModel();
            } else
            {
                um = new UserModel(user_id);

                /// what is the 'selected' button

                buttons_status.current_user_status = um._user.status_id;
            }


            switch (buttons_status.current_user_status)

            {

                case ECStatusConstants.Inactive_Value:

                    buttons_status.active_button_status = 0;

                    buttons_status.pending_button_status = 0;

                    buttons_status.inactive_button_status = 1; //selected in line current_user_status

                    if (um._user.last_login_dt.HasValue)

                        buttons_status.active_button_status = 1;

                    else

                        buttons_status.pending_button_status = 1;

                    break;

                case ECStatusConstants.Active_Value:

                    buttons_status.active_button_status = 1; //selected in line current_user_status

                    buttons_status.pending_button_status = 0;

                    buttons_status.inactive_button_status = 1;

                    break;

                case ECStatusConstants.Pending_Value:

                    buttons_status.active_button_status = 0;

                    buttons_status.pending_button_status = 1; //selected in line current_user_status

                    buttons_status.inactive_button_status = 1;

                    break;

                default:

                    //all disabled

                    buttons_status.active_button_status = 0;

                    buttons_status.pending_button_status = 0;

                    buttons_status.inactive_button_status = 0;

                    break;
            }
            return buttons_status;
        }
    }
}