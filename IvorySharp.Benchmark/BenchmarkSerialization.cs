using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace IvorySharp.Benchmark
{
    [Serializable]
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
    }
    
    [CoreJob]
    public class BenchmarkSerialization
    {
        private Person _person;
        private byte[] _serializedPerson;

        [GlobalSetup]
        public void Setup()
        {
            _person = new Person {Id = 10, Name = "John", LastName = "Doe"};
            _serializedPerson = Serialize(_person);
        }

        [Benchmark]
        public void SerializeObject()
        {
            var serializedPerson = Serialize(_person);
            GC.KeepAlive(serializedPerson);
        }

        [Benchmark]
        public void DeserializeObject()
        {
            var deserializedPerson = Deserialize<Person>(_serializedPerson);
            GC.KeepAlive(deserializedPerson);
        }
        
        public static byte[] Serialize(object value)
        {   
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                IFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, value);
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }

        public static T Deserialize<T>(byte[] byteArray)
        {   
            T returnValue;
            using (var memoryStream = new MemoryStream(byteArray))
            {
                IFormatter binaryFormatter = new BinaryFormatter();
                returnValue = (T)binaryFormatter.Deserialize(memoryStream);    
            }
            return returnValue;
        }
    }
}