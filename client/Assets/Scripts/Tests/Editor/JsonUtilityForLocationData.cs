using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Beamable.Samples.GPW.Data
{
    /// <summary>
    /// These test are expected to pass in Beamable SDK 0.17.4 and later
    /// </summary>
    [Category("Data")]
    public class JsonUtilityForLocationData
    {
        [Test]
        public void T01_ToJson_EqualsFromJson_WhenLocationData ()
        {
            // Arrange
            var objectToJson = new LocationData();
            string title = objectToJson.Title = "MyTitle";
            int randomSeed = objectToJson.RandomSeed = 99;
            string json = JsonUtility.ToJson(objectToJson);
            
            // Act
            var objectFromJson = JsonUtility.FromJson<LocationData>(json);

            // Assert
            Assert.That(objectFromJson.Title, Is.EqualTo(objectToJson.Title));
            Assert.That(objectFromJson.RandomSeed, Is.EqualTo(objectToJson.RandomSeed));
        }
    }
}