using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SK.Data
{

    public class DataConsts
    {
        public const string CONN_STR = "Server=localhost;Database=SmartKioskDev;Trusted_Connection=False;User Id=sa;Password=123456;MultipleActiveResultSets=true";
    }

    public static class EntityName
    {
        public const string POST = "Post";
        public const string POST_CONTENT = "PostContent";
    }

    public static class RoleName
    {
        public const string ADMIN = "Administrator";
    }

    public enum PostsConfigMode
    {
        [Display(Name = "Ngẫu nhiên")]
        Random = 1,
        [Display(Name = "Theo thứ tự")]
        InOrder = 2
    }

    public enum ScreenSaverPlaylistMode
    {
        [Display(Name = "Ngẫu nhiên")]
        Random = 1,
        [Display(Name = "Theo thứ tự")]
        InOrder = 2
    }

    public enum PlaylistMediaType
    {
        [Display(Name = "Video")]
        Video = 1,
        [Display(Name = "Hình ảnh")]
        Image = 2
    }

    public enum PostType
    {
        [Display(Name = "Chương trình, sự kiện")]
        ProgramEvent = 1,
        [Display(Name = "Thông báo")]
        Notification = 2,
    }

}
