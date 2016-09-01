using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    public class Mongo_FailureReport
    {
        public ObjectId _id { get; set; }
        public SDLink SentPerson { get; set; }
        public int Type { get; set; }
        public string Place { get; set; }
        public string Content { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        public DateTime SendDate { get; set; }
        public string HaveScreenshot { get; set; }

        public Mongo_FailureReport()
        {
            this._id = new ObjectId();
            this.SentPerson = new SDLink();
            this.Type = 1;
            this.Place = "";
            this.Content = "";
            this.SendDate = new DateTime();
            this.HaveScreenshot = "";
        }

        public Mongo_FailureReport(Mongo_FailureReport info)
        {
            this._id = ObjectId.GenerateNewId();
            this.SentPerson = new SDLink();
            this.Type = info.Type;
            this.Place = info.Place;
            this.Content = info.Content;
            this.SendDate = DateTime.Now;
            this.HaveScreenshot = "";
        }

        public string Get_ScreenShotLink ()
        {
            return "/Images/FailureReport" + this.HaveScreenshot + ".jpg";
        }
    }
}