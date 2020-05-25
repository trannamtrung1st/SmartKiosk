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
        public const string LOCATION = "/location";
        public const string LOCATION_CREATE = "/location/create";
        public const string DEVICE = "/device";
        public const string OWNER = "/owner";
        public const string OWNER_CREATE = "/owner/create";
        public const string OWNER_DETAIL = "/owner/{id}";
        public const string ACCOUNT = "/account";
        public const string RES_TYPE = "/res_type";
        public const string ENTITY_CATE = "/entity_cate";
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
        public const string LOCATION = "location";
        public const string DEVICE = "device";
        public const string OWNER = "owner";
        public const string ACCOUNT = "account";
        public const string RES_TYPE = "res_type";
        public const string ENTITY_CATE = "entity_cate";
    }

    public static class GeneralMessage
    {
        public const string ERROR = "Có lỗi xảy ra, liên hệ với bộ phận kỹ thuật để được hỗ trợ";
        public const string NOT_FOUND = "Không thể tìm thấy trang yêu cầu";
        public const string SOME_INP_NOT_VALID = "Một số dữ liệu gửi đi chưa phù hợp";
        public const string CREATE_SUCCESS = "Thêm mới thành công";
        public const string DELETE_SUCCESS = "Xóa thành công";
        public const string UPDATE_SUCCESS = "Cập nhật thành công";
        public const string R_U_SURE = "Chắc chắn?";
    }

}
