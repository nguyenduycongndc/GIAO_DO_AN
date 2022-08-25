using Data.DB;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class GenericBusiness
    {
        public WE_SHIPEntities context;
        public WE_SHIPEntities cnn ;
        public ResponseBusiness rpBus = new ResponseBusiness();

        public GenericBusiness(WE_SHIPEntities context = null)
        {
            this.context = context == null ? new WE_SHIPEntities() : context;
            cnn = this.context;
            //this.context.Configuration.AutoDetectChangesEnabled = false;
            //this.context.Configuration.ValidateOnSaveEnabled = false;
            //this.context.Configuration.LazyLoadingEnabled = false;
        }
    }
}
