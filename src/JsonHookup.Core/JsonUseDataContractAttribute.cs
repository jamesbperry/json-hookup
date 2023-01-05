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

        public HookupOptions ApplyTo(HookupOptions options)
            => options with
            {
                Ignore = Ignore ?? options.Ignore,
                DataContractMode = HookupMode.Explicit,
                DataMemberMode = DataMemberMode ?? options.DataMemberMode,
            };
    }
}