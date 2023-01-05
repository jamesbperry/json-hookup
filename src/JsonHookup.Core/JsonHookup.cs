using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace JsonHookup.Core
{
    public static class JsonHookup
    {
        public static void ExplicitJsonHookupModifier(JsonTypeInfo jsonTypeInfo)
            => Apply(HookupSettings.DefaultExplicit, jsonTypeInfo);

        public static void ImplicitJsonHookupModifier(JsonTypeInfo jsonTypeInfo)
            => Apply(HookupSettings.DefaultImplicit, jsonTypeInfo);

        public static Action<JsonTypeInfo> JsonHookupModifier(HookupSettings settings)
            => (JsonTypeInfo jsonTypeInfo) => Apply(settings, jsonTypeInfo);

        private static void Apply(HookupSettings settings, JsonTypeInfo jsonTypeInfo)
        {
            if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object || jsonTypeInfo.Type.IsInterface) // not supporting interfaces for now
            {
                return;
            }

            if (GetHookupAttribute(jsonTypeInfo.Type) is JsonUseDataContractAttribute hookupAttribute)
            {
                settings = hookupAttribute.ApplyTo(settings);
            }
            else if (settings.DataContractMode == HookupMode.Explicit)
            {
                return; // Don't handle this type
            }

            IEnumerable<PropertyInfo> typeProperties = GetDataMemberProperties(jsonTypeInfo.Type, settings.DataMemberMode);

            // TO DO : this might be unnecessary/undesirable, depending on when the naming policy is applied!
            JsonNamingPolicy? namingPolicy = jsonTypeInfo.Options.PropertyNamingPolicy;
            string getName(PropertyInfo p) => namingPolicy?.ConvertName(p.Name) ?? p.Name;
            IReadOnlyDictionary<string, JsonPropertyInfo> jsonProperties = jsonTypeInfo.Properties.ToDictionary(p => p.Name);

            foreach (PropertyInfo typeProperty in typeProperties)
            {
                if (typeProperty.IsDefined(typeof(JsonAttribute)))
                {
                    continue; // JsonAttributes take precedence. If one is applied to this property, ignore the DataMember attribute.
                }

                string jsonPropertyName = getName(typeProperty);
                if (jsonProperties.TryGetValue(jsonPropertyName, out JsonPropertyInfo? jsonProperty) && jsonProperty is not null)
                {
                    ApplyDataMemberConfig(typeProperty, jsonProperty, settings.Ignore);
                }
                else if (!typeProperty.IsDefined(typeof(IgnoreDataMemberAttribute)))
                {
                    JsonPropertyInfo newJsonProperty = jsonTypeInfo.CreateJsonPropertyInfo(typeProperty.PropertyType, typeProperty.Name);
                    
                    // TODO need to explicitly define getter/setter?
                    //newJsonProperty.Get = typeProperty.Get

                    ApplyDataMemberConfig(typeProperty, newJsonProperty, settings.Ignore);
                    jsonTypeInfo.Properties.Add(newJsonProperty);
                }
            }
        }

        private static void ApplyDataMemberConfig(PropertyInfo typeProperty, JsonPropertyInfo jsonProperty, HookupParts ignore)
        {
            if (typeProperty.IsDefined(typeof(IgnoreDataMemberAttribute)))
            {
                jsonProperty.ShouldSerialize = (_, _) => false;
                return;
            }

            if (typeProperty.GetCustomAttribute<DataMemberAttribute>(inherit: false) is DataMemberAttribute dataMemberAttribute)
            {
                ApplyDataMemberConfig(dataMemberAttribute, jsonProperty, ignore);
            }
        }

        private static void ApplyDataMemberConfig(DataMemberAttribute attribute, JsonPropertyInfo jsonProperty, HookupParts ignore)
        {
            if (attribute.IsNameSetExplicitly && !ignore.HasFlag(HookupParts.DataMemberName))
            {
                jsonProperty.Name = attribute.Name!;
            }

            jsonProperty.IsRequired = attribute.IsRequired;

            if (!ignore.HasFlag(HookupParts.DataMemberOrder))
            {
                // -1 is default DMA order value.
                //Negative orders in STJ go before unordered or explicitly-ordered properties.
                // TO DO: improve this logic, as DMA has additional semantics around inheritance.
                jsonProperty.Order = attribute.Order; 
            }
            
            // TODO might be able to use a custom getter to emit a default value
        }

        private static JsonUseDataContractAttribute? GetHookupAttribute(Type type)
            => type
                .GetCustomAttributes(typeof(JsonUseDataContractAttribute), inherit: false)
                .Cast<JsonUseDataContractAttribute>()
                .FirstOrDefault();

        private static IEnumerable<PropertyInfo> GetDataMemberProperties(Type type, HookupMode dataMemberMode)
        {
            IEnumerable<PropertyInfo> publicProperties = GetDataMemberProperties(type, dataMemberMode, BindingFlags.Public);
            IEnumerable<PropertyInfo> nonPublicProperties = GetDataMemberProperties(type, dataMemberMode, BindingFlags.NonPublic);
            return publicProperties.Union(nonPublicProperties);
        }

        private static IEnumerable<PropertyInfo> GetDataMemberProperties(Type type, HookupMode dataMemberMode, BindingFlags publicOrNonPublic)
        {
            PropertyInfo[] instanceProperties = type.GetProperties(BindingFlags.Instance | publicOrNonPublic);

            bool attributeIsOptional = publicOrNonPublic switch
            {
                BindingFlags.Public => dataMemberMode == HookupMode.Implicit,
                BindingFlags.NonPublic => false,
                _ => throw new NotSupportedException(),
            };

            return instanceProperties
                //.Where(p => !p.IsDefined(typeof(IgnoreDataMemberAttribute), inherit: false))
                .Where(p => attributeIsOptional || p.IsDefined(typeof(DataMemberAttribute), inherit: false));
        }


    }
}
