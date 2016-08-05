using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using EC.Models;
using EC.Models.Database;
using EC.Models.ECModel;
using EC.Models.ViewModel;

using EC.Common.Interfaces;
using EC.App_LocalResources;

namespace EC.Models.App.Case
{
    public class CaseMessagesModel : BaseEntity
    {
        public string body_tx { get; set; }
        public message _message { get; set; }
        public CaseMessagesViewModel _caseMessagesViewModel { get; set; }

        public List<CaseMessagesViewModel> CaseMessagesList(int report_id, int thread_id, int caller_id)
        {
            CaseMessagesViewModel vm_message;

            List<CaseMessagesViewModel> _list = new List<CaseMessagesViewModel>();
            UserModel um = new UserModel(caller_id);
            ReportModel rm = new ReportModel(report_id);

            List<message> _messages = um.UserMessages(report_id, thread_id).ToList();
            foreach (message _message in _messages)
            {
                vm_message = new CaseMessagesViewModel();
                vm_message = new CaseMessagesViewModel().BindMessageToViewMessage(_message, caller_id);
                _list.Add(vm_message);
            }

            AllMessagesList = _list;
            return _list;
        }

        public List<CaseMessagesViewModel> AllMessagesList { get; set; }

        public CaseMessagesModel()
        {
        }

        public CaseMessagesModel(int report_id, int thread_id, int caller_id)
        {
             List<CaseMessagesViewModel> cmvm =  CaseMessagesList(report_id, thread_id, caller_id);
        }

        public CaseMessagesModel(int message_id)
        {
            //CaseMessagesType
            message _message_temp = db.message.Where(item => item.id == message_id).FirstOrDefault();
            if (_message_temp != null)
            {
                _message.body_tx = _message_temp.body_tx;
                _message.report_id = _message_temp.report_id;
                _message.sender_id = _message_temp.sender_id;
                _message.created_dt = _message_temp.created_dt.Value;
            }
        }

        public List<CaseMessagesViewModel> AddMessage(CaseMessagesViewModel newViewModel, List<CaseMessagesViewModel> list)
        {
            list.Add(newViewModel);
            return list;
        }
    }
}