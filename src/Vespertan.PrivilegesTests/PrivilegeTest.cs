using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vespertan.Privileges
{

    public class PrivilegeTest : PrivilegeBase<int>
    {
        public PrivilegeTest(IPrivilegeContextService<int> privilegeContextService)
            : base(privilegeContextService)
        {}

        public bool SaveData => GetGrant(1);
        public bool SaveData2 => GetGrantByName();
    }
}
