using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class ProductBusiness : GenericBusiness
    {
        public ProductBusiness(WE_SHIPEntities context = null) : base()
        {
        }
        TransactionHistoryBusiness tranBus = new TransactionHistoryBusiness();
        NotifyBusiness notiBus = new NotifyBusiness();
        CouponBusiness couponBus = new CouponBusiness();
        /// <summary>
        /// Lấy ra các sản phẩm đang chạy
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="CateID"></param>
        /// <param name="SearchKey"></param>
        /// <returns></returns>
        public List<ProductModel> GetProduct(string lang, int? CateID, string SearchKey = "")
        {
            List<ProductModel> query = new List<ProductModel>();
            if (String.IsNullOrEmpty(SearchKey))
                SearchKey = "";
            query = cnn.Products.Where(u => u.Status.Equals(1) && u.IsActive.Equals(SystemParam.ACTIVE) && (CateID.HasValue ? u.CategoryID.Equals(CateID.Value) : true) && u.QTY > 0).Select(u => new ProductModel
            {
                ProductID = u.ID,
                BasePrice = u.BasePrice,
                CategoryName = lang.Equals(SystemParam.VN) ? u.Category.NameVN : u.Category.NameEN,
                CategoryID = u.CategoryID,
                Description = u.Description,
                Discount = u.Discount.Value,
                Name = lang.Equals(SystemParam.VN) ? u.NameVN : u.NameEN,
                Price = u.Price,
                QTY = u.QTY.Value,
                ListImage = u.ProductImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).Select(i => i.Path).ToList()
            }).ToList();
            query = query.Where(u => Util.Converts(u.Name).Contains(Util.Converts(SearchKey))).ToList();
            return query;
        }
        public List<OrderProductDetail> GetCart(int washerID)
        {
            List<OrderProductDetail> query = cnn.Carts.Where(u =>
            u.IsActive.Equals(SystemParam.ACTIVE)
            && u.Product.Status.Equals(SystemParam.ACTIVE)
            && u.Product.IsActive.Equals(SystemParam.ACTIVE)
            && u.Product.QTY.Value > 0
            && u.WasherID.Equals(washerID)
            ).Select(s => new OrderProductDetail
            {
                ProductID = s.ProductID,
                ID = s.ID,
                QTY = s.Product.QTY.Value,
                Name = s.Product.NameVN,
                PerPrice = s.Product.Price,
                Price = s.Product.Price,
                BasePrice = s.Product.BasePrice,
                Discouint = s.Product.Discount.Value,
                ListImage = s.Product.ProductImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).Select(i => i.Path).ToList()
            }).ToList();
            return query;
        }

        /// <summary>
        /// Lấy ra các danh mục sản phẩm
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public List<ProductCateModel> GetProductCate(string lang)
        {
            List<ProductCateModel> query = cnn.Categories.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).OrderBy(u => u.DisplayOrder.Value).Select(u => new ProductCateModel
            {
                ID = u.ID,
                Name = lang.Equals(SystemParam.VN) ? u.NameVN : u.NameEN
            }).ToList();
            return query;
        }

        /// <summary>
        /// List giỏ hàng của agent
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<ListOrder> GetOrders(int status = 0, int? agentID = null)
        {
            List<ListOrder> query = new List<ListOrder>();
            query = cnn.ProductOrders.Where(u =>
            u.IsActive.Equals(SystemParam.ACTIVE)
            && u.ProductOrderDetails.Where(d => d.IsActive.Equals(SystemParam.ACTIVE) && d.QTY > 0).Count() > 0
            && (status > 0 ? u.Status.Equals(status) : (u.Status.Equals((int)StatusOrderProduct.AdminReject) || u.Status.Equals((int)StatusOrderProduct.Complete) || u.Status.Equals((int)StatusOrderProduct.Reject)))
            && (agentID.HasValue ? u.AgentID.Equals(agentID.Value) : true)).Select(u => new ListOrder
            {
                OrderID = u.ID,
                Code = u.Code,
                Price = u.TotalPrice,
                CompleteDate = u.CompleteDate,
                ConfirmDate = u.ConfirmDate,
                CreateDate = u.CreateDate,
                QTY = u.ProductOrderDetails.Where(s => s.IsActive.Equals(SystemParam.ACTIVE)).Select(s => s.QTY).Sum(),
                Status = u.Status,
                Image = u.ProductOrderDetails.Where(s => s.IsActive.Equals(SystemParam.ACTIVE)).Select(s => s.Product.ProductImages.Select(i => i.Path).FirstOrDefault()).FirstOrDefault()
            }).OrderByDescending(u => u.CreateDate).ToList();
            return query;
        }

        /// <summary>
        /// Chi tiết giỏ hàng
        /// </summary>
        /// <param name="cartID"></param>
        /// <returns></returns>
        public OrderrDetail GetOrderDetail(int cartID)
        {
            OrderrDetail query = new OrderrDetail();
            query = cnn.ProductOrders.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ID.Equals(cartID)).Select(u => new OrderrDetail
            {
                OrderID = u.ID,
                Code = u.Code,
                BasePrice = u.BasePrice.HasValue ? u.BasePrice.Value : u.TotalPrice,
                Discount = u.Discount.HasValue ? u.Discount.Value : 0,
                Price = u.TotalPrice,
                CompleteDate = u.CompleteDate,
                ConfirmDate = u.ConfirmDate,
                CounponCode = u.CouponID.HasValue ? u.Coupon.Code : "",
                CreateDate = u.CreateDate,
                QTY = u.ProductOrderDetails.Where(s => s.IsActive.Equals(SystemParam.ACTIVE)).Select(s => s.QTY).Sum(),
                Status = u.Status,
                WasherCode = u.Agent.Code,
                WasherName = u.Agent.Name,
                WasherPhone = u.Agent.Phone,
                ListOrderProductDetail = u.ProductOrderDetails.Where(s => s.IsActive.Equals(SystemParam.ACTIVE) && s.QTY > 0).OrderByDescending(s => s.ID).Select(s => new OrderProductDetail
                {
                    ProductID = s.ProductID,
                    ID = s.ID,
                    QTY = s.QTY,
                    Name = s.Product.NameVN,
                    Description = s.Product.Description,
                    Price = s.Price,
                    PerPrice = s.Product.Price,
                    BasePrice = s.Product.BasePrice,
                    ListImage = s.Product.ProductImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).Select(i => i.Path).ToList()
                }).ToList()
            }).FirstOrDefault();
            return query;
        }

        /// <summary>
        /// Mua hàng
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="product"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public OrderrDetail EditCart(int agentID, Product product, int qty)
        {
            List<ProductOrder> list = cnn.ProductOrders.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Status.Equals((int)StatusOrderProduct.Ordering)).OrderByDescending(u => u.ID).ToList();
            if (list.Count > 0)
            {
                ProductOrder order = list.FirstOrDefault();
                ProductOrderDetail detail = cnn.ProductOrderDetails.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ProductOrderID.Equals(order.ID) && u.ProductID.Equals(product.ID)).FirstOrDefault();
                if (detail == null)
                {
                    CreateOrderProductDetail(product, qty, order.ID);
                }
                else
                {
                    EditOrderProductDetail(detail, qty);
                }
                EditOrderProduct(order);
                return GetOrderDetail(list.FirstOrDefault().ID);
            }
            else
            {
                ProductOrder order = CreateOrderProduct(agentID, product, qty);
                return GetOrderDetail(order.ID);
            }
        }

        public SystemResult ChangeStatusOrder(WasherCartInputModel item, int agentID)
        {
            var lsProduct = item.listProduct.Select(u => u.productID).ToList();
            var ListProduct = cnn.Products.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && lsProduct.Contains(u.ID)).ToList();
            foreach (Product pro in ListProduct)
            {
                int qty = item.listProduct.Where(u => u.productID.Equals(pro.ID)).Select(u => u.qty).FirstOrDefault();
                EditCart(agentID, pro, qty);
            }
            ProductOrder list = cnn.ProductOrders.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Status.Equals((int)StatusOrderProduct.Ordering)).OrderByDescending(u => u.ID).FirstOrDefault();
            SystemResult result = ChangeStatusProductOrder((int)StatusOrderProduct.WaitingAdminConfirm, list.ID, agentID);
            return result;
        }

        public int CreateOrderProduct(WasherCartInputModel item, int agentID, TokenOutputModel checktoken)
        {
            List<int> LSProductID = item.listProduct.Where(u => u.qty > 0).Select(u => u.productID).ToList();
            Member washer = cnn.Members.Where(u => u.AgentID.Value.Equals(agentID)).FirstOrDefault();
            List<Product> LisProduct = cnn.Products.Where(u => u.Status.Equals(SystemParam.ACTIVE) && u.QTY > 0 && u.IsActive.Equals(SystemParam.ACTIVE) && LSProductID.Contains(u.ID)).ToList();
            LisProduct = LisProduct.Where(u => u.QTY >= item.listProduct.Where(s => s.productID.Equals(u.ID)).Select(s => s.qty).FirstOrDefault()).ToList();
            if (LSProductID.Count != LisProduct.Count)
                return 0;
            List<ProductOrderDetail> orderDetail = item.listProduct.Where(u => u.qty > 0).Select(u => new ProductOrderDetail
            {
                CreateDate = DateTime.Now,
                IsActive = SystemParam.ACTIVE,
                ProductID = u.productID,
                QTY = u.qty,
                Price = u.qty * LisProduct.Where(s => s.ID.Equals(u.productID)).FirstOrDefault().Price,
            }).ToList();
            ProductOrder order = new ProductOrder
            {
                Code = GetOrderProductCode(agentID),
                AgentID = agentID,
                CreateDate = DateTime.Now,
                IsActive = SystemParam.ACTIVE,
                Status = (int)StatusOrderProduct.WaitingAdminConfirm,
                ProductOrderDetails = orderDetail,
                TotalPrice = orderDetail.Select(u => u.Price).Sum(),
                BasePrice = orderDetail.Select(u => u.Price).Sum(),
            };
            // check coupon
            if (!String.IsNullOrEmpty(item.counponCode) && item.counponCode.Length > 0)
            {
                CouponOutputModel counpon = couponBus.checkCoupon(checktoken, item.counponCode, 0, 0);
                if (counpon == null)
                    return -2;
                int distcount = 0;
                if (counpon.TypeDiscount.Equals(Constant.COUPON_TYPE_PERCENT))
                {
                    distcount = (int)(order.BasePrice.Value * counpon.Discount) / 100;
                }
                else
                {
                    if (order.BasePrice <= counpon.Discount)
                        distcount = order.BasePrice.Value;
                    else
                        distcount = counpon.Discount;
                }
                order.TotalPrice -= distcount;
                order.Discount = distcount;
                order.CouponID = counpon.CouponID;
                Coupon cp = cnn.Coupons.Find(counpon.CouponID);
                if (cp.QTY > 0 && cp.Remain > 0)
                    cp.Remain--;
            }
            // check tiền
            int profitWallets = washer.Wallets.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.TYPE.Equals(Constant.WALLET_WITHDRAW)).Select(u => u.Balance).FirstOrDefault();
            int despositWallets = washer.Wallets.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.TYPE.Equals(Constant.WALLET_NO_WITHDRAW)).Select(u => u.Balance).FirstOrDefault();
            if (despositWallets + profitWallets < order.TotalPrice)
                return -1;
            cnn.ProductOrders.Add(order);
            List<Cart> listCart = cnn.Carts.Where(u =>
            u.IsActive.Equals(SystemParam.ACTIVE)
            && u.Product.Status.Equals(SystemParam.ACTIVE)
            && u.WasherID.Equals(agentID)
            && LSProductID.Contains(u.ProductID)
            ).ToList();
            foreach (Cart cart in listCart)
            {
                cart.IsActive = 0;
            }
            cnn.SaveChanges();
            if (order.TotalPrice > 0)
                Payment(order, washer);
            ChangeQtyProduct(order, (int)StatusOrderProduct.WaitingAdminConfirm);
            return order.ID;
        }
        public void Payment(ProductOrder order, Member washer)
        {
            List<Wallet> lsWallet = cnn.Wallets.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.MemberID.Equals(washer.ID)).ToList();
            Wallet profitWallet = lsWallet.Where(u => u.TYPE.Equals(Constant.WALLET_WITHDRAW)).FirstOrDefault();
            Wallet despositWallet = lsWallet.Where(u => u.TYPE.Equals(Constant.WALLET_NO_WITHDRAW)).FirstOrDefault();
            string code1 = GetOrderProductCode(profitWallet.ID);
            string code2 = GetOrderProductCode(despositWallet.ID);
            if (profitWallet.Balance >= order.TotalPrice)
            {
                tranBus.CreateTransaction(washer.ID, order.TotalPrice, Constant.TYPE_TRANSACTION_WASHER_ORDER_PRODUCT, code1, null, order.ID, Constant.WALLET_WITHDRAW, SystemParam.VN, 1);
                order.ProfitPoint = order.TotalPrice;
                order.DepositPoint = 0;
            }
            else
            {
                int point = order.TotalPrice - profitWallet.Balance;
                if (profitWallet.Balance > 0)
                    tranBus.CreateTransaction(washer.ID, profitWallet.Balance, Constant.TYPE_TRANSACTION_WASHER_ORDER_PRODUCT, code1, null, order.ID, Constant.WALLET_WITHDRAW, SystemParam.VN, 1);
                tranBus.CreateTransaction(washer.ID, point, Constant.TYPE_TRANSACTION_WASHER_ORDER_PRODUCT, code2, null, order.ID, Constant.WALLET_NO_WITHDRAW, SystemParam.VN, 1);
                order.ProfitPoint = profitWallet.Balance;
                order.DepositPoint = order.TotalPrice;
            }
            cnn.SaveChanges();
        }

        /// <summary>
        /// Lấy mã đơn hàng
        /// </summary>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public string GetOrderProductCode(int agentID)
        {
            return Util.CreateMD5(DateTime.Now.ToString() + agentID.ToString()).Substring(0, 10);
        }

        /// <summary>
        /// tạo mới giỏ hàng
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="product"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public ProductOrder CreateOrderProduct(int agentID, Product product, int qty)
        {
            List<ProductOrderDetail> Product = CreateOrderProductDetail(product, qty);
            ProductOrder order = new ProductOrder
            {
                Code = GetOrderProductCode(agentID),
                AgentID = agentID,
                CreateDate = DateTime.Now,
                IsActive = SystemParam.ACTIVE,
                Status = (int)StatusOrderProduct.Ordering,
                ProductOrderDetails = Product,
                TotalPrice = Product.Select(u => u.Price).FirstOrDefault()
            };
            cnn.ProductOrders.Add(order);
            cnn.SaveChanges();
            return order;
        }

        /// <summary>
        /// Tạo mới đơn hàng trong giỏ
        /// </summary>
        /// <param name="product"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public List<ProductOrderDetail> CreateOrderProductDetail(Product product, int qty, int? orderID = null)
        {
            List<ProductOrderDetail> data = new List<ProductOrderDetail>();
            if (!orderID.HasValue)
            {
                ProductOrderDetail orderDetail = new ProductOrderDetail
                {
                    CreateDate = DateTime.Now,
                    IsActive = SystemParam.ACTIVE,
                    ProductID = product.ID,
                    QTY = qty,
                    Price = qty * product.Price,
                };
                data.Add(orderDetail);
            }
            else
            {
                ProductOrderDetail orderDetail = new ProductOrderDetail
                {
                    CreateDate = DateTime.Now,
                    IsActive = SystemParam.ACTIVE,
                    ProductID = product.ID,
                    QTY = qty,
                    ProductOrderID = orderID.Value,
                    Price = qty * product.Price,
                };
                cnn.ProductOrderDetails.Add(orderDetail);
                cnn.SaveChanges();
            }
            return data;
        }

        /// <summary>
        /// Cập nhật lại số lượng
        /// </summary>
        /// <param name="product"></param>
        /// <param name="qty"></param>
        public void EditOrderProductDetail(ProductOrderDetail product, int qty)
        {
            product.QTY = qty;
            product.Price = qty * product.Product.Price;
            cnn.SaveChanges();
        }

        /// <summary>
        /// Sửa lại giá tiền
        /// </summary>
        /// <param name="order"></param>
        public void EditOrderProduct(ProductOrder order)
        {
            order.TotalPrice = cnn.ProductOrderDetails.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ProductOrderID.Equals(order.ID) && u.Product.Status.Equals(SystemParam.ACTIVE) && u.QTY > 0).Select(u => u.Price).Sum();
            cnn.SaveChanges();
        }

        public SystemResult ChangeStatusProductOrder(int status, int orderID, int? agentID = null, string note = "")
        {
            ProductOrder order = cnn.ProductOrders.Find(orderID);
            if (order == null || order.IsActive.Equals(SystemParam.INACTIVE) || order.ProductOrderDetails.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.QTY > 0).Count() == 0)
                return resultBus.ErrorResult(agentID.HasValue ? MessVN.NOT_FOUND_MESS : MessEN.NOT_FOUND_MESS);
            if (agentID.HasValue)
            {
                if (status.Equals((int)StatusOrderProduct.Reject))
                {
                    if (order.Status.Equals((int)StatusOrderProduct.WaitingAdminConfirm))
                    {
                        ChangeQtyProduct(order, status);
                        order.Status = status;
                        cnn.SaveChanges();
                        // hoàn trả tiền
                        RefurnMoney(order);
                        return resultBus.SucessResult(true, SystemParam.SUCCES_STR);
                    }
                    return resultBus.ErrorResult("Bạn không thể huỷ đơn hàng này");
                }
                return resultBus.ErrorResult("Sai dữ liệu");
            }
            // web
            else
            {
                if (order.Status.Equals(status))
                    return resultBus.SucessResult(false);
                switch (status)
                {
                    case (int)StatusOrderProduct.AdminConfirm:
                        {
                            return ChangeStatusAdminConfirm(order);
                        }
                    case (int)StatusOrderProduct.Complete:
                        {
                            return ChangeStatusComplete(order);
                        }
                    case (int)StatusOrderProduct.AdminReject:
                        {
                            return ChangeStatusAdminReject(order, note);
                        }
                }
                return resultBus.ErrorResult("Param Error");
            }
            return resultBus.SucessResult(true);
        }

        public string ChangeQtyProduct(ProductOrder order, int status)
        {
            order.Status = status;
            List<ProductOrderDetail> lsProductOrderD = cnn.ProductOrderDetails.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.QTY > 0 && u.ProductOrderID.Equals(order.ID)).ToList();
            List<int> ListID = lsProductOrderD.Select(s => s.ProductID).ToList();
            List<Product> lsProduct = cnn.Products.Where(u => ListID.Contains(u.ID)).ToList();
            foreach (Product pro in lsProduct)
            {
                int qty = lsProductOrderD.Where(u => u.ProductID.Equals(pro.ID)).FirstOrDefault().QTY;
                if (status.Equals((int)StatusOrderProduct.WaitingAdminConfirm))
                {
                    if (pro.QTY < qty)
                        return "Sản phẩm " + pro.NameVN + " không đủ hàng";
                    else
                        pro.QTY -= qty;
                }
                else
                {
                    pro.QTY += qty;
                }
            }
            cnn.SaveChanges();
            return SystemParam.SUCCES_STR;
        }

        public SystemResult CheckChangeStatusPay(ProductOrder order, Member agent)
        {
            int profitWallet = agent.Wallets.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.TYPE.Equals(Constant.WALLET_WITHDRAW)).Select(u => u.Balance).FirstOrDefault();
            int despositWallet = agent.Wallets.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.TYPE.Equals(Constant.WALLET_NO_WITHDRAW)).Select(u => u.Balance).FirstOrDefault();
            if (despositWallet + profitWallet < order.TotalPrice)
                return resultBus.ErrorResult("Bạn không đủ tiền để thực hiện gia dịch này", (int)ErrorCode.NotEnoughMoney);
            List<ProductOrderDetail> lsProductDetail = cnn.ProductOrderDetails.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.QTY > 0 && u.ProductOrderID.Equals(order.ID)).ToList();
            List<int> lsID = lsProductDetail.Select(s => s.ProductID).ToList();
            List<Product> lsProduct = cnn.Products.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && lsID.Contains(u.ID)).ToList();
            lsProduct = lsProduct.Where(u => u.QTY < lsProductDetail.Where(s => s.ProductID.Equals(u.ID)).FirstOrDefault().QTY).ToList();
            if (lsProduct.Count() > 0)
            {
                return resultBus.ErrorResult("Sản phẩm " + lsProduct.FirstOrDefault().NameVN + " không đủ hàng. Chúng tôi đã cập nhật lại số lượng trong giỏ hàng của bạn");
            }
            if (lsProductDetail.Where(u => u.Product.Status.Equals(0)).Count() > 0)
            {
                return resultBus.ErrorResult("Sản phẩm " + lsProduct.FirstOrDefault().NameVN + " đang không còn hoạt đông. Chúng tối đã cập nhật lại giỏ hàng của bạn");
            }
            return resultBus.SucessResult(SystemParam.SUCCES_STR, SystemParam.SUCCES_STR);
        }

        public string ChangeQtyOrderProductDetail(int CartID)
        {
            List<ProductOrderDetail> lsProductOrderD = cnn.ProductOrderDetails.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ProductOrderID.Equals(CartID)).ToList();
            List<int> lsProductID = lsProductOrderD.Select(s => s.ProductID).ToList();
            List<Product> lsProduct = cnn.Products.Where(u => lsProductID.Contains(u.ID)).ToList();
            string mess = "";
            foreach (ProductOrderDetail Detail in lsProductOrderD)
            {
                Product pro = lsProduct.Where(u => u.ID.Equals(Detail.ProductID)).FirstOrDefault();
                if (pro.Status.Equals(SystemParam.INACTIVE))
                {
                    Detail.IsActive = SystemParam.INACTIVE;
                    mess = "Trạng thái của sản phẩm đã bị thay đổi";
                }
                else if (pro.QTY < Detail.QTY)
                {
                    Detail.QTY = pro.QTY.Value;
                    mess = "Trạng thái của sản phẩm đã bị thay đổi";
                }
            }
            cnn.SaveChanges();
            return mess;
        }
        // api web
        public List<ProductViewModel> Search(string name, int? categoryId, string fromDate, string toDate)
        {
            DateTime? startDate = Util.ConvertDate(fromDate);
            DateTime? endDate = Util.ConvertDate(toDate);
            if (endDate != null)
                endDate = endDate.Value.AddDays(1);
            var query = (from p in cnn.Products
                         where p.IsActive.Equals(SystemParam.ACTIVE)
                         && (categoryId.HasValue ? p.CategoryID.Equals(categoryId.Value) : true)
                         && (startDate.HasValue ? p.CreateDate >= startDate.Value : true)
                         && (endDate.HasValue ? p.CreateDate <= endDate.Value : true)
                         select new ProductViewModel
                         {
                             ID = p.ID,
                             Code = p.ProductCode,
                             CategoryName = p.Category.NameEN,
                             NameEN = p.NameEN,
                             NameVN = p.NameVN,
                             Price = p.BasePrice,
                             Status = p.Status,
                             CreatedDate = p.CreateDate
                         }).ToList();
            query = query.Where(x => Util.Converts(x.NameEN.ToLower()).Contains(Util.Converts(name.ToLower())) || Util.Converts(x.NameVN.ToLower()).Contains(Util.Converts(name.ToLower()))).OrderByDescending(x => x.CreatedDate).ToList();
            return query;
        }
        public List<CategoryViewModel> GetListCategory()
        {
            var list = cnn.Categories.Where(x => !x.IsActive.Equals(SystemParam.INACTIVE)).Select(x =>
                new CategoryViewModel
                {
                    Name = x.NameEN,
                    ID = x.ID
                }).ToList();
            return list;
        }
        public SystemResult ChangeStatusAdminConfirm(ProductOrder order)
        {
            int status = (int)StatusOrderProduct.AdminConfirm;
            if (order.Status != (int)StatusOrderProduct.WaitingAdminConfirm)
                return resultBus.ErrorResult("Cant change status this order");
            order.Status = status;
            order.ConfirmDate = DateTime.Now;
            cnn.SaveChanges();
            sendnoti(order, "Đơn " + order.Code + " của bạn đã được xác nhận", Constant.NOTI_ORDER_PRODUCT);
            return resultBus.SucessResult(true);
        }
        public void sendnoti(ProductOrder order, string content, int type)
        {
            notiBus.CreateNoti(order.Agent.Members.FirstOrDefault().ID, type, null, null, order.ID, SystemParam.VN, content, true);
        }
        public void RefurnMoney(ProductOrder order)
        {
            List<MembersTransactionHistory> lsTranssaction = cnn.MembersTransactionHistories.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ProductOrderID.Value.Equals(order.ID)).ToList();
            foreach (MembersTransactionHistory tran in lsTranssaction)
            {
                string code = GetOrderProductCode(tran.ID);
                tranBus.CreateTransaction(tran.MemberID, tran.Point, Constant.TYPE_TRANSACTION_WASHER_CANCEL_ORDER_PRODUCT, code, null, tran.ProductOrderID, tran.Wallet.TYPE, SystemParam.VN, 1);
            }
        }
        public SystemResult ChangeStatusComplete(ProductOrder order)
        {
            int status = (int)StatusOrderProduct.Complete;
            if (order.Status != (int)StatusOrderProduct.AdminConfirm)
                return resultBus.ErrorResult("Cant change status this order");
            order.Status = status;
            order.CompleteDate = DateTime.Now;
            cnn.SaveChanges();
            sendnoti(order, "Đơn " + order.Code + " của bạn đã được hoàn thành", Constant.NOTI_ORDER_PRODUCT_HISTORY);
            return resultBus.SucessResult(true);
        }
        public SystemResult ChangeStatusAdminReject(ProductOrder order, string content)
        {
            int status = (int)StatusOrderProduct.AdminReject;
            if (order.Status == (int)StatusOrderProduct.Complete)
                return resultBus.ErrorResult("Cant change status this order");
            order.Status = status;
            cnn.SaveChanges();
            RefurnMoney(order);
            ChangeQtyProduct(order, status);
            sendnoti(order, "Đơn " + order.Code + " của bạn đã bị huỷ với lý do: " + content, Constant.NOTI_ORDER_PRODUCT_HISTORY);
            return resultBus.SucessResult(true);
        }

        public ProductDetailViewModel Detail(int ID)
        {
            ProductDetailViewModel query = new ProductDetailViewModel();
            query = cnn.Products.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ID.Equals(ID)).Select(u => new ProductDetailViewModel
            {
                ProductID = u.ID,
                BasePrice = u.BasePrice,
                CategoryName = u.Category.NameEN,
                CategoryID = u.CategoryID,
                Description = u.Description,
                Discount = u.Discount.Value,
                Name = u.NameEN,
                NameVN = u.NameVN,
                Code = u.ProductCode,
                DisplayOrder = u.DisplayOrder.Value,
                Status = u.Status,
                Price = u.Price,
                QTY = u.QTY.Value,
                ListImage = u.ProductImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).Select(i => i.Path).ToList(),
                Isdelete = cnn.ProductOrders.Where(x => x.ProductOrderDetails.Where(c => c.ProductID == u.ID && x.IsActive == SystemParam.ACTIVE && (x.Status == (int)StatusOrderProduct.AdminConfirm || x.Status == (int)StatusOrderProduct.AdminConfirm)).Count() > 0).Count() > 0 ? false : true
            }).FirstOrDefault();
            return query;
        }
        public SystemResult CreateProduct(ProductDetailViewModel item)
        {
            var pro = cnn.Products.Where(u => !u.IsActive.Equals(SystemParam.INACTIVE) && u.ProductCode.Equals(item.Code)).Count();
            if (pro > 0)
                return resultBus.ErrorResult("Code already in use");
            Product product = new Product
            {
                BasePrice = item.BasePrice,
                CategoryID = item.CategoryID,
                CreateDate = DateTime.Now,
                Description = item.Description,
                Discount = item.Discount,
                DisplayOrder = item.DisplayOrder,
                IsActive = SystemParam.ACTIVE,
                QTY = item.QTY,
                NameEN = item.Name,
                NameVN = item.NameVN,
                Price = (item.BasePrice * (100 - item.Discount) / 100),
                Status = item.Status,
                ProductCode = item.Code,
                Point = 0,
                ProductImages = item.ListImage.Select(s => new ProductImage
                {
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now,
                    Path = s,
                }).ToList()
            };
            cnn.Products.Add(product);
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult Edit(ProductDetailViewModel item)
        {
            var pro = cnn.Products.Find(item.ProductID);
            if (!pro.IsActive.Equals(SystemParam.ACTIVE))
                return resultBus.ErrorResult("Data not found ");
            pro.BasePrice = item.BasePrice;
            pro.CategoryID = item.CategoryID;
            pro.CreateDate = DateTime.Now;
            pro.Description = item.Description;
            pro.Discount = item.Discount;
            pro.DisplayOrder = item.DisplayOrder;
            pro.IsActive = SystemParam.ACTIVE;
            pro.QTY = item.QTY;
            pro.NameEN = item.Name;
            pro.NameVN = item.NameVN;
            pro.Price = (item.BasePrice * (100 - item.Discount) / 100);
            pro.Status = item.Status;
            cnn.ProductImages.RemoveRange(pro.ProductImages);
            if (item.ListImage != null)
                pro.ProductImages = item.ListImage.Select(s => new ProductImage
                {
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now,
                    Path = s,
                }).ToList();
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult Delete(int ID)
        {
            var pro = cnn.Products.Find(ID);
            if (!pro.IsActive.Equals(SystemParam.ACTIVE))
                return resultBus.ErrorResult("Data not found ");
            var lsProduct = cnn.ProductOrderDetails.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ProductID.Equals(ID) && u.QTY > 0).Count();
            if (lsProduct > 0)
                return resultBus.ErrorResult("Cant delete this product");
            pro.IsActive = SystemParam.INACTIVE;
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }

        public void DeleteCart(List<int> lsProductID, int washerID)
        {
            List<Cart> listCart = cnn.Carts.Where(u =>
            u.IsActive.Equals(SystemParam.ACTIVE)
            && u.Product.Status.Equals(SystemParam.ACTIVE)
            && u.WasherID.Equals(washerID)
            && lsProductID.Contains(u.ProductID)
            ).ToList();
            foreach (Cart cart in listCart)
            {
                cart.IsActive = 0;
            }
            cnn.SaveChanges();
        }
        public string UpdateCart(int washerID, int productID, int type)
        {
            List<Cart> listCart = cnn.Carts.Where(u =>
            u.IsActive.Equals(SystemParam.ACTIVE)
            && u.Product.Status.Equals(SystemParam.ACTIVE)
            && u.WasherID.Equals(washerID)
            ).ToList();
            Product pro = cnn.Products.Where(u => u.IsActive.Equals(1) && u.Status.Equals(1) && u.QTY > 0 && u.ID.Equals(productID)).FirstOrDefault();
            if (pro == null)
                return "Sản phẩm không tồn tại";
            var cart = listCart.Where(u => u.ProductID.Equals(productID)).FirstOrDefault();
            if (type.Equals(SystemParam.ACTIVE))
            {
                if (cart != null)
                    return SystemParam.SUCCES_STR;
                Cart carts = new Cart
                {
                    WasherID = washerID,
                    CreateDate = DateTime.Now,
                    IsActive = SystemParam.ACTIVE,
                    ProductID = productID,
                };
                cnn.Carts.Add(carts);
            }
            else
            {
                if (cart == null)
                    return SystemParam.SUCCES_STR;
                cart.IsActive = SystemParam.INACTIVE;
            }
            cnn.SaveChanges();
            return SystemParam.SUCCES_STR;


        }
    }
}
