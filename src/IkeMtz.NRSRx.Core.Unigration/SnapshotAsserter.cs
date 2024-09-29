using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Provides methods for asserting that snapshots of text are equal.
  /// </summary>
  public static class SnapshotAsserter
  {
    /// <summary>
    /// Asserts that each line in the expected text is equal to the corresponding line in the actual text.
    /// </summary>
    /// <param name="expected">The expected text.</param>
    /// <param name="actual">The actual text.</param>
    public static void AssertEachLineIsEqual(string expected, string actual)
    {
      var expectedLines = expected.Split("\n").Select(x => x.Trim()).ToArray();
      var actualLines = actual.Split("\n").Select(x => x.Trim()).ToArray();

      for (int i = 0; actualLines.Length > i; i++)
      {
        Assert.AreEqual(expectedLines[i], actualLines[i], $"string mismatch in line #{i}.");
      }
    }
  }
}
