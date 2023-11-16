using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface IStoargeQueueService
    {
        public Task<string> SendMessageToQueueAsync<MessageModel>(MessageModel data,string error);
    }
}
