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
    public class CartBusiness : GenericBusiness
    {
        public string fullUrl = Util.getFullUrl();
        public CartBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        public string ConvertTopingIDtoName(string TopingID)
        {
            if (TopingID != null && TopingID != "")
            {
                List<int> listToping = TopingID.Split(',').Select(Int32.Parse).ToList();
                foreach (var item in listToping)
                {
                    var toping = cnn.ServicePrices.Where(x => x.ID == item).Select(x => x.Name).FirstOrDefault();
                    if (listToping.IndexOf(item) == 0)
                    {
                        TopingID = toping;
                    }
                    else
                    {
                        TopingID += toping;
                    }
                    if (listToping.IndexOf(item) != (listToping.Count - 1))
                    {
                        TopingID += ",";
                    }
                }
            }
            return TopingID;
        }
        public List<int> ConvertTopingID(string TopingID)
        {
            try
            {
                if (TopingID != null && TopingID != "")
                {
                    List<int> listToping = TopingID.Split(',').Select(Int32.Parse).ToList();
                    return listToping;
                }
                else
                {
                    return new List<int>(); 
                }
                
            }
            catch(Exception)
            {
                return new List<int>();
            }
           
        }
        // Lấy giỏ hàng theo ID
        public CartDetailOutput GetCartByID(int CartID)
        {
            try
            {
                var cart = (from c in cnn.Carts
                            join sp in cnn.ServicePrices on c.ServicePriceID equals sp.ID
                            where c.ID == CartID && c.IsActive == SystemParam.ACTIVE
                            select new CartDetailOutput
                            {
                                ID = c.ID,
                                BasePrice = c.BasePrice,
                                Price = c.Price,
                                SumPrice = c.Price * c.Quantity,
                                Name = sp.Name,
                                ServicePriceID = sp.ID,
                                Note = c.Note,
                                Quantity = c.Quantity,
                                Toping = c.Toping
                            }).FirstOrDefault();
                cart.ImageURL = fullUrl + cnn.ServiceImages.Where(x => x.ServiceID == cart.ServicePriceID).Select(x => x.Image).FirstOrDefault();
                cart.Toping = ConvertTopingIDtoName(cart.Toping);
                return cart;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public List<CartDetailOutput> GetCartDetailOutputs(int ShopID, int CustomerID)
        {
            try
            {
                var model = (from c in cnn.Carts
                             join sp in cnn.ServicePrices on c.ServicePriceID equals sp.ID
                             where c.ShopID == ShopID && c.CustomerID == CustomerID && c.IsActive == SystemParam.ACTIVE && sp.IsActive == SystemParam.ACTIVE
                             select new
                             {
                                 CartID = c.ID,
                                 BasePrice = c.BasePrice,
                                 Price = c.Price,
                                 Name = sp.Name,
                                 ServicePriceID = sp.ID,
                                 Note = c.Note,
                                 Quantity = c.Quantity,
                                 Toping = c.Toping
                             }).AsEnumerable().Select(x => new CartDetailOutput
                             {
                                 ID = x.CartID,
                                 BasePrice = x.BasePrice,
                                 Price = x.Price,
                                 SumPrice = x.Price * x.Quantity,
                                 Name = x.Name,
                                 ImageURL = fullUrl + cnn.ServiceImages.Where(s => s.ServiceID == x.ServicePriceID).Select(s => s.Image).FirstOrDefault(),
                                 ServicePriceID = x.ServicePriceID,
                                 Note = x.Note,
                                 Quantity = x.Quantity,
                                 Toping = ConvertTopingIDtoName(x.Toping),
                                 TopingIDs = ConvertTopingID(x.Toping)
                             }).ToList();

                return model;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public List<CartDetailOutput> GetOrderServiceDetails(int OrderServiceID)
        {
            try
            {
                var model = (from ods in cnn.OrderServiceDetails
                             join sp in cnn.ServicePrices on ods.ServicePriceID equals sp.ID
                             where ods.IsActive == SystemParam.ACTIVE && ods.OrderServiceID == OrderServiceID
                             select new
                             {
                                 OrderDetailID = ods.ID,
                                 BasePrice = ods.BasePrice,
                                 Price = ods.Price,
                                 Name = sp.Name,
                                 ServicePriceID = sp.ID,
                                 Note = ods.Note,
                                 Quantity = ods.Quantity,
                                 Toping = ods.Toping
                             }).AsEnumerable().Select(x => new CartDetailOutput
                             {
                                 ID = x.OrderDetailID,
                                 BasePrice = x.BasePrice,
                                 Price = x.Price,
                                 SumPrice = x.Price * x.Quantity,
                                 Name = x.Name,
                                 ImageURL = fullUrl + cnn.ServiceImages.Where(s => s.ServiceID == x.ServicePriceID).Select(s => s.Image).FirstOrDefault(),
                                 ServicePriceID = x.ServicePriceID,
                                 Note = x.Note,
                                 Quantity = x.Quantity,
                                 Toping = ConvertTopingIDtoName(x.Toping)
                             }).ToList();
                return model;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public int RemoveCart(int CustomerID)
        {
            try
            {
                var carts = cnn.Carts.Where(x => x.CustomerID == CustomerID && x.IsActive == SystemParam.ACTIVE).ToList();
                foreach(var item in carts)
                {
                    item.IsActive = SystemParam.ACTIVE_FALSE;
                }
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch(Exception e)
            {
                return SystemParam.ERROR;
            }
        }
        // Thêm vào giỏ hàng
        public int AddCart(int CustomerID, int ServicePriceID, List<int> TopingIDs, int Quantity)
        {
            try
            {
                var servicePrice = cnn.ServicePrices.FirstOrDefault(x => x.ID == ServicePriceID && x.IsActive == SystemParam.ACTIVE);
                if (servicePrice == null)
                {
                    return SystemParam.ITEM_CHANGED;
                }
                string toping = "";
                var topingPrice = 0;
                if (TopingIDs == null || TopingIDs.Count == 0)
                {
                    toping = null;
                }
                else
                {
                    TopingIDs.Sort();
                    foreach (var item in TopingIDs)
                    {
                        var servicePriceToping = cnn.ServicePrices.Where(x => x.ID == item && x.Type == SystemParam.SERVICE_TYPE_TOPING).FirstOrDefault();
                        if (servicePriceToping == null)
                            return SystemParam.TOPING_NOT_FOUND;
                        toping += item.ToString();
                        if (TopingIDs.IndexOf(item) != (TopingIDs.Count - 1))
                        {
                            toping += ",";
                        }
                        topingPrice += servicePriceToping.Price;
                    }
                }
                var cart = cnn.Carts.FirstOrDefault(x => x.CustomerID == CustomerID && x.ServicePriceID == ServicePriceID && x.Toping == toping && x.IsActive == SystemParam.ACTIVE);
                if (cart == null)
                {
                    if (Quantity <= 0)
                    {
                        return SystemParam.ADD_CART_INVALID_QUANTITY;
                    }
                    cart = new Cart
                    {
                        ServicePriceID = ServicePriceID,
                        CustomerID = CustomerID,
                        ShopID = servicePrice.ShopID,
                        Price = servicePrice.Price + topingPrice,
                        BasePrice = servicePrice.BasePrice + topingPrice,
                        Quantity = Quantity,
                        Toping = toping,
                        IsActive = SystemParam.ACTIVE,
                        CreatedDate = DateTime.Now
                    };
                    cnn.Carts.Add(cart);
                }
                else
                {
                    if (Quantity <= 0)
                    {
                        cart.IsActive = SystemParam.ACTIVE_FALSE;
                    }
                    else
                    {
                        cart.Price = servicePrice.Price + topingPrice;
                        cart.BasePrice = servicePrice.BasePrice + topingPrice;
                        cart.Quantity += Quantity;
                    }
                }
                cnn.SaveChanges();
                return cart.ID;
            }
            catch (Exception e)
            {
                return SystemParam.ERROR;
            }
        }
        // Cập nhật số lượng giỏ hàng
        public int UpdateCartQuantity(int CartID, int Quantity,int CustomerID)
        {
            try
            {
                var cart = cnn.Carts.FirstOrDefault(x => x.ID == CartID && x.IsActive == SystemParam.ACTIVE);
                if (cart == null)
                {
                    return SystemParam.CART_NOT_FOUND;
                }
                var listCart = cnn.Carts.Where(x => x.CustomerID == CustomerID && x.IsActive == SystemParam.ACTIVE && x.ShopID == cart.ShopID).ToList();
                //var check = false;
                //foreach(var item in listCart)
                //{
                //    var servicePrices = cnn.Services.FirstOrDefault(x => x.ID == item.ServicePriceID && x.IsActive == SystemParam.ACTIVE);
                //    if(servicePrices == null)
                //    {
                //        item.IsActive = SystemParam.ACTIVE_FALSE;
                //        cnn.SaveChanges();
                //        check = true;
                //    }
                //}
                //if (check)
                //{
                //    return SystemParam.CART_CHANGED;
                //}

                var servicePrice = cnn.ServicePrices.FirstOrDefault(x => x.ID == cart.ServicePriceID && x.IsActive == SystemParam.ACTIVE);
                if(servicePrice == null)
                {
                    cart.IsActive = SystemParam.ACTIVE_FALSE;
                    cnn.SaveChanges();
                    return SystemParam.CART_CHANGED;
                }
                var topingPrice = 0;
                if (cart.Toping != null && cart.Toping != "")
                {
                    List<int> listToping = cart.Toping.Split(',').Select(Int32.Parse).ToList();
                    foreach (var item in listToping)
                    {
                        var topingPrices = cnn.ServicePrices.Where(x => x.ID == item && x.IsActive == SystemParam.ACTIVE).FirstOrDefault();
                        if (topingPrices == null)
                        {
                            cart.IsActive = SystemParam.ACTIVE_FALSE;
                            cnn.SaveChanges();
                            return SystemParam.CART_CHANGED;
                        }
                        topingPrice += topingPrices.Price;
                    }
                }
                if (Quantity <= 0)
                {
                    cart.IsActive = SystemParam.ACTIVE_FALSE;
                }
                else
                {
                    cart.Price = servicePrice.Price + topingPrice;
                    cart.BasePrice = servicePrice.BasePrice + topingPrice;
                    cart.Quantity = Quantity;
                }
                cnn.SaveChanges();
                return cart.ID;
            }
            catch (Exception e)
            {
                return SystemParam.ERROR;
            }
        }
        // Cập nhật ghi chú giỏ hàng
        public int UpdateCartNote(int CartID, string Note)
        {
            try
            {
                var cart = cnn.Carts.FirstOrDefault(x => x.ID == CartID && x.IsActive == SystemParam.ACTIVE);
                if (cart == null)
                {
                    return SystemParam.CART_NOT_FOUND;
                }
                cart.Note = Note;
                cnn.SaveChanges();
                return cart.ID;
            }
            catch (Exception e)
            {
                return SystemParam.ERROR;
            }
        }
    }
}
