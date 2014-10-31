using System;

namespace NzbDrone.Core.Restrictions
{
    public class RestrictionRule
    {
        public RestrictionType Type { get; set; }
        public String Text { get; set; }
    }

    public enum RestrictionType
    {
        Or = 0,
        And = 1
    }
}
