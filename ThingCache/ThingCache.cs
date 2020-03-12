using System.Collections.Generic;
using NUnit.Framework;
using FakeItEasy;
using FluentAssertions;

namespace MockFramework
{
    public class ThingCache
    {
        private readonly IDictionary<string, Thing> dictionary
            = new Dictionary<string, Thing>();
        private readonly IThingService thingService;

        public ThingCache(IThingService thingService)
        {
            this.thingService = thingService;
        }

        public Thing Get(string thingId)
        {
            Thing thing;
            if (dictionary.TryGetValue(thingId, out thing))
                return thing;
            if (thingService.TryRead(thingId, out thing))
            {
                dictionary[thingId] = thing;
                return thing;
            }
            return null;
        }
    }

    [TestFixture]
    public class ThingCache_Should
    {
        private IThingService thingService;
        private ThingCache thingCache;

        private const string thingId1 = "TheDress";
        private Thing thing1 = new Thing(thingId1);

        private const string thingId2 = "CoolBoots";
        private Thing thing2 = new Thing(thingId2);

        [SetUp]
        public void SetUp()
        {
            thingService = A.Fake<IThingService>();
            thingCache = new ThingCache(thingService);
        }

        [Test]
        public void TryReadFromEmptyCache_ShouldReturnNull()
        {
            thingCache.Get(thingId1)
                .Should().BeNull();
            A.CallTo(() => thingService.TryRead(thingId1, out thing1))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void TryReadFromCacheAfterFirstTryRead_ServiceShouldBeCalledExactlyOnce()
        {
            A.CallTo(() => thingService.TryRead(thingId1, out thing1)).Returns(true);
            thingCache.Get(thingId1);
            A.CallTo(() => thingService.TryRead(thingId1, out thing1))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void TryReadFromCacheAfterFirstTryRead_ShouldReturnCorrectValue()
        {
            A.CallTo(() => thingService.TryRead(thingId1, out thing1)).Returns(true);
            var returned = thingCache.Get(thingId1);
            returned.Should().Be(thing1);
        }

        [Test]
        public void TryReadFromCacheAfterTryReadThoThings_ServiceShouldBeCalledExactlyForBothThings()
        {
            A.CallTo(() => thingService.TryRead(thingId1, out thing1)).Returns(true);
            A.CallTo(() => thingService.TryRead(thingId2, out thing2)).Returns(true);

            var returnedThing1 = thingCache.Get(thingId1);
            var returnedThing2 = thingCache.Get(thingId2);

            A.CallTo(() => thingService.TryRead(thingId1, out thing1))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => thingService.TryRead(thingId2, out thing2))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void TryReadFromCacheAfterTryReadThoThings_ShouldReturnThisThingsCorrectly()
        {
            A.CallTo(() => thingService.TryRead(thingId1, out thing1)).Returns(true);
            A.CallTo(() => thingService.TryRead(thingId2, out thing2)).Returns(true);

            var returnedThing1 = thingCache.Get(thingId1);
            var returnedThing2 = thingCache.Get(thingId2);

            returnedThing1.Should().Be(thing1);
            returnedThing2.Should().Be(thing2);
        }

        //TODO: написать простейший тест, а затем все остальные
        //Live Template tt работает!
    }
}