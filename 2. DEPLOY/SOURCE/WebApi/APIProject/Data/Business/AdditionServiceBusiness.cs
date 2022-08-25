using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class AdditionServiceBusiness : GenericBusiness
    {
        public AdditionServiceBusiness(WE_SHIPEntities context = null) : base()
        {
        }

        public SystemResult AddAdditionServiceToMainService(int? mainService, int? addtionService)
        {
            try
            {
                MainServiceAdditionService service = new MainServiceAdditionService();
                service.AdditionServiceID = addtionService.Value;
                service.MainServiceID = mainService.Value;
                service.IsActive = SystemParam.ACTIVE;
                service.CreateDate = DateTime.Now;
                cnn.MainServiceAdditionServices.Add(service);
                cnn.SaveChanges();
                return resultBus.SucessResult(null);
            }
            catch (Exception ex)
            {
                return resultBus.ErrorResult(ex.ToString());
            }
        }
        public SystemResult Delete(int mainService, int addtionService)
        {
            MainServiceAdditionService service = cnn.MainServiceAdditionServices.Where(u => u.MainServiceID.Equals(mainService) && u.AdditionServiceID.Equals(addtionService)).FirstOrDefault();
            if (service != null) {
                service.IsActive = 0;
            }
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult ListService(int TYPE)
        {
            try
            {
                List<AdditionServiceModel> listAdd = cnn.Services.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && !u.Type.Equals(TYPE)).OrderBy(u => u.OrderDisplay.Value).Select(u => new AdditionServiceModel
                {
                    ID = u.ID,
                    Name = u.NameEN
                }).ToList();
                return resultBus.SucessResult(listAdd);
            }
            catch (Exception ex)
            {
                return resultBus.ErrorResult(ex.ToString());
            }
        }
    }
}
