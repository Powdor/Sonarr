using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using NzbDrone.Common;
using NzbDrone.Common.Disk;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Download;
using NzbDrone.Core.MediaFiles.Commands;
using NzbDrone.Core.MediaFiles.EpisodeImport;
using NzbDrone.Core.Messaging.Commands;

namespace NzbDrone.Core.MediaFiles
{
    public class DownloadedEpisodesCommandService : IExecute<DownloadedEpisodesScanCommand>
    {
        private readonly IDownloadedEpisodesImportService _downloadedEpisodesImportService;
        private readonly IDownloadTrackingService _downloadTrackingService;
        private readonly IDiskProvider _diskProvider;
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public DownloadedEpisodesCommandService(IDownloadedEpisodesImportService downloadedEpisodesImportService,
                                                IDownloadTrackingService downloadTrackingService,
                                                IDiskProvider diskProvider,
                                                IConfigService configService,
                                                Logger logger)
        {
            _downloadedEpisodesImportService = downloadedEpisodesImportService;
            _downloadTrackingService = downloadTrackingService;
            _diskProvider = diskProvider;
            _configService = configService;
            _logger = logger;
        }

        private List<ImportResult> ProcessDroneFactoryFolder(DownloadedEpisodesScanCommand message)
        {
            var downloadedEpisodesFolder = _configService.DownloadedEpisodesFolder;

            if (String.IsNullOrEmpty(downloadedEpisodesFolder))
            {
                _logger.Trace("Drone Factory folder is not configured");
                return new List<ImportResult>();
            }

            if (!_diskProvider.FolderExists(downloadedEpisodesFolder))
            {
                _logger.Warn("Drone Factory folder [{0}] doesn't exist.", downloadedEpisodesFolder);
                return new List<ImportResult>();
            }

            return _downloadedEpisodesImportService.ProcessRootFolder(new System.IO.DirectoryInfo(downloadedEpisodesFolder));
        }

        private List<ImportResult> ProcessFolder(DownloadedEpisodesScanCommand message)
        {
            if (!_diskProvider.FolderExists(message.Path))
            {
                _logger.Warn("Folder specified for import scan [{0}] doesn't exist.", message.Path);
                return new List<ImportResult>();
            }

            if (message.DownloadClientId.IsNotNullOrWhiteSpace())
            {
                var trackedDownload = _downloadTrackingService.GetQueuedDownloads().Where(v => v.DownloadItem.DownloadClientId == message.DownloadClientId).FirstOrDefault();

                if (trackedDownload == null)
                {
                    // TODO: Debug, Info or Warn?
                    _logger.Warn("External directory scan request for unknown download {0}, attempting normal import. [{1}]", message.DownloadClientId, message.Path);

                    return _downloadedEpisodesImportService.ProcessFolder(new System.IO.DirectoryInfo(message.Path));
                }
                else
                {
                    var results = _downloadedEpisodesImportService.ProcessFolder(new System.IO.DirectoryInfo(message.Path), trackedDownload.DownloadItem);
                    if (results.Any() && results.All(v => v.Result == ImportResultType.Imported))
                    {
                        trackedDownload.State = TrackedDownloadState.Imported;
                    }
                    return results;
                }
            }
            else
            {
                return _downloadedEpisodesImportService.ProcessFolder(new System.IO.DirectoryInfo(message.Path));
            }
        }

        public void Execute(DownloadedEpisodesScanCommand message)
        {
            List<ImportResult> importResults;

            try
            {
                if (message.Path.IsNotNullOrWhiteSpace())
                {
                    importResults = ProcessFolder(message);
                }
                else
                {
                    importResults = ProcessDroneFactoryFolder(message);
                }

                if (importResults == null || !importResults.Any(v => v.Result == ImportResultType.Imported))
                {
                    // TODO: report error in cmd
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("An error occurred during the Downloaded Episodes scan.", ex);
                message.Failed(ex);
            }
        }
    }
}
