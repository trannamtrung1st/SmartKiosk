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
        public const string LOCATION_DETAIL = "/location/{id}";
        public const string LOCATION_CREATE = "/location/create";
        public const string LOCATION_DASHBOARD = "/location/{loc_id}/dashboard";
        public const string LOCATION_RESOURCE = "/location/{loc_id}/resource";
        public const string LOCATION_RESOURCE_CREATE = "/location/{loc_id}/resource/create";
        public const string LOCATION_RESOURCE_DETAIL = "/location/{loc_id}/resource/{id}";
        public const string LOCATION_POST = "/location/{loc_id}/post";
        public const string LOCATION_POST_CREATE = "/location/{loc_id}/post/create";
        public const string LOCATION_POST_DETAIL = "/location/{loc_id}/post/{id}";
        public const string LOCATION_CONFIG = "/location/{loc_id}/config";
        public const string LOCATION_SCHEDULE = "/location/{loc_id}/schedule";
        public const string LOCATION_DEVICE = "/location/{loc_id}/device";
        public const string LOCATION_BUILDING = "/location/{loc_id}/building";
        public const string LOCATION_BUILDING_CREATE = "/location/{loc_id}/building/create";
        public const string LOCATION_BUILDING_DETAIL = "/location/{loc_id}/building/{id}";
        public const string LOCATION_FLOOR = "/location/{loc_id}/floor";
        public const string LOCATION_AREA = "/location/{loc_id}/area";
        public const string DEVICE = "/device";
        public const string DEVICE_CREATE = "/device/create";
        public const string DEVICE_DETAIL = "/device/{id}";
        public const string OWNER = "/owner";
        public const string OWNER_CREATE = "/owner/create";
        public const string OWNER_DETAIL = "/owner/{id}";
        public const string ACCOUNT = "/account";
        public const string ACCOUNT_CREATE = "/account/create";
        public const string ACCOUNT_DETAIL = "/account/{id}";
        public const string RES_TYPE = "/restype";
        public const string RES_TYPE_CREATE = "/restype/create";
        public const string RES_TYPE_DETAIL = "/restype/{id}";
        public const string ENTITY_CATE = "/etcate";
        public const string ENTITY_CATE_CREATE = "/etcate/create";
        public const string ENTITY_CATE_DETAIL = "/etcate/{id}";
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
        public const string LOCATION_DETAIL = "location_detail";
        public const string LOCATION_DASHBOARD = "location_dashboard";
        public const string LOCATION_RESOURCE = "location_res";
        public const string LOCATION_POST = "location_post";
        public const string LOCATION_CONFIG = "location_config";
        public const string LOCATION_SCHEDULE = "location_schedule";
        public const string LOCATION_DEVICE = "location_device";
        public const string LOCATION_BUILDING = "location_building";
        public const string LOCATION_FLOOR = "location_floor";
        public const string LOCATION_AREA = "location_area";
        public const string DEVICE = "device";
        public const string OWNER = "owner";
        public const string ACCOUNT = "account";
        public const string RES_TYPE = "restype";
        public const string ENTITY_CATE = "etcate";
    }

    public static class GeneralMessage
    {
        public const string ERROR = "Có lỗi xảy ra, liên hệ với bộ phận kỹ thuật để được hỗ trợ";
        public const string NOT_FOUND = "Không thể tìm thấy trang yêu cầu";
        public const string SOME_INP_NOT_VALID = "Một số dữ liệu gửi đi chưa phù hợp";
        public const string CREATE_SUCCESS = "Thêm mới thành công";
        public const string DELETE_SUCCESS = "Xóa thành công";
        public const string UPDATE_SUCCESS = "Cập nhật thành công";
        public const string SUCCESS = "Thành công";
        public const string R_U_SURE = "Chắc chắn?";
    }

}
