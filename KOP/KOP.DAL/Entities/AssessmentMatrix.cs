﻿using KOP.Common.Enums;

namespace KOP.DAL.Entities
{
    public class AssessmentMatrix
    {
        public int Id { get; set; }
        public int MinAssessmentMatrixResultValue { get; set; }
        public int MaxAssessmentMatrixResultValue { get; set; }
        public SystemAssessmentTypes SystemType { get; set; }

        public List<AssessmentType> AssessmentTypes { get; set; } = new();
        public List<AssessmentMatrixElement> Elements { get; set; } = new();
    }
}