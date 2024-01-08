using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        public bool GetGrant([CallerMemberName] string memberName = null)
        {
            var privilegeId = GetType().GetMember(memberName)?.First().GetCustomAttribute<PrivilegeInfoAttribute>()?.Id;
            if (privilegeId == null || privilegeId is not TPrivilegeKey)
            {
                throw new InvalidOperationException($"Member {memberName} dosen't contains attribute PrivilegeInfo with set filed Id of type {typeof(TPrivilegeKey)}");
            }
            return privilegeService.GetEvaluatedGrantForUserOrDefault((TPrivilegeKey)privilegeId, userId);
        }
    }
}
