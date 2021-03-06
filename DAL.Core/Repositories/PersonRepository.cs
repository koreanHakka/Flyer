﻿using DAL.Core.Entities;
using DAL.Core.Interfaces;
using DAL.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Core.Repositories
{
	public class PersonRepository : IPersonRepository
	{
		private readonly ICoreContextFactory _dbContextFactory;
		public PersonRepository(ICoreContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<PersonEntity> GetPerson(Guid uid, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.PersonEntities
					.Include(x => x.PersonImageContentEntity)
					.Include(x => x.FriendList)
						.ThenInclude(x => x.Friend)
							.ThenInclude(x => x.PersonImageContentEntity)
					.Include(x => x.City)
					.SingleOrDefaultAsync(x => x.PersonUid == uid, cancellationToken);
			}
		}

		public async Task CreatePerson(Guid personUid, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				await context.AddAsync(new PersonEntity { PersonUid = personUid }, cancellationToken);
				await context.SaveChangesAsync(cancellationToken);
			}
		}

		public async Task<List<string>> GetLoginList(string login)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.PersonEntities.Where(x => x.Login.StartsWith(login)).Select(x => x.Login).ToListAsync();
			}
		}

		public async Task<List<long>> GetPersonSwipeHistory(long personId)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.PersonSwipeHistoryEntities.Where(x => x.PersonId == personId)
					.Select(x => x.EventId).ToListAsync();
			}
		}

		public async Task UpdatePerson(PersonEntity person, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				context.Update(person);
				await context.SaveChangesAsync(cancellationToken);
			}
		}

		public async Task<bool> CheckPersonExistence(Guid personUid, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.PersonEntities.AnyAsync(x => x.PersonUid == personUid, cancellationToken);
			}
		}

		public async Task<bool> CheckPersonFriendExistence(Guid personUid, Guid friendUid, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.PersonFriendListEntities
					.AnyAsync(p => p.Person.PersonUid == personUid && p.Friend.PersonUid == friendUid);
			}
		}

		public async Task AddFriendToPerson(Guid personUid, Guid friendUid, bool isApproved, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				var person = await context.PersonEntities
					.FirstOrDefaultAsync(p => p.PersonUid == personUid);

				var friend = await context.PersonEntities
					.FirstOrDefaultAsync(p => p.PersonUid == friendUid);

				var personToFriendEntity = new PersonFriendListEntity { PersonId = person.PersonId, FriendId = friend.PersonId, IsApproved = isApproved };

				await context.AddAsync(personToFriendEntity);

				await context.SaveChangesAsync();
			}
		}

		public async Task RemoveFriendFromPerson(Guid personUid, Guid friendUid, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				var personToFriendEntity = await context.PersonFriendListEntities
					.Include(x => x.Person)
					.Include(x => x.Friend)
					.SingleOrDefaultAsync(p => p.Person.PersonUid == personUid && p.Friend.PersonUid == friendUid);

				context.Remove(personToFriendEntity);

				await context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<PersonEntity>> GetPersonListByPage(Guid personUid, RepositoryGetPersonListFilter filter, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				var personFriendList = await context.PersonFriendListEntities.Include(x => x.Person).Where(x => x.Person.PersonUid == personUid).Select(x => x.FriendId).ToListAsync();

				var query = context.PersonEntities
					.Include(x => x.PersonImageContentEntity)
					.Include(x => x.FriendList)
						.ThenInclude(x => x.Friend)
							.ThenInclude(x => x.PersonImageContentEntity)
					.Include(x => x.City)
					.AsNoTracking();

				query = query.Where(x => x.PersonUid != personUid);

				if (!string.IsNullOrWhiteSpace(filter.Query))
				{
					query = query.Where(p => (p.Name != null && p.Name.Contains(filter.Query) ||
						p.Login != null && p.Login.Contains(filter.Query)));
				}

				if (filter.CityId.HasValue)
				{
					query = query.Where(p => p.CityId == filter.CityId);
				}

				if (personFriendList.Any())
				{
					query = query.OrderByDescending(x => personFriendList.Contains(x.PersonId));
				}

				return await query.Skip(filter.PageSize * (filter.PageNumber - 1))
					.Take(filter.PageSize)
					.ToListAsync(cancellationToken);
			}
		}

		public async Task<List<PersonEntity>> GetAllPersonFriends(Guid personUid, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.PersonFriendListEntities
					.Include(p => p.Friend)
						.ThenInclude(x => x.City)
					.Include(p => p.Friend)
						.ThenInclude(f => f.PersonImageContentEntity)
					.Where(p => p.Person.PersonUid == personUid)
					.Select(p => p.Friend)
					.ToListAsync(cancellationToken);
			}
		}

		public async Task<PersonEntity> GetRandomPerson(RepositoryRandomPersonFilter filter, long personId)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				var query = context.PersonEntities.Include(x => x.Events).AsNoTracking();

				query = query.Where(x => x.PersonId != personId &&
					!x.Events.Any(x => x.EventId == filter.EventId) &&
					!filter.IgnoringPersonList.Contains(x.PersonId) &&
					!string.IsNullOrEmpty(x.Name) &&
					x.CityId.HasValue);
				
				if (filter.MinAge.HasValue)
				{
					query = query.Where(x => x.Age >= filter.MinAge.Value);
				}
				if (filter.MaxAge.HasValue)
				{
					query = query.Where(x => x.Age <= filter.MaxAge.Value);
				}
				if (filter.CityId.HasValue)
				{
					query = query.Where(x => x.CityId == filter.CityId);
				}

				var random = new Random();

				var personList = await query.Select(x => x.PersonId).ToListAsync();
				if (!personList.Any())
				{
					return null;
				}
				var randomPersonId = personList.ElementAt(random.Next(0, personList.Count()));

				return await context.PersonEntities
					.Include(x => x.PersonImageContentEntity)
					.Include(x => x.FriendList)
						.ThenInclude(x => x.Friend)
							.ThenInclude(x => x.PersonImageContentEntity)
					.Include(x => x.City)
					.SingleOrDefaultAsync(x => x.PersonId == randomPersonId);
			}
		}

		public async Task AddPersonSwipeHistoryRecord(PersonSwipeHistoryEntity entity)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				await context.AddAsync(entity);
				await context.SaveChangesAsync();
			}
		}

		public async Task<bool> CheckPersonExistence(Guid personUid, string login, CancellationToken cancellationToken = default)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.PersonEntities.AnyAsync(x => personUid != x.PersonUid && x.Login == login, cancellationToken);
			}
		}

		public async Task RemovePersonImage(PersonImageContentEntity entity)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				context.PersonImageContentEntities.Remove(entity);
				await context.SaveChangesAsync();
			}
		}

		public async Task ConfirmFriend(Guid uid, Guid friendGuid)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				var entity = await context.PersonFriendListEntities.Include(x => x.Person).Include(x => x.Friend).SingleOrDefaultAsync(x => x.Person.PersonUid == uid && x.Friend.PersonUid == friendGuid);
				if (entity != null)
				{
					entity.IsApproved = true;
					context.Update(entity);
					await context.SaveChangesAsync();
				}
			}
		}

		public async Task<List<PersonEntity>> GetNewFriends(Guid uid)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.PersonFriendListEntities
					.Include(x => x.Person)
					.Where(x => x.Person.PersonUid == uid && !x.IsApproved)
					.Select(x => x.Friend)
					.ToListAsync();
			}
		}

		public async Task AddFeedback(FeedbackEntity entity)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				await context.FeedbackEntities.AddAsync(entity);
				await context.SaveChangesAsync();
			}
		}

		public async Task<List<PersonEntity>> GetPersonList(List<Guid> personUids)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.PersonEntities
					.Include(x => x.PersonImageContentEntity)
					.Include(x => x.FriendList)
						.ThenInclude(x => x.Friend)
							.ThenInclude(x => x.PersonImageContentEntity)
					.Include(x => x.City)
					.Where(x => personUids.Contains(x.PersonUid))
					.ToListAsync();
			}
		}

		public async Task<PersonEntity> GetPersonByToken(string token)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.PersonEntities
					.SingleOrDefaultAsync(x => x.Token == token);
			}
		}

		public async Task RemoveTokenForEveryPerson(string token)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				var entities = context.PersonEntities.Where(x => x.Token == token);
				if (await entities.AnyAsync())
				{
					foreach (var entity in entities)
					{
						entity.Token = null;
					}
					context.UpdateRange(entities);
					await context.SaveChangesAsync();
				}
			}
		}

		public async Task<PersonImageContentEntity> GetPersonImage(Guid imageUid)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				return await context.PersonImageContentEntities.SingleOrDefaultAsync(x => x.PersonImageContentUid == imageUid);
			}
		}

		public async Task AddReport(PersonReportEntity reportEntity)
		{
			using (var context = _dbContextFactory.CreateDbContext())
			{
				await context.PersonReportEntities.AddAsync(reportEntity);
				await context.SaveChangesAsync();
			}
		}
	}
}