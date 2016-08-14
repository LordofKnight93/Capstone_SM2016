﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class OutsideDonator
    {
        public ObjectId OutsideDonatorID { get; set; }
        public string Name { get; set; }
        public int IndentifyID { get; set; }
        public string Address { get; set; }
        public double Amount { get; set; }
        public DateTime ReceivedDate { get; set; }
        public int Method { get; set; }
        public bool IsReceived { get; set; }
        public OutsideDonator()
        {
            this.OutsideDonatorID = ObjectId.GenerateNewId();
            this.Name = "";
            this.IndentifyID = 0;
            this.Address = "";
            this.Amount = 0;
            this.ReceivedDate = new DateTime();
            this.Method = 0;
            this.IsReceived = false;
        }
    }
}