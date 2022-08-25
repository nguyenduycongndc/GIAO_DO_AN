using Data.DB;
using Data.Model.APIApp;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class OrderImageBusiness : GenericBusiness
    {
        public OrderImageBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        NotifyBusiness noti = new NotifyBusiness();
        OneSignalBusiness oneSignalBus = new OneSignalBusiness();
        public void AddOrderServiceImage(string Url, OrderService order, int imageRequiteID, string lang)
        {
            OrderServiceImage imageF = cnn.OrderServiceImages.Where(U => U.OrderServiceID.Equals(order.ID) && U.ServiceRequiteImageID.Value.Equals(imageRequiteID)).FirstOrDefault();
            ServiceRequiteImage require = cnn.ServiceRequiteImages.Find(imageRequiteID);
            int ID = cnn.Members.Where(u => u.CustomerID.Value.Equals(order.CustomerID)).FirstOrDefault().ID;
            if (imageF == null)
            {
                OrderServiceImage image = new OrderServiceImage();
                image.CreateDate = DateTime.Now;
                image.IsActive = SystemParam.ACTIVE;
                image.OrderServiceID = order.ID;
                image.Path = Url;
                image.ServiceRequiteImageID = imageRequiteID;
                cnn.OrderServiceImages.Add(image);
            }
            else
            {
                imageF.Path = Url;
                imageF.CreateDate = DateTime.Now;
            }
            cnn.SaveChanges();
        }

        public string AddOrderServiceImageCar(List<string> listUrl, int orderID, int type, string node, string lang)
        {
            OrderService order = cnn.OrderServices.Find(orderID);
            if (order == null)
                return lang.Equals(SystemParam.VN) ? MessVN.NOT_FOUND_MESS : MessEN.NOT_FOUND_MESS;
            foreach (string url in listUrl)
            {
                OrderServiceImageCar query = new OrderServiceImageCar();
                query.Image = url;
                query.Type = type;
                query.IsActive = SystemParam.ACTIVE;
                query.CreateDate = DateTime.Now;
                query.ComboCode = order.CodeCombo;
                query.OrderID = order.ID;
                cnn.OrderServiceImageCars.Add(query);
            }
            if (type.Equals(Constant.TYPE_ORDER_SERVICE_IMAGE_CAR))
            {
                order.CarNote = node;
            }
            else
            {
                order.ParkNote = node;
            }
            cnn.SaveChanges();
            return SystemParam.SUCCES_STR;
        }
        public int DeleteImage(List<int> ListID)
        {
            var listImage = cnn.OrderServiceImageCars.Where(u => ListID.Contains(u.ID)).ToList();
            foreach (var image in listImage)
            {
                image.IsActive = SystemParam.INACTIVE;
                Util.DeleteIamgeLocal(image.Image);
            }
            cnn.SaveChanges();
            return listImage.Select(u => u.OrderID.Value).FirstOrDefault();
        }
    }
}
