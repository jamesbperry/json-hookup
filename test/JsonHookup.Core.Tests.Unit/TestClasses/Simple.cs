using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace JsonHookup.Core.Tests.Unit.TestClasses
{
    public record class SimpleDC
    {
        [DataMember(Name = "Key", Order = 1)]
        public Guid Id { get; init; }

        public string? Name { get; init; }

        [DataMember(Order = 0)]
        public string? Job { get; init; }

        [IgnoreDataMember]
        public string? InternalInfo { get; init; }
    }

    public record class SimpleSTJ
    {
        [JsonPropertyName("Key")]
        [JsonPropertyOrder(1)]
        public Guid Id { get; init; }

        public string? Name { get; init; }

        [JsonPropertyOrder(0)]
        public string? Job { get; init; }

        [JsonIgnore]
        public string? InternalInfo { get; init; }
    }
}
