using System;

namespace JsonHookup.Core
{
    public record class HookupSettings
    {
        public HookupParts Ignore { get; init; }

        public HookupMode DataContractMode { get; init; }

        public HookupMode DataMemberMode { get; init; }

        public static HookupSettings DefaultExplicit { get; } = new()
        {
            Ignore = HookupParts.None,
            DataContractMode = HookupMode.Explicit,
            DataMemberMode = HookupMode.Explicit,
        };

        public static HookupSettings DefaultImplicit { get; } = new()
        {
            Ignore = HookupParts.None,
            DataContractMode = HookupMode.Implicit,
            DataMemberMode = HookupMode.Implicit,
        };
    }

    [Flags]
    public enum HookupParts
    {
        None,
        DataMemberName,
        DataMemberOrder,
    }

    public enum HookupMode
    {
        Explicit = 0,
        Implicit,
    }
}
