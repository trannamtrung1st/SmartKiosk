using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.WebAdmin.Models
{

    public interface ILocationPageModel : IInfoPageModel
    {
        public int LocId { get; set; }
    }
}
