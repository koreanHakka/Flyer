﻿using DAL.Core.Entities;
using DAL.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Core.Interfaces
{
	public interface IEventRepository
	{
		Task CreateEvent(EventEntity eventEntity, CancellationToken cancellationToken = default);
		Task<EventEntity> GetEvent(Guid eventUid, CancellationToken cancellationToken = default);
		Task<List<EventEntity>> GetEvents(Guid personUid, CancellationToken cancellationToken = default);
		Task UpdateEvent(EventEntity eventEntity, CancellationToken cancellationToken = default);
		Task<bool> CheckEventExistence(Guid eventUid, CancellationToken cancellationToken = default);
		Task<string> GetEventNameByChatId(long chatId, CancellationToken cancellationToken = default);
		Task AddParticipant(PersonToEventEntity entity);
		Task UpdateParticipant(PersonToEventEntity entity);
		Task RemoveParticipant(PersonToEventEntity entity);
		Task<PersonToEventEntity> GetParticipant(Guid personUid, Guid eventUid);
		Task<EventEntity> GetRandomEvent(RepositoryRandomEventFilter filter);
		Task<List<EventEntity>> SearchForEvent(RepositoryEventSearchFilter repositoryFilter);
	}
}