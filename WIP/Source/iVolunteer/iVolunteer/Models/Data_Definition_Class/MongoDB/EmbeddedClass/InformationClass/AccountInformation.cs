using iVolunteer.Common;
namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store user's account infomation
    /// </summary>
    public class AccountInformation
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string IdentifyID { get; set; }
        public string AvtImgLink { get; set; }
        public string CoverImgLink { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActivate { get; set; }
        public bool IsConfirmed { get; set; }

        public AccountInformation()
        {
            this.Email = "";
            this.Password = "";
            this.DisplayName = "";
            this.IdentifyID = "";
            this.AvtImgLink = "";
            this.CoverImgLink = "";
            this.IsAdmin = Constant.IS_USER;
            this.IsActivate = Constant.IS_ACTIVATE;
            this.IsConfirmed = Constant.IS_NOT_CONFIRMED;
        }
    }
}