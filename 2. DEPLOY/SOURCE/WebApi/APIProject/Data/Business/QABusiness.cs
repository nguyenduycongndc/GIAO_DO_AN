using Data.DB;
using Data.Model;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class QABusiness : GenericBusiness
    {
        public QABusiness(WE_SHIPEntities context = null) : base()
        {

        }

        public List<QAModel> getQA(string lang)
        {
            int type = lang.Equals(SystemParam.VN) ? 1 : 2;
            List<QAModel> query = cnn.QAs.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Value.Equals(type)).OrderBy(u =>u.OrderDisplay.Value).Select(u => new QAModel
            {
                ID = u.ID,
                Answer = u.Answer,
                Question = u.Question
            }).ToList();
            return query;
        }
        public SystemResult search(string search = "")
        {
            List<QAModels> query = cnn.QAs.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Question.Contains(search)).Select(u => new QAModels
            {
                ID = u.ID,
                Question = u.Question,
                Answer = u.Answer,
                OrderDisplay = u.OrderDisplay.Value,
                CreateDate = u.CreateDate,
                Type = u.Type.HasValue ? u.Type.Value : 1
            }).OrderByDescending(u => u.CreateDate).ToList();
            return resultBus.SucessResult(query);
        }
        public SystemResult Detail(int ID)
        {
            QAModels query = cnn.QAs.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ID.Equals(ID)).Select(u => new QAModels
            {
                ID = u.ID,
                Question = u.Question,
                Answer = u.Answer,
                OrderDisplay = u.OrderDisplay.Value,
                CreateDate = u.CreateDate,
                Type = u.Type.Value
            }).FirstOrDefault();
            return resultBus.SucessResult(true);
        }
        public SystemResult CreateQA(QAModels item) {
            QA qa = new QA();
            qa.Question = item.Question;
            qa.Answer = item.Answer;
            qa.OrderDisplay = item.OrderDisplay;
            qa.IsActive = SystemParam.ACTIVE;
            qa.CreateDate = DateTime.Now;
            qa.Type = item.Type;
            cnn.QAs.Add(qa);
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult UpdateQA(QAModels item)
        {
            QA qa = cnn.QAs.Find(item.ID);
            if(qa == null)
                return resultBus.ErrorResult(MessEN.NOT_FOUND_MESS);
            qa.Question = item.Question;
            qa.Answer = item.Answer;
            qa.OrderDisplay = item.OrderDisplay;
            qa.Type = item.Type;
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult DeleteQA(int ID)
        {
            QA qa = cnn.QAs.Find(ID);
            if (qa == null)
                return resultBus.ErrorResult(MessEN.NOT_FOUND_MESS);
            qa.IsActive = SystemParam.INACTIVE;
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
    }
}
