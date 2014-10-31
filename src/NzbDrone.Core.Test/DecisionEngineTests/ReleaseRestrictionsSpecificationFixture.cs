using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.DecisionEngine.Specifications;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Restrictions;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Tv;

namespace NzbDrone.Core.Test.DecisionEngineTests
{
    [TestFixture]
    public class ReleaseRestrictionsSpecificationFixture : CoreTest<ReleaseRestrictionsSpecification>
    {
        private RemoteEpisode _parseResult;

        [SetUp]
        public void Setup()
        {
            _parseResult = new RemoteEpisode
                           {
                               Series = new Series
                                        {
                                            Tags = new HashSet<Int32>()
                                        },
                               Release = new ReleaseInfo
                                         {
                                             Title = "Dexter.S08E01.EDITED.WEBRip.x264-KYR"
                                         }
                           };
        }

        private void GivenRestictionRules(List<RestrictionRule> required, List<RestrictionRule> ignored)
        {
            Mocker.GetMock<IRestrictionService>()
                  .Setup(s => s.AllForTags(It.IsAny<HashSet<Int32>>()))
                  .Returns(new List<Restriction>
                           {
                               new Restriction
                               {
                                   Required = required,
                                   Ignored = ignored
                               }
                           });
        }

        [Test]
        public void should_be_true_when_restrictions_are_empty()
        {
            Mocker.GetMock<IRestrictionService>()
                  .Setup(s => s.AllForTags(It.IsAny<HashSet<Int32>>()))
                  .Returns(new List<Restriction>());

            Subject.IsSatisfiedBy(_parseResult, null).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_be_true_when_title_contains_all_anded_required_terms()
        {
            GivenRestictionRules(new List<RestrictionRule>
                                 {
                                     new RestrictionRule { Type = RestrictionType.And, Text = "WEBRip" },
                                     new RestrictionRule { Type = RestrictionType.And, Text = "x264" }
                                 },
                                 new List<RestrictionRule>());

            Subject.IsSatisfiedBy(_parseResult, null).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_be_true_when_title_contains_one_ored_required_term()
        {
            GivenRestictionRules(new List<RestrictionRule>
                                 {
                                     new RestrictionRule { Type = RestrictionType.Or, Text = "WEBRip" },
                                     new RestrictionRule { Type = RestrictionType.Or, Text = "optional" }
                                 },
                                 new List<RestrictionRule>());

            Subject.IsSatisfiedBy(_parseResult, null).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_be_false_when_title_does_not_contain_all_anded_required_terms()
        {
            GivenRestictionRules(new List<RestrictionRule>
                                 {
                                     new RestrictionRule { Type = RestrictionType.And, Text = "WEBRip" },
                                     new RestrictionRule { Type = RestrictionType.And, Text = "required" }
                                 },
                                 new List<RestrictionRule>());

            Subject.IsSatisfiedBy(_parseResult, null).Accepted.Should().BeFalse();
        }

        [Test]
        public void should_be_false_when_title_does_not_contain_any_ored_required_terms()
        {
            GivenRestictionRules(new List<RestrictionRule>
                                 {
                                     new RestrictionRule { Type = RestrictionType.Or, Text = "opt" },
                                     new RestrictionRule { Type = RestrictionType.Or, Text = "optional" }
                                 },
                                 new List<RestrictionRule>());

            Subject.IsSatisfiedBy(_parseResult, null).Accepted.Should().BeFalse();
        }

        [Test]
        public void should_be_true_when_title_does_not_contain_all_anded_ignored_terms()
        {
            GivenRestictionRules(new List<RestrictionRule>(),
                                 new List<RestrictionRule>
                                 {
                                     new RestrictionRule {Type = RestrictionType.And, Text = "WEBRip"},
                                     new RestrictionRule {Type = RestrictionType.And, Text = "ignored"}
                                 });
                                 

            Subject.IsSatisfiedBy(_parseResult, null).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_be_true_when_title_does_not_contain_any_ored_ignored_terms()
        {
            GivenRestictionRules(new List<RestrictionRule>(),
                                 new List<RestrictionRule>
                                 {
                                     new RestrictionRule {Type = RestrictionType.Or, Text = "iggy"},
                                     new RestrictionRule {Type = RestrictionType.Or, Text = "ignored"}
                                 });

            Subject.IsSatisfiedBy(_parseResult, null).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_be_false_when_title_contains_all_anded_ignored_terms()
        {
            GivenRestictionRules(new List<RestrictionRule>(),
                                 new List<RestrictionRule>
                                 {
                                     new RestrictionRule {Type = RestrictionType.And, Text = "edited"},
                                     new RestrictionRule {Type = RestrictionType.And, Text = "WEBRip"}
                                 });

            Subject.IsSatisfiedBy(_parseResult, null).Accepted.Should().BeFalse();
        }
    }
}
