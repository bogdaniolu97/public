﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransportTycoon.Library.Contracts;
using TransportTycoon.Library.Models;
using TransportTycoon.Library.Services;

namespace TransportTycoon.Tests
{
    [TestClass]
    [TestCategory("End to End")]
    public class EndToEndTests
    {
        private Endpoint[] endpoints;

        private App app;

        [TestInitialize]
        public void SetUp()
        {
            endpoints = new[]
            {
                new Endpoint("Factory"),
                new Endpoint("Port"),
                new Endpoint("A"),
                new Endpoint("B"),
            };

            var links = new[]
            {
                new Link(endpoints[0], endpoints[1], 1),
                new Link(endpoints[1], endpoints[2], 4),
                new Link(endpoints[0], endpoints[3], 5),
            };

            IMap map = new Map(endpoints, links);

            var vehicles = new[]
            {
                new Vehicle(endpoints[0]),
                new Vehicle(endpoints[0]),
                new Vehicle(endpoints[1]),
            };

            app = new App(map, new VehicleList(vehicles));
        }

        [TestClass]
        public class KnownResults : EndToEndTests
        {
            [TestMethod]
            public void Test1()
            {
                Assert.AreEqual(5, app.Run(endpoints[0], GetEndpoints("B")));
            }

            [TestMethod]
            public void Test2()
            {
                Assert.AreEqual(5, app.Run(endpoints[0], GetEndpoints("A")));
            }

            [TestMethod]
            public void Test3()
            {
                Assert.AreEqual(5, app.Run(endpoints[0], GetEndpoints("AB")));
            }

            [TestMethod]
            public void Test4()
            {
                Assert.AreEqual(5, app.Run(endpoints[0], GetEndpoints("BB")));
            }

            [TestMethod]
            public void Test5()
            {
                Assert.AreEqual(7, app.Run(endpoints[0], GetEndpoints("ABB")));
            }
        }

        [TestClass]
        public class UnknownResults : EndToEndTests
        {
            [TestMethod]
            public void Test1()
            {
                Assert.AreEqual(0, app.Run(endpoints[0], GetEndpoints("AABABBAB")));
            }

            [TestMethod]
            public void Test2()
            {
                Assert.AreEqual(0, app.Run(endpoints[0], GetEndpoints("ABBBABAAABBB")));
            }
        }

        //

        private IEnumerable<Endpoint> GetEndpoints(string str)
        {
            return str
                .Select(it => it.ToString())
                .Select(name => endpoints.First(it => it.Name == name))
                .ToArray();
        }
    }
}