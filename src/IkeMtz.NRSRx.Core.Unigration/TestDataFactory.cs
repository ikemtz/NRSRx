using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public static partial class TestDataFactory
  {
    static readonly Random random;
    static TestDataFactory()
    {
      random = new Random(DateTime.UtcNow.Millisecond);
    }
    public static readonly Regex NumberAfterSpace = new(@" \d", RegexOptions.None);

    public static TENTITY CreateIdentifiable<TENTITY>(TENTITY value = null)
      where TENTITY : class, IIdentifiable<Guid>, new()
    {
      _ = CreateIdentifiable<TENTITY, Guid>(value);
      value.Id = Guid.NewGuid();
      return value;
    }

    public static TENTITY CreateIdentifiable<TENTITY, TIdentityType>(TENTITY value = null)
      where TENTITY : class, IIdentifiable<TIdentityType>, new()
      where TIdentityType : IComparable
    {
      if (value == null)
      {
        value = new TENTITY();
      }
      value.Id = default;
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

    public static string StringGenerator(int maxLength, bool allowSpaces = false, string characterSet = CharacterSets.AlphaChars)
    {
      var sb = new StringBuilder();
      var charsetLength = characterSet.Length;
      Enumerable.Range(1, maxLength).ToList().ForEach(x =>
        sb.Append(characterSet.ElementAt(random.Next(charsetLength))));
      var result = CapitalizeFirstChar(sb, characterSet);
      if (allowSpaces)
      {
        return InjectSpaces(maxLength, random, result, characterSet).Trim();
      }
      return result.ToString().Trim();
    }

    public static string InjectSpaces(int length, Random random, StringBuilder sb, string characterSet)
    {
      var averageWordSize = 5; //English (4.7)
      var spacesPerString = (length / averageWordSize);
      var previousSpace = 2;
      for (int i = 0; i < spacesPerString; i++)
      {
        var nextSpace = random.Next(previousSpace, previousSpace + averageWordSize + 1);
        sb = sb.Insert(Math.Min(nextSpace, sb.Length), " ");
        previousSpace = nextSpace + 2;
      }
      var result = sb.ToString();
      if (characterSet.Any(char.IsLetter))
      {
        result = NumberAfterSpace.Replace(result, " ");
      }
      while(result.Contains("  "))
      {
        result = result.Replace("  ", " ");
      }
      return result[..Math.Min(length, result.Length)];
    }

    public static StringBuilder CapitalizeFirstChar(StringBuilder result, string characterSet)
    {
      if (characterSet.Any(char.IsUpper))
      {
        var firstChar = result[0];
        while (char.IsNumber(firstChar))
        {
          result = result.Remove(0, 1);
          firstChar = result[0];
        }
        if (char.IsLower(firstChar))
        {
          firstChar = char.ToUpper(firstChar);
          result = result.Remove(0, 1).Insert(0, firstChar.ToString());
        }
      }
      return result;
    }
  }
}
