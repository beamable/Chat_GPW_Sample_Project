using System.Collections.Generic;
using Beamable.Common.Content;
using NUnit.Framework;
using UnityEngine;

namespace Beamable.Samples.GPW.Data.Content
{
    
    public class TestContentObject : ContentObject
    {
        //  Fields ---------------------------------------
        public string Title = "";
        public int RandomSeed = 1;
    }
    
    /// <summary>
    /// These are expected to fail in Beamable SDK 0.17.4
    ///
    /// At that version, parsing child classes of Beamable's ContentObject is broken.
    /// </summary>
    [Category("Content")]
    public class JsonUtilityForGPWContentFails_SDK0_17_4Test
    {
       
        [Test]
        public void T01_ToJson_EqualsFromJson_WhenTestContentObject()
        {
            // Arrange
            var objectToJson = ScriptableObject.CreateInstance<TestContentObject>();
            string title = objectToJson.Title = "MyTitle";
            int randomSeed = objectToJson.RandomSeed = 99;
            string json = JsonUtility.ToJson(objectToJson);
            
            // Act
            var objectFromJson = JsonUtility.FromJson<TestContentObject>(json);

            // Assert
            Assert.That(objectFromJson.Title, Is.EqualTo(objectToJson.Title));
            Assert.That(objectFromJson.RandomSeed, Is.EqualTo(objectToJson.RandomSeed));
        }
        
        [Test]
        public void T02_ToJson_EqualsFromJson_WhenArrayLocationData()
        {
            // Arrange
            var objectToJson = new LocationData();
            string title = objectToJson.Title = "MyTitle";
            int randomSeed = objectToJson.RandomSeed = 99;
           
            var listObjectToJson = new LocationData[1];
            listObjectToJson[0] = objectToJson;
            string json = JsonUtility.ToJson(listObjectToJson);
            Debug.Log("json: " + json);
            
            // Act
            var listObjectFromJson = JsonUtility.FromJson<LocationData[]>(json);
            
            // Assert
            Assert.That(listObjectFromJson.GetType(), Is.EqualTo(listObjectToJson.GetType()));
            Assert.That(listObjectToJson.Length, Is.EqualTo(1));
            Assert.That(listObjectFromJson.Length, Is.EqualTo(1));
            Assert.That(listObjectFromJson[0].Title, Is.EqualTo(listObjectToJson[0].Title));
            Assert.That(listObjectFromJson[0].RandomSeed, Is.EqualTo(listObjectToJson[0].RandomSeed));
        }

        
        [Test]
        public void T03_ToJson_EqualsFromJson_WhenListTestContentObject()
        {
            // Arrange
            var objectToJson = ScriptableObject.CreateInstance<TestContentObject>();
            string title = objectToJson.Title = "MyTitle";
            int randomSeed = objectToJson.RandomSeed = 99;
            
            var listObjectToJson = new List<TestContentObject>();
            listObjectToJson.Add(objectToJson);
            string json = JsonUtility.ToJson(listObjectToJson);
            
            // Act
            var listObjectFromJson = JsonUtility.FromJson<List<TestContentObject>>(json);

            // Assert
            Assert.That(listObjectFromJson.GetType(), Is.EqualTo(listObjectToJson.GetType()));
            Assert.That(listObjectToJson.Count, Is.EqualTo(1));
            Assert.That(listObjectFromJson.Count, Is.EqualTo(1));
            Assert.That(listObjectFromJson[0].Title, Is.EqualTo(listObjectToJson[0].Title));
            Assert.That(listObjectFromJson[0].RandomSeed, Is.EqualTo(listObjectToJson[0].RandomSeed));
        }
    }
}