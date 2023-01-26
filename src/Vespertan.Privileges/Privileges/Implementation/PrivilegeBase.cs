using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Vespertan.Privileges
{
    public abstract class PrivilegeBase<TPrivilegeKey>
    {
        private readonly IPrivilegeContextService<TPrivilegeKey> privilegeContextService;
        private Dictionary<string, TPrivilegeKey> privilegeMap;
        private readonly string separator = ".";

        public PrivilegeBase(IPrivilegeContextService<TPrivilegeKey> privilegeContextService)
        {
            this.privilegeContextService = privilegeContextService;
        }

        public PrivilegeBase(IPrivilegeContextService<TPrivilegeKey> privilegeContextService, string separator)
        {
            this.privilegeContextService = privilegeContextService;
            this.separator = separator;
        }

        private void InitPrivilegeMap()
        {
            var privilegeMapTemp = new Dictionary<string, TPrivilegeKey>();

            var className = GetType().Name;
            var privilegeInfoAttribute = (PrivilegeInfoAttribute)GetType().GetCustomAttributes(typeof(PrivilegeInfoAttribute), true).FirstOrDefault();
            if (privilegeInfoAttribute != null)
            {
                className = privilegeInfoAttribute.Name?.Trim() ?? string.Empty;
            }

            if (className != string.Empty)
            {
                className += separator;
            }

            foreach (var prop in GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(bool))
                {
                    var privilegeInfo = (PrivilegeInfoAttribute)prop.GetCustomAttributes(typeof(PrivilegeInfoAttribute), false).FirstOrDefault();
                    if (privilegeInfo == null)
                    {
                        if (typeof(TPrivilegeKey) == typeof(string))
                        {
                            privilegeMapTemp[prop.Name] = (TPrivilegeKey)(object)$"{className}{prop.Name}";
                        }
                        else
                        {
                            throw new InvalidOperationException($"Missing PrivilegeInfoAttribute for the property {prop.Name}. It is requaried for TPrivilegeKey diffrent of string");
                        }
                    }
                    else if (privilegeInfo.Id == null || Equals(privilegeInfo.Id, default(TPrivilegeKey)))
                    {
                        throw new InvalidOperationException($"Null or default value for type {typeof(TPrivilegeKey)} for property Id of attribute PrivilegeInfoAttribute is not allowed");
                    }
                    else if (privilegeMapTemp.Values.Contains((TPrivilegeKey)privilegeInfo.Id))
                    {
                        throw new InvalidOperationException($"Value {privilegeInfo.Id} is already used. Id must be uniqued");
                    }
                    else
                    {
                        privilegeMapTemp[prop.Name] = (TPrivilegeKey)privilegeInfo.Id;
                    }
                }
            }
            privilegeMap = privilegeMapTemp;
        }

        protected bool GetGrant(TPrivilegeKey privilegeId)
        {
            return privilegeContextService.GetGrant(privilegeId);
        }

        protected bool GetGrantByName([CallerMemberName]string privilegeItemName = null)
        {
            if (privilegeMap == null)
            {
                InitPrivilegeMap();
            }

            return privilegeContextService.GetGrant(privilegeMap[privilegeItemName]);
        }


        public static Dictionary<string, PrivilegeInfoAttribute> GetPrivilegeInfoAttributeForProperties(Type type)
        {
            var privilegeInfoAttributes = new Dictionary<string, PrivilegeInfoAttribute>();
            foreach (var prop in type.GetProperties())
            {
                if (prop.PropertyType == typeof(bool))
                {
                    var privilegeInfoAttribute = (PrivilegeInfoAttribute)prop.GetCustomAttributes(typeof(PrivilegeInfoAttribute), false).FirstOrDefault();
                    privilegeInfoAttributes[prop.Name] = privilegeInfoAttribute;
                }
            }

            return privilegeInfoAttributes;
        }

        public static Dictionary<TEnum, PrivilegeInfoAttribute> GetPrivilegeInfoAttributeFromEnum<TEnum>() where TEnum : Enum
        {
            var privilegeInfoAttributes = new Dictionary<TEnum, PrivilegeInfoAttribute>();
            var @enum = Activator.CreateInstance(typeof(TEnum));
            foreach (var field in typeof(TEnum).GetFields())
            {
                var enumValue = field.GetValue(@enum);
                    var privilegeInfoAttribute = (PrivilegeInfoAttribute)field.GetCustomAttributes(typeof(PrivilegeInfoAttribute), false).FirstOrDefault();
                    privilegeInfoAttributes[(TEnum)enumValue] = privilegeInfoAttribute;
            }

            return privilegeInfoAttributes;
        }
    }
}

