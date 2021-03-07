using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Core.Domain.Auth
{
    public class User : IdentityUser<Guid>
    {
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
        /// Признак удалённого пользователя
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Список предприятий пользователя
        /// </summary>
        public ICollection<Enterprise> Enterprises { get; set; }

        /// <summary>
        /// Дата, до которой активен пользователь
        /// </summary>
        public DateTime ExpirationDate { get; set; }
    }
}
