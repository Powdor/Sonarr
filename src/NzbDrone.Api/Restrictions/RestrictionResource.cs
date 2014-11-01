using System;
using System.Collections.Generic;
using NzbDrone.Api.REST;
using NzbDrone.Core.Restrictions;

namespace NzbDrone.Api.Restrictions
{
    public class RestrictionResource : RestResource
    {
        public List<RestrictionRule> Required { get; set; }
        public List<RestrictionRule> Preferred { get; set; }
        public List<RestrictionRule> Ignored { get; set; }
        public HashSet<Int32> Tags { get; set; }
    }
}
