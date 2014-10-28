using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace NzbDrone.Core.Restrictions
{
    public class RestrictionService
    {
        private readonly IRestrictionRepository _repo;
        private readonly Logger _logger;

        public RestrictionService(IRestrictionRepository repo, Logger logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public List<Restriction> All()
        {
            return _repo.All().ToList();
        }

        public List<Restriction> AllForTag(Int32 tagId)
        {
            return _repo.All().Where(r => r.Tags.Contains(tagId)).ToList();
        }

        public List<Restriction> AllForTags(HashSet<Int32> tagIds)
        {
            return _repo.All().Where(r => r.Tags.Intersect(tagIds).Any()).ToList();
        }
    }
}
