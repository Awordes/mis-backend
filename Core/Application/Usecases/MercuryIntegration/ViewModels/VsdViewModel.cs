﻿using System;

namespace Core.Application.Usecases.MercuryIntegration.ViewModels
{
    public class VsdViewModel
    {        
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime? ProductDate { get; set; }

        public DateTime? IssueDate { get; set; }

        public DateTime? ProcessDate { get; set; }

        public decimal? Volume { get; set; }

        public string ProductGlobalId { get; set; }
    }
}