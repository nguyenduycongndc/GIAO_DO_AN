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
    public class IntroBusiness : GenericBusiness
    {
        public IntroBusiness(WE_SHIPEntities context = null) : base()
        {


        }

        public IntroAppOutputModel ListIntro(int type, string lang)
        {
            int typelang = 1;
            if (!lang.Equals("vi"))
            {

                lang = "en";
                typelang = 2;
            }
            IntroAppOutputModel query = new IntroAppOutputModel();
            query.ListIntro = cnn.Introes.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Value.Equals(type) && u.Content.Equals(lang)).OrderBy(u => u.DisplayOrder.Value).Select(u => new
            {
                Content = u.Content,
                ListImage = u.Image
            }).ToList();
            query.ListProlicy = cnn.Policies.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Role.Value.Equals(type) && u.Lang.HasValue && u.Lang.Value.Equals(typelang)).Select(u => new
            {
                u.Name,
                u.Type,
                u.Content
            }).ToList();
            return query;
        }
    }
}
