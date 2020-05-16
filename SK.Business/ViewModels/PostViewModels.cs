using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.ViewModels
{

    public class PostWithContentViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedTimeDisplay { get; set; }
        public int ContentId { get; set; }
        public string ImageUrl { get; set; }
        public string Lang { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
