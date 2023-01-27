using System.Collections.Generic;

namespace Vespertan.Privileges
{
    public class MemoryPrivilegeServiceEx : EditablePrivilegesServiceBase<int, int, int>
    {
        private MemoryPrivilegeServiceEx()
        {
            OnGroupAdd += (s, e) => groups.Add(e);
            OnGroupRemove += (s, e) => groups.Remove(e);
            OnUserAdd += (s, e) => users.Add(e);
            OnUserRemove += (s, e) => users.Remove(e);
            OnPrivilegeAdd += (s, e) => privileges.Add(e);
            OnPrivilegeRemove += (s, e) => privileges.Remove(e);
            OnPrivilegeRelationAdd += (s, e) => privilegeRelations.Add(e);
            OnPrivilegeRelationRemove += (s, e) => privilegeRelations.Remove(e);
            OnPrivilegeGrantAdd += (s, e) => privilegeGrants.Add(e);
            OnPrivilegeGrantRemove += (s, e) => privilegeGrants.Remove(e);
            OnPrivilegeGrantUpdate += (s, e) =>
            {
                privilegeGrants.Remove(new PrivilegeGrant<int, int, int>
                {
                    UserId = e.UserId,
                    GroupId = e.GroupId,
                    PrivilegeId = e.PrivilegeId,
                    Grant = !e.Grant
                });
                privilegeGrants.Add(e);
            };
        }

        private static MemoryPrivilegeServiceEx _instance;
        public static MemoryPrivilegeServiceEx GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MemoryPrivilegeServiceEx();
            }
            return _instance;
        }
        private List<int> privileges = new List<int>();
        private List<int> users = new List<int>();
        private List<int> groups = new List<int>();
        private List<PrivilegeGrant<int, int, int>> privilegeGrants = new List<PrivilegeGrant<int, int, int>>();
        private List<PrivilegeRelation<int, int>> privilegeRelations = new List<PrivilegeRelation<int, int>>();
        public override IEnumerable<int> GetPrivileges()
        {
            return privileges;
        }

        private List<int> _privileges2;
        public override IEnumerable<int> GetUsers()
        {
            return users;
        }

        public override IEnumerable<int> GetGroups()
        {
            return groups;
        }

        public override IEnumerable<PrivilegeGrant<int, int, int>> GetPrivilegeGrants()
        {
            return privilegeGrants;
        }

        public override IEnumerable<PrivilegeRelation<int, int>> GetPrivilegeRelations()
        {
            return privilegeRelations;
        }
    }
}
