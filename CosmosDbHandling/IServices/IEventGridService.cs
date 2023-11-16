using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseModel;

namespace Services.IServices
{
    public interface IEventGridService
    {
        public Task<string> SendEventAsync(MessageingModel eventData);
    }
}
