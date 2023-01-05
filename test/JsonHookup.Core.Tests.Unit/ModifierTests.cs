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
            JsonSerializerOptions jsonOptionsWithHookup = GetJsonOptions(namingPolicy).AddHookup();

            SimpleDC objDC = new() 
            {
                Id = Guid.NewGuid(),
                Name = "Mike",
                Job = "Monster",
                InternalInfo = "This should be ignored"
            };

            SimpleSTJ objSTJ = new SimpleSTJ() with
            {
                Id = objDC.Id,
                Name = objDC.Name,
                Job = objDC.Job,
                InternalInfo = objDC.InternalInfo,
            };

            objDC.Should().BeEquivalentTo(objSTJ);

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
                TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers = { JsonHookup.ImplicitJsonHookupModifier },
                }
            };
        }
    }
}