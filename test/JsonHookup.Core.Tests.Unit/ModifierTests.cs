using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using Xunit;
using JsonHookup.Core.Tests.Unit.TestClasses;
using System;
using FluentAssertions;
using System.Text.Json.Serialization;

namespace JsonHookup.Core.Tests.Unit
{
    public class ModifierTests
    {
        [Theory]
        [InlineData(JsonKnownNamingPolicy.Unspecified)]
        [InlineData(JsonKnownNamingPolicy.CamelCase)]
        public void Naming_Policy_Is_Respected(JsonKnownNamingPolicy namingPolicy)
        {
            JsonSerializerOptions jsonOptions = GetJsonOptions(namingPolicy);
            JsonSerializerOptions jsonOptionsWithHookup = GetJsonOptions(namingPolicy).AddHookup(HookupOptions.DefaultImplicit);

            SimpleDC objDC = new() 
            {
                Id = Guid.NewGuid(),
                Name = "Mike",
                Job = "Monster",
                Age = 42,
                InternalInfo = "This should be ignored"
            };

            SimpleSTJ_Implicit objSTJ = new SimpleSTJ_Implicit() with
            {
                Id = objDC.Id,
                Name = objDC.Name,
                Job = objDC.Job,
                Age = objDC.Age,
                InternalInfo = objDC.InternalInfo,
            };

            string actualJson = JsonSerializer.Serialize(objDC, jsonOptionsWithHookup);
            string expectedJson = JsonSerializer.Serialize(objSTJ, jsonOptions);

            actualJson.Should().Be(expectedJson);
        }

        [Fact]
        public void Explicit_Data_Contract_Honors_Attributes()
        {
            JsonSerializerOptions jsonOptions = GetJsonOptions(JsonKnownNamingPolicy.CamelCase);
            JsonSerializerOptions jsonOptionsWithHookup = GetJsonOptions(JsonKnownNamingPolicy.CamelCase).AddHookup(HookupOptions.DefaultExplicit);

            SimpleDC objDC = new()
            {
                Id = Guid.NewGuid(),
                Name = "Mike",
                Job = "Monster",
                Age = 42,
                InternalInfo = "This should be ignored"
            };

            SimpleSTJ_Explicit objSTJ = new SimpleSTJ_Explicit() with
            {
                Id = objDC.Id,
                // No name
                Job = objDC.Job,
                Age = objDC.Age,
                InternalInfo = objDC.InternalInfo,
            };

            string actualJson = JsonSerializer.Serialize(objDC, jsonOptionsWithHookup);
            string expectedJson = JsonSerializer.Serialize(objSTJ, jsonOptions);

            actualJson.Should().Be(expectedJson);
        }

        private static JsonSerializerOptions GetJsonOptions(JsonKnownNamingPolicy namingPolicyKey)
        {
            JsonNamingPolicy? namingPolicy = namingPolicyKey switch
            {
                JsonKnownNamingPolicy.CamelCase => JsonNamingPolicy.CamelCase,
                JsonKnownNamingPolicy.Unspecified => null,
                _ => throw new NotImplementedException(),
            };

            return new JsonSerializerOptions()
            {
                PropertyNamingPolicy = namingPolicy,
            };
        }
    }
}