using System;
using System.Collections.Generic;
using Core.Application.Common.Mapping;
using Core.Application.Usecases.Enterprises.ViewModels;
using Core.Domain.Auth;

namespace Core.Application.Usecases.Users.ViewModels
{
    public class UserViewModel: IMapFrom<User>
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; }

        /// <summary>
        /// Название организации
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Контакты пользователя
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// Логин от системы Меркурий
        /// </summary>
        public string MercuryLogin { get; set; }

        /// <summary>
        /// пароль от системы Меркурий
        /// </summary>
        public string MercuryPassword { get; set; }

        /// <summary>
        /// Логин Ветис.API
        /// </summary>
        public string ApiLogin { get; set; }

        /// <summary>
        /// Пароль Ветис.API
        /// </summary>
        public string ApiPassword { get; set; }

        /// <summary>
        /// Ключ Ветис.API
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Идентификатор заявителя
        /// </summary>
        public string IssuerId { get; set; }

        /// <summary>
        /// Идентификатор организации
        /// </summary>
        public string EnterpriseId { get; set; }

        /// <summary>
        /// Разрешение редактирования
        /// </summary>
        public bool EditAllow { get; set; }

        /// <summary>
        /// Признак удалённого пользователя
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Список предприятий пользователя
        /// </summary>
        public ICollection<EnterpriseViewModel> Enterprises { get; set; }

        /// <summary>
        /// Список ролей пользователя
        /// </summary>
        public ICollection<string> Roles { get; set; }
        
        /// <summary>
        /// Дата, до которой активен пользователь
        /// </summary>
        public DateTime ExpirationDate { get; set; }
    }
}