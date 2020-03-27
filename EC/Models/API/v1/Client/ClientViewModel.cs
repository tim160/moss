using System.Collections.Generic;

namespace EC.Models.API.v1.Client
{
    public class ClientViewModel
    {
        public int Total { get; set; }
        public List<ClientModel> Items { get; set; }
    }
}