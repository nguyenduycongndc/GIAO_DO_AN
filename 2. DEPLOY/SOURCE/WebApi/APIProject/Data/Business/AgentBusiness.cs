using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using Newtonsoft.Json;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;


namespace Data.Business
{
    public class AgentBusiness : GenericBusiness
    {
        PlaceBusiness placeBus = new PlaceBusiness();
        AreaBusiness areaBus = new AreaBusiness();
        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        public AgentBusiness(WE_SHIPEntities context = null) : base()
        {
            if (context != null)
            {
                this.cnn = context;
            }
        }

        public void SendOTP(string phone, string otpCode)
        {
            try
            {
                OTPInputModel otp = new OTPInputModel();
                otp.from = OTPRelease.ALIAS;
                otp.To = phone;
                otp.type = 1;
                otp.scheduled = "";
                otp.message = otpCode;
                otp.requestId = "";
                otp.useUnicode = 0;
                otp.maxMt = 0;
                string value = JsonConvert.SerializeObject(otp);
                var a = apiBus.OTPRequest(value);
            }
            catch { }
        }
        // Tìm Kiếm Đại Lý
        public List<CreateAgentInputModel> Search(string CodeOrName, string Phone, string Email, int? Status, string FromDate, string ToDate)
        {
            try
            {
                List<CreateAgentInputModel> list = new List<CreateAgentInputModel>();
                var query = (from a in cnn.Members
                             join ag in cnn.Agents on a.AgentID equals (ag.ID)
                             where a.IsActive != SystemParam.INACTIVE && a.AgentID.HasValue
                             select new CreateAgentInputModel()
                             {
                                 ID = a.AgentID.Value,
                                 Code = a.Agent.Code,
                                 Name = a.Agent.Name,
                                 Phone = a.User,
                                 Email = a.Agent.Email,
                                 CreatedDate = a.CreateDate,
                                 AcceptService = a.Agent.AcceptService.Value,
                                 Status = a.IsActive,
                                 Rating = a.Agent.Rating,
                                 MemberId = a.ID,
                                 CommisstitonName = a.Agent.CommissionID.HasValue ? a.Agent.ConfigCommission.CarAutocareAcaDemy : ""
                             }).ToList();

                if (Phone != null && Phone != "")
                {

                    query = query.Where(x => x.Phone.Contains(Phone)).ToList();
                }
                if (CodeOrName != null && CodeOrName != "")
                {
                    query = query.Where(x => Util.Converts(x.Code.ToLower()).Contains(Util.Converts(CodeOrName.ToLower())) || Util.Converts(x.Name.ToLower()).Contains(Util.Converts(CodeOrName.ToLower()))).ToList();
                }
                if (Email != null && Email != "")
                {
                    query = query.Where(x => x.Email.Contains(Email)).ToList();
                }
                if (Status != null)
                {
                    query = query.Where(x => x.Status.Equals(Status.Value)).ToList();
                }
                if (FromDate != "" && FromDate != null)
                {
                    DateTime? fd = Util.ConvertDate(FromDate);
                    query = query.Where(x => x.CreatedDate >= fd).ToList();
                }

                if (ToDate != "" && ToDate != null)
                {
                    DateTime? td = Util.ConvertDate(ToDate);
                    td = td.Value.AddDays(1);
                    query = query.Where(x => x.CreatedDate <= td).ToList();
                }

                if (query != null && query.Count() > 0)
                    return query.OrderByDescending(x => x.ID).ToList();
                else
                    return new List<CreateAgentInputModel>();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<CreateAgentInputModel>();
            }
        }
        public List<int> ListAgentId()
        {
            List<int> list = cnn.Members.Where(x => x.IsActive != SystemParam.INACTIVE && x.AgentID.HasValue).Select(x => x.AgentID.Value).ToList();
            return list;
        }
        public List<CreateAgentInputModel> GetListAgentCheckAddMoney(List<string> listphone)
        {
            if (listphone.Count > 0)
            {
                var query = cnn.Members.Where(x => x.IsActive != SystemParam.INACTIVE && x.AgentID != null && listphone.Any(l => x.CustomerID.HasValue ? x.Customer.Phone == l : x.Agent.Phone == l)).Select(x => new CreateAgentInputModel()
                {
                    ID = x.ID,
                    Name = x.Agent.Name + " - " + x.Agent.Phone,
                    MemberId = x.ID,
                    Lang = x.Lang == null ? "vi" : x.Lang,
                    Status = x.IsActive
                }).ToList();
                return query;
            }
            else
            {
                return new List<CreateAgentInputModel>();
            }
        }
        public List<Customer> LoadAgent()
        {
            List<Customer> listAgent = new List<Customer>();
            var query = from c in cnn.Customers
                        where c.IsActive.Equals(SystemParam.ACTIVE)
                        orderby c.Name ascending
                        select c;
            if (query != null && query.Count() > 0)
            {
                listAgent = query.ToList();
                return listAgent;
            }
            else
            {
                return new List<Customer>();
            }
        }

