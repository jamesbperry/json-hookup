using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace JsonHookup.Core.Tests.Unit.TestClasses
{
    // No data contract here
    public record class SimpleDMs
    {
        [DataMember(Name = "Key", Order = 1)]
        public Guid Id { get; init; }

        public string? Name { get; init; }

        [DataMember(Order = 2)]
        public string? Job { get; init; }

        [DataMember(Order = 0)]
        public int? Age { get; init; }

        [IgnoreDataMember]
        public string? InternalInfo { get; init; }
    }

    [DataContract]
    public record class SimpleDC
    {
        [DataMember(Name = "Key", Order = 1)]
        public Guid Id { get; init; }

        public string? Name { get; init; }

        [DataMember(Order = 2)]
        public string? Job { get; init; }

        [DataMember(Order = 0)]
        public int? Age { get; init; }

        [IgnoreDataMember]
        public string? InternalInfo { get; init; }
    }

    public record class SimpleSTJ_Implicit
    {
        [JsonPropertyName("Key")]
        [JsonPropertyOrder(1)]
        public Guid Id { get; init; }

        public string? Name { get; init; }

        [JsonPropertyOrder(2)]
        public string? Job { get; init; }

        [JsonPropertyOrder(0)]
        public int? Age { get; init; }

        [JsonIgnore]
        public string? InternalInfo { get; init; }
    }

    public record class SimpleSTJ_Explicit
    {
        [JsonPropertyName("Key")]
        [JsonPropertyOrder(1)]
        public Guid Id { get; init; }

        // no Name

        [JsonPropertyOrder(2)]
        public string? Job { get; init; }

        [JsonPropertyOrder(0)]
        public int? Age { get; init; }

        [JsonIgnore]
        public string? InternalInfo { get; init; }
    }
}
