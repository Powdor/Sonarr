﻿using System.Collections.Generic;
using System.Linq;
using NzbDrone.Core.Datastore.Events;
using NzbDrone.Core.Download.Pending;
using NzbDrone.Core.Messaging.Commands;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Queue;

namespace NzbDrone.Api.Queue
{
    public class QueueModule : NzbDroneRestModuleWithSignalR<QueueResource, Core.Queue.Queue>,
                               IHandle<UpdateQueueEvent>, IHandle<PendingReleasesUpdatedEvent>
    {
        private readonly IQueueService _queueService;
        private readonly IPendingReleaseService _pendingReleaseService;

        public QueueModule(ICommandExecutor commandExecutor, IQueueService queueService, IPendingReleaseService pendingReleaseService)
            : base(commandExecutor)
        {
            _queueService = queueService;
            _pendingReleaseService = pendingReleaseService;
            GetResourceAll = GetQueue;
        }

        private List<QueueResource> GetQueue()
        {
            return ToListResource(GetQueueItems);
        }

        private IEnumerable<Core.Queue.Queue> GetQueueItems()
        {
            var queue = _queueService.GetQueue();
            var pending = _pendingReleaseService.GetPendingQueue();

            return queue.Concat(pending);
        }

        public void Handle(UpdateQueueEvent message)
        {
            BroadcastResourceChange(ModelAction.Sync);
        }

        public void Handle(PendingReleasesUpdatedEvent message)
        {
            BroadcastResourceChange(ModelAction.Sync);
        }
    }
}