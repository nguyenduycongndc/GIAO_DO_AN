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
    public class BankBusiness : GenericBusiness
    {
        public BankBusiness(WE_SHIPEntities context = null) : base()
        {
        }
        CustomerBusiness cusBus = new CustomerBusiness();
        public List<BankOutputModel> GetListBank(string lang)
        {
            List<BankOutputModel> listBrank = cnn.Banks.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new BankOutputModel
            {
                BankID = u.ID,
                BankName = lang==SystemParam.VN?u.Name:u.NameEn,
                Code = u.Code
            }).ToList();
            return listBrank;
        }
        public UserInforOutputModel createBank(TokenOutputModel checkToken, BankDetailModel item, string lang) {
            BankMember bank = new BankMember();
            bank.Acount = item.Acount;
            bank.BankID = item.BankID;
            bank.AcountOwner = item.AcountOwner;
            bank.IsActive = SystemParam.ACTIVE;
            bank.CraeteDate = DateTime.Now;
            bank.MemberID = checkToken.MemberID;
            cnn.BankMembers.Add(bank);
            cnn.SaveChanges();
            return cusBus.GetuserInfor(checkToken.CustomerID, checkToken.AgentID,lang);
        }
        public UserInforOutputModel updateBank(TokenOutputModel checkToken, BankDetailModel item ,string lang)
        {
            BankMember bank = cnn.BankMembers.Find(item.ID);
            bank.Acount = item.Acount;
            bank.BankID = item.BankID;
            bank.AcountOwner = item.AcountOwner;
            cnn.SaveChanges();
            return cusBus.GetuserInfor(checkToken.CustomerID, checkToken.AgentID, lang);
        }
        public UserInforOutputModel Deletebank(TokenOutputModel checkToken, int ID, string lang) {
            BankMember bank = cnn.BankMembers.Find(ID);
            bank.IsActive = SystemParam.INACTIVE;
            cnn.SaveChanges();
            return cusBus.GetuserInfor(checkToken.CustomerID, checkToken.AgentID, lang);
        }
    }
}
