namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Provides predefined sets of characters for various use cases.
  /// </summary>
  public sealed class CharacterSets
  {
    /// <summary>
    /// Uppercase alphabetic characters (A-Z).
    /// </summary>
    public const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Lowercase alphabetic characters (a-z).
    /// </summary>
    public const string LowerCase = "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Numeric characters (0-9).
    /// </summary>
    public const string Numeric = "0123456789";

    /// <summary>
    /// Special characters.
    /// </summary>
    public const string Special = "~`!@#$%^&*()_-+={}[]|\\<>,.?/:;\"'";

    /// <summary>
    /// Alphabetic characters (uppercase and lowercase).
    /// </summary>
    public const string AlphaChars = UpperCase + LowerCase;

    /// <summary>
    /// Alphanumeric characters (alphabetic and numeric).
    /// </summary>
    public const string AlphaNumericChars = AlphaChars + Numeric;

    /// <summary>
    /// All characters (alphanumeric and special).
    /// </summary>
    public const string AllChars = AlphaNumericChars + Special;
  }
}
