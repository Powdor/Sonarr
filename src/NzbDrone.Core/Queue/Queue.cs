﻿using System;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Qualities;
using NzbDrone.Core.Tv;
using NzbDrone.Core.Parser.Model;

namespace NzbDrone.Core.Queue
{
    public class Queue : ModelBase
    {
        public Series Series { get; set; }
        public Episode Episode { get; set; }
        public QualityModel Quality { get; set; }
        public Decimal Size { get; set; }
        public String Title { get; set; }
        public Decimal Sizeleft { get; set; }
        public TimeSpan? Timeleft { get; set; }
        public DateTime? EstimatedCompletionTime { get; set; }
        public String Status { get; set; }
        public String ErrorMessage { get; set; }
        public RemoteEpisode RemoteEpisode { get; set; }
    }
}
