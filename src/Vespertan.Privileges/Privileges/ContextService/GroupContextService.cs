using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vespertan.Privileges
{
    public class GroupContextService<TUserKey, TPrivilegeKey, TGroupKey> : IPrivilegeContextService<TPrivilegeKey>
    {
        private readonly IPrivilegeService<TUserKey, TPrivilegeKey, TGroupKey> privilegeService;
        private readonly TGroupKey groupId;
       
        public GroupContextService(TGroupKey groupId, IPrivilegeService<TUserKey, TPrivilegeKey, TGroupKey> privilegeService)
        {
            this.groupId = groupId;
            this.privilegeService = privilegeService;
        }
        
        public bool GetGrant(TPrivilegeKey privilegeId)
        {
            return privilegeService.GetEvaluatedGrantForGroupOrDefault(privilegeId, groupId);
        }
    }
}
