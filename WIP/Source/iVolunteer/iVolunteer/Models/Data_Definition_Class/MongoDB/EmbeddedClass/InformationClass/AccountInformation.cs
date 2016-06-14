using iVolunteer.Common;
using iVolunteer.Models.Data_Definition_Class.ViewModel;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store user's account infomation
    /// </summary>
    public class AccountInformation
    {
        public string UserID { get; set; }
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
            this.UserID = "";
            this.Email = "";
            this.Password = "";
            this.DisplayName = "";
            this.IdentifyID = "";
            this.AvtImgLink = Constant.DEFAULT_AVATAR;
            this.CoverImgLink = Constant.DEFAULT_COVER;
            this.IsAdmin = Constant.IS_USER;
            this.IsActivate = Constant.IS_ACTIVATE;
            this.IsConfirmed = Constant.IS_NOT_CONFIRMED;
        }

        public AccountInformation(RegisterModel registerModel)
        {
            this.Email = registerModel.Email;
            this.Password = registerModel.Password;
            this.DisplayName = registerModel.RealName;
            this.IdentifyID = registerModel.IdentifyID;
            this.AvtImgLink = Constant.DEFAULT_AVATAR;
            this.CoverImgLink = Constant.DEFAULT_COVER;
            this.IsAdmin = Constant.IS_USER;
            this.IsActivate = Constant.IS_ACTIVATE;
            this.IsConfirmed = Constant.IS_NOT_CONFIRMED;
        }
    }
}