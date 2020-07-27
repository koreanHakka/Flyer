﻿using DAL.Core.Entities;
using DAL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Core.Interfaces
{
	public interface IPersonRepository
	{
		Task<PersonEntity> GetPerson(Guid uid, CancellationToken cancellationToken = default);
		Task CreatePerson(Guid personUid, CancellationToken cancellationToken = default);
		Task UpdatePerson(PersonEntity person, CancellationToken cancellationToken = default);
		Task AddFriendToPerson(Guid personUid, Guid friendUid, CancellationToken cancellationToken = default);
		Task RemoveFriendFromPerson(Guid personUid, Guid friendUid, CancellationToken cancellationToken = default);
		Task<bool> CheckPersonExistence(Guid personUid, CancellationToken cancellationToken = default);
		Task<bool> CheckPersonFriendExistence(Guid personUid, Guid friendUid, CancellationToken cancellationToken = default);
		Task<IEnumerable<PersonEntity>> GetPersonListByPage(int pageNumber, int pageSize, string query = null, CancellationToken cancellationToken = default);
		Task<List<PersonEntity>> GetAllPersonFriends(Guid personUid, CancellationToken cancellationToken = default);
		Task<PersonEntity> GetRandomPerson(RepositoryRandomPersonFilter filter, long personId);
	}
}