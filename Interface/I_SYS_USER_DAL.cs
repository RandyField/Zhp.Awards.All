using Common.Enum;
using EFModel;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  Interface
{
    public interface I_SYS_USER_DAL : I_BASE_Interface<SYS_USER_MODEL>
    {
        LoginStatus Login(string loginName, string password, bool isSSO, ref string message);

        void Logout();
    }
}
