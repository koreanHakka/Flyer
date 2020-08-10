﻿using System;
using System.Threading.Tasks;

namespace BLL.Core.Interfaces
{
	public interface IImageLogic
	{
		Task<Guid> SaveImage(byte[] content);
		Task<byte[]> GetImage(Guid imageUid);
	}
}