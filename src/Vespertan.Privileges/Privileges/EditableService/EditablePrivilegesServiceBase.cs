using System;
using System.Collections.Generic;
using System.Linq;
using Vespertan.Utils.Data;

namespace Vespertan.Privileges
{
    public class EditablePrivilegesServiceBase<TUserKey, TPrivilegeKey, TGroupKey>
        : PrivilegeServiceBase<TUserKey, TPrivilegeKey, TGroupKey>, IEditablePrivilegesService<TUserKey, TPrivilegeKey, TGroupKey>
    {
        public EditablePrivilegesServiceBase(IEnumerable<TPrivilegeKey> privilegeList = null, IEnumerable<TUserKey> userList = null, IEnumerable<TGroupKey> groupList = null,
            IEnumerable<PrivilegeRelation<TUserKey, TGroupKey>> privilegeRelationList = null,
            IEnumerable<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>> privilegeGrantList = null)
        {
            if (privilegeList == null)
            {
                this.privilegeList = new Dictionary<TPrivilegeKey, PrivilegeState>();
            }
            else
            {
                if (privilegeList.Contains(default))
                {
                    throw new NotSupportedException($"{nameof(privilegeList)}: Default values for type {nameof(TPrivilegeKey)} is not supported");
                }
                else if (privilegeList.Distinct().Count() != privilegeList.Count())
                {
                    throw new NotSupportedException($"{nameof(privilegeList)}: Only distinct values are supported");
                }
                this.privilegeList = privilegeList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
            }

            if (userList == null)
            {
                this.userList = new Dictionary<TUserKey, PrivilegeState>();
            }
            else
            {
                if (userList.Contains(default))
                {
                    throw new NotSupportedException($"{nameof(userList)}: Default values for type {nameof(TUserKey)} is not supported");
                }
                else if (userList.Distinct().Count() != userList.Count())
                {
                    throw new NotSupportedException($"{nameof(userList)}: Only distinct values are supported");
                }
                this.userList = userList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
            }

            if (groupList == null)
            {
                this.groupList = new Dictionary<TGroupKey, PrivilegeState>();
            }
            else
            {
                if (groupList.Contains(default))
                {
                    throw new NotSupportedException($"{nameof(groupList)}: Default values for type {nameof(TGroupKey)} is not supported");
                }
                else if (groupList.Distinct().Count() != groupList.Count())
                {
                    throw new NotSupportedException($"{nameof(groupList)}: Only distinct values are supported");
                }
                this.groupList = groupList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
            }

            if (privilegeRelationList == null)
            {
                this.privilegeRelationList = new Dictionary<PrivilegeRelation<TUserKey, TGroupKey>, PrivilegeState>();
            }
            else
            {
                if (privilegeRelationList.Contains(default))
                {
                    throw new NotSupportedException($"{nameof(privilegeRelationList)}: Default values for type {nameof(TGroupKey)} is not supported");
                }
                else if (privilegeRelationList.Distinct().Count() != privilegeRelationList.Count())
                {
                    throw new NotSupportedException($"{nameof(privilegeRelationList)}: Only distinct values are supported");
                }
                this.privilegeRelationList = privilegeRelationList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
            }

            if (privilegeGrantList == null)
            {
                this.privilegeGrantList = new Dictionary<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>, PrivilegeState>();
            }
            else
            {
                if (privilegeGrantList.Contains(default))
                {
                    throw new NotSupportedException($"{nameof(privilegeGrantList)}: Default values for type {nameof(TGroupKey)} is not supported");
                }
                else if (privilegeGrantList.Distinct().Count() != privilegeGrantList.Count())
                {
                    throw new NotSupportedException($"{nameof(privilegeGrantList)}: Only distinct values are supported");
                }
                this.privilegeGrantList = privilegeGrantList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
            }
        }

        private bool session = false;
        private Dictionary<TPrivilegeKey, PrivilegeState> privilegeList;
        private Dictionary<TUserKey, PrivilegeState> userList;
        private Dictionary<TGroupKey, PrivilegeState> groupList;
        private Dictionary<PrivilegeRelation<TUserKey, TGroupKey>, PrivilegeState> privilegeRelationList;
        private Dictionary<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>, PrivilegeState> privilegeGrantList;

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

        public void Reset()
        {
            var privilegeList = GetPrivileges();
            var userList = GetUsers();
            var groupList = GetGroups();
            var privilegeRelationList = GetPrivilegeRelations();
            var privilegeGrantList = GetPrivilegeGrants();

            if (privilegeList == null)
            {
                this.privilegeList = new Dictionary<TPrivilegeKey, PrivilegeState>();
            }
            else
            {
                if (privilegeList.Contains(default))
                {
                    throw new NotSupportedException($"{nameof(privilegeList)}: Default values for type {nameof(TPrivilegeKey)} is not supported");
                }
                else if (privilegeList.Distinct().Count() != privilegeList.Count())
                {
                    throw new NotSupportedException($"{nameof(privilegeList)}: Only distinct values are supported");
                }
                this.privilegeList = privilegeList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
            }

            if (userList == null)
            {
                this.userList = new Dictionary<TUserKey, PrivilegeState>();
            }
            else
            {
                if (userList.Contains(default))
                {
                    throw new NotSupportedException($"{nameof(userList)}: Default values for type {nameof(TUserKey)} is not supported");
                }
                else if (userList.Distinct().Count() != userList.Count())
                {
                    throw new NotSupportedException($"{nameof(userList)}: Only distinct values are supported");
                }
                this.userList = userList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
            }

            if (groupList == null)
            {
                this.groupList = new Dictionary<TGroupKey, PrivilegeState>();
            }
            else
            {
                if (groupList.Contains(default))
                {
                    throw new NotSupportedException($"{nameof(groupList)}: Default values for type {nameof(TGroupKey)} is not supported");
                }
                else if (groupList.Distinct().Count() != groupList.Count())
                {
                    throw new NotSupportedException($"{nameof(groupList)}: Only distinct values are supported");
                }
                this.groupList = groupList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
            }

            if (privilegeRelationList == null)
            {
                this.privilegeRelationList = new Dictionary<PrivilegeRelation<TUserKey, TGroupKey>, PrivilegeState>();
            }
            else
            {
                if (privilegeRelationList.Contains(default))
                {
                    throw new NotSupportedException($"{nameof(privilegeRelationList)}: Default values for type {nameof(TGroupKey)} is not supported");
                }
                else if (privilegeRelationList.Distinct().Count() != privilegeRelationList.Count())
                {
                    throw new NotSupportedException($"{nameof(privilegeRelationList)}: Only distinct values are supported");
                }
                this.privilegeRelationList = privilegeRelationList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
            }

            if (privilegeGrantList == null)
            {
                this.privilegeGrantList = new Dictionary<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>, PrivilegeState>();
            }
            else
            {
                if (privilegeGrantList.Contains(default))
                {
                    throw new NotSupportedException($"{nameof(privilegeGrantList)}: Default values for type {nameof(TGroupKey)} is not supported");
                }
                else if (privilegeGrantList.Distinct().Count() != privilegeGrantList.Count())
                {
                    throw new NotSupportedException($"{nameof(privilegeGrantList)}: Only distinct values are supported");
                }
                this.privilegeGrantList = privilegeGrantList?.ToDictionary(p => p, p => PrivilegeState.Unmodifed);
            }
        }

        public void AddPrivilege(TPrivilegeKey privilegeId)
        {
            if (session)
            {
                if (privilegeList.ContainsKey(privilegeId))
                {
                    if (privilegeList[privilegeId] == PrivilegeState.Removed)
                    {
                        privilegeList[privilegeId] = PrivilegeState.Unmodifed;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Privilege with Id {privilegeId} already exists.");
                    }
                }
                else
                {
                    privilegeList[privilegeId] = PrivilegeState.Added;
                }
            }
            else
            {
                if (privilegeList.ContainsKey(privilegeId))
                {
                    throw new InvalidOperationException($"Privilege with Id {privilegeId} already exists.");
                }
                else
                {
                    privilegeList[privilegeId] = PrivilegeState.Unmodifed;
                    OnPrivilegeAdd?.Invoke(this, privilegeId);
                }
            }
        }

        public void RemovePrivilege(TPrivilegeKey privilegeId)
        {
            if (session)
            {
                if (privilegeList.ContainsKey(privilegeId))
                {
                    privilegeList[privilegeId] = PrivilegeState.Removed;
                }
            }
            else
            {
                if (privilegeList.ContainsKey(privilegeId))
                {
                    privilegeList.Remove(privilegeId);
                    OnPrivilegeRemove?.Invoke(this, privilegeId);
                }
            }
        }

        public void AddUser(TUserKey userId)
        {
            if (session)
            {
                if (userList.ContainsKey(userId))
                {
                    if (userList[userId] == PrivilegeState.Removed)
                    {
                        userList[userId] = PrivilegeState.Unmodifed;
                    }
                    else
                    {
                        throw new InvalidOperationException($"User with Id {userId} already exists.");
                    }
                }
                else
                {
                    userList[userId] = PrivilegeState.Added;
                }
            }
            else
            {
                if (userList.ContainsKey(userId))
                {
                    throw new InvalidOperationException($"User with Id {userId} already exists.");
                }
                else
                {
                    userList[userId] = PrivilegeState.Unmodifed;
                    OnUserAdd?.Invoke(this, userId);
                }
            }
        }

        public void RemoveUser(TUserKey userId)
        {
            if (session)
            {
                if (userList.ContainsKey(userId))
                {
                    userList[userId] = PrivilegeState.Removed;
                }
            }
            else
            {
                if (userList.ContainsKey(userId))
                {
                    userList.Remove(userId);
                    OnUserRemove?.Invoke(this, userId);
                }
            }
        }

        public void AddGroup(TGroupKey groupId)
        {
            if (session)
            {
                if (groupList.ContainsKey(groupId))
                {
                    if (groupList[groupId] == PrivilegeState.Removed)
                    {
                        groupList[groupId] = PrivilegeState.Unmodifed;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Group with Id {groupId} already exists.");
                    }
                }
                else
                {
                    groupList[groupId] = PrivilegeState.Added;
                }
            }
            else
            {
                if (groupList.ContainsKey(groupId))
                {
                    throw new InvalidOperationException($"Group with Id {groupId} already exists.");
                }
                else
                {
                    groupList[groupId] = PrivilegeState.Unmodifed;
                    OnGroupAdd?.Invoke(this, groupId);
                }
            }
        }

        public void RemoveGroup(TGroupKey groupId)
        {
            if (session)
            {
                if (groupList.ContainsKey(groupId))
                {
                    groupList[groupId] = PrivilegeState.Removed;
                }
            }
            else
            {
                if (groupList.ContainsKey(groupId))
                {
                    groupList.Remove(groupId);
                    OnGroupRemove?.Invoke(this, groupId);
                }
            }
        }

        public void AttachUserToGroup(TUserKey userId, TGroupKey groupId)
        {
            var privilegeRelation = new PrivilegeRelation<TUserKey, TGroupKey>(groupId, userId);
            if (session)
            {
                if (privilegeRelationList.ContainsKey(privilegeRelation))
                {
                    if (privilegeRelationList[privilegeRelation] == PrivilegeState.Removed)
                    {
                        privilegeRelationList[privilegeRelation] = PrivilegeState.Unmodifed;
                    }
                }
                else
                {
                    privilegeRelationList[privilegeRelation] = PrivilegeState.Added;
                }
            }
            else
            {
                if (!privilegeRelationList.ContainsKey(privilegeRelation))
                {
                    privilegeRelationList[privilegeRelation] = PrivilegeState.Unmodifed;
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
                if (privilegeRelationList.ContainsKey(privilegeRelation))
                {
                    if (privilegeRelationList[privilegeRelation] == PrivilegeState.Removed)
                    {
                        privilegeRelationList[privilegeRelation] = PrivilegeState.Unmodifed;
                    }
                }
                else
                {
                    privilegeRelationList[privilegeRelation] = PrivilegeState.Added;
                }
            }
            else
            {
                if (!privilegeRelationList.ContainsKey(privilegeRelation))
                {
                    privilegeRelationList[privilegeRelation] = PrivilegeState.Unmodifed;
                    OnPrivilegeRelationAdd?.Invoke(this, privilegeRelation);
                }

            }
        }

        public void DetachGroupFromGroup(TGroupKey subGroupId, TGroupKey groupId)
        {
            var privilegeRelation = new PrivilegeRelation<TUserKey, TGroupKey>(groupId, subGroupId);
            if (session)
            {
                if (privilegeRelationList.ContainsKey(privilegeRelation))
                {
                    if (privilegeRelationList[privilegeRelation] == PrivilegeState.Added)
                    {
                        privilegeRelationList.Remove(privilegeRelation);
                    }
                }
                else
                {
                    privilegeRelationList[privilegeRelation] = PrivilegeState.Removed;
                }
            }
            else
            {
                if (privilegeRelationList.ContainsKey(privilegeRelation))
                {
                    privilegeRelationList.Remove(privilegeRelation);
                    OnPrivilegeRelationRemove?.Invoke(this, privilegeRelation);
                }

            }
        }

        public void DetachUserFromGroup(TUserKey userId, TGroupKey groupId)
        {
            var privilegeRelation = new PrivilegeRelation<TUserKey, TGroupKey>(groupId, userId);
            if (session)
            {
                if (privilegeRelationList.ContainsKey(privilegeRelation))
                {
                    if (privilegeRelationList[privilegeRelation] == PrivilegeState.Added)
                    {
                        privilegeRelationList.Remove(privilegeRelation);
                    }
                }
                else
                {
                    privilegeRelationList[privilegeRelation] = PrivilegeState.Removed;
                }
            }
            else
            {
                if (privilegeRelationList.ContainsKey(privilegeRelation))
                {
                    privilegeRelationList.Remove(privilegeRelation);
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
                    if (privilegeGrantList.ContainsKey(invertedPrivilegeGrant))
                    {
                        if (privilegeGrantList[invertedPrivilegeGrant] == PrivilegeState.Added)
                        {
                            privilegeGrantList.Remove(invertedPrivilegeGrant);
                        }
                        else if (privilegeGrantList[invertedPrivilegeGrant] == PrivilegeState.Unmodifed)
                        {
                            privilegeGrantList[invertedPrivilegeGrant] = PrivilegeState.Removed;
                        }
                    }

                    if (privilegeGrantList.ContainsKey(privilegeGrant))
                    {
                        if (privilegeGrantList[invertedPrivilegeGrant] == PrivilegeState.Removed)
                        {
                            privilegeGrantList[invertedPrivilegeGrant] = PrivilegeState.Unmodifed;
                        }
                    }
                    else
                    {
                        privilegeGrantList[privilegeGrant] = PrivilegeState.Added;
                    }
                }
                else
                {
                    if (privilegeGrantList.ContainsKey(invertedPrivilegeGrant))
                    {
                        privilegeGrantList.Remove(invertedPrivilegeGrant);
                        privilegeGrantList[privilegeGrant] = PrivilegeState.Unmodifed;
                        OnPrivilegeGrantUpdate?.Invoke(this, privilegeGrant);
                    }
                    else if (!privilegeGrantList.ContainsKey(privilegeGrant))
                    {
                        privilegeGrantList[privilegeGrant] = PrivilegeState.Unmodifed;
                        OnPrivilegeGrantAdd?.Invoke(this, privilegeGrant);
                    }
                }
            }
            else
            {
                if (session)
                {
                    foreach (var privilageStatus in privilegeGrantList.Keys.Where(p => Equals(p.UserId, userId) && Equals(p.PrivilegeId, privilegeId)).ToList())
                    {
                        if (privilegeGrantList[privilageStatus] == PrivilegeState.Added)
                        {
                            privilegeGrantList.Remove(privilageStatus);
                        }
                        else if (privilegeGrantList[privilageStatus] == PrivilegeState.Unmodifed)
                        {
                            privilegeGrantList[privilageStatus] = PrivilegeState.Removed;
                        }
                    }
                }
                else
                {
                    foreach (var privilageStatus in privilegeGrantList.Keys.Where(p => Equals(p.UserId, userId) && Equals(p.PrivilegeId, privilegeId)).ToList())
                    {
                        privilegeGrantList.Remove(privilageStatus);
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
                    if (privilegeGrantList.ContainsKey(invertedPrivilegeGrant))
                    {
                        if (privilegeGrantList[invertedPrivilegeGrant] == PrivilegeState.Added)
                        {
                            privilegeGrantList.Remove(invertedPrivilegeGrant);
                        }
                        else if (privilegeGrantList[invertedPrivilegeGrant] == PrivilegeState.Unmodifed)
                        {
                            privilegeGrantList[invertedPrivilegeGrant] = PrivilegeState.Removed;
                        }
                    }

                    if (privilegeGrantList.ContainsKey(privilegeGrant))
                    {
                        if (privilegeGrantList[invertedPrivilegeGrant] == PrivilegeState.Removed)
                        {
                            privilegeGrantList[invertedPrivilegeGrant] = PrivilegeState.Unmodifed;
                        }
                    }
                    else
                    {
                        privilegeGrantList[privilegeGrant] = PrivilegeState.Added;
                    }
                }
                else
                {
                    if (privilegeGrantList.ContainsKey(invertedPrivilegeGrant))
                    {
                        privilegeGrantList.Remove(invertedPrivilegeGrant);
                        privilegeGrantList[privilegeGrant] = PrivilegeState.Unmodifed;
                        OnPrivilegeGrantUpdate?.Invoke(this, privilegeGrant);
                    }
                    else if (!privilegeGrantList.ContainsKey(privilegeGrant))
                    {
                        privilegeGrantList[privilegeGrant] = PrivilegeState.Unmodifed;
                        OnPrivilegeGrantAdd?.Invoke(this, privilegeGrant);
                    }
                }
            }
            else
            {
                if (session)
                {
                    foreach (var privilageStatus in privilegeGrantList.Keys.Where(p => Equals(p.GroupId, groupId) && Equals(p.PrivilegeId, privilegeId)).ToList())
                    {
                        if (privilegeGrantList[privilageStatus] == PrivilegeState.Added)
                        {
                            privilegeGrantList.Remove(privilageStatus);
                        }
                        else if (privilegeGrantList[privilageStatus] == PrivilegeState.Unmodifed)
                        {
                            privilegeGrantList[privilageStatus] = PrivilegeState.Removed;
                        }
                    }
                }
                else
                {
                    foreach (var privilageStatus in privilegeGrantList.Keys.Where(p => Equals(p.GroupId, groupId) && Equals(p.PrivilegeId, privilegeId)).ToList())
                    {
                        privilegeGrantList.Remove(privilageStatus);
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
                privilegeGrantList.Remove(item.Key);
                OnPrivilegeGrantRemove?.Invoke(this, item.Key);
            }

            foreach (var item in GetRelationsChanges().Where(p => p.Value == PrivilegeState.Removed))
            {
                privilegeRelationList.Remove(item.Key);
                OnPrivilegeRelationRemove?.Invoke(this, item.Key);
            }

            foreach (var item in GetGroupsChanges().Where(p => p.Value == PrivilegeState.Removed))
            {
                groupList.Remove(item.Key);
                OnGroupRemove?.Invoke(this, item.Key);
            }

            foreach (var item in GetUsersChanges().Where(p => p.Value == PrivilegeState.Removed))
            {
                userList.Remove(item.Key);
                OnUserRemove?.Invoke(this, item.Key);
            }

            foreach (var item in GetPrivilegesChanges().Where(p => p.Value == PrivilegeState.Removed))
            {
                privilegeList.Remove(item.Key);
                OnPrivilegeRemove?.Invoke(this, item.Key);
            }


            foreach (var item in GetPrivilegesChanges().Where(p => p.Value == PrivilegeState.Added))
            {
                privilegeList[item.Key] = PrivilegeState.Unmodifed;
                OnPrivilegeAdd?.Invoke(this, item.Key);
            }

            foreach (var item in GetUsersChanges().Where(p => p.Value == PrivilegeState.Added))
            {
                userList[item.Key] = PrivilegeState.Unmodifed;
                OnUserAdd?.Invoke(this, item.Key);
            }

            foreach (var item in GetGroupsChanges().Where(p => p.Value == PrivilegeState.Added))
            {
                groupList[item.Key] = PrivilegeState.Unmodifed;
                OnGroupAdd?.Invoke(this, item.Key);
            }

            foreach (var item in GetRelationsChanges().Where(p => p.Value == PrivilegeState.Added))
            {
                privilegeRelationList[item.Key] = PrivilegeState.Unmodifed;
                OnPrivilegeRelationAdd?.Invoke(this, item.Key);
            }

            foreach (var item in GetPrivilegeGrantsChanges().Where(p => p.Value == PrivilegeState.Added))
            {
                privilegeGrantList[item.Key] = PrivilegeState.Unmodifed;
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

                privilegeGrantList[item.Key] = PrivilegeState.Unmodifed;
                privilegeGrantList.Remove(invertedPrivilegeGrant);
                OnPrivilegeGrantUpdate?.Invoke(this, item.Key);
            }

            session = false;

            OnCommitSession(this, EventArgs.Empty);
        }

        public void RollbackSession()
        {
            foreach (var privilegeKeyValuePair in privilegeList.Where(p => p.Value != PrivilegeState.Unmodifed))
            {
                if (privilegeKeyValuePair.Value == PrivilegeState.Removed)
                {
                    privilegeList[privilegeKeyValuePair.Key] = PrivilegeState.Unmodifed;
                }
                else if (privilegeKeyValuePair.Value == PrivilegeState.Added)
                {
                    privilegeList.Remove(privilegeKeyValuePair.Key);
                }
            }

            foreach (var userKeyValuePair in userList.Where(p => p.Value != PrivilegeState.Unmodifed))
            {
                if (userKeyValuePair.Value == PrivilegeState.Removed)
                {
                    userList[userKeyValuePair.Key] = PrivilegeState.Unmodifed;
                }
                else if (userKeyValuePair.Value == PrivilegeState.Added)
                {
                    userList.Remove(userKeyValuePair.Key);
                }
            }

            foreach (var groupKeyValuePair in groupList.Where(p => p.Value != PrivilegeState.Unmodifed))
            {
                if (groupKeyValuePair.Value == PrivilegeState.Removed)
                {
                    groupList[groupKeyValuePair.Key] = PrivilegeState.Unmodifed;
                }
                else if (groupKeyValuePair.Value == PrivilegeState.Added)
                {
                    groupList.Remove(groupKeyValuePair.Key);
                }
            }

            foreach (var privilegeRelationKeyValuePair in privilegeRelationList.Where(p => p.Value != PrivilegeState.Unmodifed))
            {
                if (privilegeRelationKeyValuePair.Value == PrivilegeState.Removed)
                {
                    privilegeRelationList[privilegeRelationKeyValuePair.Key] = PrivilegeState.Unmodifed;
                }
                else if (privilegeRelationKeyValuePair.Value == PrivilegeState.Added)
                {
                    privilegeRelationList.Remove(privilegeRelationKeyValuePair.Key);
                }
            }

            foreach (var privilegeGrantKeyValuePair in privilegeGrantList.Where(p => p.Value != PrivilegeState.Unmodifed))
            {
                if (privilegeGrantKeyValuePair.Value == PrivilegeState.Removed)
                {
                    privilegeGrantList[privilegeGrantKeyValuePair.Key] = PrivilegeState.Unmodifed;
                }
                else if (privilegeGrantKeyValuePair.Value == PrivilegeState.Added)
                {
                    privilegeGrantList.Remove(privilegeGrantKeyValuePair.Key);
                }
            }

            session = false;
        }

        public Dictionary<TPrivilegeKey, PrivilegeState> GetPrivilegesChanges()
        {
            if (session)
            {
                return privilegeList.Where(p => p.Value != PrivilegeState.Unmodifed).ToDictionary(p => p.Key, p => p.Value);
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
                return groupList.Where(p => p.Value != PrivilegeState.Unmodifed).ToDictionary(p => p.Key, p => p.Value);
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
                return userList.Where(p => p.Value != PrivilegeState.Unmodifed).ToDictionary(p => p.Key, p => p.Value);
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
                return privilegeRelationList.Where(p => p.Value != PrivilegeState.Unmodifed).ToDictionary(p => p.Key, p => p.Value);
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
                var privilegeGrantListTemp = new Dictionary<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>, PrivilegeState>();
                var privilegeGrantListGrouped = privilegeGrantList
                    .Where(p => p.Value != PrivilegeState.Unmodifed)
                    .GroupBy(p => new { p.Key.PrivilegeId, p.Key.UserId, p.Key.GroupId });
                foreach (var privilegeGrantGr in privilegeGrantListGrouped)
                {
                    if (privilegeGrantGr.Count() == 1)
                    {
                        privilegeGrantListTemp[privilegeGrantGr.First().Key] = privilegeGrantGr.First().Value;
                    }
                    else
                    {
                        var newGrant = privilegeGrantGr.Where(p => p.Value == PrivilegeState.Added).First();
                        privilegeGrantListTemp[newGrant.Key] = PrivilegeState.Updated;
                    }
                }
                return privilegeGrantListTemp;
            }
            else
            {
                return new Dictionary<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>, PrivilegeState>();
            }
        }

        public override IEnumerable<TPrivilegeKey> GetPrivileges()
        {
            return privilegeList
                .Where(p => p.Value != PrivilegeState.Removed)
                .Select(p => p.Key);
        }

        public override IEnumerable<TUserKey> GetUsers()
        {
            return userList
                .Where(p => p.Value != PrivilegeState.Removed)
                .Select(p => p.Key);
        }

        public override IEnumerable<TGroupKey> GetGroups()
        {
            return groupList
                .Where(p => p.Value != PrivilegeState.Removed)
                .Select(p => p.Key);
        }

        public override IEnumerable<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>> GetPrivilegeGrants()
        {
            return privilegeGrantList
                .Where(p => p.Value != PrivilegeState.Removed)
                .Select(p => p.Key);
        }

        public override IEnumerable<PrivilegeRelation<TUserKey, TGroupKey>> GetPrivilegeRelations()
        {
            return privilegeRelationList
                .Where(p => p.Value != PrivilegeState.Removed)
                .Select(p => p.Key);
        }
    }
}
