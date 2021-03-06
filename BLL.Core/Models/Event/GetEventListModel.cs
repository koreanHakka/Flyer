﻿using Constants;
using System;
using System.Collections.Generic;

namespace BLL.Core.Models.Event
{
	public class GetEventListModel
	{
		public Guid EventUid { get; set; }
		public string Name { get; set; }
		public double XCoordinate { get; set; }
		public double YCoordinate { get; set; }
		public string Description { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public EventStatus Status { get; set; }
		public List<EventType> Types { get; set; }
		public Guid? EventPrimaryImageContentUid { get; set; }
		public bool? IsAdministrator { get; set; }
		public ParticipantStatus? ParticipantStatus { get; set; }
		public bool? AnyPersonWaitingForApprove { get; set; }
		public bool IsOpenForInvitations { get; set; }
		public bool IsOnline { get; set; }
		public long? CityId { get; set; }
		public string CityName { get; set; }
		public Guid ChatUid { get; set; }
	}
}