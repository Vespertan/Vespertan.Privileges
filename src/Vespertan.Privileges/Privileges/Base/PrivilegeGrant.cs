namespace Vespertan.Privileges
{
    public struct PrivilegeGrant<TUserKey, TPrivilegeKey, TGroupKey>
    {
        public PrivilegeGrant(TPrivilegeKey privilegeId, TUserKey userId, bool grant)
        {
            PrivilegeId = privilegeId;
            UserId = userId;
            GroupId = default;
            Grant = grant;
        }

        public PrivilegeGrant(TPrivilegeKey privilegeId, TGroupKey groupId, bool grant)
        {
            PrivilegeId = privilegeId;
            UserId = default;
            GroupId = groupId;
            Grant = grant;
        }

        public TPrivilegeKey PrivilegeId { get; set; }
        public TUserKey UserId { get; set; }
        public TGroupKey GroupId { get; set; }
        public bool Grant { get; set; }
    }
}
