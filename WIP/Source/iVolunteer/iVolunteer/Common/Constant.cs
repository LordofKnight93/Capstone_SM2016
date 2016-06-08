using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iVolunteer.Common
{
    public static class Constant
    {
        public const bool IS_PUBLIC = true;
        public const bool IS_PRIVATE = false;

        public const bool IS_ACTIVATE = true;
        public const bool IS_BANNED = true;

        public const bool IS_CONFIRMED = true;
        public const bool IS_NOT_CONFIRMED = true;

        public const int DIRECT_RELATION = 1;
        public const int INDIRECT_RELATION = 2;

        public const bool IS_ADMIN = true;
        public const bool IS_USER = false;

        public const string DEFAULT_AVATAR = "Avater";
        public const string DEFAULT_COVER = "Cover";

        public const bool IS_RECRUITING = true;
        public const bool IS_NOT_RECRUITING = false;
    }
}