using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class FinanceItem
    {
        public ObjectId FinanceItemID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào khoản chi.")]
        [DisplayName("Khoản chi")]
        public string Term { get; set; }
        [DisplayName("Người thanh toán")]
        public SDLink Payer { get; set; }
        [DisplayName("Số tiền")]
        public double Amount { get; set; }
        [DisplayName("Thời gian")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        public DateTime Date { get; set; }
        public FinanceItem()
        {
            this.FinanceItemID = new ObjectId();
            this.Term = "";
            this.Payer = new SDLink();
            this.Amount = 0;
            this.Date = new DateTime();
        }

        public FinanceItem(FinanceItem item)
        {
            this.FinanceItemID = ObjectId.GenerateNewId();
            this.Term = item.Term;
            this.Payer = item.Payer;
            this.Amount = item.Amount;
            this.Date = item.Date;
        }
    }
}