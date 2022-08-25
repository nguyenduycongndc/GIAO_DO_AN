using APIProject.Models;
using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class NewsBusiness : GenericBusiness
    {
        string urlHost = Util.getFullUrl();
        public NewsBusiness(WE_SHIPEntities context = null) : base()
        {
        }
        //   OneSignalBusiness oneSignalBus = new OneSignalBusiness();
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);

        //Lấy list new home screen
        public List<NewsAppOutputModel> GetNewsHomeScreen(int type)
        {
            List<NewsAppOutputModel> data = new List<NewsAppOutputModel>();
            data = cnn.News.Where(n => n.IsActive.Equals(SystemParam.ACTIVE) && n.Type.Equals(type))
                .Select(n => new NewsAppOutputModel
                {
                    NewsID = n.ID,
                    Content = n.Content,
                    CreateDate = n.CreateDate,
                    Title = n.Title,
                    Type = n.Type,
                    UrlImage = urlHost + n.UrlImage,
                    Description = n.Description,
                    Link = n.Link
                }).OrderByDescending(n => n.CreateDate).ToList();
            return data;
        }

        //Lấy danh sách tin tức
        public IPagedList<NewsAppOutputModel> GetListNews(int page, int limit, int? type, int? status)
        {
            try
            {
                var data = cnn.News.Where(n => n.IsActive.Equals(SystemParam.ACTIVE) && (type.HasValue && type.Value > 0 ? n.Type.Equals(type.Value) : true)
                                     && (status.HasValue ? n.Status.Equals(status.Value) : true))
                .Select(n => new NewsAppOutputModel()
                {
                    NewsID = n.ID,
                    Content = n.Content,
                    CreateDate = n.CreateDate,
                    Title = n.Title,
                    Type = n.Type,
                    UrlImage = urlHost + n.UrlImage,
                    Description = n.Description,
                    Link = n.Link
                }).OrderByDescending(n => n.NewsID).ToPagedList(page, limit);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<NewsAppOutputModel>().ToPagedList(1, 1);
            }
        }

        //Lấy chi tiết tin tức
        public JsonResultModel GetNewsDetail(int id)
        {
            try
            {
                NewsAppOutputModel data = new NewsAppOutputModel();
                News n = cnn.News.Find(id);
                data.NewsID = n.ID;
                data.Content = n.Content;
                data.CreateDate = n.CreateDate;
                data.Title = n.Title;
                data.UrlImage = urlHost + n.UrlImage;
                data.Type = n.Type;
                data.Description = n.Description;
                data.Link = n.Link;
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        //public NewsAppOutputModel GetNews(int newID)
        //{
        //    List<NewsAppOutputModel> query = new List<NewsAppOutputModel>();
        //    var ListNews = from n in cnn.News
        //                   where n.IsActive.Equals(SystemParam.ACTIVE) && n.Status.Equals(SystemParam.ACTIVE) && n.ID.Equals(newID)
        //                   orderby n.CreateDate descending
        //                   select new NewsAppOutputModel
        //                   {
        //                       NewsID = n.ID,
        //                       Content = n.Content,
        //                       CreateDate = n.CreateDate,
        //                       Title = n.Title,
        //                       Type = n.Type,
        //                       UrlImage = n.UrlImage.ToLower().Contains("http") ? n.UrlImage : "http://" + HttpContext.Current.Request.Url.Authority + "/" + n.UrlImage,
        //                       Description = n.Description,
        //                       CategoryID = n.CategoryID,
        //                       IsShowCreate=n.IsShowCreate.HasValue?n.IsShowCreate.Value:1
        //                   };
        //    return ListNews.FirstOrDefault();
        //}

        // tạo lô bài viết tin tức


        public List<Promotion> CreatePromotion(int Displayorder)
        {
            List<Promotion> query = new List<Promotion>();
            Promotion pro = new Promotion();
            pro.IsActive = SystemParam.ACTIVE;
            pro.CreateDate = DateTime.Now;
            pro.DisplayOrder = Displayorder;
            query.Add(pro);
            return query;
        }

        //public CategoryNew GetCategoryByName(int Type)
        //{
        //    try
        //    {
        //        string categoryName = "";
        //        switch (Type)
        //        {
        //            case SystemParam.TYPE_ADS:
        //                categoryName = SystemParam.TYPE_ADS_STRING;
        //                break;
        //            case SystemParam.TYPE_EVENT:
        //                categoryName = SystemParam.TYPE_EVENT_STRING;
        //                break;
        //            case SystemParam.TYPE_NEWS:
        //                categoryName = SystemParam.TYPE_NEWS_STRING;
        //                break;
        //            case SystemParam.TYPE_PRODUCT:
        //                categoryName = SystemParam.TYPE_PRODUCT_STRING;
        //                break;
        //            case SystemParam.TYPE_PROMOTION:
        //                categoryName = SystemParam.TYPE_PROMOTION_STRING;
        //                break;
        //            case SystemParam.TYPE_PRICE_QUOTE:
        //                categoryName = SystemParam.TYPE_PRICE_QUOTE_STRING;
        //                break;
        //            default:
        //                break;
        //        }
        //        var categoryNews = cnn.CategoryNews.Where(u => u.NameEN.Equals(categoryName)).FirstOrDefault();
        //        return categoryNews;
        //    }
        //    catch
        //    {
        //        return new CategoryNew();
        //    }
        //}

        //chỉnh sửa bài viết
        //public int UpdateNewsDekko(ListNewsWebOutputModel item)
        //{
        //    try
        //    {
        //        var cate = cnn.CategoryNews.Where(u => u.NameEN.Equals("Addvertiment")).FirstOrDefault();
        //        if (cate != null && item.CategoryID.Equals(cate.ID))
        //        {
        //            var ListNews = cnn.News.Where(u => u.CategoryID.Equals(cate.ID)).ToList();
        //            foreach (var ne in ListNews)
        //            {
        //                ne.CategoryID = 1;
        //            }
        //        }
        //        News news = cnn.News.Find(item.ID);
        //        news.CategoryID = item.CategoryID;
        //        news.Title = item.Title;
        //        news.Name = item.Title;
        //        news.UserID = item.UserID;
        //        news.Description = item.Description;
        //        news.Content = item.Content;
        //        news.Type = item.Type;
        //        news.Status = item.Status;
        //        news.IsShowCreate = item.ShowCreate;
        //        news.TypeSend = item.TypeSend;
        //        news.UrlImage = item.UrlImage;
        //        news.OrderDisplay = item.OrderDisplay;
        //        if (item.CategoryID == 2)
        //        {
        //            var data = cnn.News.Where(x => x.IsActive == SystemParam.ACTIVE && x.CategoryID == 2).ToList();
        //            data.ForEach(x => x.CategoryID = 1);
        //            cnn.SaveChanges();
        //        }
        //        if (item.IsBanner == 0)
        //        {
        //            Promotion pro = cnn.Promotions.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.NewsID.Equals(item.ID)).FirstOrDefault();
        //            if (pro != null)
        //            {
        //                pro.IsActive = SystemParam.INACTIVE;
        //            }
        //            news.Link = null;
        //        }
        //        else
        //        {
        //            news.Link = item.Link;
        //            Promotion pro = cnn.Promotions.Where(u => u.NewsID.Equals(item.ID)).FirstOrDefault();
        //            if (pro != null)
        //            {
        //                pro.IsActive = SystemParam.ACTIVE;
        //                pro.DisplayOrder = item.OrderDisplay;
        //            }
        //            else
        //            {
        //                Promotion pros = new Promotion();
        //                pros.IsActive = SystemParam.ACTIVE;
        //                pros.CreateDate = DateTime.Now;
        //                pros.DisplayOrder = item.OrderDisplay;
        //                pros.NewsID = item.ID;
        //                cnn.Promotions.Add(pros);
        //            }
        //        }
        //        cnn.SaveChanges();
        //        return SystemParam.SUCCESS;
        //    }
        //    catch
        //    {
        //        return SystemParam.ERROR;
        //    }
        //}

        //xóa bài viết
        public int DeleteNews(int ID)
        {
            try
            {
                News news = cnn.News.Find(ID);
                news.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        //xóa bài viết
        public int ChangeStatus(int ID)
        {
            try
            {
                News news = cnn.News.Find(ID);
                if (news.Status == SystemParam.ACTIVE)
                    news.Status = SystemParam.INACTIVE;
                else
                    news.Status = SystemParam.ACTIVE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }


        //danh sách các sản phầm con cho bài viết vê  danh mục sản phẩm
        //public List<CategoryModel> getItemNewsCate(string lang)
        //{
        //    try
        //    {
        //        List<CategoryModel> listCategory = new List<CategoryModel>();
        //        var query = from c in cnn.CategoryNews
        //                    where c.IsActive == SystemParam.ACTIVE
        //                    select new CategoryModel
        //                    {
        //                        CategoryID = c.ID,
        //                        Name = lang.Equals(SystemParam.VN) ? c.NameVN : c.NameEN,
        //                    };

        //        if (query != null && query.Count() > 0)
        //        {
        //            listCategory = query.ToList();
        //            return listCategory;
        //        }
        //        else
        //            return listCategory;
        //    }
        //    catch (Exception)
        //    {
        //        return new List<CategoryModel>();
        //    }

        //}


        //public int GetOrderDisplay()
        //{
        //    return cnn.News.Count() + 1;
        //}

        //lấy danh sách tácc giả
        public List<ListUserWebOutputModel> GetListAuthor()
        {
            try
            {
                var ListUser = from user in cnn.Users
                               where user.IsActive == SystemParam.ACTIVE
                               select new ListUserWebOutputModel()
                               {
                                   ID = user.ID,
                                   Name = user.UserName,
                                   CreateDate = user.CreatedDate
                               };
                if (ListUser != null && ListUser.Count() > 0)
                {
                    ListUser = ListUser.Distinct();
                    return ListUser.ToList();

                }
                else
                {
                    return new List<ListUserWebOutputModel>();
                }

            }
            catch
            {
                return new List<ListUserWebOutputModel>();
            }
        }

        //lấy thông tin chi tiết 1 bài viết
        //public ListNewsWebOutputModel GetNewsDetail(int ID)
        //{
        //    try
        //    {
        //        ListNewsWebOutputModel newsDetail = new ListNewsWebOutputModel();

        //        var query = (from n in cnn.News
        //                     where n.IsActive.Equals(SystemParam.ACTIVE) && n.ID.Equals(ID)
        //                     select new ListNewsWebOutputModel
        //                     {
        //                         ID = n.ID,
        //                         CategoryName = n.CategoryNew.NameEN,
        //                         Title = n.Title,
        //                         Status = n.Status,
        //                         Type = n.Type,
        //                         CreateDate = n.CreateDate,
        //                         IsActive = n.IsActive,
        //                         OrderDisplay = n.OrderDisplay.Value,
        //                         CreateUser = n.User.UserName,
        //                         CategoryID = n.CategoryID,
        //                         Content = n.Content,
        //                         Description = n.Description,
        //                         TypeSend = n.TypeSend.Value,
        //                         UrlImage = n.UrlImage,
        //                         IsBanner = (n.Promotions.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).ToList().Count > 0 ? 1 : 0),
        //                         ShowCreate = n.IsShowCreate.HasValue ? n.IsShowCreate.Value : 1,
        //                         Link=n.Link

        //                     }).FirstOrDefault();
        //        if (query != null && query.ID > 0)
        //        {
        //            return newsDetail = query;
        //        }
        //        return newsDetail;
        //    }
        //    catch
        //    {
        //        return new ListNewsWebOutputModel();
        //    }
        //}

        //thêm mới
        public JsonResultModel CreateNew(ListNewsWebOutputModel creatNew, int userID)
        {
            try
            {
                News newWeb = new News();
                newWeb.UserID = userID;
                newWeb.Name = creatNew.Name;
                //newWeb.CategoryID = creatNew.CategoryID;
                newWeb.Type = creatNew.Type;
                newWeb.Title = creatNew.Title;
                newWeb.Description = creatNew.Description;
                newWeb.Content = creatNew.Content;
                newWeb.UrlImage = creatNew.UrlImage;
                newWeb.OrderDisplay = creatNew.OrderDisplay;
                newWeb.Status = creatNew.Status;
                newWeb.Link = "";
                newWeb.CreateDate = DateTime.Now;
                newWeb.IsActive = creatNew.IsActive;
                newWeb.Type = creatNew.Type;
                newWeb.TypeSend = creatNew.TypeSend;
                newWeb.IsShowCreate = true;
                newWeb.isAdvertusement = 1;
                
                cnn.News.Add(newWeb);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, newWeb);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }
        //thêm nháp
        public JsonResultModel DraffNew(ListNewsWebOutputModel creatNew, int userID)
        {
            try
            {
                News newWeb = new News();
                newWeb.UserID = userID;
                newWeb.Title = creatNew.Title;
                newWeb.Type = creatNew.Type;
                newWeb.Description = creatNew.Description;
                newWeb.Status = creatNew.Status;
                newWeb.UrlImage = creatNew.UrlImage;
                newWeb.OrderDisplay = creatNew.OrderDisplay;
                newWeb.Content = creatNew.Content;
                newWeb.Link = "";
                newWeb.Name = creatNew.Name;
                //newWeb.CategoryID = creatNew.CategoryID;
                newWeb.CreateDate = DateTime.Now;
                newWeb.IsActive = creatNew.IsActive;
                cnn.News.Add(newWeb);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, newWeb);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }
        //binding dữ liệu tin tức
        public ListNewsWebOutputModel ViewNewsDetail(int id)
        {
            try
            {
                ListNewsWebOutputModel data = new ListNewsWebOutputModel();
                News n = cnn.News.Find(id);
                data.ID = n.ID;
                data.Content = n.Content;
                data.CreateDate = n.CreateDate;
                data.Title = n.Title;
                data.UrlImage = n.UrlImage;
                data.Type = n.Type;
                data.Description = n.Description;
                data.Link = n.Link;
                data.Status = n.Status;
                data.OrderDisplay = n.OrderDisplay;
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new ListNewsWebOutputModel();
            }
        }

        public ListNewsWebOutputModel DisplayNewsDetail(int id)
        {
            try
            {
                ListNewsWebOutputModel data = new ListNewsWebOutputModel();
                News n = cnn.News.Find(id);
                data.ID = n.ID;
                data.Content = n.Content;
                data.CreateDate = n.CreateDate;
                data.Title = n.Title;
                data.UrlImage = n.UrlImage;
                data.Type = n.Type;
                data.Description = n.Description;
                data.Link = n.Link;
                data.Status = n.Status;
                data.OrderDisplay = n.OrderDisplay;
                data.ListNewsRelate = cnn.News.Where(news => news.IsActive.Equals(SystemParam.ACTIVE) && (n.Status == 1) && news.Type.Equals(n.Type))
                    .Select(news => new ListNewsRelate
                    {
                        ID = news.ID,
                        Title = news.Title,
                        Description = news.Description,
                        Image = urlHost + news.UrlImage
                    }).ToList();
                data.ListAdvertisement = cnn.News.Where(adver => adver.IsActive.Equals(SystemParam.ACTIVE) && (n.Status == 1) && adver.Type == SystemParam.NEWS_TYPE_ADVERTISEMENT)
                    .Select(adver => new ListAdvertisement
                    {
                        ID = adver.ID,
                        Title = adver.Title,
                        Description = adver.Description,
                        Image = urlHost + adver.UrlImage
                    }).OrderByDescending(nw => nw.ID).Take(2).ToList();
                return data;

            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new ListNewsWebOutputModel();
            }
        }

        //lưu khi sửa
        public bool SaveEditNew(ListNewsWebOutputModel req)
        {
            News x = cnn.News.Find(req.ID);

            if (x != null)
            {
                //x.Name = req.Name;
                //x.UrlImage = req.UrlImage;
                //x.Content = string.IsNullOrEmpty(req.Content) ? "" : req.Content;
                //x.Description = string.IsNullOrEmpty(req.Description) ? "" : req.Description;
                //x.Link = string.IsNullOrEmpty(req.Link) ? "" : req.Link;
                //x.Status = req.Status;
                //x.OrderDisplay = req.OrderDisplay;

                x.Title = req.Title;
                x.Description = req.Description;
                x.Status = req.Status;
                x.UrlImage = req.UrlImage;
                x.OrderDisplay = req.OrderDisplay;
                x.Content = req.Content;
                x.Link = string.IsNullOrEmpty(req.Link) ? "" : req.Link;
                x.Name = req.Name;
                x.Type = req.Type;
                //x.CategoryID = 1;
                //x.CreateDate = DateTime.Now;
            }

            cnn.SaveChanges();
            return true;
        }
        //Tìm kiếm
        public IPagedList<ListNewsWebOutputModel> SearchView(int Page, string title, int? Status)
        {
            try
            {
                var data = cnn.News.Where(n => n.IsActive != 0
                && (!String.IsNullOrEmpty(title) ? n.Title.Contains(title) : true)
                && (Status != null ? Status == n.Status : true)
                ).Select(New => new ListNewsWebOutputModel
                {
                    ID = New.ID,
                    Name = New.Name,
                    Title = New.Title,
                    Description = New.Description,
                    Content = New.Content,
                    UrlImage = New.UrlImage,
                    OrderDisplay = New.OrderDisplay,
                    Status = New.Status,
                    Type = New.Type,
                    TypeSend = New.TypeSend,
                    IsActive = New.IsActive,
                    CreateDate = New.CreateDate,
                    UserID = New.UserID,
                    CreateUser = New.User.UserName
                //}).OrderByDescending(New => New.OrderDisplay).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
                }).OrderBy(New => New.OrderDisplay).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<ListNewsWebOutputModel>().ToPagedList(1, 1);
            }
        }
        //Xóa
        public JsonResultModel DeleteNew(int id)
        {
            try
            {
                News news = cnn.News.Find(id);
                news.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, news);
            }
            catch (Exception ex)
            {
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }

        public List<ListNewsWebOutputModel> Display_Promotion()
        {
            try
            {
                var data = cnn.News.Where(n => n.IsActive != 0 && (n.Status == 1)
                && (n.Type == SystemParam.NEWS_TYPE_PROMOTION)
                ).Select(promotion => new ListNewsWebOutputModel
                {
                    ID = promotion.ID,
                    Name = promotion.Name,
                    Title = promotion.Title,
                    Description = promotion.Description,
                    Content = promotion.Content,
                    UrlImage = urlHost + promotion.UrlImage,
                    CreateDate = promotion.CreateDate
                }).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ListNewsWebOutputModel>();
            }
        }

        public List<ListNewsWebOutputModel> Display_New()
        {
            try
            {
                var data = cnn.News.Where(n => n.IsActive != 0 && (n.Status == 1)
                && (n.Type == SystemParam.NEWS_TYPE_NEWS)
                ).Select(promotion => new ListNewsWebOutputModel
                {
                    ID = promotion.ID,
                    Name = promotion.Name,
                    Title = promotion.Title,
                    Description = promotion.Description,
                    Content = promotion.Content,
                    UrlImage = urlHost + promotion.UrlImage,
                    CreateDate = promotion.CreateDate
                }).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ListNewsWebOutputModel>();
            }
        }

        public List<ListNewsWebOutputModel> Display_Advertisement()
        {
            try
            {
                var data = cnn.News.Where(n => n.IsActive != 0 && (n.Status == 1)
                && (n.Type == SystemParam.NEWS_TYPE_ADVERTISEMENT)
                ).Select(ad => new ListNewsWebOutputModel
                {
                    ID = ad.ID,
                    Name = ad.Name,
                    Title = ad.Title,
                    Description = ad.Description,
                    Content = ad.Content,
                    UrlImage = urlHost + ad.UrlImage,
                    CreateDate = ad.CreateDate
                }).OrderByDescending(adv => adv.ID).Take(2).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ListNewsWebOutputModel>();
            }
        }

        public List<ListNewsWebOutputModel> Display_BannerFood()
        {
            try
            {
                var data = cnn.News.Where(n => n.IsActive != 0 && (n.Status == 1)
                && (n.Type == SystemParam.NEWS_TYPE_BANER_FOOD)
                ).Select(bf => new ListNewsWebOutputModel
                {
                    ID = bf.ID,
                    Name = bf.Name,
                    Title = bf.Title,
                    Description = bf.Description,
                    Content = bf.Content,
                    UrlImage = urlHost + bf.UrlImage,
                    CreateDate = bf.CreateDate
                }).OrderByDescending(b => b.ID).Take(1).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ListNewsWebOutputModel>();
            }
        }



    }
}
