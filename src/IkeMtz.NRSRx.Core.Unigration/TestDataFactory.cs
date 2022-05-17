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
      var charsetLength = characterSet.Length;
      Enumerable.Range(1, length).ToList().ForEach(x =>
        sb.Append(characterSet.ElementAt(random.Next(charsetLength))));
      var result = CapitalizeFirstChar(sb.ToString(), characterSet);
      if (allowSpaces)
      {
        result = InjectSpaces(length, random, result, characterSet);
      }
      return result;
    }

    public static string InjectSpaces(int length, Random random, string result, string characterSet)
    {
      var averageWordSize = 5; //English (4.7)
      var spacesPerString = (length / averageWordSize);
      var previousSpace = 2;
      for (int i = 0; i < spacesPerString; i++)
      {
        var nextSpace = random.Next(previousSpace, previousSpace + averageWordSize + 1);
        result = result.Insert(Math.Min(nextSpace, result.Length), " ");
        previousSpace = nextSpace + 2;
      }
      if (characterSet.Any(char.IsLetter))
      {
        result = NumberAfterSpace.Replace(result, " ");
      }
      result = result.Trim().Replace("  ", " ");
      return result[..length];
    }

    public static string CapitalizeFirstChar(string result, string characterSet)
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
