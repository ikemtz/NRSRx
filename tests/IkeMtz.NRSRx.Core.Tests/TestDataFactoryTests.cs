using System.Linq;
using System.Text.RegularExpressions;
using IkeMtz.NRSRx.Core.Unigration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class TestDataFactoryTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public void GenerateAllChars()
    {
      var result = TestDataFactory.StringGenerator(50, true, CharacterSets.AlphaNumericChars);
      Assert.IsFalse(result.Contains("  "));
      Assert.IsTrue(char.IsUpper(result.First()));
      Assert.AreEqual(result.Length, 50);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void GenerateAlphaChars()
    {
      var result = TestDataFactory.StringGenerator(50);
      StringAssert.DoesNotMatch(result, new Regex(@"/d"));
      Assert.IsFalse(result.Contains("  "));
      Assert.IsTrue(char.IsUpper(result.First()));
      Assert.AreEqual(result.Length, 50);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void GenerateAlphaCharsWithSpaces()
    {
      var result = TestDataFactory.StringGenerator(50, true);
      Assert.IsFalse(result.Contains("  "));
      Assert.AreEqual(result.Length, 50);
      Assert.IsTrue(char.IsUpper(result.First()));
    }
  }
}
