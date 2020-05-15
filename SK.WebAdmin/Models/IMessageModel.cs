using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.WebAdmin.Models
{
    public interface IMessageModel : IReturnViewModel
    {
        string Message { get; set; }
        string MessageTitle { get; set; }
    }
}
