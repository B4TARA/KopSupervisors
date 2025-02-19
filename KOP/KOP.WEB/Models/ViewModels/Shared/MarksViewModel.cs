﻿using KOP.Common.Dtos;

namespace KOP.WEB.Models.ViewModels.Shared
{
    public class MarksViewModel
    {
        public int GradeId { get; set; }
        public List<MarkTypeDto> MarkTypes { get; set; } = new();
        public bool IsFinalized { get; set; }

        public bool ViewAccess { get; set; }
        public bool EditAccess { get; set; }
    }
}