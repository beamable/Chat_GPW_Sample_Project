using System.Collections.Generic;
using Beamable.Common.Content;
using NUnit.Framework;
using UnityEngine;

namespace Beamable.Samples.GPW.Content
{
    
    public class TestObject 
    {
        //  Fields ---------------------------------------
        public string Title = "";
        public int RandomSeed = 1;
    }
    
    public class TestContentObject : ContentObject
    {
        //  Fields ---------------------------------------
        public string Title = "";
        public int RandomSeed = 1;
    }
    
    /// <summary>
    /// Tests that JsonUtility properly cares for GPW game content
    /// </summary>
    [Category("Content")]
    public class JsonUtilityForGPWContentTest
    {
        [Test]
        public void T01a_FromJson_StringLength0_WhenNull()
        {
            // Arrange
            
            // Act
            string json = JsonUtility.ToJson(null);
            
            // Assert
            Assert.That(json.Length, Is.EqualTo(0));
        }
        
        [Test]
        public void T01b_FromJson_StringIsBrackets_WhenTypeIsString ()
        {
            // Arrange
            var value = "Hello World";
            var objectToJson = value;
            
            // Act
            string json = JsonUtility.ToJson(objectToJson);
            
            // Assert
            Assert.That(json, Is.EqualTo("{}"));
        }
        
        [Test]
        public void T01c_FromJson_StringLengthGT2_WhenTypeIsString ()
        {
            // Arrange
            var value = "Hello World";
            var objectToJson = value;
            
            // Act
            string json = JsonUtility.ToJson(objectToJson);
            
            // Assert
            Assert.That(json.Length, Is.GreaterThan(2));
        }
        
        [Test]
        public void T01d_FromJson_StringLengthGT2_WhenTypeIsInt()
        {
            // Arrange
            var value = (int)3;
            var objectToJson = value;
            
            // Act
            string json = JsonUtility.ToJson(objectToJson);
            
            // Assert
            Assert.That(json.Length, Is.GreaterThan(2));
        }
        
        [Test]
        public void T01e_FromJson_StringLengthGT2_WhenTypeIsTestObject()
        {
            // Arrange
            var value = new TestObject();
            value.Title = "MyTitle";
            value.RandomSeed = 99;
            var objectToJson = value;
            
            // Act
            string json = JsonUtility.ToJson(objectToJson);
            
            // Assert
            Assert.That(json.Length, Is.GreaterThan(2));
        }
        
        [Test]
        public void T01f_FromJson_StringLengthGT2_WhenTypeIsTestContentObject()
        {
            // Arrange
            var value = ScriptableObject.CreateInstance<TestContentObject>();
            value.Title = "MyTitle";
            value.RandomSeed = 99;
            var objectToJson = value;
            
            // Act
            string json = JsonUtility.ToJson(objectToJson);
            
            // Assert
            Assert.That(json.Length, Is.GreaterThan(2));
        }
        
        [Test]
        public void T02_ToJson_EqualsFromJson_WhenString ()
        {
            // Arrange
            var objectToJson = "Hello World!";
            string json = JsonUtility.ToJson(objectToJson);
            
            // Act
            var objectFromJson = JsonUtility.FromJson<string>(json);

            // Assert
            Assert.That(objectFromJson.GetType(), Is.EqualTo(objectToJson.GetType()));
            Assert.That(objectFromJson, Is.EqualTo(objectToJson));
        }
        
        [Test]
        public void T03_ToJson_EqualsFromJson_WhenListString()
        {
            // Arrange
            var objectToJson = "Hello World!";
            var listObjectToJson = new List<string>();
            listObjectToJson.Add(objectToJson);
            string json = JsonUtility.ToJson(listObjectToJson);
            
            // Act
            var listObjectFromJson = JsonUtility.FromJson<List<string>>(json);

            // Assert
            Assert.That(listObjectFromJson.GetType(), Is.EqualTo(listObjectToJson.GetType()));
            Assert.That(listObjectToJson.Count, Is.EqualTo(1));
            Assert.That(listObjectFromJson.Count, Is.EqualTo(1));
            Assert.That(listObjectFromJson[0], Is.EqualTo(listObjectToJson[0]));
        }

        [Test]
        public void T04_ToJson_EqualsFromJson_WhenTestObject ()
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
        public void T05_ToJson_EqualsFromJson_WhenListTestObject ()
        {
            // Arrange
            var objectToJson = new TestObject();
            string title = objectToJson.Title = "MyTitle";
            int randomSeed = objectToJson.RandomSeed = 99;
           
            var listObjectToJson = new List<TestObject>();
            listObjectToJson.Add(objectToJson);
            string json = JsonUtility.ToJson(listObjectToJson);
            
            // Act
            var listObjectFromJson = JsonUtility.FromJson<List<TestObject>>(json);

            // Assert
            Assert.That(listObjectFromJson.GetType(), Is.EqualTo(listObjectToJson.GetType()));
            Assert.That(listObjectToJson.Count, Is.EqualTo(1));
            Assert.That(listObjectFromJson.Count, Is.EqualTo(1));
            Assert.That(listObjectFromJson[0].Title, Is.EqualTo(listObjectToJson[0].Title));
            Assert.That(listObjectFromJson[0].RandomSeed, Is.EqualTo(listObjectToJson[0].RandomSeed));
        }
        
        [Test]
        public void T06_ToJson_EqualsFromJson_WhenTestContentObject()
        {
            // Arrange
            var objectToJson = ScriptableObject.CreateInstance<TestContentObject>();
            string title = objectToJson.Title = "MyTitle";
            int randomSeed = objectToJson.RandomSeed = 99;
            string json = JsonUtility.ToJson(objectToJson);
            
            // Act
            var objectFromJson = JsonUtility.FromJson<TestContentObject>(json);

            // Assert
            Assert.That(objectFromJson.GetType(), Is.EqualTo(objectToJson.GetType()));
            Assert.That(objectFromJson.Title, Is.EqualTo(objectToJson.Title));
            Assert.That(objectFromJson.RandomSeed, Is.EqualTo(objectToJson.RandomSeed));
        }
        
        [Test]
        public void T07_ToJson_EqualsFromJson_WhenListTestContentObject()
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