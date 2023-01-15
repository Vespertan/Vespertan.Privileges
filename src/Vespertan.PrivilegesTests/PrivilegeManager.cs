using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vespertan.Privileges
{
    public class PrivilegeManager
    {
        public PrivilegeManager(IPrivilegeContextService<int> privilegeContextService)
        {
            PrivilegeTest = new PrivilegeTest(privilegeContextService);
        }

        //1000
        public PrivilegeTest PrivilegeTest { get; }
    }
}
