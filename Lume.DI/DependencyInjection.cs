﻿using BLL.Authorization;
using BLL.Authorization.Interfaces;
using BLL.Core;
using BLL.Core.Interfaces;
using BLL.Notification;
using BLL.Notification.Interfaces;
using DAL.Authorization;
using DAL.AzureStorage;
using DAL.AzureStorage.Interfaces;
using DAL.Core;
using DAL.Core.Interfaces;
using DAL.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Lume.DI
{
    public static class DependencyInjection
    {
        public static void RegisterLogics(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationLogic, AuthorizationLogic>();
            services.AddSingleton<IImageLogic, ImageLogic>();
            services.AddSingleton<IEventLogic, EventLogic>();
            services.AddSingleton<IPersonLogic, PersonLogic>();
            services.AddSingleton<IChatLogic, ChatLogic>();
            services.AddSingleton<ICityLogic, CityLogic>();
            services.AddSingleton<IPushNotificationService, PushNotificationService>();
            services.AddSingleton<IBadgeLogic, BadgeLogic>();
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationRepository, AuthorizationRepository>();
            services.AddSingleton<IPersonRepository, PersonRepository>();
            services.AddSingleton<IChatRepository, ChatRepository>();
            services.AddSingleton<IEventRepository, EventRepository>();
            services.AddSingleton<ICityRepository, CityRepository>();
            services.AddSingleton<IAzureStorageRepository, AzureStorageRepository>();
            services.AddSingleton<IBadgeRepository, BadgeRepository>();
        }

        public static void RegisterFactories(this IServiceCollection services)
        {
            services.AddSingleton<ICoreContextFactory, CoreContextFactory>();
            services.AddSingleton<AuthorizationContextFactory>();
        }

        public static void RegisterValidations(this IServiceCollection services)
        {
            services.AddTransient<IAuthorizationValidation, AuthorizationValidation>();
            services.AddTransient<IImageValidation, ImageValidation>();
            services.AddTransient<IEventValidation, EventValidation>();
            services.AddTransient<IPersonValidation, PersonValidation>();
            services.AddTransient<IChatValidation, ChatValidation>();
            services.AddTransient<IFriendValidation, FriendValidation>();
        }
    }
}
