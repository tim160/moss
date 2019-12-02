using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Constants;

namespace EC.Models.ViewModels
{
    public class UserLevelViewModel
    {
        public UserStatusButtonsStatus GetStatusButtonsState(int user_id)
        {
            UserStatusButtonsStatus buttons_status = new UserStatusButtonsStatus();

            //user session_user = (user)Session[ECGlobalConstants.CurrentUserMarcker];

            //if (session_user == null || session_user.id == 0)

            //    return buttons_status;

            if (user_id <= 0)
            {
                return buttons_status;
            }

            UserModel um = new UserModel(user_id);

            /// what is the 'selected' button

            buttons_status.current_user_status = um._user.status_id;

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
    public class UserStatusButtonsStatus
    {
        public int active_button_status = 0;
        public int pending_button_status = 0;
        public int inactive_button_status = 0;
        public int current_user_status = -1; // ili inactive?
    }
}