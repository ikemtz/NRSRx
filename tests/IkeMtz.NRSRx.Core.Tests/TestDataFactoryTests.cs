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
    [TestCategory(TestCategories.Unit)]
    public void GenerateAllChars()
    {
      var result = TestDataFactory.StringGenerator(50, true, CharacterSets.AlphaNumericChars);
      TestContext.WriteLine("Generated String is: {0}", result);
      Assert.DoesNotContain("  ", result, "String has at least two consecutive spaces.");
      Assert.IsTrue(char.IsUpper(result.First()), $"First Character is not capitalized: {result.First()}");
      Assert.IsGreaterThanOrEqualTo(result.Length, 50, $"Generated string was over the maximum: 50, actual: {result.Length}");
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void GenerateAlphaChars()
    {
      var result = TestDataFactory.StringGenerator(50);
      StringAssert.DoesNotMatch(result, new Regex(@"/d"));
      Assert.DoesNotContain("  ", result);
      Assert.IsTrue(char.IsUpper(result.First()));
      Assert.IsLessThanOrEqualTo(result.Length, 45, $"generated string is not long enough, expected: 50, actual: {result.Length}");
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void GenerateAlphaCharsWithSpaces()
    {
      var result = TestDataFactory.StringGenerator(50, true);
      Assert.DoesNotContain("  ", result);
      Assert.IsLessThanOrEqualTo(result.Length, 45, $"generated string is not long enough, expected: 50, actual: {result.Length}");
      Assert.IsTrue(char.IsUpper(result.First()));
    }
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void GenerateLowerChars()
    {
      var result = TestDataFactory.StringGenerator(50, true, CharacterSets.LowerCase);
      Assert.DoesNotContain("  ", result);
      Assert.IsTrue(char.IsLower(result.First()));
      Assert.IsLessThanOrEqualTo(result.Length, 45, $"generated string is not long enough, expected: 50, actual: {result.Length}");
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void GenerateNumbers()
    {
      var result = TestDataFactory.StringGenerator(6, false, characterSet: CharacterSets.Numeric);
      Assert.DoesNotContain("  ", result);
      Assert.AreEqual(6, result.Length);
      Assert.IsLessThanOrEqualTo(result.Length, 6, $"generated string is not long enough, expected: 6, actual: {result.Length}");
      Assert.IsTrue(char.IsNumber(result.First()));
    }
  }
}
