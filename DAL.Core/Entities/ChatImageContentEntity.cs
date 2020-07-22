﻿using System;

namespace DAL.Core.Entities
{
	public class ChatImageContentEntity
	{
		public const string TableName = "ChatImageContent";

		public long ChatImageContentId { get; set; }
		public Guid ChatImageContentUid { get; set; }
		public byte[] Content { get; set; }
		public long ChatMessageId { get; set; }
	}
}
