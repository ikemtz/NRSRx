using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public static class SnapshotAsserter
  {
    public static void AssertEachLineIsEqual(string expected, string actual)
    {
      var expectedLines = expected.Split("\n").Select(x => x.Trim()).ToArray();
      var actualLines = actual.Split("\n").Select(x => x.Trim()).ToArray();

      for (int i = 0; actualLines.Length > i; i++)
      {
        Assert.AreEqual(expectedLines[i], actualLines[i]);
      }
    }
  }
}
