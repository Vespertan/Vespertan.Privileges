using System;
using System.Collections.Generic;
using Vespertan.Utils.Data;

namespace Vespertan.Privileges
{
    public interface IEditablePrivilegesService<TUserKey, TPrivilegeKey, TGroupKey> : IPrivilegeService<TUserKey, TPrivilegeKey, TGroupKey>
    {
        event EventHandler<TPrivilegeKey> OnPrivilegeAdd;
        event EventHandler<TPrivilegeKey> OnPrivilegeRemove;

        event EventHandler<TUserKey> OnUserAdd;
        event EventHandler<TUserKey> OnUserRemove;

        event EventHandler<TGroupKey> OnGroupAdd;
        event EventHandler<TGroupKey> OnGroupRemove;

        event EventHandler<PrivilegeRelation<TUserKey, TGroupKey>> OnPrivilegeRelationAdd;
        event EventHandler<PrivilegeRelation<TUserKey, TGroupKey>> OnPrivilegeRelationRemove;

        event EventHandler<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>> OnPrivilegeGrantAdd;
        event EventHandler<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>> OnPrivilegeGrantUpdate;
        event EventHandler<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>> OnPrivilegeGrantRemove;

        event EventHandler OnCommitSession;
        event EventHandler OnRollbackSession;
        
        void AddPrivilege(TPrivilegeKey privilegeId);
        void RemovePrivilege(TPrivilegeKey privilegeId);

        void AddUser(TUserKey userId);
        void RemoveUser(TUserKey userId);

        void AddGroup(TGroupKey groupId);
        void RemoveGroup(TGroupKey groupId);

        void AttachUserToGroup(TUserKey userId, TGroupKey groupId);
        void AttachGroupToGroup(TGroupKey subGroupId, TGroupKey parentGroupId);

        void DetachUserFromGroup(TUserKey userId, TGroupKey groupId);
        void DetachGroupFromGroup(TGroupKey subGroupId, TGroupKey parentGroupId);

        void SetGrantForUser(TPrivilegeKey privilegeId, TUserKey userId, bool? grant);
        void SetGrantForGroup(TPrivilegeKey privilegeId, TGroupKey groupId, bool? grant);

        Dictionary<TPrivilegeKey, PrivilegeState> GetPrivilegesChanges();
        Dictionary<TGroupKey, PrivilegeState> GetGroupsChanges();
        Dictionary<TUserKey, PrivilegeState> GetUsersChanges();
        Dictionary<PrivilegeRelation<TUserKey,TGroupKey>, PrivilegeState> GetRelationsChanges();
        Dictionary<PrivilegeGrant<TUserKey, TPrivilegeKey,TGroupKey>, PrivilegeState> GetPrivilegeGrantsChanges();

        void CreateSession();
        void CommitSession();
        void RollbackSession();
    }
}
