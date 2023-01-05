using System;

namespace JsonHookup.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class JsonUseDataContractAttribute : Attribute
    {
        public JsonUseDataContractAttribute(
            HookupParts? ignore = null,
            HookupMode? dataMemberMode = null)
        {
            Ignore = ignore;
            DataMemberMode = dataMemberMode;
        }

        public HookupParts? Ignore { get; }
        public HookupMode? DataMemberMode { get; }

        public HookupSettings ApplyTo(HookupSettings settings)
            => settings with
            {
                Ignore = Ignore ?? settings.Ignore,
                DataContractMode = HookupMode.Explicit,
                DataMemberMode = DataMemberMode ?? settings.DataMemberMode,
            };
    }
}