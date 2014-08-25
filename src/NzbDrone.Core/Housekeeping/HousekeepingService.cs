﻿using System;
using System.Collections.Generic;
using NLog;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.Core.Lifecycle;
using NzbDrone.Core.Messaging.Commands;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Housekeeping
{
    public class HousekeepingService : IExecute<HousekeepingCommand>, IHandleAsync<ApplicationStartedEvent>
    {
        private readonly IEnumerable<IHousekeepingTask> _housekeepers;
        private readonly Logger _logger;
        private readonly IDatabase _mainDb;

        public HousekeepingService(IEnumerable<IHousekeepingTask> housekeepers, IDatabase mainDb, Logger logger)
        {
            _housekeepers = housekeepers;
            _logger = logger;
            _mainDb = mainDb;
        }

        private void Clean()
        {
            _logger.Info("Running housecleaning tasks");

            foreach (var housekeeper in _housekeepers)
            {
                try
                {
                    housekeeper.Clean();
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Error running housekeeping task: " + housekeeper.GetType().FullName, ex);
                }
            }

            //Only Vaccuum the DB in production
            if (RuntimeInfoBase.IsProduction)
            {
                // Vacuuming the log db isn't needed since that's done hourly at the TrimLogCommand.
                _logger.Debug("Compressing main database after housekeeping");
                _mainDb.Vacuum();
            }
        }

        public void Execute(HousekeepingCommand message)
        {
            Clean();
        }

        public void HandleAsync(ApplicationStartedEvent message)
        {
            Clean();
        }
    }
}
