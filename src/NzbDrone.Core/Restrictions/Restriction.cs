using System;
using System.Collections.Generic;
using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Restrictions
{
    public class Restriction : ModelBase
    {
        public List<RestrictionRule> Required { get; set; }
        public List<RestrictionRule> Preferred { get; set; }
        public List<RestrictionRule> Ignored { get; set; }
        public HashSet<Int32> Tags { get; set; }

        public Restriction()
        {
            Required = new List<RestrictionRule>();
            Preferred = new List<RestrictionRule>();
            Ignored = new List<RestrictionRule>();
            Tags = new HashSet<Int32>();
        }
    }
}
