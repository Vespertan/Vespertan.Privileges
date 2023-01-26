using System.Collections.Generic;

namespace Vespertan.Privileges
{
    public interface IPrivilegeService<TUserKey, TPrivilegeKey, TGroupKey>
    {
        IEnumerable<TPrivilegeKey> GetPrivileges();
        
        IEnumerable<TUserKey> GetUsers();

        IEnumerable<TGroupKey> GetGroups();

        IEnumerable<PrivilegeRelation<TUserKey, TGroupKey>> GetPrivilegeRelations();
        
        IEnumerable<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>> GetPrivilegeGrants();

        bool DefaultGrantValue { get; }

        IEnumerable<TGroupKey> GetExplicitDependedGroups(TGroupKey groupId);
        IEnumerable<TUserKey> GetExplicitDependedUsers(TGroupKey groupId);
        IEnumerable<TGroupKey> GetImplicitDependedGroups(TGroupKey groupId);
        IEnumerable<TUserKey> GetImplicitDependedUsers(TGroupKey groupId);
        IEnumerable<TGroupKey> GetImplicitParentGroupsForUser(TUserKey userId);
        IEnumerable<TGroupKey> GetImplicitParentGroupsForGroup(TGroupKey groupId);
        IEnumerable<TGroupKey> GetExplicitParentGroupsForUser(TUserKey userId);
        IEnumerable<TGroupKey> GetExplicitParentGroupsForGroup(TGroupKey groupId);

        bool? GetGrantForGroup(TPrivilegeKey privilegeId, TGroupKey groupId);
        bool? GetEvaluatedGrantForGroup(TPrivilegeKey privilegeId, TGroupKey groupId);
        bool GetEvaluatedGrantForGroupOrDefault(TPrivilegeKey privilegeId, TGroupKey groupId);

        bool? GetGrantForUser(TPrivilegeKey privilegeId, TUserKey userId);
        bool? GetEvaluatedGrantForUser(TPrivilegeKey privilegeId, TUserKey userId);
        bool GetEvaluatedGrantForUserOrDefault(TPrivilegeKey privilegeId, TUserKey userId);

        IPrivilegeContextService<TPrivilegeKey> GetUserContextService(TUserKey userId);
        IPrivilegeContextService<TPrivilegeKey> GetGroupContextService(TGroupKey groupId);

        IEnumerable<TPrivilegeKey> GetUserPrivileges(TUserKey userId);
        IEnumerable<TPrivilegeKey> GetGroupPrivileges(TGroupKey gruopId);
    }
}
