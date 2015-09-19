﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Licensing.Library.Contracts;
using Renfield.Licensing.Library.Models;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
{
  [TestClass]
  public class RemoteCheckerClientTests
  {
    private Mock<Sys> sys;
    private Mock<Remote> remote;
    private Mock<ResponseParser> parser;

    private RemoteCheckerClient sut;

    [TestInitialize]
    public void SetUp()
    {
      sys = new Mock<Sys>();
      remote = new Mock<Remote>();
      parser = new Mock<ResponseParser>();

      sut = new RemoteCheckerClient(sys.Object, remote.Object, parser.Object);
    }

    [TestClass]
    public class Check : RemoteCheckerClientTests
    {
      [TestMethod]
      public void RequestsTheProcessorId()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.Check(registration);

        sys.Verify(it => it.GetProcessorId());
      }

      [TestMethod]
      public void SendsKeyToTheServer()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.Check(registration);

        remote.Verify(it => it.Get(It.Is<string>(s => s.StartsWith("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}"))));
      }

      [TestMethod]
      public void SendsProcessorIdToTheServer()
      {
        var registration = ObjectMother.CreateRegistration();
        sys
          .Setup(it => it.GetProcessorId())
          .Returns("1");

        sut.Check(registration);

        remote.Verify(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId=1"));
      }

      [TestMethod]
      public void ParsesTheResponse()
      {
        var registration = ObjectMother.CreateRegistration();
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId="))
          .Returns("abc");

        sut.Check(registration);

        parser.Verify(it => it.Parse("abc"));
      }

      [TestMethod]
      public void UpdatesExpirationDateToReturnedValue()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Expiration = ObjectMother.OldDate;
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId="))
          .Returns("abc");
        parser
          .Setup(it => it.Parse("abc"))
          .Returns(new RemoteResponse {Key = "{D98F6376-94F7-4D82-AA37-FC00F0166700}", Expiration = ObjectMother.NewDate});

        sut.Check(registration);

        Assert.AreEqual(ObjectMother.NewDate, registration.Expiration);
      }

      [TestMethod]
      public void UpdatesExpirationDateToMinimumValueIfResponseIsInvalid()
      {
        var registration = ObjectMother.CreateRegistration();
        registration.Expiration = ObjectMother.OldDate;
        remote
          .Setup(it => it.Get("Key={D98F6376-94F7-4D82-AA37-FC00F0166700}&ProcessorId="))
          .Returns("abc");
        parser
          .Setup(it => it.Parse("abc"))
          .Returns(new RemoteResponse {Key = "def"});

        sut.Check(registration);

        Assert.AreEqual(DateTime.MinValue, registration.Expiration);
      }
    }

    [TestClass]
    public class Submit : RemoteCheckerClientTests
    {
      [TestMethod]
      public void RequestsTheProcessorId()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.Submit(registration);

        sys.Verify(it => it.GetProcessorId());
      }

      [TestMethod]
      public void EncodesTheRegistrationDetails()
      {
        var registration = ObjectMother.CreateRegistration();

        sut.Submit(registration);

        sys.Verify(it => it.Encode(
          It.Is<IEnumerable<KeyValuePair<string, string>>>(
            e => e.Any(pair => pair.Value == "{D98F6376-94F7-4D82-AA37-FC00F0166700}"))));
      }

      [TestMethod]
      public void SendsTheRegistrationDetailsToTheServer()
      {
        var registration = ObjectMother.CreateRegistration();
        sys
          .Setup(it => it.Encode(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
          .Returns("abc");

        sut.Submit(registration);

        remote.Verify(it => it.Post("abc"));
      }
    }
  }
}