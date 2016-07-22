using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iVolunteer.Common
{
    public static class Status
    {
        //permission status
        public const bool IS_PUBLIC = true;
        public const bool IS_PRIVATE = false;
        //activation status
        public const bool IS_ACTIVATE = true;
        public const bool IS_BANNED = false;
        //confirmation status
        public const bool IS_CONFIRMED = true;
        public const bool IS_NOT_CONFIRMED = false;
        //recruitment status
        public const bool IS_RECRUITING = true;
        public const bool IS_NOT_RECRUITING = false;
        // pinned status
        public const bool IS_PINNED = true;
        public const bool IS_NOT_PINNED = false;
        // seen status
        public const bool IS_SEEN = true;
        public const bool IS_NOT_SEEN = false;
        // status of request
        public const bool ACCEPTED = true;
        public const bool PENDING = false;
        //project ongoing status
        public const bool ONGOING = true;
        public const bool ENDED = false;
    }

    public static class Default
    {
        //default avt, cover
        public const string DEFAULT_AVATAR = "/Images/DefaultAvatar.jpg";
        public const string DEFAULT_COVER = "/Images/DefaultCover.jpg";
    }

    public static class Role
    {
        //role
        public const bool IS_ADMIN = true;
        public const bool IS_USER = false;
    }
    public static class AcAcRelation
    {
        public const int FRIEND_RELATION = 1;
        public const int REPORT_RELATION = 2;
    }

    public static class AcGrRelation
    {
        public const int LEADER_RELATION = 1;
        public const int MEMBER_RELATION = 2;
        public const int FOLLOW_RELATION = 3;
        public const int REPORT_RELATION = 4;
    }
    public static class AcPrRelation
    {
        public const int ORGANIZE_RELATION = 0;
        public const int LEADER_RELATION = 1;
        public const int MEMBER_RELATION = 2;
        public const int SPONSOR_RELATION = 3;
        public const int FOLLOW_RELATION = 4;
        public const int SUGGESTED_RELATION = 5;
        public const int INVITED_RELATION = 6;
        public const int REPORT_RELATION = 7;
    }

    public static class GrPrRelation
    {
        public const int ORGANIZE_RELATION = 1;
        public const int MEMBER_RELATION = 2;
        public const int SPONSOR_RELATION = 3;
    }

    public static class AcPoRelation
    {
        public const int CREATOR_RELATION = 1;
        public const int FOLLOW_RELATION = 2;
        public const int LIKE_RELATION = 3;
    }

    public static class AcAlRelation
    {
        public const int CREATOR_RELATION = 1;
        public const int FOLLOW_RELATION = 2;
        public const int LIKE_RELATION = 3;
    }

    public static class AcImRelation
    {
        public const int CREATOR_RELATION = 1;
        public const int FOLLOW_RELATION = 2;
        public const int LIKE_RELATION = 3;
    }

    public static class Error
    {
        public const string WRONG_PASSWORD = "Mật khẩu không chính xác.";
        public const string ACCOUNT_NOT_EXIST = "Tài khoản không tồn tại";
        public const string ACCOUNT_EXIST = "Địa chỉ email này đã được đăng ký.";
        public const string ACCOUNT_BANNED = "Tài khoản này đã bị khóa.";
        public const string EMAIL_NOT_CONFIRM = "Tài khoản chưa được xác nhận";
        public const string IDENTIFYID_EXIST = "Số chứng minh thư đã được sử dụng";
        public const string UNEXPECT_ERROR = "Có lỗi xảy ra, vui lòng thử lại sau ít phút";
        public const string INVALID_INFORMATION = "Thông tin không hợp lệ, vui lòng nhập lại.";
        public const string ACCESS_DENIED = "Trang này không tồn tại hoặc bạn không có quyền truy cập.";
        public const string DISPLAYNAME_FAIL = "Không thể đổi tên quá nhiều lần trong vòng 90 ngày.";
        public const string EMAIL_INVALID = " Email không hợp lệ ";
        public const string PASSWORD_INVALID = "Mật khẩu phải có ít nhất 1 ký tự hoa, 1 ký tự thường, 1 ký tự số, không có ký tự đặc biệt và độ dài từ 8 đến 15 ký tự.";
        public const string IDENTIFYID_INVALID = "Số chứng minh thư không hợp lệ";
        public const string PHONE_INVALID = "Số điện thoại không hợp lệ";
    }

    public static class Handler
    {
        public const string USER = "User";
        public const string PROJECT = "Project";
        public const string GROUP = "Group";
        public const string ALBUM = "Album";
        public const string POST = "Post";
        public const string IMAGE = "Image";
    }

    public static class Gender
    {
        public const bool MALE = true;
        public const bool FEMALE = false;
    }
    public static class Notify
    {
        public const int JOIN_GROUP_REQUEST = 1;
        public const int JOIN_PROJECT_REQUEST = 2;
    }
}