using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.Parser;
using NzbDrone.Core.Test.Framework;

namespace NzbDrone.Core.Test.ParserTests
{

    [TestFixture]
    public class SingleEpisodeParserFixture : CoreTest
    {
        [TestCase("Sonny.With.a.Chance.S02E15", "Sonny.With.a.Chance", 2, 15)]
        [TestCase("Two.and.a.Half.Me.103.720p.HDTV.X264-DIMENSION", "Two.and.a.Half.Me", 1, 3)]
        [TestCase("Two.and.a.Half.Me.113.720p.HDTV.X264-DIMENSION", "Two.and.a.Half.Me", 1, 13)]
        [TestCase("Two.and.a.Half.Me.1013.720p.HDTV.X264-DIMENSION", "Two.and.a.Half.Me", 10, 13)]
        [TestCase("Chuck.4x05.HDTV.XviD-LOL", "Chuck", 4, 5)]
        [TestCase("The.Girls.Next.Door.S03E06.DVDRip.XviD-WiDE", "The.Girls.Next.Door", 3, 6)]
        [TestCase("Degrassi.S10E27.WS.DSR.XviD-2HD", "Degrassi", 10, 27)]
        [TestCase("Parenthood.2010.S02E14.HDTV.XviD-LOL", "Parenthood 2010", 2, 14)]
        [TestCase("Hawaii Five 0 S01E19 720p WEB DL DD5 1 H 264 NT", "Hawaii Five 0", 1, 19)]
        [TestCase("The Event S01E14 A Message Back 720p WEB DL DD5 1 H264 SURFER", "The Event", 1, 14)]
        [TestCase("Adam Hills In Gordon St Tonight S01E07 WS PDTV XviD FUtV", "Adam Hills In Gordon St Tonight", 1, 7)]
        [TestCase("Adam Hills In Gordon St Tonight S01E07 WS PDTV XviD FUtV", "Adam Hills In Gordon St Tonight", 1, 7)]
        [TestCase("Adventure.Inc.S03E19.DVDRip.XviD-OSiTV", "Adventure.Inc", 3, 19)]
        [TestCase("S03E09 WS PDTV XviD FUtV", "", 3, 9)]
        [TestCase("5x10 WS PDTV XviD FUtV", "", 5, 10)]
        [TestCase("Castle.2009.S01E14.HDTV.XviD-LOL", "Castle 2009", 1, 14)]
        [TestCase("Pride.and.Prejudice.1995.S03E20.HDTV.XviD-LOL", "Pride and Prejudice 1995", 3, 20)]
        [TestCase("The.Office.S03E115.DVDRip.XviD-OSiTV", "The.Office", 3, 115)]
        [TestCase(@"Parks and Recreation - S02E21 - 94 Meetings - 720p TV.mkv", "Parks and Recreation", 2, 21)]
        [TestCase(@"24-7 Penguins-Capitals- Road to the NHL Winter Classic - S01E03 - Episode 3.mkv", "24-7 Penguins-Capitals- Road to the NHL Winter Classic", 1, 3)]
        [TestCase("Adventure.Inc.S03E19.DVDRip.\"XviD\"-OSiTV", "Adventure.Inc", 3, 19)]
        [TestCase("Hawaii Five-0 (2010) - 1x05 - Nalowale (Forgotten/Missing)", "Hawaii Five-0 (2010)", 1, 5)]
        [TestCase("Hawaii Five-0 (2010) - 1x05 - Title", "Hawaii Five-0 (2010)", 1, 5)]
        [TestCase("House - S06E13 - 5 to 9 [DVD]", "House", 6, 13)]
        [TestCase("The Mentalist - S02E21 - 18-5-4", "The Mentalist", 2, 21)]
        [TestCase("Breaking.In.S01E07.21.0.Jump.Street.720p.WEB-DL.DD5.1.h.264-KiNGS", "Breaking In", 1, 7)]
        [TestCase("CSI.525", "CSI", 5, 25)]
        [TestCase("King of the Hill - 10x12 - 24 Hour Propane People [SDTV]", "King of the Hill", 10, 12)]
        [TestCase("Brew Masters S01E06 3 Beers For Batali DVDRip XviD SPRiNTER", "Brew Masters", 1, 6)]
        [TestCase("24 7 Flyers Rangers Road to the NHL Winter Classic Part01 720p HDTV x264 ORENJI", "24 7 Flyers Rangers Road to the NHL Winter Classic", 1, 1)]
        [TestCase("24 7 Flyers Rangers Road to the NHL Winter Classic Part 02 720p HDTV x264 ORENJI", "24 7 Flyers Rangers Road to the NHL Winter Classic", 1, 2)]
        [TestCase("24-7 Flyers-Rangers- Road to the NHL Winter Classic - S01E01 - Part 1", "24 7 Flyers Rangers Road to the NHL Winter Classic", 1, 1)]
        [TestCase("S6E02-Unwrapped-(Playing With Food) - [DarkData]", "", 6, 2)]
        [TestCase("S06E03-Unwrapped-(Number Ones Unwrapped) - [DarkData]", "", 6, 3)]
        [TestCase("The Mentalist S02E21 18 5 4 720p WEB DL DD5 1 h 264 EbP", "The Mentalist", 2, 21)]
        [TestCase("01x04 - Halloween, Part 1 - 720p WEB-DL", "", 1, 4)]
        [TestCase("extras.s03.e05.ws.dvdrip.xvid-m00tv", "Extras", 3, 5)]
        [TestCase("castle.2009.416.hdtv-lol", "Castle 2009", 4, 16)]
        [TestCase("hawaii.five-0.2010.217.hdtv-lol", "Hawaii Five-0 (2010)", 2, 17)]
        [TestCase("Looney Tunes - S1936E18 - I Love to Singa", "Looney Tunes", 1936, 18)]
        [TestCase("American_Dad!_-_7x6_-_The_Scarlett_Getter_[SDTV]", "American Dad!", 7, 6)]
        [TestCase("Falling_Skies_-_1x1_-_Live_and_Learn_[HDTV-720p]", "Falling Skies", 1, 1)]
        [TestCase("Top Gear - 07x03 - 2005.11.70", "Top Gear", 7, 3)]
        [TestCase("Glee.S04E09.Swan.Song.1080p.WEB-DL.DD5.1.H.264-ECI", "Glee", 4, 9)]
        [TestCase("S08E20 50-50 Carla [DVD]", "", 8, 20)]
        [TestCase("Cheers S08E20 50-50 Carla [DVD]", "Cheers", 8, 20)]
        [TestCase("S02E10 6-50 to SLC [SDTV]", "", 2, 10)]
        [TestCase("Franklin & Bash S02E10 6-50 to SLC [SDTV]", "Franklin & Bash", 2, 10)]
        [TestCase("The_Big_Bang_Theory_-_6x12_-_The_Egg_Salad_Equivalency_[HDTV-720p]", "The Big Bang Theory", 6, 12)]
        [TestCase("Top_Gear.19x06.720p_HDTV_x264-FoV", "Top Gear", 19, 6)]
        [TestCase("Portlandia.S03E10.Alexandra.720p.WEB-DL.AAC2.0.H.264-CROM.mkv", "Portlandia", 3, 10)]
        [TestCase("(Game of Thrones s03 e - \"Game of Thrones Season 3 Episode 10\"", "Game of Thrones", 3, 10)]
        [TestCase("House.Hunters.International.S05E607.720p.hdtv.x264", "House.Hunters.International", 5, 607)]
        [TestCase("Adventure.Time.With.Finn.And.Jake.S01E20.720p.BluRay.x264-DEiMOS", "Adventure.Time.With.Finn.And.Jake", 1, 20)]
        [TestCase("Hostages.S01E04.2-45.PM.[HDTV-720p].mkv", "Hostages", 1, 4)]
        [TestCase("S01E04", "", 1, 4)]
        [TestCase("1x04", "", 1, 4)]
        [TestCase("10.Things.You.Dont.Know.About.S02E04.Prohibition.HDTV.XviD-AFG", "10 Things You Dont Know About", 2, 4)]
        [TestCase("30 Rock - S01E01 - Pilot.avi", "30 Rock", 1, 1)]
        [TestCase("666 Park Avenue - S01E01", "666 Park Avenue", 1, 1)]
        [TestCase("Warehouse 13 - S01E01", "Warehouse 13", 1, 1)]
        [TestCase("Don't Trust The B---- in Apartment 23.S01E01", "Don't Trust The B---- in Apartment 23", 1, 1)]
        [TestCase("Warehouse.13.S01E01", "Warehouse.13", 1, 1)]
        [TestCase("Dont.Trust.The.B----.in.Apartment.23.S01E01", "Dont.Trust.The.B----.in.Apartment.23", 1, 1)]
        [TestCase("24 S01E01", "24", 1, 1)]
        [TestCase("24.S01E01", "24", 1, 1)]
        [TestCase("Homeland - 2x12 - The Choice [HDTV-1080p].mkv", "Homeland", 2, 12)]
        [TestCase("Homeland - 2x4 - New Car Smell [HDTV-1080p].mkv", "Homeland", 2, 4)]
        [TestCase("Top Gear - 06x11 - 2005.08.07", "Top Gear", 6, 11)]
        [TestCase("The_Voice_US_s06e19_04.28.2014_hdtv.x264.Poke.mp4", "The Voice US", 6, 19)]
        [TestCase("the.100.110.hdtv-lol", "The 100", 1, 10)]
        [TestCase("2009x09 [SDTV].avi", "", 2009, 9)]
        [TestCase("S2009E09 [SDTV].avi", "", 2009, 9)]
        [TestCase("Shark Week S2009E09 [SDTV].avi", "Shark Week", 2009, 9)]
        [TestCase("St_Elsewhere_209_Aids_And_Comfort", "St Elsewhere", 2, 9)]
        [TestCase("[Impatience] Locodol - 0x01 [720p][34073169].mkv", "Locodol", 0, 1)]
        [TestCase("South.Park.S15.E06.City.Sushi", "South Park", 15, 6)]
        [TestCase("South Park - S15 E06 - City Sushi", "South Park", 15, 6)]
        //[TestCase("", "", 0, 0)]
        public void should_parse_single_episode(string postTitle, string title, int seasonNumber, int episodeNumber)
        {
            var result = Parser.Parser.ParseTitle(postTitle);
            result.Should().NotBeNull();
            result.EpisodeNumbers.Should().HaveCount(1);
            result.SeasonNumber.Should().Be(seasonNumber);
            result.EpisodeNumbers.First().Should().Be(episodeNumber);
            result.SeriesTitle.Should().Be(title.CleanSeriesTitle());
            result.AbsoluteEpisodeNumbers.Should().BeEmpty();
            result.FullSeason.Should().BeFalse();
        }
    }
}
