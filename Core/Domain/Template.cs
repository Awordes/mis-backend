﻿using System;

namespace Core.Domain
{
    public class Template: FileBase
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }
    }
}