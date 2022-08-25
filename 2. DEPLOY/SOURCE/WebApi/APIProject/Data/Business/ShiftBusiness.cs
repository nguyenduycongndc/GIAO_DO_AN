using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class ShiftBusiness : GenericBusiness
    {
        public ShiftBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        public List<string> ListShiftTime()
        {
            List<string> ShiftTime = new List<string>(0);
            int shift = int.Parse(cnn.Configs.Where(u => u.NameConstant.Equals(Constant.SHIFT_TIME)).FirstOrDefault().ValueConstant);
            int times = 24 * 60 / shift;
            for (int index = 0; index <= times; index++)
            {
                string time = DateTime.Today.AddMinutes(index * shift).ToString("HH:mm");
                ShiftTime.Add(time);
            }
            return ShiftTime.Distinct().ToList();
        }

        public List<ShiftInWeekModel> GetListShift()
        {
            var lsDay = cnn.Days.ToList();
            List<ShiftInWeekModel> output = new List<ShiftInWeekModel>();
            for (int index = 0; index < 7; index++)
            {
                DateTime time = DateTime.Now.AddDays(index);
                var day = time.DayOfWeek.ToString();
                var Days = lsDay.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Day1.Equals(day)).Select(u => u).FirstOrDefault();
                List<string> listShift = Days.ClassShift.Shifts.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ClassShiftID.Value.Equals(Days.ClassShiftID)).OrderBy(u => u.ID).Select(u => u.shift1).ToList();
                if (index == 0)
                {
                    int timeWaiting = int.Parse(cnn.Configs.Where(u => u.NameConstant.Equals("TimeWaiting")).FirstOrDefault().ValueConstant);
                    DateTime date = DateTime.Now.AddMinutes(timeWaiting);
                    var a = date.TimeOfDay;
                    listShift = listShift.Where(u => DateTime.ParseExact(u, SystemParam.CONVERT_DATETIME_HOUR, null).TimeOfDay >= date.TimeOfDay).ToList();
                }
                ShiftInWeekModel data = new ShiftInWeekModel { Date = time.ToString(SystemParam.CONVERT_DATETIME), ListShift = listShift };
                if (listShift.Count > 0)
                {
                    output.Add(data);
                }
            }
            return output;
        }
        public List<ShiftOutputModel> GetShift()
        {
            List<ShiftOutputModel> output = cnn.Shifts.OrderBy(u => u.ID).Select(u => new ShiftOutputModel
            {
                ID = u.ID,
                IsActive = u.IsActive,
                shift = u.shift1
            }).ToList();
            return output;
        }
        public List<ShiftViewModel> Shift()
        {
            List<ShiftViewModel> query = new List<ShiftViewModel>();
            query = cnn.ClassShifts.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new ShiftViewModel
            {
                ID = u.ID,
                Name = u.Name,
                ListDays = u.Days.Select(s => new DayViewModel
                {
                    ID = s.ID,
                    Day = s.Day1,
                }).ToList(),
                ListShift = u.Shifts.OrderBy(s => s.ID).Select(s => new ShiftOutputModel
                {
                    ID = s.ID,
                    IsActive = s.IsActive,
                    shift = s.shift1
                }).ToList()
            }).ToList();
            return query;
        }
        public SystemResult UpdateShift(ShiftViewModel item)
        {
            var listShift = cnn.Shifts.Where(u => u.ClassShiftID.Value.Equals(item.ID)).ToList();
            if (item.ListDays.Count == 0)
                return resultBus.ErrorResult("Please select day");
            List<int> listDayID = item.ListDays.Select(u => u.ID).ToList();
            var listDay = cnn.Days.Where(u => listDayID.Contains(u.ID) && !u.ClassShiftID.Equals(item.ID)).ToList();
            foreach (DB.Day day in listDay)
            {
                day.ClassShiftID = item.ID;
            }
            foreach (ShiftOutputModel input in item.ListShift)
            {
                Shift shift = listShift.Where(u => u.ID.Equals(input.ID)).FirstOrDefault();
                if (shift != null)
                    shift.IsActive = input.IsActive;
            }
            cnn.SaveChanges();
            return resultBus.SucessResult("");
        }
        public SystemResult DeleteShift(int ID)
        {
            var listShift = cnn.ClassShifts.Find(ID);
            listShift.IsActive = SystemParam.INACTIVE;
            cnn.SaveChanges();
            return resultBus.SucessResult("");
        }
        public SystemResult CreateShifts(int time)
        {
            var shiftConfig = cnn.Configs.Where(u => u.NameConstant.Equals(Constant.SHIFT_TIME)).FirstOrDefault();
            shiftConfig.ValueConstant = time.ToString();
            List<string> listShift = ListShiftTime();
            ClassShift s = cnn.ClassShifts.OrderByDescending(u => u.ID).FirstOrDefault();
            ClassShift classShift = new ClassShift
            {
                ID = s != null ? s.ID + 1 : 1,
                IsActive = SystemParam.ACTIVE,
                Name = "",
                Time = 1,
                CreateDate = DateTime.Now,
                Shifts = listShift.Select(u => new DB.Shift
                {
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now,
                    shift1 = u,
                }).ToList(),
            };
            cnn.ClassShifts.Add(classShift);
            cnn.SaveChanges();
            return resultBus.SucessResult(Shift().Where(u => u.ID.Equals(classShift.ID)).FirstOrDefault());
        }

        public SystemResult CreateShift(int time)
        {
            var shiftConfig = cnn.Configs.Where(u => u.NameConstant.Equals(Constant.SHIFT_TIME)).FirstOrDefault();
            shiftConfig.ValueConstant = time.ToString();
            var lsshift = cnn.Shifts.ToList();
            cnn.Shifts.RemoveRange(lsshift);
            cnn.SaveChanges();
            var classShift = cnn.ClassShifts.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            List<string> listShift = ListShiftTime();
            foreach (var cl in classShift)
            {
                List<Shift> shifts = listShift.Select(u => new DB.Shift
                {
                    ClassShiftID = cl.ID,
                    CreateDate = DateTime.Now,
                    IsActive = SystemParam.ACTIVE,
                    shift1 = u
                }).ToList();
                cnn.Shifts.AddRange(shifts);
            }
            cnn.SaveChanges();
            return resultBus.SucessResult("");
        }

        public List<ShiftOutputModel> UpdateShift(List<ShiftOutputModel> listInput, int ID = 1)
        {
            var listShift = cnn.Shifts.Where(u => u.ClassShiftID.Value.Equals(ID)).ToList();
            foreach (ShiftOutputModel input in listInput)
            {
                Shift shift = listShift.Where(u => u.ID.Equals(input.ID)).FirstOrDefault();
                if (shift != null)
                    shift.IsActive = input.IsActive;
            }
            cnn.SaveChanges();
            return GetShift();
        }
        public ShiftViewModel ShiftDetail(int shiftID)
        {
            var model = Shift().Where(x => x.ID.Equals(shiftID)).FirstOrDefault();
            return model;
        }
    }
}
