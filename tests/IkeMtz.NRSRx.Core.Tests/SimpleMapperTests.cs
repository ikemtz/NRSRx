using System;
using System.Collections.Generic;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class SimpleMapperTests : BaseUnigrationTests
  {
    [TestMethod, TestCategory("Unit")]
    public void ValidateSimpleMapperWithNullables()
    {
      var source = new ValidationTestObjectA()
      {
        Id = Guid.NewGuid(),
        SampleDecimal = 65.99m,
        SampleGuid = Guid.NewGuid(),
        SampleInt = 69,
        SampleString = "Happy Days",
        SampleObject = "Sample Object",
      };
      source.SampleStrings.Add("This better work!");
      var dest = SimpleMapper<ValidationTestObjectA>.Instance.Convert(source);
      Assert.AreEqual(Guid.Empty, dest.Id);
      Assert.IsEmpty(dest.SampleStrings);
      Assert.AreEqual(source.SampleDecimal, dest.SampleDecimal);
      Assert.AreEqual(source.SampleGuid, dest.SampleGuid);
      Assert.AreEqual(source.SampleInt, dest.SampleInt);
      Assert.AreEqual(source.SampleString, dest.SampleString);
      Assert.AreEqual(source.SampleObject, dest.SampleObject);

      Assert.AreEqual(source.SampleNullableInt, dest.SampleNullableInt);
      Assert.AreEqual(source.SampleNullableDecimal, dest.SampleNullableDecimal);
      Assert.AreEqual(source.SampleNullableGuid, dest.SampleNullableGuid);
    }

    [TestMethod, TestCategory("Unit")]
    public void ValidateSimpleMapperWithEmptyGuids()
    {
      var source = new ValidationTestObjectA()
      {
        Id = Guid.Empty,
        SampleDecimal = 65.99m,
        SampleGuid = Guid.Empty,
        SampleInt = 69,
        SampleString = "Happy Days",
        SampleObject = "Sample Object",
      };
      source.SampleStrings.Add("This better work!");
      var dest = SimpleMapper<ValidationTestObjectA>.Instance.Convert(source);
      Assert.AreEqual(source.Id, dest.Id);
      Assert.IsEmpty(dest.SampleStrings);
      Assert.AreEqual(source.SampleDecimal, dest.SampleDecimal);
      Assert.AreEqual(source.SampleGuid, dest.SampleGuid);
      Assert.AreEqual(source.SampleInt, dest.SampleInt);
      Assert.AreEqual(source.SampleString, dest.SampleString);
      Assert.AreEqual(source.SampleObject, dest.SampleObject);

      Assert.AreEqual(source.SampleNullableInt, dest.SampleNullableInt);
      Assert.AreEqual(source.SampleNullableDecimal, dest.SampleNullableDecimal);
      Assert.AreEqual(source.SampleNullableGuid, dest.SampleNullableGuid);
    }

    [TestMethod, TestCategory("Unit")]
    public void ValidateSimpleMapper()
    {
      var source = new ValidationTestObjectA()
      {
        Id = Guid.NewGuid(),
        SampleDecimal = 65.99m,
        SampleGuid = Guid.NewGuid(),
        SampleInt = 69,
        SampleString = "Happy Days",
        SampleObject = "Sample Object",
        SampleNullableInt = 69 + 2,
        SampleNullableDecimal = 99.345m,
        SampleNullableGuid = Guid.NewGuid(),
        SampleNullNumber1 = 1,
        SampleNullNumber2 = 5.6m,
        SampleNullNumber5 = 67,
      };
      source.SampleStrings.Add("This better work!");
      var dest = SimpleMapper<ValidationTestObjectA, ValidationTestObjectB>.Instance.Convert(source);
      Assert.AreEqual(Guid.Empty, dest.Id);
      Assert.IsEmpty(dest.SampleStrings);
      Assert.AreEqual(source.SampleDecimal, dest.SampleDecimal);
      Assert.AreEqual(source.SampleGuid, dest.SampleGuid);
      Assert.AreEqual(source.SampleInt, dest.SampleInt);
      Assert.AreEqual(source.SampleString, dest.SampleString);
      Assert.AreEqual(source.SampleObject, dest.SampleObject);

      Assert.AreEqual(source.SampleNullableInt, dest.SampleNullableInt);
      Assert.AreEqual(source.SampleNullableDecimal, dest.SampleNullableDecimal);
      Assert.AreEqual(source.SampleNullableGuid, dest.SampleNullableGuid);

      _ = SimpleMapper<ValidationTestObjectB, ValidationTestObjectA>.Instance.Convert(dest);
    }
  }

  public class ValidationTestObjectA : IIdentifiable
  {
    public ValidationTestObjectA()
    {
      SampleStrings = new HashSet<string>();
    }
    public Guid Id { get; set; }
    public ICollection<string> SampleStrings { get; }
    public string SampleString { get; set; }
    public Guid SampleGuid { get; set; }
    public Object SampleObject { get; set; }
    public decimal SampleDecimal { get; set; }
    public int? SampleNullNumber1 { get; set; }
    public decimal? SampleNullNumber2 { get; set; }
    public int SampleNullNumber3 { get; set; }
    public decimal SampleNullNumber4 { get; set; }
    public long? SampleNullNumber5 { get; set; }
    public long SampleNullNumber6 { get; set; }
    public int SampleInt { get; set; }
    public int? SampleNullableInt { get; set; }
    public decimal? SampleNullableDecimal { get; set; }
    public Guid? SampleNullableGuid { get; set; }
  }


  public class ValidationTestObjectB : IIdentifiable
  {
    public ValidationTestObjectB()
    {
      SampleStrings = new HashSet<string>();
    }
    public Guid Id { get; set; }
    public ICollection<string> SampleStrings { get; }
    public string SampleString { get; set; }
    public Guid SampleGuid { get; set; }
    public Object SampleObject { get; set; }
    public decimal SampleDecimal { get; set; }
    public int SampleInt { get; set; }
    public int? SampleNullableInt { get; set; }
    public float? SampleNullNumber1 { get; set; }
    public float? SampleNullNumber2 { get; set; }
    public float SampleNullNumber3 { get; set; }
    public float SampleNullNumber4 { get; set; }
    public float SampleNullNumber5 { get; set; }
    public float? SampleNullNumber6 { get; set; }
    public decimal? SampleNullableDecimal { get; set; }
    public Guid? SampleNullableGuid { get; set; }
  }
}
