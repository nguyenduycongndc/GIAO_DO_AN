using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Utils
{
    class EnumConstant
    {
    }
    public enum TypeSentOrder
    {
        FirstWasher = 1,
        Before30Min,
        Before30Min1,
        Before30Min2,
        finish1,
        finish2,
    }
    public enum StatusOrderProduct {
        Reject = 0,
        Ordering = 1,
        WaitingAdminConfirm = 2,
        AdminConfirm = 3,
        Complete = 4,
        AdminReject = 5,
    }

    public enum TypeServiceBonus {
        Extra = 1,
        BirthDay = 2
    }
    public enum ErrorCode {
        NotEnoughMoney = 109,

    }
    public enum TypeTransaction {
        Subtract = 0,
        Add = 1
    }
    public enum RoleUser
    {
        Admin = 1,
        Accountant,
        Marketing,
    }
}
