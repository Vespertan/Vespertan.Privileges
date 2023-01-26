using System;
using System.Collections.Generic;
using System.Linq;

namespace Vespertan.Privileges
{
    public abstract class PrivilegeServiceBase<TUserKey, TPrivilegeKey, TGroupKey> : IPrivilegeService<TUserKey, TPrivilegeKey, TGroupKey>
    {
        public PrivilegeServiceBase()
        {

        }

        public PrivilegeServiceBase(bool defaultGrantValue)
        {
            DefaultGrantValue = defaultGrantValue;
        }
       
        public bool DefaultGrantValue { get; protected set; } = false;
        public object Context { get; protected set; }

        public abstract IEnumerable<TPrivilegeKey> GetPrivileges();

        public abstract IEnumerable<TUserKey> GetUsers();

        public abstract IEnumerable<TGroupKey> GetGroups();

        public abstract IEnumerable<PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>> GetPrivilegeGrants();

        public abstract IEnumerable<PrivilegeRelation<TUserKey, TGroupKey>> GetPrivilegeRelations();

        #region GetExplicit...

        public IEnumerable<TGroupKey> GetExplicitDependedGroups(TGroupKey groupId)
        {
            var dependents = new List<TGroupKey>();

            var relations = GetPrivilegeRelations()
                .Where(p => !Equals(p.SubGroupId, default(TGroupKey)))
                .ToList();

            var groupsToCheck = relations
                .Where(p => Equals(p.GroupId, groupId))
                .Select(p => p.SubGroupId)
                .ToList();

            while (groupsToCheck.Count == 0)
            {
                TGroupKey id = groupsToCheck[0];
                dependents.Add(id);
                groupsToCheck.Remove(id);
                groupsToCheck.AddRange(relations.Where(p => Equals(p.GroupId, id)).Select(p => p.SubGroupId));
            }

            return dependents;
        }

        public IEnumerable<TUserKey> GetExplicitDependedUsers(TGroupKey groupId)
        {
            return GetPrivilegeRelations()
                .Where(p => !Equals(p.UserId, default(TUserKey)))
                .Where(p => Equals(p.GroupId, groupId))
                .Select(p => p.UserId)
                .ToList();
        }

        public IEnumerable<TGroupKey> GetExplicitParentGroupsForUser(TUserKey userId)
        {
            return GetPrivilegeRelations()
                .Where(p => Equals(p.UserId, userId))
                .Select(p => p.GroupId);
        }

        public IEnumerable<TGroupKey> GetExplicitParentGroupsForGroup(TGroupKey groupId)
        {
            return GetPrivilegeRelations()
                .Where(p => Equals(p.SubGroupId, groupId))
                .Select(p => p.GroupId);
        }

        #endregion

        #region GetImplicit...

        public IEnumerable<TGroupKey> GetImplicitDependedGroups(TGroupKey groupId)
        {
            var implicitGroups = new List<TGroupKey>();
            foreach (var gr in GetExplicitDependedGroups(groupId))
            {
                if (!implicitGroups.Contains(gr))
                {
                    implicitGroups.AddRange(GetExplicitDependedGroups(gr));
                    implicitGroups.Add(gr);
                }
            }
            return implicitGroups;
        }

        public IEnumerable<TUserKey> GetImplicitDependedUsers(TGroupKey groupId)
        {
            var implicitUsers = new List<TUserKey>();
            foreach (var gr in GetImplicitDependedGroups(groupId))
            {
                implicitUsers.AddRange(GetExplicitDependedUsers(gr));
            }
            return implicitUsers.Distinct();
        }

        public IEnumerable<TGroupKey> GetImplicitParentGroupsForUser(TUserKey userId)
        {
            var implicitGroups = new List<TGroupKey>();
            foreach (var gr in GetExplicitParentGroupsForUser(userId))
            {
                if (!implicitGroups.Contains(gr))
                {
                    implicitGroups.AddRange(GetImplicitParentGroupsForGroup(gr));
                    implicitGroups.Add(gr);
                }
            }
            return implicitGroups;
        }

