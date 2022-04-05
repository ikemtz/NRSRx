namespace IkeMtz.NRSRx.Core.Unigration
{
  public sealed class CharacterSets
  {
    public const string AllCaps = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string AlphaChars = AllCaps + "abcdefghijklmnopqrstuvwxyz";
    public const string AlphaNumericChars = AlphaChars + "0123456789";
    public const string AllChars = AlphaNumericChars + "~`!@#$%^&*()_-+={}[]|\\<>,.?/:;\"'";
  }
}
