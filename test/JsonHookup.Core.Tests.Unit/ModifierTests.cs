using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using Xunit;
using JsonHookup.Core.Tests.Unit.TestClasses;
using System;
using FluentAssertions;

namespace JsonHookup.Core.Tests.Unit
{
    public class ModifierTests
    {
        private readonly JsonSerializerOptions _jsonOptions;
        public ModifierTests()
        {
            _jsonOptions = new()
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers = { JsonHookup.ImplicitJsonHookupModifier },
                }
            };
        }

        [Fact]
        public void Test1()
        {
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

            string actualJson = JsonSerializer.Serialize(objDC, _jsonOptions);
            string expectedJson = JsonSerializer.Serialize(objSTJ);

            actualJson.Should().Be(expectedJson);
        }
    }
}