using System;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Common;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Helpers;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    public class ImageModel
    {
        public ImageInformation[] Files { get; set; }
    }
}
