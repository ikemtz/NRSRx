using System;

namespace IkeMtz.NRSRx.Core.Models
{
    public interface IAuditable
    {
        string CreatedBy { get; set; }
        string UpdatedBy { get; set; }
        DateTimeOffset CreatedOnUtc { get; set; }
        DateTimeOffset? UpdatedOnUtc { get; set; }
    }
}
