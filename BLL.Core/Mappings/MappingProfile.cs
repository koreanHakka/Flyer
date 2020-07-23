﻿using AutoMapper;
using BLL.Core.Models.Chat;
using BLL.Core.Models.Event;
using BLL.Core.Models.Person;
using Constants;
using DAL.Core.Entities;
using DAL.Core.Models;
using System;
using System.Linq;

namespace BLL.Core.Mappings
{
	public class MappingProfile: Profile
	{
		public MappingProfile()
		{
			CreateMap<RandomEventFilter, RepositoryRandomEventFilter>();

			CreateMap<PersonEntity, PersonModel>()
				.ForMember(dest => dest.ImageContentUid, 
				opt => opt.MapFrom(src => src.PersonImageContentEntity == null ? (Guid?)null : src.PersonImageContentEntity.PersonImageContentUid));

			CreateMap<AddEventModel, EventEntity>()
				.ForMember(dest => dest.EventStatusId, opt => opt.MapFrom(src => (long)src.Status))
				.ForMember(dest => dest.EventTypeId, opt => opt.MapFrom(src => (long)src.Type));

			CreateMap<EventEntity, GetEventModel>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => (EventStatus)src.EventStatusId))
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => (EventType)src.EventTypeId))
				.ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants.Select(x => x.Person)))
				.ForMember(dest => dest.EventImageContentUid, opt => opt.MapFrom(src => src.EventImageContent == null ? (Guid?)null : src.EventImageContent.EventImageContentUid))
				.ForMember(dest => dest.ChatUid, opt => opt.MapFrom(src => src.Chat == null ? (Guid?)null : src.Chat.ChatUid));

			CreateMap<EventEntity, GetEventListModel>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => (EventStatus)src.EventStatusId))
				.ForMember(dest => dest.Type, opt => opt.MapFrom(src => (EventType)src.EventTypeId));

			CreateMap<ChatMessageEntity, ChatMessageModel>()
				.ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ChatImageContentEntities.Select(x => x.ChatImageContentUid)))
				.ForMember(dest => dest.MessageContent, opt => opt.MapFrom(src => src.Content))
				.ForMember(dest => dest.MessageUid, opt => opt.MapFrom(src => src.ChatMessageUid))
				.ForMember(dest => dest.PersonUid, opt => opt.MapFrom(src => src.Author.PersonUid))
				.ForMember(dest => dest.PersonName, opt => opt.MapFrom(src => src.Author.Name))
				.ForMember(dest => dest.PersonImageUid, opt => opt.MapFrom(src => src.Author.PersonImageContentEntity == null ? (Guid?)null : src.Author.PersonImageContentEntity.PersonImageContentUid));

			CreateMap<ChatEntity, ChatModel>();

			CreateMap<ChatEntity, ChatListModel>();
		}
	}
}
