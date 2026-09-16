using System.ComponentModel;

namespace IkeMtz.Samples.Models.V1
{
  [DefaultValue(Unknown)]
  public enum Gender
  {
    Male = 'M',
    Female = 'F',
    Other = 'O',
    Unknown = 'U'
  }
}
