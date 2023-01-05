# json-hookup
For when you wish System.Text.Json would honor DataContract and DataMember attributes

## Overview
JsonHookup uses 

This is especially useful when a library abstracts their `ISerializer`, but the DTO classes are all decorated using `[DataMember]` attributes.

## Background
`System.Text.Json` infamously refuses to honor `System.Runtime.Serialization` constructs such as `[DataContract]`, `[DataMember]`, and `[IgnoreDataMember]`. And historically before .NET 6/7, that was more defensible: System.Text.Json couldn't do much more than honor custom property names without involving cumbersome extensibility.

But as of .NET7, a foothold has appeared: [Modifiers for the Type Info Resolver](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/custom-contracts)

## Usage


```
// Apply JsonHookup to an instance of JsonSerializerOptions cloned from the static Default
JsonSerializerOptions jsonOptionsFromDefault = JsonHookup.AddHookupToDefault(HookupOptions.DefaultExplicit);

JsonSerializer.Serialize(myObject, jsonOptionsFromDefault);

// or

// Apply JsonHookup to some existing JsonSerializerOptions
JsonSerializerOptions existingJsonOptions;
someExistingOptions.AddHookup(HookupOptions.DefaultExplicit);

JsonSerializer.Serialize(myObject, existingJsonOptions);

// or

JsonSerializerOptions existingJsonOptions;
HookupOptions hookupOptions = new() // For example...
{ 
  Ignore = HookupParts.DataMemberOrder, // ignore data member order
  DataContractMode = HookupMode.Explicit, // use the hookup only on classes decorated with [DataContract] or [UseDataContract]
  DataMemberMode = HookupMode.Explicit, // only include properties decorated with [DataMember]
}
existingJsonOptions.AddHookup(hookupOptions);

JsonSerializer.Serialize(myObject, existingJsonOptions);

```
### Serializer options lifetime
Note: general guidance in System.Text.Json is to re-use an instance of `JsonSerializerOptions` as widely as possible. This guidance still applies with JsonHookup.

## Target Framework
JsonHookup depends on functionality added in .NET 7.

## Status
This project is completely experimental.

## Known Issues as of v0.1.1
- Interface types are not supported. Only concrete `class` or `struct` types are supported.
- `DataContractMode = Explicit` is a bit stupid/broken. It should honor a `[DataContract]` attribute but does not.
- Emit Default Value is ignored
- Testing is skeletal at best
- The only supported TypeInfoResolver is `DefaultJsonTypeInfoResolver`
