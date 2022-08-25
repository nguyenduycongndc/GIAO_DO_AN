using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class ReviewBusiness : GenericBusiness
    {
        public ReviewBusiness(WE_SHIPEntities context = null) : base()
        {
        }
        NotifyBusiness notiBus = new NotifyBusiness();
        int[] statusOrder = { Constant.ORDER_STATUS_CONFIRM, Constant.ORDER_STATUS_WASHING, Constant.ORDER_STATUS_CONFIRM_WASHING, Constant.ORDER_STATUS_COMPLETE };
        public string CreateReview(ReviewInputModel item, string lang)
        {
            OrderService order = cnn.OrderServices.Find(item.orderServiceID);
            if (order == null)
                return lang.Equals(SystemParam.VN) ? MessVN.NOT_FOUND_MESS : MessEN.NOT_FOUND_MESS;
            if (order.Status != Constant.ORDER_STATUS_COMPLETE)
                return lang.Equals(SystemParam.VN) ? MessVN.REVIEW_ERROR : MessEN.REVIEW_ERROR;
            order.IsRate = 1;
            order.Rate = Convert.ToInt32(item.point);
            order.NoteRate = item.Note;
            cnn.SaveChanges();
            Agent washer = cnn.Agents.Find(order.AgentID.Value);
            List<int> listRate = cnn.OrderServices.Where(u => u.AgentID.Value.Equals(washer.ID) && u.IsRate.Value.Equals(1)).Select(u => u.Rate.Value).ToList();
            double rating = (double)listRate.Sum() / listRate.Count();
            washer.Rating = Math.Round(rating, 1);
            cnn.SaveChanges();
            notiBus.CreateNoti(washer.Members.FirstOrDefault().ID, Constant.NOTI_REVIEW, item.orderServiceID, null, null, lang, item.point.ToString(), true);
            return SystemParam.SUCCES_STR;
        }
        public SystemResult Search(string searchKey = "", string fromDate = "", string toDate = "", double? searchRating = null)
        {
            DateTime? fDate = Util.ConvertDate(fromDate);
            DateTime? tDate = Util.ConvertDate(toDate);
            if (tDate.HasValue)
                tDate = tDate.Value.AddDays(1);
            var listOrderService = cnn.OrderServices.Where(u => 
            u.IsActive.Equals(SystemParam.ACTIVE) 
            && u.AgentID.HasValue 
            && statusOrder.ToList().Contains(u.Status) 
            && (fDate.HasValue ? u.BookingDate.Value >= fDate.Value : true) 
            && (tDate.HasValue ? u.BookingDate.Value <= tDate.Value : true)).ToList();
            var listAgent = cnn.Members.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.AgentID.HasValue && (u.Agent.Name.Contains(searchKey) || u.Agent.Code.Contains(searchKey)) && (searchRating.HasValue ? u.Agent.Rating.Value >= searchRating.Value : true)).ToList();
            List<ReviewModel> query = listAgent.Select(u => new ReviewModel
            {
                ID = u.AgentID.Value,
                NumberOfComment = listOrderService.Where(o => o.AgentID.Value.Equals(u.AgentID.Value) && !String.IsNullOrEmpty(o.NoteRate) && o.NoteRate.Length > 0).ToList().Count,
                NumberOfCompleteOrder = listOrderService.Where(o => o.AgentID.Value.Equals(u.AgentID.Value) && o.Status.Equals(Constant.ORDER_STATUS_COMPLETE)).ToList().Count,
                NumberOfRating = listOrderService.Where(o => o.IsRate.HasValue && o.IsRate.Value.Equals(1) && o.AgentID.Value.Equals(u.AgentID.Value)).Count(),
                WasherName = u.Agent.Name,
                WasherCode = u.Agent.Code,
                Rating = u.Agent.Rating.HasValue ? u.Agent.Rating.Value : 0,
                RatingOfAdmin = u.Agent.RatingAdmin,
                DateTime = listOrderService.Where(o => o.AgentID.Value.Equals(u.AgentID.Value) && !String.IsNullOrEmpty(o.NoteRate) && o.NoteRate.Length > 0).Select(o => o.CompletedDate).FirstOrDefault()
            }).ToList();
            return resultBus.SucessResult(query);
        }

        public SystemResult Detail(int ID)
        {
            ReviewDetail query = new ReviewDetail();
            var listOrderService = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.IsRate.Value.Equals(1) && u.AgentID.HasValue && statusOrder.ToList().Contains(u.Status)).ToList();
            var listAgent = cnn.Agents.Where(u => u.ID.Equals(ID)).ToList();
            query.review = listAgent.Select(u => new ReviewModel
            {
                ID = u.ID,
                NumberOfComment = listOrderService.Where(o => o.AgentID.Value.Equals(u.ID) && !String.IsNullOrEmpty(o.NoteRate) && o.NoteRate.Length > 0).ToList().Count,
                NumberOfCompleteOrder = listOrderService.Where(o => o.AgentID.Value.Equals(u.ID) && o.Status.Equals(Constant.ORDER_STATUS_COMPLETE)).ToList().Count,
                NumberOfRating = listOrderService.Where(o => o.AgentID.Value.Equals(u.ID)).Count(),
                WasherName = u.Name,
                WasherCode = u.Code,
                Rating = u.Rating.HasValue ? u.Rating.Value : 0,
                RatingOfAdmin = u.RatingAdmin
            }).FirstOrDefault();
            query.ListReviewInformation = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.IsRate.Value.Equals(1) && u.AgentID.Value.Equals(ID)).OrderByDescending(u => u.ID).Select(u => new ReviewInformation
            {
                DateTime = u.CompletedDate.Value,
                Note = u.NoteRate,
                Rating = u.Rate.Value,
                Phone = u.Customer.Phone,
                CustomerName = u.Customer.Name,
                washerNote = u.WasherNote,
                listImage = u.OrderServiceImageCars.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && i.Type.Equals(Constant.TYPE_ORDER_SERVICE_IMAGE_CAR)).Select(i => new ImageCarAngParkService { image = i.Image, ID = i.ID }).ToList()
            }).ToList();
            return resultBus.SucessResult(query);
        }

        public SystemResult DetailReviewCustomer(int ID)
        {
            List<ReviewInformation> query = new List<ReviewInformation>();
            query = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.IsRate.Value.Equals(1) && u.CustomerID.Equals(ID) && statusOrder.ToList().Contains(u.Status)).OrderByDescending(u =>u.ID).Select(u => new ReviewInformation
            {
                DateTime = u.CompletedDate.Value,
                Note = u.NoteRate,
                Rating = u.Rate.Value,
                Phone = u.Agent.Phone,
                CustomerName = u.Customer.Name,
                washerNote = u.WasherNote,
                listImage = u.OrderServiceImageCars.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && i.Type.Equals(Constant.TYPE_ORDER_SERVICE_IMAGE_CAR)).Select(i => new ImageCarAngParkService { image = i.Image, ID = i.ID }).ToList()
            }).ToList();
            return resultBus.SucessResult(query);
        }
        public SystemResult Update(int ID, string ratingOfAdmin) {
            Agent agent = cnn.Agents.Find(ID);
            if(agent == null)
                return resultBus.ErrorResult(MessEN.NOT_FOUND_MESS);
            agent.RatingAdmin = ratingOfAdmin;
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }

        //Export file excel list review
        public ExcelPackage ExportListReviews(string searchKey = "", string FromDate = "", string toDate = "", double rateNumber = 0)
        {
            try
            {
                List<ReviewModel> data = (List<ReviewModel>)Search(searchKey, FromDate, toDate, rateNumber).Result;
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/listReview.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 3;
                int no = 1;

                if (data != null && data.Count() > 0)
                {
                    foreach (var dt in data)
                    {
                        sheet.Cells[row, 1].Value = no;
                        sheet.Cells[row, 2].Value = dt.WasherCode;
                        sheet.Cells[row, 3].Value = dt.WasherName;
                        sheet.Cells[row, 4].Value = dt.NumberOfRating;
                        sheet.Cells[row, 5].Value = dt.Rating;
                        sheet.Cells[row, 6].Value = dt.RatingOfAdmin;
                        sheet.Cells[row, 6].Value = dt.NumberOfComment;
                        row++;
                        no++;
                    }
                }
                return pack;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ExcelPackage();
            }
        }
    }
}
