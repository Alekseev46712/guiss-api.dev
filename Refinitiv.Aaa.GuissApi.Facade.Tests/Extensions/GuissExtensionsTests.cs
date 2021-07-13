using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Refinitiv.Aaa.GuissApi.Data.Models;
using Refinitiv.Aaa.GuissApi.Facade.Extensions;
using Refinitiv.Aaa.GuissApi.Facade.Models;

namespace Refinitiv.Aaa.GuissApi.Facade.Tests.Extensions
{
    [TestFixture]
    public class GuissExtensionsTests
    {
        private readonly IFixture fixture = new Fixture();

        [Test]
        public void GuissDtoToGuissMapsAllProperties()
        {
            var dto = fixture.Create<GuissDb>();

            var template = GuissExtensions.Map(dto);

            template.Should().BeEquivalentTo(dto, options => options.ExcludingMissingMembers());
        }

        [Test]
        public void NullGuissDtoToGuissReturnsNull()
        {
            GuissDb db = null;
            GuissExtensions.Map(db).Should().BeNull();
        }

        [Test]
        public void GuissToGuissDtoMapsAllProperties()
        {
            var template = fixture.Create<Guiss>();
            var dto = GuissExtensions.Map(template);

            dto.Should().BeEquivalentTo(template,options => options.ExcludingMissingMembers());
        }

        [Test]
        public void NullGuissToGuissDtoReturnsNull()
        {
            Guiss template = null;
            GuissExtensions.Map(template).Should().BeNull();
        }
    }
}
