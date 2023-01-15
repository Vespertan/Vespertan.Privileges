namespace Vespertan.Privileges
{
    public struct PrivilegeRelation<TUserKey, TGroupKey>
    {
        public PrivilegeRelation(TGroupKey groupId, TUserKey userId)
        {
            GroupId = groupId;
            UserId = userId;
            SubGroupId = default;
        }

        public PrivilegeRelation(TGroupKey groupId, TGroupKey subGroupId)
        {
            GroupId = groupId;
            SubGroupId = subGroupId;
            UserId = default;
        }

        public TGroupKey GroupId { get; set; }
        public TGroupKey SubGroupId { get; set; }
        public TUserKey UserId { get; set; }
    }
}
