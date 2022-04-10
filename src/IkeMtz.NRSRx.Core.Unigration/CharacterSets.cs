namespace IkeMtz.NRSRx.Core.Unigration
{
  public sealed class CharacterSets
  {
    public const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    public const string Numeric = "0123456789";
    public const string Special = "~`!@#$%^&*()_-+={}[]|\\<>,.?/:;\"'";
    public const string AlphaChars = UpperCase + LowerCase;
    public const string AlphaNumericChars = AlphaChars + Numeric;
    public const string AllChars = AlphaNumericChars + Special;
  }
}
