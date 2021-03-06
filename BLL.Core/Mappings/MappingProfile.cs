﻿using AutoMapper;
using BLL.Core.Models.Badge;
using BLL.Core.Models.Chat;
using BLL.Core.Models.City;
using BLL.Core.Models.Event;
using BLL.Core.Models.Person;
using Constants;
using DAL.Core.Entities;
using DAL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Core.Mappings
{
	public class MappingProfile: Profile
	{
		public MappingProfile()
		{
			CreateMap<RandomEventFilter, RepositoryRandomEventFilter>()
				.ForMember(dest => dest.EventTypes, opt => opt.MapFrom(src => src.EventTypes.Select(x => (long)x).ToList()));

			CreateMap<EventSearchFilter, RepositoryEventSearchFilter>();

			CreateMap<RandomPersonFilter, RepositoryRandomPersonFilter>();

			CreateMap<GetPersonListFilter, RepositoryGetPersonListFilter>();

			CreateMap<CityEntity, CityModel>();

			CreateMap<PersonEntity, PersonModel>()
				.ForMember(dest => dest.ImageContentUid,
				opt => opt.MapFrom(src => src.PersonImageContentEntity == null ? (Guid?)null : src.PersonImageContentEntity.PersonImageContentUid))
				.ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.City == null ? (long?)null : src.City.CityId))
				.ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City == null ? string.Empty : src.City.CityName));

			CreateMap<PersonEntity, PersonEventModel>()
				.ForMember(dest => dest.ImageContentUid,
				opt => opt.MapFrom(src => src.PersonImageContentEntity == null ? (Guid?)null : src.PersonImageContentEntity.PersonImageContentUid))
				.ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City == null ? string.Empty : src.City.CityName));

			CreateMap<AddEventModel, EventEntity>()
				.ForMember(dest => dest.EventStatusId, opt => opt.MapFrom(src => (long)src.Status))
				.ForMember(dest => dest.EventTypes, opt => opt.MapFrom(src => new List<EventTypeToEventEntity>(src.Types.Select(x => new EventTypeToEventEntity { EventTypeId = (long)x }))));

			CreateMap<EventEntity, GetEventModel>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => (EventStatus)src.EventStatusId))
				.ForMember(dest => dest.Types, opt => opt.MapFrom(src => src.EventTypes.Select(x => (EventType)x.EventTypeId)))
				.ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants.Select(x => x.Person)))
				.ForMember(dest => dest.EventPrimaryImageContentUid, 
				opt => opt.MapFrom(src => src.EventImageContentEntities == null ? (Guid?)null : src.EventImageContentEntities.SingleOrDefault(x => x.IsPrimary.HasValue && x.IsPrimary.Value).EventImageContentUid))
				.ForMember(dest => dest.Images, 
				opt => opt.MapFrom(
					src => src.EventImageContentEntities == null ? new List<Guid>() : src.EventImageContentEntities.Where(x => x.IsPrimary.HasValue && !x.IsPrimary.Value).Select(x => x.EventImageContentUid).ToList()))
				.ForMember(dest => dest.ChatUid, opt => opt.MapFrom(src => src.Chat == null ? (Guid?)null : src.Chat.ChatUid))
				.ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.City == null ? (long?)null : src.City.CityId))
				.ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City == null ? string.Empty : src.City.CityName));

			CreateMap<EventEntity, GetEventListModel>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => (EventStatus)src.EventStatusId))
				.ForMember(dest => dest.Types, opt => opt.MapFrom(src => src.EventTypes.Select(x => (EventType)x.EventTypeId)))
				.ForMember(dest => dest.EventPrimaryImageContentUid,
				opt => opt.MapFrom(src => src.EventImageContentEntities == null ? (Guid?)null : src.EventImageContentEntities.SingleOrDefault(x => x.IsPrimary.HasValue && x.IsPrimary.Value).EventImageContentUid))
				.ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.City == null ? (long?)null : src.City.CityId))
				.ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City == null ? string.Empty : src.City.CityName))
				.ForMember(dest => dest.ChatUid, opt => opt.MapFrom(src => src.Chat.ChatUid));

			CreateMap<ChatMessageEntity, ChatMessageModel>()
				.ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ChatImageContentEntities.Select(x => x.ChatImageContentUid)))
				.ForMember(dest => dest.MessageContent, opt => opt.MapFrom(src => src.Content))
				.ForMember(dest => dest.MessageUid, opt => opt.MapFrom(src => src.ChatMessageUid))
				.ForMember(dest => dest.PersonUid, opt => opt.MapFrom(src => src.Author.PersonUid))
				.ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.Author.Name))
				.ForMember(dest => dest.PersonImageUid, opt => opt.MapFrom(src => src.Author.PersonImageContentEntity == null ? (Guid?)null : src.Author.PersonImageContentEntity.PersonImageContentUid));

			CreateMap<ChatEntity, ChatModel>();

			CreateMap<ChatEntity, ChatListModel>();

			CreateMap<FeedbackModel, FeedbackEntity>();

			CreateMap<PersonToBadgeEntity, BadgeModel>()
				.ForMember(dest => dest.BadgeImageUid, opt => opt.MapFrom(src => src.Badge.BadgeImageUid));

			CreateMap<BadgeEntity, BadgeModel>()
				.ForMember(dest => dest.Name, opt => opt.Ignore())
				.ForMember(dest => dest.BadgeName, opt => opt.MapFrom(src => (BadgeNames)src.BadgeId));
		}
	}
}
