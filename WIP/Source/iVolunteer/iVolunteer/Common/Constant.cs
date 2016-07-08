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
    public static class Relation
    {
        //realation type
        public const int LEADER_RELATION = 1;
        public const int MEMBER_RELATION = 2;
        public const int SPONSOR_RELATION = 3;
        public const int FOLLOW_RELATION = 4;
        public const int LIKE_RELATION = 5;
    }

    public static class PostType
    {
        //post type
        public const int POST_NORMAL = 1;
        public const int POST_IMAGE = 2;
        public const int POST_ANNOUNCE = 3;
    }

    public class RequestType
    {
        public const int JOIN_REQUEST = 1;
        public const int SPONSOR_REQUEST = 2;
        public const int FRIEND_REQUEST = 3;
        public const int SUGGEST_REQUEST = 4;
        public const int INVITE_TO = 5;
    }

    public static class RequestContent
    {
        public const string JOIN_REQUEST = " muốn tham gia.";
        public const string SPONSOR_REQUEST = " muốn tài trợ, quyên góp cho sự kiện.";
        public const string FRIEND_REQUEST = " gửi lời mời kết bạn.";
        public const string SUGGEST_REQUEST = " đề xuất ";
        public const string INVITE_TO = " mời bạn tham gia ";
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
}