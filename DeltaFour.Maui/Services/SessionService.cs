using DeltaFour.Maui.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaFour.Maui.Services
{
    public interface ISession
    {
        LocalUser? CurrentUser { get; set; }
        bool IsAuthenticated { get; set; }
    }
    public sealed class Session : ISession
     {
       public LocalUser? CurrentUser { get; set; }
       public bool IsAuthenticated { get; set; }
     }
   
}
