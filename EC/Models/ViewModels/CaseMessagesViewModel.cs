using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models.Database;
using EC.App_LocalResources;
using EC.Common.Interfaces;
using EC.Core.Common;
using EC.Models.ECModel;

namespace EC.Models.ViewModel
{
    public class CaseMessagesViewModel
    {
        public int Id { get; set; }
        public string body_tx { get; set; }
        public int report_id { get; set; }
        public int sender_id { get; set; }
        public int reporter_access { get; set; }
        public string created_dt { get; set; }
        public string sender_name { get; set; }
        public string sender_photo { get; set; }
        public bool isReaded { get; set; }

        public CaseMessagesViewModel BindMessageToViewMessage(message _message, int caller_id)
        {
            IDateTimeHelper m_DateTimeHelper = new DateTimeHelper();
            GlobalFunctions glb = new GlobalFunctions();
            UserModel um = new UserModel(caller_id);
            CaseMessagesViewModel vm_message = new CaseMessagesViewModel();
            ReportModel rm = new ReportModel(_message.report_id);

            string _month_name = "";
            string _sender_name = "";

            #region message_sender_name
            user temp_sender = um.GetById(_message.sender_id);

            if ((temp_sender != null) && (temp_sender.id != 0))
            {
                _sender_name = "";
                if (_message.sender_id == caller_id)
                {
                    _sender_name = GlobalRes.You;
                }
                else
                {
                    if (temp_sender.role_id == 8)
                    {
                        if ((rm._report.incident_anonymity_id == 1) || (rm._report.incident_anonymity_id == 2))
                        {
                            _sender_name = GlobalRes.anonymous_reporter;
                        }
                        else
                        {
                            _sender_name = GlobalRes.Reporter;
                        }
                    }
                    else
                    {
                        if (_message.sender_id != caller_id)
                        {
                            _sender_name = (temp_sender.first_nm + " " + temp_sender.last_nm).Trim();
                        }
                    }
                }
            }
            #endregion

            vm_message.sender_name = _sender_name;
            vm_message.sender_id = _message.sender_id;

            #region sender photo
            if (temp_sender.photo_path.Trim() != "")
            {
                vm_message.sender_photo = temp_sender.photo_path.Trim();
            }
            else
            {
                vm_message.sender_photo = "~/Content/Icons/anonimousReporterIcon.png";
            }
            #endregion

            #region Created Date
            vm_message.created_dt = "";
            if (_message.created_dt.HasValue)
            {
                _month_name = m_DateTimeHelper.GetFullMonth(_message.created_dt.Value.Month);
                vm_message.created_dt = _month_name + " " + _message.created_dt.Value.Day.ToString();
            }
            #endregion

            vm_message.Id = _message.id;
            vm_message.body_tx = _message.body_tx;
            vm_message.reporter_access = _message.reporter_access;
            vm_message.report_id = report_id;

            var _extended = new MessageExtended(vm_message.Id, caller_id);
            vm_message.isReaded = _extended.IsRead();

            return vm_message;
        }
    }
}