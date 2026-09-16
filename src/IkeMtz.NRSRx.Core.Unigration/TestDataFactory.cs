using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Provides methods for generating test data.
  /// </summary>
  public static partial class TestDataFactory
  {
    static readonly Random random;

    static TestDataFactory()
    {
      random = new Random(DateTime.UtcNow.Millisecond);
    }

    /// <summary>
    /// A regular expression to match a number after a space.
    /// </summary>
    public static readonly Regex NumberAfterSpace = new(@" \d", RegexOptions.None);

    /// <summary>
    /// Creates an identifiable entity with a new GUID.
    /// </summary>
    /// <typeparam name="TENTITY">The type of the entity.</typeparam>
    /// <param name="value">An optional existing entity to update.</param>
    /// <returns>The created or updated entity.</returns>
    public static TENTITY CreateIdentifiable<TENTITY>(TENTITY? value = null)
      where TENTITY : class, IIdentifiable<Guid>, new()
    {
      value = CreateIdentifiable<TENTITY, Guid>(value);
      value.Id = Guid.NewGuid();
      return value;
    }

    /// <summary>
    /// Creates an identifiable entity with a default identity value.
    /// </summary>
    /// <typeparam name="TENTITY">The type of the entity.</typeparam>
    /// <typeparam name="TIdentityType">The type of the identity.</typeparam>
    /// <param name="value">An optional existing entity to update.</param>
    /// <returns>The created or updated entity.</returns>
    public static TENTITY CreateIdentifiable<TENTITY, TIdentityType>(TENTITY? value = null)
      where TENTITY : class, IIdentifiable<TIdentityType>, new()
      where TIdentityType : IComparable
    {
      value ??= new TENTITY();
      value.Id = default;
      return value;
    }

    /// <summary>
    /// Generates a random string of the specified length.
    /// </summary>
    /// <param name="maxLength">The maximum length of the string.</param>
    /// <param name="allowSpaces">Whether to allow spaces in the string.</param>
    /// <param name="characterSet">The set of characters to use.</param>
    /// <returns>The generated string.</returns>
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

    /// <summary>
    /// Injects spaces into a string at random positions.
    /// </summary>
    /// <param name="length">The length of the string.</param>
    /// <param name="random">The random number generator.</param>
    /// <param name="sb">The string builder containing the string.</param>
    /// <param name="characterSet">The set of characters to use.</param>
    /// <returns>The string with spaces injected.</returns>
    public static string InjectSpaces(int length, Random random, StringBuilder sb, string characterSet)
    {
      var averageWordSize = 5; // English (4.7)
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
      while (result.Contains("  "))
      {
        result = result.Replace("  ", " ");
      }
      return result[..Math.Min(length, result.Length)];
    }

    /// <summary>
    /// Capitalizes the first character of a string if the character set contains uppercase letters.
    /// </summary>
    /// <param name="result">The string builder containing the string.</param>
    /// <param name="characterSet">The set of characters to use.</param>
    /// <returns>The string builder with the first character capitalized.</returns>
    public static StringBuilder CapitalizeFirstChar(StringBuilder result, string characterSet)
    {
      if (characterSet.Any(char.IsUpper) && !char.IsUpper(result[0]))
      {
        result = result.Insert(0, "G");
      }
      return result;
    }

    /// <summary>
    /// Generates a collection of unique entities using the provided generator function.
    /// </summary>
    /// <typeparam name="TEntity">The entity type produced by the <paramref name="generator"/>. Must implement <see cref="IIdentifiable{TIdentityType}"/>.</typeparam>
    /// <typeparam name="TIdentityType">The type of the identity value for the entity's <c>Id</c>. Must implement <see cref="IComparable"/>.</typeparam>
    /// <param name="generator">A function that produces new instances of <typeparamref name="TEntity"/>.</param>
    /// <param name="count">The number of unique entities to generate.</param>
    /// <param name="duplicateDetector">An optional comparer that determines whether two entities are considered the same. 
    /// If null, entities are considered equal when their <c>Id</c> values are equal using the default equality comparer for <typeparamref name="TIdentityType"/>.</param>
    /// <returns>A collection containing <paramref name="count"/> unique entities generated by <paramref name="generator"/>.</returns>
    public static ICollection<TEntity> GenerateUniqueObjects<TEntity, TIdentityType>(Func<TEntity> generator, int count, Func<TEntity, TEntity, bool>? duplicateDetector = null)
      where TIdentityType : IComparable
      where TEntity : class, IIdentifiable<TIdentityType>
    {
      return GenerateUniqueObjects(generator, count, duplicateDetector ?? ((x, y) => EqualityComparer<TIdentityType>.Default.Equals(x.Id, y.Id)));
    }

    /// <summary>
    /// Generates a collection of unique values by repeatedly invoking the supplied <paramref name="generator"/> until
    /// <paramref name="count"/> unique items have been produced.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity produced by <paramref name="generator"/>.</typeparam>
    /// <param name="generator">A function that produces new instances of <typeparamref name="TEntity"/>. This function will be invoked repeatedly until <paramref name="count"/> unique items are produced.</param>
    /// <param name="count">The number of unique entities to generate. If zero, an empty collection is returned.</param>
    /// <param name="duplicateDetector">
    /// An optional function used to determine whether two instances are considered duplicates. The function is invoked
    /// with an existing item and a newly generated candidate and should return <c>true</c> when the two are considered the same.
    /// If <c>null</c>, the default comparison uses the C# reference equality operator (<c>==</c>).
    /// </param>
    /// <returns>
    /// A collection containing exactly <paramref name="count"/> entities produced by <paramref name="generator"/> that are
    /// unique according to <paramref name="duplicateDetector"/> (or reference equality when the detector is <c>null</c>).
    /// </returns>
    public static ICollection<TEntity> GenerateUniqueObjects<TEntity>(Func<TEntity> generator, int count, Func<TEntity, TEntity, bool>? duplicateDetector = null)
    where TEntity : class
    {
      duplicateDetector ??= (x, y) => x == y;
      var uniqueValues = new List<TEntity>();
      while (uniqueValues.Count < count)
      {
        var newValue = generator();
        if (!uniqueValues.Any(a => duplicateDetector(a, newValue)))
        {
          uniqueValues.Add(newValue);
        }
      }
      return uniqueValues;
    }
  }
}
