using System;
using System.Collections.Generic;
using System.Linq;
using Vespertan.Utils.Data;

namespace Vespertan.Privileges
{
    public abstract class EditablePrivilegesServiceBase<TUserKey, TPrivilegeKey, TGroupKey>
        : PrivilegeServiceBase<TUserKey, TPrivilegeKey, TGroupKey>, IEditablePrivilegesService<TUserKey, TPrivilegeKey, TGroupKey>
    {
        private bool session = false;
        private Dictionary<TPrivilegeKey, PrivilegeState> privilegeList;
        private Dictionary<TPrivilegeKey, PrivilegeState> PrivilegeList { get => privilegeList ??= InitPrivilegeList(); }
        private Dictionary<TUserKey, PrivilegeState> userList;
        private Dictionary<TUserKey, PrivilegeState> UserList { get => userList ??= InitUserList(); }
        private Dictionary<TGroupKey, PrivilegeState> groupList;
        private Dictionary<TGroupKey, PrivilegeState> GroupList { get => groupList ??= InitGroupList(); }
        private Dictionary<PrivilegeRelation<TUserKey, TGroupKey>, PrivilegeState> privilegeRelationList;
        private Dictionary<PrivilegeRelation<TUserKey, TGroupKey>, PrivilegeState> PrivilegeRelationList { get => privilegeRelationList ??= InitPrivilegeRelationList(); }
        private Dictionary<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>, PrivilegeState> privilegeGrantList;
        private Dictionary<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>, PrivilegeState> PrivilegeGrantList { get => privilegeGrantList ??= InitPrivilegeGrantList(); }


        public event EventHandler<TPrivilegeKey> OnPrivilegeAdd;
        public event EventHandler<TPrivilegeKey> OnPrivilegeRemove;

        public event EventHandler<TUserKey> OnUserAdd;
        public event EventHandler<TUserKey> OnUserRemove;

        public event EventHandler<TGroupKey> OnGroupAdd;
        public event EventHandler<TGroupKey> OnGroupRemove;

        public event EventHandler<PrivilegeRelation<TUserKey, TGroupKey>> OnPrivilegeRelationAdd;
        public event EventHandler<PrivilegeRelation<TUserKey, TGroupKey>> OnPrivilegeRelationRemove;

        public event EventHandler<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>> OnPrivilegeGrantAdd;
        public event EventHandler<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>> OnPrivilegeGrantRemove;
        public event EventHandler<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>> OnPrivilegeGrantUpdate;

        public event EventHandler OnCommitSession;
        public event EventHandler OnRollbackSession;

        private Dictionary<TPrivilegeKey, PrivilegeState> InitPrivilegeList()
        {
            var privilegeList = GetPrivileges();
            if (privilegeList.Contains(default))
            {
                throw new NotSupportedException($"{nameof(privilegeList)}: Default values for type {nameof(TPrivilegeKey)} is not supported");
            }
            else if (privilegeList.Distinct().Count() != privilegeList.Count())
            {
                throw new NotSupportedException($"{nameof(PrivilegeList)}: Only distinct values are supported");
            }
            return privilegeList.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
        }
        private Dictionary<TUserKey, PrivilegeState> InitUserList()
        {
            var userList = GetUsers();
            if (userList.Contains(default))
            {
                throw new NotSupportedException($"{nameof(userList)}: Default values for type {nameof(TUserKey)} is not supported");
            }
            else if (userList.Distinct().Count() != userList.Count())
            {
                throw new NotSupportedException($"{nameof(userList)}: Only distinct values are supported");
            }
            return userList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
        }

        private Dictionary<TGroupKey, PrivilegeState> InitGroupList()
        {
            var groupList = GetGroups();
            if (groupList.Contains(default))
            {
                throw new NotSupportedException($"{nameof(groupList)}: Default values for type {nameof(TGroupKey)} is not supported");
            }
            else if (groupList.Distinct().Count() != groupList.Count())
            {
                throw new NotSupportedException($"{nameof(groupList)}: Only distinct values are supported");
            }
            return groupList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
        }

        private Dictionary<PrivilegeRelation<TUserKey, TGroupKey>, PrivilegeState> InitPrivilegeRelationList()
        {
            var privilegeRelationList = GetPrivilegeRelations();
            if (privilegeRelationList.Contains(default))
            {
                throw new NotSupportedException($"{nameof(privilegeRelationList)}: Default values for type {nameof(TGroupKey)} is not supported");
            }
            else if (privilegeRelationList.Distinct().Count() != privilegeRelationList.Count())
            {
                throw new NotSupportedException($"{nameof(privilegeRelationList)}: Only distinct values are supported");
            }
            return privilegeRelationList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
        }

        private Dictionary<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>, PrivilegeState> InitPrivilegeGrantList()
        {
            var privilegeGrantList = GetPrivilegeGrants();
            if (privilegeGrantList.Contains(default))
            {
                throw new NotSupportedException($"{nameof(privilegeGrantList)}: Default values for type {nameof(TGroupKey)} is not supported");
            }
            else if (privilegeGrantList.Distinct().Count() != privilegeGrantList.Count())
            {
                throw new NotSupportedException($"{nameof(privilegeGrantList)}: Only distinct values are supported");
            }
            return privilegeGrantList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
        }


        public void AddPrivilege(TPrivilegeKey privilegeId)
        {
            if (session)
            {
                if (PrivilegeList.ContainsKey(privilegeId))
                {
                    if (PrivilegeList[privilegeId] == PrivilegeState.Removed)
                    {
                        PrivilegeList[privilegeId] = PrivilegeState.Unmodifed;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Privilege with Id {privilegeId} already exists.");
                    }
                }
                else
                {
                    PrivilegeList[privilegeId] = PrivilegeState.Added;
                }
            }
            else
            {
                if (PrivilegeList.ContainsKey(privilegeId))
                {
                    throw new InvalidOperationException($"Privilege with Id {privilegeId} already exists.");
                }
                else
                {
                    PrivilegeList[privilegeId] = PrivilegeState.Unmodifed;
                    OnPrivilegeAdd?.Invoke(this, privilegeId);
                }
            }
        }

        public void RemovePrivilege(TPrivilegeKey privilegeId)
        {
            if (session)
            {
                if (PrivilegeList.ContainsKey(privilegeId))
                {
                    PrivilegeList[privilegeId] = PrivilegeState.Removed;
                }
            }
            else
            {
                if (PrivilegeList.ContainsKey(privilegeId))
                {
                    PrivilegeList.Remove(privilegeId);
                    OnPrivilegeRemove?.Invoke(this, privilegeId);
                }
            }
        }

        public void AddUser(TUserKey userId)
        {
            if (session)
            {
                if (UserList.ContainsKey(userId))
                {
                    if (UserList[userId] == PrivilegeState.Removed)
                    {
                        UserList[userId] = PrivilegeState.Unmodifed;
                    }
                    else
                    {
                        throw new InvalidOperationException($"User with Id {userId} already exists.");
                    }
                }
                else
                {
                    UserList[userId] = PrivilegeState.Added;
                }
            }
            else
            {
                if (UserList.ContainsKey(userId))
                {
                    throw new InvalidOperationException($"User with Id {userId} already exists.");
                }
                else
                {
                    UserList[userId] = PrivilegeState.Unmodifed;
                    OnUserAdd?.Invoke(this, userId);
                }
            }
        }

        public void RemoveUser(TUserKey userId)
        {
            if (session)
            {
                if (UserList.ContainsKey(userId))
                {
                    UserList[userId] = PrivilegeState.Removed;
                }
            }
            else
            {
                if (UserList.ContainsKey(userId))
                {
                    UserList.Remove(userId);
                    OnUserRemove?.Invoke(this, userId);
                }
            }
        }

        public void AddGroup(TGroupKey groupId)
        {
            if (session)
            {
                if (GroupList.ContainsKey(groupId))
                {
                    if (GroupList[groupId] == PrivilegeState.Removed)
                    {
                        GroupList[groupId] = PrivilegeState.Unmodifed;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Group with Id {groupId} already exists.");
                    }
                }
                else
                {
                    GroupList[groupId] = PrivilegeState.Added;
                }
            }
            else
            {
                if (GroupList.ContainsKey(groupId))
                {
                    throw new InvalidOperationException($"Group with Id {groupId} already exists.");
                }
                else
                {
                    GroupList[groupId] = PrivilegeState.Unmodifed;
                    OnGroupAdd?.Invoke(this, groupId);
                }
            }
        }

        public void RemoveGroup(TGroupKey groupId)
        {
            if (session)
            {
                if (GroupList.ContainsKey(groupId))
                {
                    GroupList[groupId] = PrivilegeState.Removed;
                }
            }
            else
            {
                if (GroupList.ContainsKey(groupId))
                {
                    GroupList.Remove(groupId);
                    OnGroupRemove?.Invoke(this, groupId);
                }
            }
        }

        public void AttachUserToGroup(TUserKey userId, TGroupKey groupId)
        {
            var privilegeRelation = new PrivilegeRelation<TUserKey, TGroupKey>(groupId, userId);
            if (session)
            {
                if (PrivilegeRelationList.ContainsKey(privilegeRelation))
                {
                    if (PrivilegeRelationList[privilegeRelation] == PrivilegeState.Removed)
                    {
                        PrivilegeRelationList[privilegeRelation] = PrivilegeState.Unmodifed;
                    }
                }
                else
                {
                    PrivilegeRelationList[privilegeRelation] = PrivilegeState.Added;
                }
            }
            else
            {
                if (!PrivilegeRelationList.ContainsKey(privilegeRelation))
                {
                    PrivilegeRelationList[privilegeRelation] = PrivilegeState.Unmodifed;
                    OnPrivilegeRelationAdd?.Invoke(this, privilegeRelation);
                }

            }
        }

        public void AttachGroupToGroup(TGroupKey subGroupId, TGroupKey groupId)
        {
            if (Equals(subGroupId, groupId))
            {
                throw new InvalidOperationException($"Arguments {nameof(subGroupId)} and {nameof(groupId)} cannot be equals");
            }

            if (GetImplicitDependedGroups(groupId).Contains(subGroupId)
                || GetImplicitParentGroupsForGroup(groupId).Contains(subGroupId))
            {
                throw new InvalidOperationException($"Cannot add a group {subGroupId} to a group {groupId} because it makes circular references");
            }

            var privilegeRelation = new PrivilegeRelation<TUserKey, TGroupKey>(groupId, subGroupId);
            if (session)
            {
                if (PrivilegeRelationList.ContainsKey(privilegeRelation))
                {
                    if (PrivilegeRelationList[privilegeRelation] == PrivilegeState.Removed)
                    {
                        PrivilegeRelationList[privilegeRelation] = PrivilegeState.Unmodifed;
                    }
                }
                else
                {
                    PrivilegeRelationList[privilegeRelation] = PrivilegeState.Added;
                }
            }
            else
            {
                if (!PrivilegeRelationList.ContainsKey(privilegeRelation))
                {
                    PrivilegeRelationList[privilegeRelation] = PrivilegeState.Unmodifed;
                    OnPrivilegeRelationAdd?.Invoke(this, privilegeRelation);
                }

            }
        }

        public void DetachGroupFromGroup(TGroupKey subGroupId, TGroupKey groupId)
        {
            var privilegeRelation = new PrivilegeRelation<TUserKey, TGroupKey>(groupId, subGroupId);
            if (session)
            {
                if (PrivilegeRelationList.ContainsKey(privilegeRelation))
                {
                    if (PrivilegeRelationList[privilegeRelation] == PrivilegeState.Added)
                    {
                        PrivilegeRelationList.Remove(privilegeRelation);
                    }
                }
                else
                {
                    PrivilegeRelationList[privilegeRelation] = PrivilegeState.Removed;
                }
            }
            else
            {
                if (PrivilegeRelationList.ContainsKey(privilegeRelation))
                {
                    PrivilegeRelationList.Remove(privilegeRelation);
                    OnPrivilegeRelationRemove?.Invoke(this, privilegeRelation);
                }

            }
        }

        public void DetachUserFromGroup(TUserKey userId, TGroupKey groupId)
        {
            var privilegeRelation = new PrivilegeRelation<TUserKey, TGroupKey>(groupId, userId);
            if (session)
            {
                if (PrivilegeRelationList.ContainsKey(privilegeRelation))
                {
                    if (PrivilegeRelationList[privilegeRelation] == PrivilegeState.Added)
                    {
                        PrivilegeRelationList.Remove(privilegeRelation);
                    }
                }
                else
                {
                    PrivilegeRelationList[privilegeRelation] = PrivilegeState.Removed;
                }
            }
            else
            {
                if (PrivilegeRelationList.ContainsKey(privilegeRelation))
                {
                    PrivilegeRelationList.Remove(privilegeRelation);
                    OnPrivilegeRelationRemove?.Invoke(this, privilegeRelation);
                }

            }
        }

        public void SetGrantForUser(TPrivilegeKey privilegeId, TUserKey userId, bool? grant)
        {
            if (grant.HasValue)
            {
                var privilegeGrant = new PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>(privilegeId, userId, grant.Value);
                var invertedPrivilegeGrant = new PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>(privilegeId, userId, !grant.Value);
                if (session)
                {
                    if (PrivilegeGrantList.ContainsKey(invertedPrivilegeGrant))
                    {
                        if (PrivilegeGrantList[invertedPrivilegeGrant] == PrivilegeState.Added)
                        {
                            PrivilegeGrantList.Remove(invertedPrivilegeGrant);
                        }
                        else if (PrivilegeGrantList[invertedPrivilegeGrant] == PrivilegeState.Unmodifed)
                        {
                            PrivilegeGrantList[invertedPrivilegeGrant] = PrivilegeState.Removed;
                        }
                    }

                    if (PrivilegeGrantList.ContainsKey(privilegeGrant))
                    {
                        if (PrivilegeGrantList[invertedPrivilegeGrant] == PrivilegeState.Removed)
                        {
                            PrivilegeGrantList[invertedPrivilegeGrant] = PrivilegeState.Unmodifed;
                        }
                    }
                    else
                    {
                        PrivilegeGrantList[privilegeGrant] = PrivilegeState.Added;
                    }
                }
                else
                {
                    if (PrivilegeGrantList.ContainsKey(invertedPrivilegeGrant))
                    {
                        PrivilegeGrantList.Remove(invertedPrivilegeGrant);
                        PrivilegeGrantList[privilegeGrant] = PrivilegeState.Unmodifed;
                        OnPrivilegeGrantUpdate?.Invoke(this, privilegeGrant);
                    }
                    else if (!PrivilegeGrantList.ContainsKey(privilegeGrant))
                    {
                        PrivilegeGrantList[privilegeGrant] = PrivilegeState.Unmodifed;
                        OnPrivilegeGrantAdd?.Invoke(this, privilegeGrant);
                    }
                }
            }
            else
            {
                if (session)
                {
                    foreach (var privilageStatus in PrivilegeGrantList.Keys.Where(p => Equals(p.UserId, userId) && Equals(p.PrivilegeId, privilegeId)).ToList())
                    {
                        if (PrivilegeGrantList[privilageStatus] == PrivilegeState.Added)
                        {
                            PrivilegeGrantList.Remove(privilageStatus);
                        }
                        else if (PrivilegeGrantList[privilageStatus] == PrivilegeState.Unmodifed)
                        {
                            PrivilegeGrantList[privilageStatus] = PrivilegeState.Removed;
                        }
                    }
                }
                else
                {
                    foreach (var privilageStatus in PrivilegeGrantList.Keys.Where(p => Equals(p.UserId, userId) && Equals(p.PrivilegeId, privilegeId)).ToList())
                    {
                        PrivilegeGrantList.Remove(privilageStatus);
                        OnPrivilegeGrantRemove?.Invoke(this, privilageStatus);
                    }
                }
            }
        }

        public void SetGrantForGroup(TPrivilegeKey privilegeId, TGroupKey groupId, bool? grant)
        {
            if (grant.HasValue)
            {
                var privilegeGrant = new PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>(privilegeId, groupId, grant.Value);
                var invertedPrivilegeGrant = new PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>(privilegeId, groupId, !grant.Value);
                if (session)
                {
                    if (PrivilegeGrantList.ContainsKey(invertedPrivilegeGrant))
                    {
                        if (PrivilegeGrantList[invertedPrivilegeGrant] == PrivilegeState.Added)
                        {
                            PrivilegeGrantList.Remove(invertedPrivilegeGrant);
                        }
                        else if (PrivilegeGrantList[invertedPrivilegeGrant] == PrivilegeState.Unmodifed)
                        {
                            PrivilegeGrantList[invertedPrivilegeGrant] = PrivilegeState.Removed;
                        }
                    }

                    if (PrivilegeGrantList.ContainsKey(privilegeGrant))
                    {
                        if (PrivilegeGrantList[invertedPrivilegeGrant] == PrivilegeState.Removed)
                        {
                            PrivilegeGrantList[invertedPrivilegeGrant] = PrivilegeState.Unmodifed;
                        }
                    }
                    else
                    {
                        PrivilegeGrantList[privilegeGrant] = PrivilegeState.Added;
                    }
                }
                else
                {
                    if (PrivilegeGrantList.ContainsKey(invertedPrivilegeGrant))
                    {
                        PrivilegeGrantList.Remove(invertedPrivilegeGrant);
                        PrivilegeGrantList[privilegeGrant] = PrivilegeState.Unmodifed;
                        OnPrivilegeGrantUpdate?.Invoke(this, privilegeGrant);
                    }
                    else if (!PrivilegeGrantList.ContainsKey(privilegeGrant))
                    {
                        PrivilegeGrantList[privilegeGrant] = PrivilegeState.Unmodifed;
                        OnPrivilegeGrantAdd?.Invoke(this, privilegeGrant);
                    }
                }
            }
            else
            {
                if (session)
                {
                    foreach (var privilageStatus in PrivilegeGrantList.Keys.Where(p => Equals(p.GroupId, groupId) && Equals(p.PrivilegeId, privilegeId)).ToList())
                    {
                        if (PrivilegeGrantList[privilageStatus] == PrivilegeState.Added)
                        {
                            PrivilegeGrantList.Remove(privilageStatus);
                        }
                        else if (PrivilegeGrantList[privilageStatus] == PrivilegeState.Unmodifed)
                        {
                            PrivilegeGrantList[privilageStatus] = PrivilegeState.Removed;
                        }
                    }
                }
                else
                {
                    foreach (var privilageStatus in PrivilegeGrantList.Keys.Where(p => Equals(p.GroupId, groupId) && Equals(p.PrivilegeId, privilegeId)).ToList())
                    {
                        PrivilegeGrantList.Remove(privilageStatus);
                        OnPrivilegeGrantRemove?.Invoke(this, privilageStatus);
                    }
                }
            }
        }

        public void CreateSession()
        {
            if (session)
            {
                throw new InvalidOperationException($"Cannot generate new session when last is not closed");
            }

            session = true;
        }

        public void CommitSession()
        {
            foreach (var item in GetPrivilegeGrantsChanges().Where(p => p.Value == PrivilegeState.Removed))
            {
                PrivilegeGrantList.Remove(item.Key);
                OnPrivilegeGrantRemove?.Invoke(this, item.Key);
            }

            foreach (var item in GetRelationsChanges().Where(p => p.Value == PrivilegeState.Removed))
            {
                PrivilegeRelationList.Remove(item.Key);
                OnPrivilegeRelationRemove?.Invoke(this, item.Key);
            }

            foreach (var item in GetGroupsChanges().Where(p => p.Value == PrivilegeState.Removed))
            {
                GroupList.Remove(item.Key);
                OnGroupRemove?.Invoke(this, item.Key);
            }

            foreach (var item in GetUsersChanges().Where(p => p.Value == PrivilegeState.Removed))
            {
                UserList.Remove(item.Key);
                OnUserRemove?.Invoke(this, item.Key);
            }

            foreach (var item in GetPrivilegesChanges().Where(p => p.Value == PrivilegeState.Removed))
            {
                PrivilegeList.Remove(item.Key);
                OnPrivilegeRemove?.Invoke(this, item.Key);
            }


            foreach (var item in GetPrivilegesChanges().Where(p => p.Value == PrivilegeState.Added))
            {
                PrivilegeList[item.Key] = PrivilegeState.Unmodifed;
                OnPrivilegeAdd?.Invoke(this, item.Key);
            }

            foreach (var item in GetUsersChanges().Where(p => p.Value == PrivilegeState.Added))
            {
                UserList[item.Key] = PrivilegeState.Unmodifed;
                OnUserAdd?.Invoke(this, item.Key);
            }

            foreach (var item in GetGroupsChanges().Where(p => p.Value == PrivilegeState.Added))
            {
                GroupList[item.Key] = PrivilegeState.Unmodifed;
                OnGroupAdd?.Invoke(this, item.Key);
            }

            foreach (var item in GetRelationsChanges().Where(p => p.Value == PrivilegeState.Added))
            {
                PrivilegeRelationList[item.Key] = PrivilegeState.Unmodifed;
                OnPrivilegeRelationAdd?.Invoke(this, item.Key);
            }

            foreach (var item in GetPrivilegeGrantsChanges().Where(p => p.Value == PrivilegeState.Added))
            {
                PrivilegeGrantList[item.Key] = PrivilegeState.Unmodifed;
                OnPrivilegeGrantAdd?.Invoke(this, item.Key);
            }


            foreach (var item in GetPrivilegeGrantsChanges().Where(p => p.Value == PrivilegeState.Updated))
            {
                var invertedPrivilegeGrant = new PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>()
                {
                    PrivilegeId = item.Key.PrivilegeId,
                    GroupId = item.Key.GroupId,
                    UserId = item.Key.UserId,
                    Grant = !item.Key.Grant
                };

                PrivilegeGrantList[item.Key] = PrivilegeState.Unmodifed;
                PrivilegeGrantList.Remove(invertedPrivilegeGrant);
                OnPrivilegeGrantUpdate?.Invoke(this, item.Key);
            }

            session = false;

            OnCommitSession(this, EventArgs.Empty);
        }

        public void RollbackSession()
        {
            foreach (var privilegeKeyValuePair in PrivilegeList.Where(p => p.Value != PrivilegeState.Unmodifed))
            {
                if (privilegeKeyValuePair.Value == PrivilegeState.Removed)
                {
                    PrivilegeList[privilegeKeyValuePair.Key] = PrivilegeState.Unmodifed;
                }
                else if (privilegeKeyValuePair.Value == PrivilegeState.Added)
                {
                    PrivilegeList.Remove(privilegeKeyValuePair.Key);
                }
            }

            foreach (var userKeyValuePair in UserList.Where(p => p.Value != PrivilegeState.Unmodifed))
            {
                if (userKeyValuePair.Value == PrivilegeState.Removed)
                {
                    UserList[userKeyValuePair.Key] = PrivilegeState.Unmodifed;
                }
                else if (userKeyValuePair.Value == PrivilegeState.Added)
                {
                    UserList.Remove(userKeyValuePair.Key);
                }
            }

            foreach (var groupKeyValuePair in GroupList.Where(p => p.Value != PrivilegeState.Unmodifed))
            {
                if (groupKeyValuePair.Value == PrivilegeState.Removed)
                {
                    GroupList[groupKeyValuePair.Key] = PrivilegeState.Unmodifed;
                }
                else if (groupKeyValuePair.Value == PrivilegeState.Added)
                {
                    GroupList.Remove(groupKeyValuePair.Key);
                }
            }

            foreach (var privilegeRelationKeyValuePair in PrivilegeRelationList.Where(p => p.Value != PrivilegeState.Unmodifed))
            {
                if (privilegeRelationKeyValuePair.Value == PrivilegeState.Removed)
                {
                    PrivilegeRelationList[privilegeRelationKeyValuePair.Key] = PrivilegeState.Unmodifed;
                }
                else if (privilegeRelationKeyValuePair.Value == PrivilegeState.Added)
                {
                    PrivilegeRelationList.Remove(privilegeRelationKeyValuePair.Key);
                }
            }

            foreach (var privilegeGrantKeyValuePair in PrivilegeGrantList.Where(p => p.Value != PrivilegeState.Unmodifed))
            {
                if (privilegeGrantKeyValuePair.Value == PrivilegeState.Removed)
                {
                    PrivilegeGrantList[privilegeGrantKeyValuePair.Key] = PrivilegeState.Unmodifed;
                }
                else if (privilegeGrantKeyValuePair.Value == PrivilegeState.Added)
                {
                    PrivilegeGrantList.Remove(privilegeGrantKeyValuePair.Key);
                }
            }

            session = false;
        }

        public Dictionary<TPrivilegeKey, PrivilegeState> GetPrivilegesChanges()
        {
            if (session)
            {
                return PrivilegeList.Where(p => p.Value != PrivilegeState.Unmodifed).ToDictionary(p => p.Key, p => p.Value);
            }
            else
            {
                return new Dictionary<TPrivilegeKey, PrivilegeState>();
            }
        }

        public Dictionary<TGroupKey, PrivilegeState> GetGroupsChanges()
        {
            if (session)
            {
                return GroupList.Where(p => p.Value != PrivilegeState.Unmodifed).ToDictionary(p => p.Key, p => p.Value);
            }
            else
            {
                return new Dictionary<TGroupKey, PrivilegeState>();
            }
        }

        public Dictionary<TUserKey, PrivilegeState> GetUsersChanges()
        {
            if (session)
            {
                return UserList.Where(p => p.Value != PrivilegeState.Unmodifed).ToDictionary(p => p.Key, p => p.Value);
            }
            else
            {
                return new Dictionary<TUserKey, PrivilegeState>();
            }
        }

        public Dictionary<PrivilegeRelation<TUserKey, TGroupKey>, PrivilegeState> GetRelationsChanges()
        {
            if (session)
            {
                return PrivilegeRelationList.Where(p => p.Value != PrivilegeState.Unmodifed).ToDictionary(p => p.Key, p => p.Value);
            }
            else
            {
                return new Dictionary<PrivilegeRelation<TUserKey, TGroupKey>, PrivilegeState>();
            }
        }

        public Dictionary<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>, PrivilegeState> GetPrivilegeGrantsChanges()
        {
            if (session)
            {
                var PrivilegeGrantListTemp = new Dictionary<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>, PrivilegeState>();
                var PrivilegeGrantListGrouped = PrivilegeGrantList
                    .Where(p => p.Value != PrivilegeState.Unmodifed)
                    .GroupBy(p => new { p.Key.PrivilegeId, p.Key.UserId, p.Key.GroupId });
                foreach (var privilegeGrantGr in PrivilegeGrantListGrouped)
                {
                    if (privilegeGrantGr.Count() == 1)
                    {
                        PrivilegeGrantListTemp[privilegeGrantGr.First().Key] = privilegeGrantGr.First().Value;
                    }
                    else
                    {
                        var newGrant = privilegeGrantGr.Where(p => p.Value == PrivilegeState.Added).First();
                        PrivilegeGrantListTemp[newGrant.Key] = PrivilegeState.Updated;
                    }
                }
                return PrivilegeGrantListTemp;
            }
            else
            {
                return new Dictionary<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>, PrivilegeState>();
            }
        }
    }
}
