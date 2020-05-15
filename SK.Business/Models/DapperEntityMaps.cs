using Dapper.FluentMap.Mapping;
using SK.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class PostWithContentMap : EntityMap<PostWithContent>
    {
        public PostWithContentMap()
        {
            Map(o => o.ContentId).ToColumn(
                $"{nameof(PostContent)}.{nameof(PostContent.Id)}");
        }
    }
}
