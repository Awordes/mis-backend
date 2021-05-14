using System;
using AutoMapper;
using Core.Application.Common.Mapping;
using Core.Domain.Applicants;

namespace Core.Application.Usecases.Applicants.ViewModels
{
    public class ApplicantViewModel: IMapFrom<Applicant>
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Электронный адрес
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreationDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Applicant, ApplicantViewModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom((src, _) =>
                    {
                        return src.Status switch {
                            ApplicantStatus.New => "Новая",
                            ApplicantStatus.Confirmed => "Подтверждённая",
                            _ => ""
                        };
                    }))
                ;
        }
    }
}