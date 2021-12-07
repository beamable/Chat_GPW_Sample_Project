using System.Collections;
using System.Collections.Generic;
using Beamable.Common;
using Beamable.Samples.GPW;
using Beamable.Samples.GPW.Content;
using Beamable.Samples.GPW.Data.Factories;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Beamable.Samples.Tests.GPW
{
    /// <summary>
    /// Demonstrates a working example of an EditMode test.
    /// </summary>
    public class DataFactoryTest
    {
        [UnityTest]
        public IEnumerator locationContentViews_CountIs1_WhenFactoryHas1()
        {
            // Arrange
            IDataFactory dataFactory = new GPWBasicDataFactory();
            
            List<LocationContent> locationContents = new List<LocationContent>();
            locationContents.Add(ScriptableObject.CreateInstance<LocationContent>());
            
            List<ProductContent> productContents = new List<ProductContent>();
            productContents.Add(ScriptableObject.CreateInstance<ProductContent>());
            
            // Act
            var promise = dataFactory.CreateLocationContentViews(locationContents, productContents);
            yield return promise.ToPromise().ToYielder();
            List<LocationContentView> locationContentViews = promise.Result;
            
            // Assert
            Assert.That(locationContentViews.Count, Is.EqualTo(1));

        }
        
        [UnityTest]
        public IEnumerator ProductContentViews_CountIs1_WhenFactoryHas1()
        {
            // Arrange
            IDataFactory dataFactory = new GPWBasicDataFactory();
            
            List<LocationContent> locationContents = new List<LocationContent>();
            locationContents.Add(ScriptableObject.CreateInstance<LocationContent>());
            
            List<ProductContent> productContents = new List<ProductContent>();
            productContents.Add(ScriptableObject.CreateInstance<ProductContent>());
            
            // Act
            var promise = dataFactory.CreateLocationContentViews(locationContents, productContents);
            yield return promise.ToPromise().ToYielder();
            List<LocationContentView> locationContentViews = promise.Result;
            
            // Assert
            Assert.That(locationContentViews[0].ProductContentViews.Count, Is.EqualTo(1));

        }
        
        [UnityTest]
        public IEnumerator MarketGoodsPrice_IsGT0_WhenDefault()
        {
            // Arrange
            IDataFactory dataFactory = new GPWBasicDataFactory();
            
            List<LocationContent> locationContents = new List<LocationContent>();
            locationContents.Add(ScriptableObject.CreateInstance<LocationContent>());
            
            List<ProductContent> productContents = new List<ProductContent>();
            productContents.Add(ScriptableObject.CreateInstance<ProductContent>());
            
            // Act
            var promise = dataFactory.CreateLocationContentViews(locationContents, productContents);
            yield return promise.ToPromise().ToYielder();
            List<LocationContentView> locationContentViews = promise.Result;
            
            // Assert
            Assert.That(locationContentViews[0].ProductContentViews[0].MarketGoods.Price, Is.GreaterThan(0));

        }
        
        [UnityTest]
        public IEnumerator MarketGoodsQuantity_IsGT0_WhenDefault()
        {
            // Arrange
            IDataFactory dataFactory = new GPWBasicDataFactory();
            
            List<LocationContent> locationContents = new List<LocationContent>();
            locationContents.Add(ScriptableObject.CreateInstance<LocationContent>());
            
            List<ProductContent> productContents = new List<ProductContent>();
            productContents.Add(ScriptableObject.CreateInstance<ProductContent>());
            
            // Act
            var promise = dataFactory.CreateLocationContentViews(locationContents, productContents);
            yield return promise.ToPromise().ToYielder();
            List<LocationContentView> locationContentViews = promise.Result;
            
            // Assert
            Assert.That(locationContentViews[0].ProductContentViews[0].MarketGoods.Quantity, Is.GreaterThan(0));

        }
    }
}