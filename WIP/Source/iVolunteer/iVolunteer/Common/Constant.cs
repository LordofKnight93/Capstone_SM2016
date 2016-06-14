using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iVolunteer.Common
{
    public static class Constant
    {
        //permission status
        public const bool IS_PUBLIC = true;
        public const bool IS_PRIVATE = false;
        //activation status
        public const bool IS_ACTIVATE = true;
        public const bool IS_BANNED = true;
        //confirmation status
        public const bool IS_CONFIRMED = true;
        public const bool IS_NOT_CONFIRMED = false;
        //realation type
        public const int DIRECT_RELATION = 1;
        public const int INDIRECT_RELATION = 2;
        //role
        public const bool IS_ADMIN = true;
        public const bool IS_USER = false;
        // default avt, cover
        public const string DEFAULT_AVATAR = "Avatar";
        public const string DEFAULT_COVER = "Cover";
        //recruitment status
        public const bool IS_RECRUITING = true;
        public const bool IS_NOT_RECRUITING = false;
        // pinned
        public const bool IS_PINNED = true;
        public const bool IS_NOT_PINNED = false;
        //post type
        public const int POST_NORMAL = 1;
        public const int POST_IMAGE = 2;
        public const int POST_ANNOUNCE = 3;
    }
}