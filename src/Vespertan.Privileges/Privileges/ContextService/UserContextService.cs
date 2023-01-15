using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vespertan.Privileges
{
    public class UserContextService<TUserKey, TPrivilegeKey, TGroupKey> : IPrivilegeContextService<TPrivilegeKey>
    {
        private readonly IPrivilegeService<TUserKey, TPrivilegeKey, TGroupKey> privilegeService;
        private readonly TUserKey userId;
        
        public UserContextService(TUserKey userId, IPrivilegeService<TUserKey, TPrivilegeKey, TGroupKey> privilegeService)
        {
            this.userId = userId;
            this.privilegeService = privilegeService;
        }
       
        public bool GetGrant(TPrivilegeKey privilegeId)
        {
            return privilegeService.GetEvaluatedGrantForUserOrDefault(privilegeId, userId);
        }
    }
}