        public List<Customer> LoadCustomer()
        {
            List<Customer> listCustomer = new List<Customer>();
            var query = from c in cnn.Customers
                        where c.IsActive.Equals(SystemParam.ACTIVE)
                        orderby c.Name ascending
                        select c;
            if (query != null && query.Count() > 0)
            {
                listCustomer = query.ToList();
                return listCustomer;
            }
            else
            {
                return new List<Customer>();
            }
        }

        public Boolean CheckExistingAgent(string agentPhone, string agentEmail)
        {
            try
            {
                var agent = cnn.Customers.Where(u => u.Phone.Equals(agentPhone) && u.IsActive.Equals(SystemParam.ACTIVE) || u.Email.Equals(agentEmail) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                if (agent != null && agent.Count() > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //public Boolean CheckExistingAgent(string agentCode)
        //{
        //    try
        //    {
        //        var agent = cnn.Agents.Where(u => u.Code.Equals(agentCode) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
        //        if (agent != null && agent.Count() > 0)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        public string CreateCodeWasher()
        {
            Agent agent = cnn.Agents.OrderByDescending(u => u.ID).FirstOrDefault();
            string id = "01";
            if (agent != null)
                id = (agent.ID + 1).ToString();
            if (id.Length < Constant.MAX_LENGT_ID)
            {
                for (int lengt = id.Length; lengt < Constant.MAX_LENGT_ID; lengt++)
                {
                    id = "0" + id;
                }
            }
            string code = Constant.KEY_CODE_WASHER + DateTime.Now.Year.ToString() + DateTime.Now.ToString("MM") + "-" + id;
            return code;
        }
        public int CreateAgent(CreateAgentInputModel input)
        {
            Member member = cnn.Members.Where(u => u.User == input.Phone && u.IsActive == SystemParam.ACTIVE && (u.AgentID.HasValue ? u.Agent.IsActive == SystemParam.ACTIVE : true) && (u.CustomerID.HasValue ? u.Customer.IsActive == SystemParam.ACTIVE : true)).FirstOrDefault();
            int MaxArea = int.Parse(cnn.Configs.Where(u => u.NameConstant.Equals(Constant.MaxArea)).Select(u => u.ValueConstant).FirstOrDefault());
            if (member != null)
                return SystemParam.RETURN_FALSE;
            Agent agent = new Agent();
            agent.Name = input.Name;
            agent.Phone = input.Phone;
            agent.Email = input.Email;
            agent.CreateDate = DateTime.Now;
            agent.Code = CreateCodeWasher();
            agent.IsInHouse = input.IsInHouse;
            agent.ModifyDate = DateTime.Now;
            agent.IsActive = SystemParam.DEACTIVE;
            agent.AcceptService = 1;
            agent.CommissionID = input.CommissionID;
            agent.DOB = input.DateOb;
            if (input.listArea != null && input.listArea.Count() <= MaxArea)
                agent.AgentAreas = areaBus.AddListArea(input.listArea);
            else
                return SystemParam.RETURN_FALSE;
            agent.Identification = input.identification;
            agent.Rating = 0;
            agent.AvatarUrl = input.AvatarUrl;
            if (input.IdentityImage != null)
            {
                agent.AgentImages = getlistImage(input.IdentityImage);
            }
            Member mem = getMember(input.Phone, input.Password, agent, null, SystemParam.ACTIVE);
            Wallet wallet = getWallet(Constant.WALLET_NO_WITHDRAW, mem, 0);
            Wallet wallet2 = getWallet(Constant.WALLET_WITHDRAW, mem, 0);
            cnn.Wallets.Add(wallet);
            cnn.Wallets.Add(wallet2);
            cnn.SaveChanges();
            Member members = cnn.Members.OrderByDescending(u => u.ID).FirstOrDefault();
            return SystemParam.RETURN_TRUE;
        }
        public int UpdateAgent(CreateAgentInputModel data)
        {
            var agent = cnn.Agents.Find(data.ID);
            Member member = cnn.Members.Where(u => u.User.Equals(data.Phone) && !u.IsActive.Equals(SystemParam.INACTIVE)).FirstOrDefault();
            if (agent != null && (member == null || member.ID == agent.Members.FirstOrDefault().ID))
            {
                agent.Name = data.Name;
                //agent.Phone = data.Phone;
                agent.Email = data.Email;
                agent.CommissionID = data.CommissionID;
                agent.DOB = data.DateOb;
                agent.IsInHouse = data.IsInHouse;
                agent.Identification = data.identification;
                agent.AvatarUrl = data.AvatarUrl;
                List<AgentImage> listImage = cnn.AgentImages.Where(u => u.AgentID.Equals(data.ID)).ToList();
                if (listImage.Count > 0)
                    cnn.AgentImages.RemoveRange(listImage);
                agent.AgentImages = getlistImage(data.IdentityImage);
                List<AgentArea> list = cnn.AgentAreas.Where(x => x.AgentID.Equals(data.ID)).ToList();
                cnn.AgentAreas.RemoveRange(list);
                agent.AgentAreas = areaBus.AddListArea(data.listArea);
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            else
            {
                return SystemParam.RETURN_FALSE;
            }
        }
        public CreateAgentInputModel AgentDetail(int? id)
        {

            var listAreaPriority = ListAreaPriority(id);
            var listBankAgent = GetListBankAgent(id);
            var listIdentityImage = GetIdentityImage(id);
            var model = (from ag in cnn.Agents
                         join mem in cnn.Members
                         on ag.ID equals mem.AgentID
                         where ag.ID.Equals(id.Value)
                         select new CreateAgentInputModel()
                         {
                             ID = ag.ID,
                             Name = ag.Name,
                             DobDetail = ag.DOB,
                             identification = ag.Identification,
                             Phone = ag.Phone,
                             Email = ag.Email,
                             IsInHouse = ag.IsInHouse.HasValue ? ag.IsInHouse.Value : 0,
                             Address = ag.Address,
                             Status = ag.IsActive,
                             AvatarUrl = String.IsNullOrEmpty(ag.AvatarUrl) ? "" : ag.AvatarUrl,
                             CommissionID = ag.CommissionID,
                             AcceptService = ag.AcceptService,
                             Rating = ag.Rating,
                             Cash = cnn.Wallets.Where(x => x.MemberID.Equals(mem.ID) && x.TYPE.Equals(Constant.WALLET_WITHDRAW)).Select(x => x.Balance).FirstOrDefault(),
                             Deposit = cnn.Wallets.Where(x => x.MemberID.Equals(mem.ID) && x.TYPE.Equals(Constant.WALLET_NO_WITHDRAW)).Select(x => x.Balance).FirstOrDefault(),
                         }).FirstOrDefault();
            if (listBankAgent != null)
            {
                model.ListBankAgent = listBankAgent;
            }
            if (listIdentityImage != null)
            {
                model.IdentityImage = listIdentityImage;
            }
            if (listAreaPriority != null)
            {
                model.ListAreaPriority = listAreaPriority;
            }
            if (model != null)
            {
                return model;
            }
            else
            {
                return new CreateAgentInputModel();
            }
        }
        public List<string> GetIdentityImage(int? id)
        {
            var query = cnn.AgentImages.Where(x => x.AgentID.Equals(id.Value)).Select(x => x.Path).ToList();
            return query;
        }
        public List<ConfigCommissionViewModel> GetListCommision()
        {
            try
            {
                var query = (from com in cnn.ConfigCommissions
                             select new ConfigCommissionViewModel()
                             {
                                 ID = com.ID,
                                 Name = com.Name,
                                 MastersBenefit = com.MastersBenefit
                             }).ToList();
                return query;
            }
            catch
            {
                return new List<ConfigCommissionViewModel>();
            }
        }
        public List<AreaOutputModel> ListAreaPriority(int? id)
        {
            try
            {
                var model = (from aga in cnn.AgentAreas
                             join ag in cnn.Agents
                             on aga.AgentID equals ag.ID
                             join a in cnn.Areas
                             on aga.AreaID equals a.ID
                             join dis in cnn.Districts
                             on a.DistrictCode equals dis.Code
                             join prov in cnn.Provinces
                             on dis.ProvinceCode equals prov.Code
                             where ag.ID.Equals(id.Value) && aga.IsActive.Equals(SystemParam.ACTIVE) && aga.Area.IsActive.Equals(SystemParam.ACTIVE)
                             select new AreaOutputModel()
                             {
                                 ID = a.ID,
                                 Name = a.Name,
                                 DistrictCode = dis.Code,
                                 DistrictName = dis.Name,
                                 ProvinceCode = prov.Code,
                                 ProvinceName = prov.Name
                             }).ToList();
                if (model != null)
                {
                    return model;
                }
                else
                {
                    return new List<AreaOutputModel>();
                }
            }
            catch
            {
                return new List<AreaOutputModel>();
            }

        }
        public List<BankAgentViewModel> GetListBankAgent(int? agentId)
        {
            try
            {
                var query = (from bmem in cnn.BankMembers
                             join bank in cnn.Banks
                             on bmem.BankID equals bank.ID
                             join mem in cnn.Members
                             on bmem.MemberID equals mem.ID
                             join ag in cnn.Agents
                             on mem.AgentID equals ag.ID
                             where ag.ID.Equals(agentId.Value)
                             select new BankAgentViewModel()
                             {
                                 BankName = bank.Name,
                                 Account = bmem.Acount,
                                 Name = ag.Name
                             }).ToList();
                if (query != null)
                {
                    return query;
                }
                else
                {
                    return new List<BankAgentViewModel>();
                }
            }
            catch
            {
                return new List<BankAgentViewModel>();
            }

        }
        public List<CreateAgentInputModel> GetListAgent()
        {
            var list = (from mem in cnn.Members
                        join ag in cnn.Agents
                        on mem.AgentID equals ag.ID
                        where mem.AgentID.HasValue && mem.IsActive.Equals(SystemParam.ACTIVE)
                        select new CreateAgentInputModel()
                        {
                            ID = ag.ID,
                            Name = ag.Name,
                            Phone = ag.Phone,
                            MemberId = mem.ID,
                            Lang = mem.Lang
                        }).ToList();
            return list;
        }
        public int AddMoneyAgent(int agentID, int money)
        {
            Wallet wal = cnn.Wallets.Where(x => x.Member.Agent.ID.Equals(agentID) && x.TYPE.Equals(1)).FirstOrDefault();
            if (wal != null)
            {
                wal.Balance = wal.Balance + money;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            else
            {
                return SystemParam.RETURN_FALSE;
            }

        }
        public ExcelPackage ExportExcel(string CodeOrName, string Phone, string Email, int? Status, string FromDate, string ToDate)
        {
            try
            {
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/List_Washer.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 3;
                int stt = 1;

                var list = Search(CodeOrName, Phone, Email, Status, FromDate, ToDate);
                foreach (var item in list)
                {
                    sheet.Row(row).Height = 20;
                    sheet.Cells[row, 1].Value = stt;
                    sheet.Cells[row, 2].Value = item.Code;
                    sheet.Cells[row, 3].Value = item.Name;
                    sheet.Cells[row, 4].Value = item.Phone;
                    sheet.Cells[row, 5].Value = item.Email;
                    sheet.Cells[row, 6].Value = item.Rating;
                    sheet.Cells[row, 7].Value = item.CommisstitonName;
                    sheet.Cells[row, 8].Value = item.CreatedDate.Value.ToString("dd/MM/yyyy");
                    sheet.Cells[row, 9].Value = item.AcceptServiceStr;
                    sheet.Cells[row, 10].Value = item.StatusStr;
                    row++;
                    stt++;
                }
                return pack;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ExcelPackage();
            }
        }
        public int DeleteAgent(int id)
        {
            Agent agent = cnn.Agents.Find(id);
            Member mem = cnn.Members.Where(x => x.AgentID != null && x.AgentID.Value.Equals(id)).FirstOrDefault();
            if (agent != null && mem != null)
            {
                agent.IsActive = 0;
                mem.IsActive = 0;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            else
            {
                return SystemParam.RETURN_FALSE;
            }

        }
        public int ChangeStatus(int id)
        {
            Agent agent = cnn.Agents.Find(id);
            Member mem = cnn.Members.Where(x => x.AgentID != null && x.AgentID.Value.Equals(id)).FirstOrDefault();
            if (agent != null && mem != null)
            {
                if (agent.IsActive == SystemParam.ACTIVE)
                {
                    agent.IsActive = 2;
                    mem.IsActive = 2;
                    cnn.SaveChanges();
                }
                else
                {
                    agent.IsActive = 1;
                    mem.IsActive = 1;
                    cnn.SaveChanges();
                }
                return SystemParam.RETURN_TRUE;
            }
            else
            {
                return SystemParam.RETURN_FALSE;
            }
        }
        public Member getMember(string phone, string pass, Agent agent, Customer cus, int isActive = 2)
        {
            if (!String.IsNullOrEmpty(pass) && pass.Length > 0)
                pass = Util.CreateMD5(pass);
            else
                pass = "";
            Member mem = new Member();
            mem.CreateDate = DateTime.Now;
            mem.IsActive = isActive;
            mem.ConfirmCode = Util.RandomNumber(100000, 999999).ToString();

            mem.User = phone;
            mem.DeviceID = "";
            mem.Password = pass;
            if (agent != null)
            {
                mem.IsActive = SystemParam.ACTIVE;
                mem.Agent = agent;
            }
            else
            {
                mem.Customer = cus;
                SendOTP(phone, mem.ConfirmCode);
            }
            // mem.ConfirmCode = "888888";
            mem.CreateDate = DateTime.Now;
            mem.ExpriceDateOTP = DateTime.Now.AddMinutes(Constant.TIME_EXPRICE_TOKEN);
            mem.Lang = SystemParam.VN;
            return mem;

        }
        public Wallet getWallet(int type, Member mem, int balance)
        {
            Wallet wallet2 = new Wallet();
            wallet2.TYPE = type;
            wallet2.Balance = 0;
            wallet2.Member = mem;
            wallet2.CreateDate = DateTime.Now;
            wallet2.IsActive = SystemParam.ACTIVE;

            return wallet2;
        }
        public List<AgentImage> getlistImage(List<string> IdentityImage)
        {
            List<AgentImage> output = new List<AgentImage>();
            if (IdentityImage != null)
                foreach (var item in IdentityImage.ToList())
                {
                    AgentImage ai = new AgentImage();
                    ai.Path = item;
                    ai.IsActive = SystemParam.ACTIVE;
                    ai.CreateDate = DateTime.Now;
                    output.Add(ai);
                }
            return output;
        }

        public bool UpdateLocationAgent(double longi, double lati, int agentID)
        {
            Agent agent = cnn.Agents.Find(agentID);
            agent.longi = longi;
            agent.lati = lati;
            agent.ModifyDate = DateTime.Now;
            cnn.SaveChanges();
            return true;
        }
        public Agent ShowEdit(int ID)
        {
            try
            {
                Agent edit = cnn.Agents.Find(ID);
                return edit;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new Agent();
            }
        }

        public int SaveEdit(int ID, string Name, string Address)
        {
            try
            {

                Agent agEdit = cnn.Agents.Find(ID);
                agEdit.Name = Name;
                //agEdit.Address = Address;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public int cancelActive(int ID)
        {
            try
            {
                //Agent agentCancel = cnn.Agents.Find(ID);
                //Customer cusAvtive = cnn.Customers.Find(agentCancel.CustomerActiveID);
                //agentCancel.CustomerActiveID = null;
                //agentCancel.ActiveDate = null;
                //agentCancel.Phone = null;
                //agentCancel.ModifiedDate = DateTime.Now;
                //cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public AgentOutputModel CheckByCode(string Code)
        {
            AgentOutputModel agents = cnn.Members.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Agent.Code.Equals(Code)).Select(u => new AgentOutputModel
            {
                ID = u.Agent.ID,
                url = u.Agent.AvatarUrl,
                rate = u.Agent.Rating.HasValue ? u.Agent.Rating.Value : 5,
                Code = u.Agent.Code,
                Name = u.Agent.Name,
                Phone = u.Agent.Phone
            }).FirstOrDefault();
            if (agents != null)
                return agents;
            else
                return null;
        }

        //public int DeleteAgent(int ID)
        //{
        //    try
        //    {
        //        //ShopBusiness shp = new ShopBusiness();
        //        //Customer delAgent = cnn.Customers.Find(ID);
        //        //delAgent.IsActive = SystemParam.NO_ACTIVE_DELETE;
        //        //if(delAgent.ShopID != null)
        //        //{
        //        //    shp.DeleteShop(delAgent.ShopID.Value);
        //        //}
        //        //cnn.SaveChanges();
        //        return SystemParam.RETURN_TRUE;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return SystemParam.RETURN_FALSE;
        //    }
        //}

        public int ImportExcel(HttpPostedFileBase ExcelFile)
        {
            try
            {
                if (ExcelFile == null || ExcelFile.ContentLength == 0)
                {
                    //ModelState.AddModelError("", "Hãy chọn một file Excel");
                    return SystemParam.FILE_NOT_FOUND;
                }
                else if (ExcelFile.FileName.EndsWith("xls") || ExcelFile.FileName.EndsWith("xlsx"))
                {
                    string path = HttpContext.Current.Server.MapPath("~/Import/" + ExcelFile.FileName);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    ExcelFile.SaveAs(path);
                    FileInfo file = new FileInfo(path);
                    ExcelPackage pack = new ExcelPackage(file);
                    ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                    int row = 3;
                    int col = 1;
                    if (sheet.Cells[row, col].Value == null)
                    {
                        return SystemParam.FILE_EMPTY;
                    }
                    object data = 0;
                    while (data != null)
                    {
                        data = sheet.Cells[row, col].Value;
                        if (data == null)
                            break;
                        string CheckCode = sheet.Cells[row, col].Value.ToString();
                        //if (cnn.Agents.Where(a => a.Code.Equals(CheckCode)).Count() > 0) {
                        //    return SystemParam.FILE_DATA_DUPLICATE;
                        //}
                        Agent item = new Agent();
                        //item.Code = sheet.Cells[row, col].Value.ToString();
                        //item.Name = sheet.Cells[row, col + 1].Value.ToString();
                        //item.Address = "Ha Noi";
                        item.CreateDate = DateTime.Now;
                        item.IsActive = SystemParam.ACTIVE;
                        cnn.Agents.Add(item);
                        row++;
                        cnn.SaveChanges();
                    }
                    return SystemParam.FILE_IMPORT_SUCCESS;
                }
                else
                {
                    return SystemParam.FILE_FORMAT_ERROR;
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.FILE_FORMAT_ERROR;
            }
        }

        public int AddPoint(string ListChecked, int Point, string Description)
        {
            string[] listBox = ListChecked.Split(',');
            List<int> boxes = new List<int>();

            foreach (var number in listBox)
            {
                if (number != null && number != "")
                {
                    boxes.Add(Convert.ToInt32(number));
                }
            }

            try
            {
                foreach (var box in boxes)
                {
                    NotifyBusiness notify = new NotifyBusiness();
                    OneSignalBusiness package = new OneSignalBusiness();
                    TransactionHistoryBusiness pointBusiness = new TransactionHistoryBusiness();
                    Customer customer = cnn.Customers.Find(box);
                    //customer.Point += Point;
                    cnn.SaveChanges();
                    string code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 6);
                    pointBusiness.CreateTransaction(box, Point, Constant.TYPE_TRANSACTION_ADD_POINT_BY_ADMIN, code, null, null, Constant.WALLET_NO_WITHDRAW, "VN", Constant.STATUS_TRANSACTION_SUCCESS, null);
                    string contennotify = "Bạn vừa được cộng " + Point + " điểm từ hệ thống";
                    notify.CreateNoti(box, SystemParam.HISPOINT_HE_THONG_CONG_DIEM, 0, null, null, "vn");
                    //if(customer.DeviceID.Length > 0)
                    //{
                    //    NotifyDataModel notifyData = new NotifyDataModel();
                    //    notifyData.id = customer.ID;
                    //    notifyData.type = SystemParam.HISPOINT_HE_THONG_CONG_DIEM;
                    //    package.StartPushNoti(notifyData, customer.DeviceID, contennotify, SystemParam.ROLE_ADMIN);
                    //    string a = notify.PushNotify(customer.ID, contennotify, SystemParam.HISPOINT_HE_THONG_CONG_DIEM);
                    //}

                }
                return 1;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return 0;
            }
        }

        public int ResetPassword(int ID)
        {
            try
            {
                var DefaultPassword = "789789";
                Customer customer = cnn.Customers.Find(ID);
                //customer.PassWord = Util.CreateMD5(DefaultPassword);
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        public double getLat(String PlusCode)
        {
            int partition = 0;
            Regex reg = new Regex("/@([0-9]*.?[0-9]*),([0-9]*.?[0-9]*),");

            Match regResult = reg.Match(PlusCode);
            String regResultString = regResult.ToString();
            regResultString = regResultString.Substring(2, regResultString.Length - 2);

            for (int i = 0; i <= regResultString.Length - 2; i++)
            {
                if (regResultString[i] == ',')
                {
                    partition = i;
                }
            }

            String lat = regResultString.Substring(0, partition);
            String longg = regResultString.Substring(partition + 1, regResultString.Length - 1 - partition - 1);

            return double.Parse(lat, CultureInfo.InvariantCulture);
        }

        public double getLong(String PlusCode)
        {
            int partition = 0;
            Regex reg = new Regex("/@([0-9]*.?[0-9]*),([0-9]*.?[0-9]*),");

            Match regResult = reg.Match(PlusCode);
            String regResultString = regResult.ToString();
            regResultString = regResultString.Substring(2, regResultString.Length - 2);

            for (int i = 0; i <= regResultString.Length - 2; i++)
            {
                if (regResultString[i] == ',')
                {
                    partition = i;
                }
            }

            String lat = regResultString.Substring(0, partition);
            String longg = regResultString.Substring(partition + 1, regResultString.Length - 1 - partition - 1);

            return double.Parse(longg, CultureInfo.InvariantCulture);
        }
    }
}
