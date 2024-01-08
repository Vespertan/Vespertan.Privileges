using System;
using System.Runtime.CompilerServices;

namespace Vespertan.Privileges
{
    public class PrivilegeInfoAttribute : Attribute
    {
        public PrivilegeInfoAttribute([CallerMemberName] string name = null) 
        {
            Name = name;
        }
        public object Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string GroupName { get; set; }
        public string AlternateName { get; set; }
    }
}