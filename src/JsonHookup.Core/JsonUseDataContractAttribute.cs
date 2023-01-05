using System;

namespace JsonHookup.Core
{
    [Obsolete("This seemed like a good idea as a way to specify per-class behavior. But a core use case of this library is predefined classes, so...")]
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