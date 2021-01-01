using System;

namespace Core.Application.Usecases.Auth.ViewModels
{
    public class AuthenticationViewModel
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string SecondName { get; set; }

        /// <summary>
        /// Флаг суперпользователя
        /// </summary>
        public bool Authority { get; set; }

        /// <summary>
        /// Access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
