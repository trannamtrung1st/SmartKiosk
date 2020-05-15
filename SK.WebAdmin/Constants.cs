using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SK.WebHelpers;

namespace SK.WebAdmin
{
    public static class Routing
    {
        public const string DASHBOARD = "/dashboard";
        public const string LOGIN = "/identity/login";
        public const string LOGOUT = "/identity/logout";
        public const string REGISTER = "/identity/register";
        public const string IDENTITY = "/identity";
        public const string POST = "/post";
        public const string POST_CLIENT = "/post/indexclient";
        public const string POST_CREATE = "/post/create";
        public const string POST_DETAIL = "/post/{id}";
        public const string ADMIN_ONLY = "/adminonly";
        public const string ACCESS_DENIED = "/accessdenied";
        public const string STATUS = "/status";
        public const string ERROR = "/error";
        public const string INDEX = "/";
        public const string FILE = "/file";
    }

    public static class AppController
    {
        public const string LANG = "language";
    }

    public static class AppCookie
    {
        public const string TOKEN = "_appuat";
    }

    public static class AppView
    {
        public const string MESSAGE = "MessageView";
        public const string STATUS = "StatusView";
    }

    public static class Menu
    {
        public const string DASHBOARD = "dashboard";
        public const string POST = "post";
        public const string POST_CLIENT = "post_client";
        public const string ADMIN_ONLY = "admin_only";
        public const string FILE = "file";
    }

    public static class GeneralMessage
    {
        public const string ERROR = "Something's wrong, please try again or contact admin";
        public const string NOT_FOUND = "Can not find requested resource";
    }

}
