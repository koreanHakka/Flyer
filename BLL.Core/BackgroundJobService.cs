﻿using BLL.Core.Interfaces;
using BLL.Notification;
using BLL.Notification.Interfaces;
using Constants;
using DAL.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Core
{
	public class BackgroundJobService : IHostedService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IBadgeLogic _badgeLogic;
        private readonly IPushNotificationService _pushNotificationService;
        private Timer _timer;
        private TimeSpan _borderTime = new TimeSpan(1, 0, 0);


        public BackgroundJobService(IEventRepository eventRepository,
            IBadgeLogic badgeLogic,
            IPushNotificationService pushNotificationService)
		{
            _eventRepository = eventRepository;
            _badgeLogic = badgeLogic;
            _pushNotificationService = pushNotificationService;
        }

        private void DoWork(object state)
        {
            var prelaunchedEvents = _eventRepository.GetListOfLatestEvents(_borderTime).Result;
            var prelaunchParticipants = prelaunchedEvents.SelectMany(x => x.Participants).Where(x => x.ParticipantStatusId == (long)ParticipantStatus.Active);
			foreach (var participant in prelaunchParticipants)
			{
				if (participant.Person != null && !string.IsNullOrWhiteSpace(participant.Person.Token))
				{
					_ = _pushNotificationService.SendPushNotification(participant.Person.Token,
						MessageTitles.EventPreLaunchNotification,
						new Dictionary<FirebaseNotificationKeys, string> { [FirebaseNotificationKeys.Url] = string.Format(FirebaseNotificationTemplates.EventUrlTemplate, participant.Event.EventUid) },
                        "Lume",
                        participant.Event.Name);
                }
            }
			if (prelaunchedEvents.Any())
			{
                _eventRepository.SetPrelaunchNotificationsFlag(prelaunchedEvents).Wait();
            }

            var closedEvents = _eventRepository.TransferEventsStatuses().Result;
            _eventRepository.RemoveOutdatedParticipants();
            _badgeLogic.AddBadges(closedEvents);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(180));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
