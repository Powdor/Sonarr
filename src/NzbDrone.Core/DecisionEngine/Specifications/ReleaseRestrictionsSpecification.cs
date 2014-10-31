using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NzbDrone.Common;
using NzbDrone.Core.IndexerSearch.Definitions;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Restrictions;

namespace NzbDrone.Core.DecisionEngine.Specifications
{
    public class ReleaseRestrictionsSpecification : IDecisionEngineSpecification
    {
        private readonly IRestrictionService _restrictionService;
        private readonly Logger _logger;

        public ReleaseRestrictionsSpecification(IRestrictionService restrictionService, Logger logger)
        {
            _restrictionService = restrictionService;
            _logger = logger;
        }

        public RejectionType Type { get { return RejectionType.Permanent; } }

        public virtual Decision IsSatisfiedBy(RemoteEpisode subject, SearchCriteriaBase searchCriteria)
        {
            _logger.Debug("Checking if release meets restrictions: {0}", subject);

            var title = subject.Release.Title;
            var restrictions = _restrictionService.AllForTags(subject.Series.Tags);
            var requiredAnd = restrictions.SelectMany(r => r.Required.Where(rule => rule.Type == RestrictionType.And)).ToList();
            var requiredOr = restrictions.SelectMany(r => r.Required.Where(rule => rule.Type == RestrictionType.Or)).ToList();
            var ignoredAnd = restrictions.SelectMany(r => r.Ignored.Where(rule => rule.Type == RestrictionType.And)).ToList();
            var ignoredOr = restrictions.SelectMany(r => r.Ignored.Where(rule => rule.Type == RestrictionType.Or)).ToList();

            if (requiredAnd.NotAll(rule => RuleApplies(rule, title)))
            {
                _logger.Debug("{0} Does not contain all the required terms: {1}", title, String.Join(", ", requiredAnd.Select(s => s.Text)));
                return Decision.Reject("Does not contain all the required terms: {0}", String.Join(", ", requiredAnd.Select(s => s.Text)));
            }

            if (requiredOr.Any() && requiredOr.None(rule => RuleApplies(rule, title)))
            {
                _logger.Debug("[{0}] Does not contain at least one required term: {1}", title, String.Join(", ", requiredOr.Select(s => s.Text)));
                return Decision.Reject("Does not contain at least one required term: {0}", String.Join(", ", requiredOr.Select(s => s.Text)));
            }

            if (ignoredAnd.Any() && ignoredAnd.All(rule => RuleApplies(rule, title)))
            {
                _logger.Debug("{0} Contains all the ignored terms: {1}", title, String.Join(", ", ignoredAnd.Select(s => s.Text)));
                return Decision.Reject("Contains all the ignored terms: {0}", String.Join(", ", ignoredAnd.Select(s => s.Text)));
            }

            if (ignoredOr.Any(rule => RuleApplies(rule, title)))
            {
                _logger.Debug("[{0}] Contains at least one ignored term: {1}", title, String.Join(", ", ignoredOr.Select(s => s.Text)));
                return Decision.Reject("Contains at least one ignored term: {0}", String.Join(", ", ignoredOr.Select(s => s.Text)));
            }

            _logger.Debug("[{0}] No restrictions apply, allowing", subject);
            return Decision.Accept();
        }

        private static Boolean RuleApplies(RestrictionRule rule, String title)
        {
            return title.ToLowerInvariant().Contains(rule.Text.ToLowerInvariant());
        }
    }
}
