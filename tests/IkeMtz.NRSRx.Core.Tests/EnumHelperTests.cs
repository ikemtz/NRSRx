using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Models.Helpers;
using IkeMtz.NRSRx.Core.Unigration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class EnumHelperTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void TestConvertCharEnumValues()
    {
      var result = EnumHelper.ConvertEnumValues<CharEnum, char, CharEnumValue>();
      Assert.AreEqual(2, result.Count());
    }
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void TestConvertIntEnumValues()
    {
      var result = EnumHelper.ConvertEnumValues<IntEnum, IntEnumValue>();
      Assert.AreEqual(2, result.Count());
    }
  }

  public enum CharEnum
  {
    ValueA = 'A',
    ValueB = 'B'
  }
  public class CharEnumValue : IEnumValue<char>
  {
    public char Id { get; set; }
    public string Name { get; set; }
  }
  public enum IntEnum
  {
    Value1 = 1,
    Value2 = 2
  }
  public class IntEnumValue : IEnumValue
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }
}
