using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using System.Linq;
using SimpleCQRS.Commands;
using SimpleCQRS.Events;

namespace SimpleCQRS.Tests
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public void AllEventsShouldBeSerializable()
        {
            var allEvents = Assembly.GetAssembly(typeof (Event))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Event)));

            foreach (var eventType in allEvents)
            {
                Console.WriteLine("Testing serialization for {0} ...", eventType.FullName);
                var e = (Event)Activator.CreateInstance(eventType);
                TestSerialization(e);
            }
        }

        [Test]
        public void AllCommandsShouldBeSerializable()
        {
            var allCommands = Assembly.GetAssembly(typeof (Command))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Command)));

            foreach (var commandType in allCommands)
            {
                Console.WriteLine("Testing serialization for {0} ...", commandType.FullName);
                var c = (Command)Activator.CreateInstance(commandType);
                TestSerialization(c);
            }
        }

        private void TestSerialization(Event e)
        {
            Guid id = Guid.NewGuid();
            e.Id = id;
            e.Version = 123;

            var deserializedEvent = ToXmlAndBack(e);

            Assert.AreEqual(id, deserializedEvent.Id);
            Assert.AreEqual(123, deserializedEvent.Version);
        }

        private void TestSerialization(Command c)
        {
            var deserializedCommand = ToXmlAndBack(c);
        }

        public static T ToXmlAndBack<T>(T source)
        {
            if(!typeof(T).IsValueType && source == null)
                throw new ArgumentNullException("source", "reference type cannot be null");

            TextWriter sw = new StringWriter();
            var serializer = new XmlSerializer(source.GetType());
            serializer.Serialize(sw, source);
            var xml = sw.ToString();

            var target = (T)serializer.Deserialize(new StringReader(xml));

            return target;
        } 
    }
}