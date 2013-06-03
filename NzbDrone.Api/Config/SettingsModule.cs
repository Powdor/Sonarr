﻿using System;
using System.Collections.Generic;
using Nancy;
using NzbDrone.Api.Extensions;
using NzbDrone.Api.REST;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Organizer;
using FluentValidation;

namespace NzbDrone.Api.Config
{
    public class NamingModule : NzbDroneRestModule<NamingConfigResource>
    {
        private readonly INamingConfigService _namingConfigService;

        public NamingModule(INamingConfigService namingConfigService)
            : base("config/naming")
        {
            _namingConfigService = namingConfigService;
            GetResourceSingle = GetNamingConfig;

            UpdateResource = UpdateNamingConfig;

            SharedValidator.RuleFor(c => c.MultiEpisodeStyle).InclusiveBetween(0, 3);
            SharedValidator.RuleFor(c => c.NumberStyle).InclusiveBetween(0, 3);
            SharedValidator.RuleFor(c => c.SeasonFolderFormat).NotEmpty();
            SharedValidator.RuleFor(c => c.Separator).Matches(@"\s|\s\-\s|\.");
        }

        private NamingConfigResource UpdateNamingConfig(NamingConfigResource resource)
        {
            return ToResource<NamingConfig>(_namingConfigService.Save, resource);
        }

        private NamingConfigResource GetNamingConfig()
        {
            return ToResource(_namingConfigService.GetConfig);
        }
    }

    public class NamingConfigResource : RestResource
    {
        public Boolean IncludeEpisodeTitle { get; set; }
        public Boolean ReplaceSpaces { get; set; }
        public Boolean UseSceneName { get; set; }
        public Int32 MultiEpisodeStyle { get; set; }
        public Int32 NumberStyle { get; set; }
        public String SeasonFolderFormat { get; set; }
        public String Separator { get; set; }
        public Boolean IncludeQuality { get; set; }
        public Boolean IncludeSeriesTitle { get; set; }
    }


    public class SettingsModule : NzbDroneApiModule
    {
        private readonly IConfigService _configService;
        private readonly IConfigFileProvider _configFileProvider;

        public SettingsModule(IConfigService configService, IConfigFileProvider configFileProvider)
            : base("/settings")
        {
            _configService = configService;
            _configFileProvider = configFileProvider;
            Get["/"] = x => GetGeneralSettings();
            Post["/"] = x => SaveGeneralSettings();

            Get["/host"] = x => GetHostSettings();          
            Post["/host"] = x => SaveHostSettings();
        }

        private Response SaveHostSettings()
        {
            var request = Request.Body.FromJson<Dictionary<string, object>>();
            _configFileProvider.SaveConfigDictionary(request);

            return GetHostSettings();
        }

        private Response GetHostSettings()
        {
            return _configFileProvider.GetConfigDictionary().AsResponse();
        }

        private Response GetGeneralSettings()
        {
            var collection = Request.Query.Collection;

            if (collection.HasValue && Boolean.Parse(collection.Value))
                return _configService.All().AsResponse();

            return _configService.AllWithDefaults().AsResponse();
        }

        private Response SaveGeneralSettings()
        {
            var request = Request.Body.FromJson<Dictionary<string, object>>();
            _configService.SaveValues(request);


            return request.AsResponse();
        }
    }
}