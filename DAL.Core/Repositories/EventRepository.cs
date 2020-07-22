﻿using DAL.Core.Entities;
using DAL.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Core.Repositories
{
	public class EventRepository : IEventRepository
	{
		private readonly CoreContextFactory _dbContextFactory;
		public EventRepository(CoreContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<bool> CheckEventExistence(Guid eventUid, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.EventEntities.AnyAsync(x => x.EventUid == eventUid, cancellationToken);
			}
		}

		public async Task CreateEvent(EventEntity eventEntity, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				await context.AddAsync(eventEntity, cancellationToken);
				await context.SaveChangesAsync(cancellationToken);
			}
		}

		public async Task<EventEntity> GetEvent(Guid eventUid, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.EventEntities
					.Include(x => x.EventStatus)
					.Include(x => x.EventType)
					.Include(x => x.EventImageContent)
					.Include(x => x.Administrator)
						.ThenInclude(x => x.PersonImageContentEntity)
					.Include(x => x.Participants)
						.ThenInclude(x => x.Person)
							.ThenInclude(x => x.PersonImageContentEntity)
					.Include(x => x.Chat)
					.SingleOrDefaultAsync(x => x.EventUid == eventUid, cancellationToken);
			}
		}

		public async Task<string> GetEventNameByChatId(long chatId, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				var eventEntity = await context.EventEntities.SingleOrDefaultAsync(x => x.ChatId == chatId);
				return eventEntity.Name;
			}
		}

		public async Task<List<EventEntity>> GetEvents(Guid personUid, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.EventEntities
					.Include(x => x.EventStatus)
					.Include(x => x.EventType)
					.Include(x => x.EventImageContent)
					.Include(x => x.Administrator)
						.ThenInclude(x => x.PersonImageContentEntity)
					.Include(x => x.Participants)
						.ThenInclude(x => x.Person)
							.ThenInclude(x => x.PersonImageContentEntity)
					.Include(x => x.Chat)
					.Where(x => x.Administrator.PersonUid == personUid || x.Participants.Any(x => x.Person.PersonUid == personUid))
					.ToListAsync();
			}
		}

		public async Task UpdateEvent(EventEntity eventEntity, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				context.Update(eventEntity);
				await context.SaveChangesAsync(cancellationToken);
			}
		}

		public async Task<PersonToEventEntity> GetParticipant(Guid personUid, Guid eventUid)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.PersonToEventEntities
					.Include(x => x.Event)
					.Include(x => x.Person)
					.SingleAsync(x => x.Event.EventUid == eventUid && x.Person.PersonUid == personUid);
			}
		}

		public async Task RemoveParticipant(PersonToEventEntity entity)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				context.Remove(entity);
				await context.SaveChangesAsync();
			}
		}

		public async Task AddParticipant(PersonToEventEntity entity)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				await context.AddAsync(entity);
				await context.SaveChangesAsync();
			}
		}

		public async Task UpdateParticipant(PersonToEventEntity entity)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				context.Update(entity);
				await context.SaveChangesAsync();
			}
		}
	}
}
