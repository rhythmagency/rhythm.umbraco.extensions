using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhythm.Extensions.Types;
using System.Threading;

namespace Rhythm.Extensions.Test
{
    /// <summary>
    /// Summary description for InstanceByKeyCacheEx
    /// </summary>
    [TestClass]
    public class InstanceByKeyCacheExTest
    {
        public InstanceByKeyCacheExTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        public sealed class Client
        {
            private List<string> FirstNames = new List<string>() { "Lina", "Kristeen", "Lenita", "Aurore", "Euna", "Tanisha", "Mia", "Alfonso", "Jonelle", "Sade", "Dale", "Roy", "Halina", "Ivy", "Jettie", "Darrick", "Felica", "Krystal", "Rolanda", "Brittaney", "Marie", "Zetta", "Miyoko", "Iluminada", "Rosaura", "Carry", "Audrey", "Milan", "Arnoldo", "Romaine", "Detra", "Darrell", "Keshia", "Renita", "Carla", "Bryce", "Angela", "Charissa", "Megan", "Genna", "Barbar", "Ciera", "Junior", "Reina", "Karlene", "Judie", "Rusty", "Lawanna", "Ja", "Justin", "Beau", "Kenya", "Alene", "Ashleigh", "Librada", "Michiko", "Verline", "Felicia", "Silvana", "Kelli", "Suzi", "Cicely", "Nicki", "Sheryll", "Jacalyn", "Helene", "Natalia", "Joey", "Malorie", "Betsy", "Soraya", "Carola", "Joette", "Weldon", "Fran", "Harley", "Keith", "Zoila", "Simon", "Maggie", "Ed", "Ethan", "Yvone", "Gregg", "Deloise", "Demetrius", "Ilana", "Gwenda", "Esteban", "Luba", "Lashanda", "Milly", "Bonita", "Aimee", "Louann", "Tilda", "Eulah", "Joslyn", "Cameron" };

            public Client()
            {
                Random rs = new Random((int)DateTime.Now.Ticks);
                FirstName = FirstNames[rs.Next() % FirstNames.Count];
                LastName = FirstNames[rs.Next() % FirstNames.Count];
            }

            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        [TestMethod]
        public void TestCache()
        {
            //Create a Cache instance with an expiration time and replenisher function
            // Replenisher returns a new client with random first and last name
            var cache = new InstanceByKeyCacheEx<Client, string>(new TimeSpan(0, 0, 10), (x) =>
            {
                return new Client(); // returns random named client on each call
            });

            var key1 = "MyKey";

            var clientOrig = cache.Get(key1);
            var clientNew = cache.Get(key1, Enums.CacheGetMethod.NoCache); // gets replenisher result

            Assert.AreNotSame(clientOrig, clientNew, "Unless a random name was generated twice client2 should be different");

            clientNew = cache.Get(key1, Enums.CacheGetMethod.FromCache); // gets cache or default regardless if expired
            Assert.AreSame(clientOrig, clientNew, "These object should be the same but FromCache generated default(T)");

            clientNew = cache.Get(key1, Enums.CacheGetMethod.NoStore); // gets cache if not expired, else gets replenisher result
            Assert.AreSame(clientOrig, clientNew, "Assume the cache hasn't expired, These object should be the same but NoStore fetched a new Client");

            Thread.Sleep(new TimeSpan(0, 0, 11)); // sit and let the cache expire

            clientNew = cache.Get(key1, Enums.CacheGetMethod.NoStore);  // gets cache if not expired, else gets replenisher result
            Assert.AreNotSame(clientOrig, clientNew, "Assume the cache has expired, These object should not be the same but FromCache fetched an identical client");
            //Assert.AreEqual(clientOrig, client2, "Assume the cache has expired, These object should not be the same but FromCache fetched an identical client");
        }
    }
}
