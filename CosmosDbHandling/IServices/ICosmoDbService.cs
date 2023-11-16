using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseModel;

namespace Services.IServices
{
    public interface ICosmoDbService
    {
        public Task<string> SaveToCosmosDB(MessageingModel data);
    }
}