        public IEnumerable<TGroupKey> GetImplicitParentGroupsForGroup(TGroupKey groupId)
        {
            var implicitGroups = new List<TGroupKey>();
            foreach (var gr in GetExplicitParentGroupsForGroup(groupId))
            {
                if (!implicitGroups.Contains(gr))
                {
                    implicitGroups.AddRange(GetExplicitParentGroupsForGroup(gr));
                    implicitGroups.Add(gr);
                }
            }
            return implicitGroups;
        }

        #endregion

        public bool? GetGrantForGroup(TPrivilegeKey privilegeId, TGroupKey groupId)
        {
            var privilegeStatus = GetPrivilegeGrants()
                .Where(p => Equals(p.PrivilegeId, privilegeId) && Equals(p.GroupId, groupId))
                .FirstOrDefault();
            return Equals(privilegeStatus, default(PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>)) ? (bool?)null : privilegeStatus.Grant;
        }

        public bool? GetGrantForUser(TPrivilegeKey privilegeId, TUserKey userId)
        {
            var privilegeStatus = GetPrivilegeGrants()
                .Where(p => Equals(p.PrivilegeId, privilegeId) && Equals(p.UserId, userId))
                .FirstOrDefault();
            return Equals(privilegeStatus, default(PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>)) ? (bool?)null : privilegeStatus.Grant;
        }

        public bool? GetEvaluatedGrantForGroup(TPrivilegeKey privilegeId, TGroupKey groupId)
        {
            bool? result = GetGrantForGroup(privilegeId, groupId);
            if (result == false)
            {
                return result.Value;
            }
            else
            {
                foreach (var grId in GetImplicitParentGroupsForGroup(groupId))
                {
                    var tempResult = GetGrantForGroup(privilegeId, grId);
                    if (tempResult.HasValue)
                    {
                        if (tempResult == false)
                        {
                            return false;
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
                return result;
            }
        }

        public bool? GetEvaluatedGrantForUser(TPrivilegeKey privilegeId, TUserKey userId)
        {
            bool? result = GetGrantForUser(privilegeId, userId);
            if (result.HasValue)
            {
                return result.Value;
            }
            else
            {
                foreach (var groupId in GetImplicitParentGroupsForUser(userId))
                {
                    var tempResult = GetGrantForGroup(privilegeId, groupId);
                    if (tempResult.HasValue)
                    {
                        if (tempResult == false)
                        {
                            return false;
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
                return result;
            }
        }

        public IEnumerable<TPrivilegeKey> GetUserPrivileges(TUserKey userId)
        {
            var list = new List<TPrivilegeKey>();
            foreach (var privilege in GetPrivileges())
            {
                if (GetEvaluatedGrantForUserOrDefault(privilege, userId))
                {
                    list.Add(privilege);
                }
            }
            return list;
        }


        public IEnumerable<TPrivilegeKey> GetGroupPrivileges(TGroupKey gruopId)
        {
            var list = new List<TPrivilegeKey>();
            foreach (var privilege in GetPrivileges())
            {
                if (GetEvaluatedGrantForGroupOrDefault(privilege, gruopId))
                {
                    list.Add(privilege);
                }
            }
            return list;
        }

        public bool GetEvaluatedGrantForGroupOrDefault(TPrivilegeKey privilegeId, TGroupKey groupId)
        {
            return GetEvaluatedGrantForGroup(privilegeId, groupId) ?? DefaultGrantValue;
        }

        public bool GetEvaluatedGrantForUserOrDefault(TPrivilegeKey privilegeId, TUserKey userId)
        {
            return GetEvaluatedGrantForUser(privilegeId, userId) ?? DefaultGrantValue;
        }

        public IPrivilegeContextService<TPrivilegeKey> GetUserContextService(TUserKey userId)
        {
            return new UserContextService<TUserKey, TPrivilegeKey, TGroupKey>(userId, this);
        }

        public IPrivilegeContextService<TPrivilegeKey> GetGroupContextService(TGroupKey groupId)
        {
            return new GroupContextService<TUserKey, TPrivilegeKey, TGroupKey>(groupId, this);
        }

    }
}
