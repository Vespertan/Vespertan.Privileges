namespace Vespertan.Privileges
{
    public class MemoryPrivilegeServiceEx : EditablePrivilegesServiceBase<int,int, int>
    {
        private MemoryPrivilegeServiceEx()
        {

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
    }
}
