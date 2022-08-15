using System.Linq;
using System.Text.RegularExpressions;
using IkeMtz.NRSRx.Core.Unigration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class TestDataFactoryTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public void GenerateAllChars()
    {
      var result = TestDataFactory.StringGenerator(50, true, CharacterSets.AlphaNumericChars); 
      Assert.IsFalse(result.Contains("  "));
      Assert.IsTrue(char.IsUpper(result.First()), "First Character is not capitalized: {0}", result.First());
      Assert.AreEqual(50, result.Length);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void GenerateAlphaChars()
    {
      var result = TestDataFactory.StringGenerator(50); 
      StringAssert.DoesNotMatch(result, new Regex(@"/d"));
      Assert.IsFalse(result.Contains("  "));
      Assert.IsTrue(char.IsUpper(result.First())); 
      Assert.IsTrue(45 <= result.Length, "generated string is not long enough, expected: 50, actual: {0}", result.Length);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void GenerateAlphaCharsWithSpaces()
    {
      var result = TestDataFactory.StringGenerator(50, true); 
      Assert.IsFalse(result.Contains("  "));
      Assert.IsTrue(45 <= result.Length, "generated string is not long enough, expected: 50, actual: {0}", result.Length);
      Assert.IsTrue(char.IsUpper(result.First()));
    }
    [TestMethod]
    [TestCategory("Unit")]
    public void GenerateLowerChars()
    {
      var result = TestDataFactory.StringGenerator(50, true, CharacterSets.LowerCase); 
      Assert.IsFalse(result.Contains("  "));
      Assert.IsTrue(char.IsLower(result.First())); 
      Assert.IsTrue(45 <= result.Length, "generated string is not long enough, expected: 50, actual: {0}", result.Length);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void GenerateNumbers()
    {
      var result = TestDataFactory.StringGenerator(6, false, characterSet: CharacterSets.Numeric); 
      Assert.IsFalse(result.Contains("  "));
      Assert.AreEqual(result.Length, 6);
      Assert.IsTrue(6 <= result.Length, "generated string is not long enough, expected: 6, actual: {0}", result.Length);
      Assert.IsTrue(char.IsNumber(result.First()));
    }
  }
}
