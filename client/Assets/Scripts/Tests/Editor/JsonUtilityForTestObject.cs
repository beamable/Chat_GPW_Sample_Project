using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Beamable.Samples.GPW.Data
{
    [Serializable]
    public class TestObject : System.Object
    {
        public string Title = "";
        public int RandomSeed = 1;

        public override string ToString()
        {
            return $"[TestObject (Title={Title}, RandomSeed={RandomSeed})]";
        }
    }
    
    [Serializable]
    public class TestObjectCollection
    {
        public List<TestObject> TestObjects = new List<TestObject>();
    }
    
    /// <summary>
    /// These test are expected to pass in Beamable SDK 0.17.4 and later
    /// </summary>
    [Category("Data")]
    public class JsonUtilityForTestObject
    {
        [Test]
        public void T01_ToJson_EqualsFromJson_WhenTestObject ()
        {
            // Arrange
            var objectToJson = new TestObject();
            string title = objectToJson.Title = "MyTitle";
            int randomSeed = objectToJson.RandomSeed = 99;
            string json = JsonUtility.ToJson(objectToJson);
            
            // Act
            var objectFromJson = JsonUtility.FromJson<TestObject>(json);

            // Assert
            Assert.That(objectFromJson.Title, Is.EqualTo(objectToJson.Title));
            Assert.That(objectFromJson.RandomSeed, Is.EqualTo(objectToJson.RandomSeed));
        }
        
        
        [Test]
        public void T02_ToJson_EqualsFromJson_WhenTestObjectCollection()
        {
            // Arrange
            var objectToJson = new TestObject();
            string title = objectToJson.Title = "MyTitle";
            int randomSeed = objectToJson.RandomSeed = 99;

            var listObjectToJson = new TestObjectCollection();
            listObjectToJson.TestObjects.Add(objectToJson);
            string json = JsonUtility.ToJson(listObjectToJson);
            
            // Act
            var listObjectFromJson = JsonUtility.FromJson<TestObjectCollection>(json);
            
            // Assert
            Assert.That(json.Length, Is.GreaterThan(0));
            Assert.That(listObjectFromJson.GetType(), Is.EqualTo(listObjectToJson.GetType()));
            Assert.That(listObjectToJson.TestObjects.Count, Is.EqualTo(1));
            Assert.That(listObjectFromJson.TestObjects.Count, Is.EqualTo(1));
            Assert.That(listObjectFromJson.TestObjects[0].Title, Is.EqualTo(listObjectToJson.TestObjects[0].Title));
            Assert.That(listObjectFromJson.TestObjects[0].RandomSeed, Is.EqualTo(listObjectToJson.TestObjects[0].RandomSeed));
        }
    }
}