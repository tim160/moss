using EC.Models.Database;
using EC.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace EC.Models
{
    public class CaseMessagesModel : BaseModel
    {
        public message _message { get; set; }
        public List<CaseMessagesViewModel> AllMessagesList { get; set; }
        public CaseMessagesModel()
        {
        }
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
        public CaseMessagesModel(int report_id, int thread_id, int caller_id)
        {
            List<CaseMessagesViewModel> cmvm = CaseMessagesList(report_id, thread_id, caller_id);
        }
    }
}
