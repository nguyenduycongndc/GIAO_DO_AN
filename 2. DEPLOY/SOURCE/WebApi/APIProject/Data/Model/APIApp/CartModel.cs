using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    
    public class AddCartInputModel
    {
        public int ServicePriceID { get; set; }
        public List<int> TopingID { get; set; }
        public int Quantity { get; set; }

    }
    public class UpdateCartQuantityInputModel
    {
        public int CartID { get; set; }
        public int Quantity { get; set; }
    }
    public class UpdateCartNoteInputModel
    {
        public int CartID { get; set; }
        public string Note { get; set; }
    }
    public class CartDetailOutput
    {
        public int ID { get; set; }
        public int ServicePriceID { get; set; }
        public string ImageURL { get; set; }
        public string Name { get; set; }
        public string Toping { get; set; }
        public List<int> TopingIDs { get; set; }
        public int Price { get; set; }
        public int BasePrice { get; set; }
        public int Quantity { get; set; }
        public int SumPrice { get; set; }
        public string Note { get; set; }
    }
}
