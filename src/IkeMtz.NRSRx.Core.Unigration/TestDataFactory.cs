using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public static partial class TestDataFactory
  {
    public static readonly Regex NumberAfterSpace = new(@" \d", RegexOptions.None);

    public static TENTITY CreateIdentifiable<TENTITY>(TENTITY value = null)
      where TENTITY : class, IIdentifiable, new()
    {
      if (value == null)
      {
        value = new TENTITY();
      }
      value.Id = Guid.NewGuid();
      return value;
    }

    public static TENTITY CreateAuditable<TENTITY>(TENTITY value = null)
      where TENTITY : class, IAuditable, new()
    {
      if (value == null)
      {
        value = new TENTITY();
      }
      value.CreatedBy = "Auditable Factory";
      value.CreatedOnUtc = DateTime.UtcNow;
      return value;
    }

    public static string StringGenerator(int length, bool allowSpaces = false, string characterSet = CharacterSets.AlphaChars, int? seed = null)
    {
      var random = new Random(seed ?? DateTime.UtcNow.Millisecond);
      var sb = new StringBuilder();
      Enumerable.Range(1, length).ToList().ForEach(x =>
      sb.Append(characterSet.ElementAt(random.Next((x == 1 ? CharacterSets.UpperCase : characterSet).Length))));
      var result = sb.ToString();
      if (allowSpaces)
      {
        var averageWordSize = 5; //English (4.7)
        var spacesPerString = length / averageWordSize;
        var previousSpace = 1;
        for (int i = 0; i < spacesPerString; i++)
        {
          var nextSpace = random.Next(previousSpace, previousSpace + averageWordSize);
          result = result.Insert(nextSpace, " ");
          previousSpace = nextSpace + 2;
        }
        result = NumberAfterSpace.Replace(result, " ");
        result = result.Replace("  ", " ");
        return result[..length];
      }
      return result;
    }
  }
}
