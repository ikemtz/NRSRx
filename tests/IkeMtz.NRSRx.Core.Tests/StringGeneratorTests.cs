using System.Linq;
using IkeMtz.NRSRx.Core.Unigration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IkeMtz.NRSRx.Core.Unigration.TestDataFactory;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class StringGeneratorTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void GenerateStringWithSpacesTest()
    {
      string? lastString = null;
      //Trying to replicate random out of index issue and need enough runs to replicate
      Enumerable.Range(0, 10000).ToList().ForEach((x) =>
       {
         var result = StringGenerator(1000, true);
         Assert.AreNotEqual(lastString, result);
         Assert.IsTrue(990 <= result.Length, "generated string is not long enough, expected: 990, actual: {0}", result.Length);
         lastString = result;
       });
    }
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void GenerateStringsTest()
    {
      string? lastString = null;
      //Trying to replicate random out of index issue and need enough runs to replicate
      Enumerable.Range(0, 10000).ToList().ForEach((x) =>
      {
        var result = StringGenerator(1000, false);
        Assert.AreNotEqual(lastString, result);
        Assert.IsTrue(990 <= result.Length, "generated string is not long enough, expected: 990, actual: {0}", result.Length);
        lastString = result;
      });
    }
  }
}
