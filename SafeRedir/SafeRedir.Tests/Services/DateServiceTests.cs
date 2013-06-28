﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SafeRedir.Services;

namespace Renfield.SafeRedir.Tests.Services
{
  [TestClass]
  public class DateServiceTests
  {
    private DateService sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new DateService();
    }

    [TestClass]
    public class GetRange : DateServiceTests
    {
      [TestMethod]
      public void Between()
      {
        var result = sut.GetRange(new DateTime(2000, 1, 1), new DateTime(2001, 3, 5));

        Assert.AreEqual("records between 2000-01-01 and 2001-03-05", result);
      }

      [TestMethod]
      public void FromDate()
      {
        var result = sut.GetRange(new DateTime(2000, 1, 1), DateTime.MaxValue);

        Assert.AreEqual("records after 2000-01-01", result);
      }

      [TestMethod]
      public void ToDate()
      {
        var result = sut.GetRange(DateTime.MinValue, new DateTime(2001, 3, 5));

        Assert.AreEqual("records before 2001-03-05", result);
      }

      [TestMethod]
      public void AllRecords()
      {
        var result = sut.GetRange(DateTime.MinValue, DateTime.MaxValue);

        Assert.AreEqual("all records", result);
      }
    }
  }
}