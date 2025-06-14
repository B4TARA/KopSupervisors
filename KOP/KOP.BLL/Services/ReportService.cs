﻿using KOP.BLL.Interfaces;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;
using KOP.DAL;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;

namespace KOP.BLL.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssessmentService _assessmentService;
        private readonly IRecommendationService _recommendationService;
        private readonly ISupervisorService _supervisorService;

        public ReportService(ApplicationDbContext context, IUnitOfWork unitOfWork, IAssessmentService assessmentService,
            IRecommendationService recommendationService, ISupervisorService supervisorService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _assessmentService = assessmentService;
            _recommendationService = recommendationService;
            _supervisorService = supervisorService;
        }

        public async Task<byte[]> GenerateGradesReport(int gradeId)
        {
            var grade = await _context.Grades
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.Id == gradeId)
            .Select(x => new GradeExtendedDto
            {
                Qn2 = x.Qn2,
                UserId = x.UserId,
                EndDate = x.EndDate,
                StartDate = x.StartDate,
                KPIsConclusion = x.KPIsConclusion,
                QualificationConclusion = x.QualificationConclusion,
                StrategicTasksConclusion = x.StrategicTasksConclusion,
                ManagmentCompetenciesConclusion = x.ManagmentCompetenciesConclusion,
                CorporateCompetenciesConclusion = x.CorporateCompetenciesConclusion,

                ManagmentCompetenciesId = x.Assessments
                    .Where(a => a.AssessmentType.SystemAssessmentType == SystemAssessmentTypes.ManagementCompetencies)
                    .Select(a => a.Id)
                    .FirstOrDefault(),

                QualificationDto = x.Qualification == null ? null : new QualificationDto
                {
                    QualificationResult = x.Qualification.QualificationResult,
                    CurrentExperienceYears = x.Qualification.CurrentExperienceYears,
                    CurrentJobPositionName = x.Qualification.CurrentJobPositionName,
                    CurrentExperienceMonths = x.Qualification.CurrentExperienceMonths,
                    EmploymentContarctTerminations = x.Qualification.EmploymentContarctTerminations,
                    CurrentStatusDate = x.Qualification.CurrentStatusDate,
                    CurrentJobStartDate = x.Qualification.CurrentJobStartDate,
                    PreviousJobs = x.Qualification.PreviousJobs
                        .OrderBy(pj => pj.StartDate)
                        .Select(pj => new PreviousJobDto
                        {
                            PositionName = pj.PositionName,
                            OrganizationName = pj.OrganizationName,
                            EndDate = pj.EndDate,
                            StartDate = pj.StartDate,
                        })
                        .ToList(),
                    HigherEducations = x.Qualification.HigherEducations
                        .OrderBy(he => he.StartDate)
                        .Select(he => new HigherEducationDto
                        {
                            Education = he.Education,
                            Speciality = he.Speciality,
                            QualificationName = he.QualificationName,
                            EndDate = he.EndDate,
                            StartDate = he.StartDate,
                        })
                        .ToList(),
                },

                ValueJudgmentDto = x.ValueJudgment == null ? null : new ValueJudgmentDto
                {
                    Strengths = x.ValueJudgment.Strengths,
                    BehaviorToCorrect = x.ValueJudgment.BehaviorToCorrect,
                    RecommendationsForDevelopment = x.ValueJudgment.RecommendationsForDevelopment,
                },

                MarkTypeDtoList = x.Marks
                    .Select(m => m.MarkType)
                    .Distinct()
                    .Select(mt => new MarkTypeDto
                    {
                        Name = mt.Name,
                        Description = mt.Description,
                        Marks = x.Marks
                            .Where(m => m.MarkTypeId == mt.Id)
                            .Select(m => new MarkDto
                            {
                                Period = m.Period,
                                PercentageValue = m.PercentageValue,
                            })
                            .ToList(),
                    })
                    .ToList(),

                KpiDtoList = x.Kpis
                    .Select(k => new KpiDto
                    {
                        Name = k.Name,
                        CalculationMethod = k.CalculationMethod,
                        CompletionPercentage = k.CompletionPercentage,
                        PeriodEndDateTime = k.PeriodEndDate.ToDateTime(TimeOnly.MinValue),
                        PeriodStartDateTime = k.PeriodStartDate.ToDateTime(TimeOnly.MinValue),
                    })
                    .ToList(),

                ProjectDtoList = x.Projects
                    .Select(p => new ProjectDto
                    {
                        SP = p.SP,
                        Name = p.Name,
                        Stage = p.Stage,
                        UserRole = p.UserRole,
                        AverageKpi = p.AverageKpi,
                        SuccessRate = p.SuccessRate,
                        EndDateTime = p.EndDate.ToDateTime(TimeOnly.MinValue),
                        StartDateTime = p.StartDate.ToDateTime(TimeOnly.MinValue),
                    })
                    .ToList(),

                StrategicTaskDtoList = x.StrategicTasks
                    .Select(s => new StrategicTaskDto
                    {
                        Name = s.Name,
                        Remark = s.Remark,
                        Purpose = s.Purpose,
                        PlanResult = s.PlanResult,
                        FactResult = s.FactResult,
                        PlanDateTime = s.PlanDate.ToDateTime(TimeOnly.MinValue),
                        FactDateTime = s.FactDate.ToDateTime(TimeOnly.MinValue),
                    })
                    .ToList(),

                TrainingEventDtoList = x.TrainingEvents
                    .Select(t => new TrainingEventDto
                    {
                        Name = t.Name,
                        Status = t.Status,
                        EndDate = t.EndDate,
                        StartDate = t.StartDate,
                        Competence = t.Competence,
                    })
                    .ToList(),
            })
            .FirstOrDefaultAsync();

            if (grade == null)
                throw new Exception($"Grade with ID {gradeId} not found.");

            var user = await _unitOfWork.Users
                .GetAsync(x => x.Id == grade.UserId);

            if (user == null)
                throw new Exception($"User with ID {grade.UserId} not found.");

            var managmentSummary = await _assessmentService.GetAssessmentSummary(grade.ManagmentCompetenciesId);

            using var memoryStream = new MemoryStream();
            using var document = new XWPFDocument();

            // Устанавливаем альбомную ориентацию документа
            SetLandscapeOrientation(document);

            // Добавляем заголовок документа
            AddParagraph(document, "Материал для оценки деятельности Руководителя ЗАО «МТБанк»", true, "Cambria", 11, ParagraphAlignment.CENTER);
            AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

            // Добавляем главную таблицу «Общие сведения об оцениваемом Руководителе» (11 строк, 3 столбца)
            var generalInfoTable = document.CreateTable(12, 3);
            FillGeneralInfoTable(generalInfoTable, managmentSummary, user, grade);
            AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

            // Добавляем заголовок пункта 2.1
            AddParagraph(document, "2.1. Результаты деятельности руководителя, достигнутые им при исполнении должностных обязанностей (позадачник)", true, "Cambria", 10, ParagraphAlignment.LEFT);
            AddParagraph(document, $"Данные предоставил {user.FullName}, {user.Position}", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
            AddParagraph(document, "Таблица 1.", false, "Cambria", 10, ParagraphAlignment.RIGHT);

            // Добавляем таблицу для пункта 2.1 "Подзадачник"
            var strategicTasksTable = document.CreateTable(2 + grade.StrategicTaskDtoList.Count, 7);
            FillStrategicTasksTable(strategicTasksTable, grade);
            AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

            // Добавляем заголовок пункта 2.2
            AddParagraph(document, "2.2. Результаты выполнения стратегических проектов.", true, "Cambria", 10, ParagraphAlignment.LEFT);
            AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

            // Добавляем строку с описанием показателя Qn2 по всем проектам
            if (grade.ProjectDtoList.Any())
            {
                AddParagraph(document, $"Выполнение стратегических проектов за отчетный период, Qn2 = {grade.Qn2} %", true, "Cambria", 10, ParagraphAlignment.LEFT);
            }
            else
            {
                AddParagraph(document, $"За оцениваемый период {user.FullName} не являлся (лась) заказчиком или руководителем какого-либо стратегического проекта", true, "Cambria", 10, ParagraphAlignment.LEFT);
            }
            AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

            // Добавляем "Проекты"
            foreach (var project in grade.ProjectDtoList)
            {
                AddParagraph(document, $"{user.FullName} является {project.UserRole} стратегического проекта \"{project.Name}\".", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, $" Проект {project.Stage}.", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, $"Дата открытия проекта {project.StartDate}", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, $"Дата окончания проекта(план) {project.EndDate}", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, $"Коэффициент успешности проекта, % = {project.SuccessRate}", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, $"Средний KPI проекта, % = {project.AverageKpi}", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, $"Оценка реализации проекта SP, % = {project.SP}", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
            }
            AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

            // Добавляем заголовок пункта 2.3
            AddParagraph(document, "2.3. Результаты деятельности руководителя, достигнутые им при исполнении должностных обязанностей.", true, "Cambria", 10, ParagraphAlignment.LEFT);
            AddParagraph(document, $"Исполнение ключевых показателей эффективности деятельности (KPI по ТС за {grade.StartDate} по {grade.EndDate}).", false, "Cambria", 10, ParagraphAlignment.LEFT);
            AddParagraph(document, "Таблица 2.", false, "Cambria", 10, ParagraphAlignment.RIGHT);

            // Добавляем таблицу для пункта 2.3 "KPI"
            var kpiPeriods = grade.KpiDtoList
            .GroupBy(kpi => new
            {
                StartDate = kpi.PeriodStartDate,  // Группируем по датам без времени
                EndDate = kpi.PeriodEndDate
            })
            .Select(group => new KpiPeriod(
                $"{group.Key.StartDate:dd.MM.yyyy} - {group.Key.EndDate:dd.MM.yyyy}",
                group.ToList()
            ))
            .ToList();

            var kpiTable = document.CreateTable(2 + grade.KpiDtoList.Count + kpiPeriods.Count(), 4);
            FillKpiTable(kpiTable, grade, kpiPeriods);
            AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

            // Добавляем заголовок пункта 2.4
            AddParagraph(document, "2.4. Управленческие компетенции руководителя.", true, "Cambria", 10, ParagraphAlignment.LEFT);
            AddParagraph(document, "Таблица 3.", false, "Cambria", 10, ParagraphAlignment.RIGHT);

            // Добавляем таблицу для пункта 2.4 "УК" (9 строк, 6 столбцов)
            var competenciesTable = document.CreateTable(9, 6);
            await FillCompetenciesTable(competenciesTable, managmentSummary);
            AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

            // Добавляем заголовок пункта 2.5
            AddParagraph(document, "Таблица 4.", false, "Cambria", 10, ParagraphAlignment.RIGHT);

            // Добавляем таблицу для пункта 2.5 "Квалификация руководителя" (1 строка, 3 столбца)
            var qualificationTable = document.CreateTable(1, 3);
            FillQualificationTable(qualificationTable, user, grade);
            AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

            // Добавляем таблицу «Общий вывод по оценке Руководителя» (2 строки, 1 столбец)
            var conclusionTable = document.CreateTable(2, 1);
            await FillConclusionTable(conclusionTable, managmentSummary, user, gradeId);

            // Сохраняем документ в MemoryStream
            document.Write(memoryStream);

            return memoryStream.ToArray();
        }

        public async Task<byte[]> GenerateUpcomingGradesReport(int supervisorId)
        {
            // Получаем данные
            var users = await _supervisorService.GetUsersWithAnyUpcomingGradeForSupervisor(supervisorId);

            using var memoryStream = new MemoryStream();

            // Создание новой рабочей книги
            using var workbook = new XSSFWorkbook();

            // Создание нового листа
            var sheet = workbook.CreateSheet("Sheet1");

            // Создаем стиль для заголовков
            var headerStyle = workbook.CreateCellStyle();
            var headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);

            // Добавление заголовков
            var headerRow = sheet.CreateRow(0);
            var headers = new[] { "ФИО", "Подразделение", "Должность", "Дата окончания контракта", "Дата предстоящей оценки" };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
            }

            // Добавление данных
            var rowCount = 1;
            foreach (var user in users)
            {
                var row = sheet.CreateRow(rowCount++);

                row.CreateCell(0).SetCellValue(user.FullName);
                row.CreateCell(1).SetCellValue(user.SubdivisionFromFile);
                row.CreateCell(2).SetCellValue(user.Position);
                row.CreateCell(3).SetCellValue(user.ContractEndDate);
                row.CreateCell(4).SetCellValue(user.NextGradeStartDate);
            }

            // Авто-размер колонок
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            workbook.Write(memoryStream);
            return memoryStream.ToArray();
        }

        private void SetLandscapeOrientation(XWPFDocument document)
        {
            var section = new CT_SectPr();
            section.pgSz.orient = ST_PageOrientation.landscape;
            section.pgSz.w = (ulong)(842 * 20);
            section.pgSz.h = (ulong)(595 * 20);
            document.Document.body.sectPr = section;
        }
        private void AddParagraph(XWPFDocument document, string text, bool bold, string fontFamily, int fontSize, ParagraphAlignment alignment)
        {
            var paragraph = document.CreateParagraph();
            paragraph.Alignment = alignment;
            var run = paragraph.CreateRun();
            run.IsBold = bold;
            run.FontFamily = fontFamily;
            run.FontSize = fontSize;
            run.SetText(text);
        }
        private void FillGeneralInfoTable(XWPFTable table, AssessmentSummaryDto summaryDto, User user, GradeExtendedDto gradeDto)
        {
            var interpretationLevel = summaryDto.AverageAssessmentInterpretation != null ? summaryDto.AverageAssessmentInterpretation.Level : "-";
            var interpretationCompetence = summaryDto.AverageAssessmentInterpretation != null ? summaryDto.AverageAssessmentInterpretation.Competence : "Не удалось определить интерпретацию";

            // Строка 0: объединяем ячейки 1 и 2 для двухколоночного вида
            table.GetRow(0).MergeCells(1, 2);

            // Строка 0: Заголовок
            AddTextToCellWithFormatting(table.GetRow(0).GetCell(0), "1.", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(table.GetRow(0).GetCell(1), "Общие сведения об оцениваемом Руководителе", true, ParagraphAlignment.LEFT, "#F2F4F0");

            // Строка 1: Дата
            AddTextToCellWithFormatting(table.GetRow(1).GetCell(0), "1.1.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(1).GetCell(1), "Дата", false, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(1).GetCell(2), "", false, ParagraphAlignment.LEFT);

            // Строка 2: ФИО руководителя
            AddTextToCellWithFormatting(table.GetRow(2).GetCell(0), "1.2.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(2).GetCell(1), "ФИО оцениваемого Руководителя", false, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(2).GetCell(2), user.FullName, false, ParagraphAlignment.LEFT);

            // Строка 3: Должность
            AddTextToCellWithFormatting(table.GetRow(3).GetCell(0), "1.3.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(3).GetCell(1), "Должность", false, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(3).GetCell(2), user.Position, false, ParagraphAlignment.LEFT);

            // Строка 4: Подразделение
            AddTextToCellWithFormatting(table.GetRow(4).GetCell(0), "1.4.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(4).GetCell(1), "Подразделение", false, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(4).GetCell(2), user.SubdivisionFromFile, false, ParagraphAlignment.LEFT);

            // Строка 5: Оцениваемый период
            AddTextToCellWithFormatting(table.GetRow(5).GetCell(0), "1.5.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(5).GetCell(1), "Оцениваемый период", false, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(5).GetCell(2), $"{gradeDto.StartDate} по {gradeDto.EndDate}", false, ParagraphAlignment.LEFT);

            // Строка 6: Критерии оценки и краткие выводы
            AddTextToCellWithFormatting(table.GetRow(6).GetCell(0), "2.", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(table.GetRow(6).GetCell(1), "Критерии Оценки:", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(table.GetRow(6).GetCell(2), "Краткие выводы:", true, ParagraphAlignment.LEFT, "#F2F4F0");

            // Строка 7: Вывод по стратегическим задачам
            AddTextToCellWithFormatting(table.GetRow(7).GetCell(0), "2.1.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(7).GetCell(1), "Результаты деятельности руководителя, достигнутые им при исполнении должностных обязанностей. (Таблица 1)", true, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(7).GetCell(2), gradeDto.StrategicTasksConclusion, false, ParagraphAlignment.LEFT);

            // Строка 8: Стратегические проекты
            AddTextToCellWithFormatting(table.GetRow(8).GetCell(0), "2.2.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(8).GetCell(1), "Результаты выполнения стратегических проектов.", true, ParagraphAlignment.LEFT);
            if (gradeDto.ProjectDtoList.Any())
            {
                AddTextToCellWithFormatting(table.GetRow(8).GetCell(2), $"Выполнение стратегических проектов за отчетный период, Qn2 = {gradeDto.Qn2} %", alignment: ParagraphAlignment.LEFT, removePreviousParagraph: false);
            }
            else
            {
                AddTextToCellWithFormatting(table.GetRow(8).GetCell(2), $"За оцениваемый период {user.FullName} не являлся (лась) заказчиком или руководителем какого-либо стратегического проекта", alignment: ParagraphAlignment.LEFT, removePreviousParagraph: false);
            }
            foreach (var project in gradeDto.ProjectDtoList)
            {
                AddTextToCellWithFormatting(table.GetRow(8).GetCell(2), $"{user.FullName} является {project.UserRole} стратегического проекта {project.Name}.", alignment: ParagraphAlignment.LEFT, removePreviousParagraph: false);
                AddTextToCellWithFormatting(table.GetRow(8).GetCell(2), $"Проект {project.Stage}.", alignment: ParagraphAlignment.LEFT, removePreviousParagraph: false);
                AddTextToCellWithFormatting(table.GetRow(8).GetCell(2), $"Дата открытия проекта {project.StartDate}.", alignment: ParagraphAlignment.LEFT, removePreviousParagraph: false);
                AddTextToCellWithFormatting(table.GetRow(8).GetCell(2), $"Дата окончания проекта (план) {project.EndDate}.", alignment: ParagraphAlignment.LEFT, removePreviousParagraph: false);
                AddTextToCellWithFormatting(table.GetRow(8).GetCell(2), $"Коэффициент успешности проекта, % = {project.SuccessRate}", alignment: ParagraphAlignment.LEFT, removePreviousParagraph: false);
                AddTextToCellWithFormatting(table.GetRow(8).GetCell(2), $"Средний KPI проекта, % = {project.AverageKpi}", alignment: ParagraphAlignment.LEFT, removePreviousParagraph: false);
                AddTextToCellWithFormatting(table.GetRow(8).GetCell(2), $"Оценка реализации проекта, % = {project.SP}", alignment: ParagraphAlignment.LEFT, removePreviousParagraph: false);
            }

            // Строка 9: Вывод по KPI
            AddTextToCellWithFormatting(table.GetRow(9).GetCell(0), "2.3.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(9).GetCell(1), "Результаты выполнения ключевых показателей эффективности деятельности. (Таблица 2)", true, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(9).GetCell(2), gradeDto.KPIsConclusion, false, ParagraphAlignment.LEFT);

            // Строка 10: Управленческие компетенции
            AddTextToCellWithFormatting(table.GetRow(10).GetCell(0), "2.4.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(10).GetCell(1), "Оценка управленческих компетенций (Таблица 3)", true, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(10).GetCell(2), $"Уровень управленческих компетенций – {interpretationLevel}. {interpretationCompetence}", false, ParagraphAlignment.LEFT);

            // Строка 11: Квалификация руководителя
            AddTextToCellWithFormatting(table.GetRow(11).GetCell(0), "2.5.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(11).GetCell(1), "Квалификация Руководителя", true, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(11).GetCell(2), gradeDto.QualificationConclusion, false, ParagraphAlignment.LEFT);
        }

        private void FillStrategicTasksTable(XWPFTable table, GradeExtendedDto gradeDto)
        {
            // Вертикальное объединение для столбцов 0, 1 и 4 на 2 строки
            SetVerticalMerge(table, 0, 0, 2);
            SetVerticalMerge(table, 0, 1, 2);
            SetVerticalMerge(table, 0, 6, 2);

            // Горизонтальное объединение ячеек для «Срока» и «Результата»
            table.GetRow(0).MergeCells(2, 3);
            table.GetRow(0).MergeCells(3, 4);

            // Строка 0: Заголовок таблицы
            var row0 = table.GetRow(0);
            AddTextToCellWithFormatting(row0.GetCell(0), "Название проекта / стратегической задачи", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row0.GetCell(1), "Цель проекта / стратегической задачии", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row0.GetCell(2), "Срок", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row0.GetCell(3), "Результат", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row0.GetCell(4), "Примечание (в случае несоответствия)", true, ParagraphAlignment.CENTER, "#F2F4F0");

            // Строка 1: Подзаголовки для объединённых ячеек
            var row1 = table.GetRow(1);
            AddTextToCellWithFormatting(row1.GetCell(2), "План (дата)", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row1.GetCell(3), "Факт (дата)", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row1.GetCell(4), "План", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row1.GetCell(5), "Факт", true, ParagraphAlignment.CENTER, "#F2F4F0");

            var rowCounter = 2;
            foreach (var strategicTask in gradeDto.StrategicTaskDtoList)
            {
                var row = table.GetRow(rowCounter);
                AddTextToCellWithFormatting(row.GetCell(0), strategicTask.Name, false, ParagraphAlignment.LEFT);
                AddTextToCellWithFormatting(row.GetCell(1), strategicTask.Purpose, false, ParagraphAlignment.LEFT);
                AddTextToCellWithFormatting(row.GetCell(2), $"{strategicTask.PlanDate}", false, ParagraphAlignment.LEFT);
                AddTextToCellWithFormatting(row.GetCell(3), $"{strategicTask.FactDate}", false, ParagraphAlignment.LEFT);
                AddTextToCellWithFormatting(row.GetCell(4), strategicTask.PlanResult, false, ParagraphAlignment.LEFT);
                AddTextToCellWithFormatting(row.GetCell(5), strategicTask.FactResult, false, ParagraphAlignment.LEFT);

                if (!string.IsNullOrEmpty(strategicTask.Remark))
                {
                    foreach (var newline in strategicTask.Remark.Split("\r\n"))
                    {
                        AddTextToCellWithFormatting(row.GetCell(6), newline, false, ParagraphAlignment.LEFT, removePreviousParagraph: false);
                    }
                }

                rowCounter++;
            }
        }

        private void FillKpiTable(XWPFTable table, GradeExtendedDto gradeDto, List<KpiPeriod> kpiPeriods)
        {
            // Строка 0: Заголовок таблицы
            var row0 = table.GetRow(0);
            AddTextToCellWithFormatting(row0.GetCell(0), "№", true, ParagraphAlignment.LEFT, "#F2F4F0");
            AddTextToCellWithFormatting(row0.GetCell(1), "Показатель KPI", true, ParagraphAlignment.LEFT, "#F2F4F0");
            AddTextToCellWithFormatting(row0.GetCell(2), "% выполнения", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row0.GetCell(3), "Расчеты показателя", true, ParagraphAlignment.CENTER, "#F2F4F0");

            var rowIndex = 1;
            var kpiIndex = 1;
            foreach (var date in kpiPeriods)
            {
                // Строка с названием месяца и года (объединённая на всю ширину)
                var periodRow = table.GetRow(rowIndex);
                table.GetRow(rowIndex).MergeCells(0, 3);
                AddTextToCellWithFormatting(periodRow.GetCell(0), date.Period, true);
                rowIndex++;

                // Строки с данными по KPI для месяцев
                foreach (var kpi in date.KpiDtoList)
                {
                    var dataRow = table.GetRow(rowIndex);
                    AddTextToCellWithFormatting(dataRow.GetCell(0), $"{kpiIndex}");
                    AddTextToCellWithFormatting(dataRow.GetCell(1), $"{kpi.Name}");
                    AddTextToCellWithFormatting(dataRow.GetCell(2), $"{kpi.CompletionPercentage}");
                    AddTextToCellWithFormatting(dataRow.GetCell(3), $"{kpi.CalculationMethod}");
                    rowIndex++;
                    kpiIndex++;
                }
            }
        }

        private async Task FillCompetenciesTable(XWPFTable table, AssessmentSummaryDto summaryDto)
        {
            // Вертикальное объединение ячеек
            SetVerticalMerge(table, 1, 0, 2);
            SetVerticalMerge(table, 1, 1, 2);

            // Горизонтальное объединение ячеек
            table.GetRow(0).MergeCells(0, 5);
            table.GetRow(1).MergeCells(2, 3);
            table.GetRow(1).MergeCells(3, 4);
            table.GetRow(8).MergeCells(0, 1);

            // Row 0: Заголовок таблицы
            AddTextToCellWithFormatting(table.GetRow(0).GetCell(0), "Оценка (от 0 до 13, где 13 – высший балл)", true, ParagraphAlignment.CENTER, "#F2F4F0");

            // Row 1: Заголовки логических колонок
            var row1 = table.GetRow(1);
            AddTextToCellWithFormatting(row1.GetCell(0), "Управленческие компетенции", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row1.GetCell(1), "Описание компетенции", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row1.GetCell(2), $"Самооценка", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row1.GetCell(3), "Оценки непосредственного руководителя", true, ParagraphAlignment.CENTER, "#F2F4F0");

            // Row 2: Подзаголовки для самооценки и оценки руководителя
            var row2 = table.GetRow(2);
            AddTextToCellWithFormatting(row2.GetCell(2), "балл", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row2.GetCell(3), "интерпретация", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row2.GetCell(4), "балл", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row2.GetCell(5), "интерпретация", true, ParagraphAlignment.CENTER, "#F2F4F0");

            for (int i = 1; i < summaryDto.RowsWithElements.Count; i++)
            {
                var elementsByRow = summaryDto.RowsWithElements[i].ToList();
                var selfAssessmentRowValue = summaryDto.SelfAssessmentResultValues?.FirstOrDefault(x => x.AssessmentMatrixRow == i)?.Value;
                var supervisorAssessmentRowValue = summaryDto.SupervisorAssessmentResultValues?.FirstOrDefault(x => x.AssessmentMatrixRow == i)?.Value;
                // Округление selfAssessmentRowValue до int
                int? roundedSelfAssessmentRowValue = selfAssessmentRowValue.HasValue
                    ? (int)Math.Round(selfAssessmentRowValue.Value) : null; // Если значение null, то roundedSelfAssessmentRowValue будет null

                // Округление supervisorAssessmentRowValue до int
                int? roundedSupervisorAssessmentRowValue = supervisorAssessmentRowValue.HasValue
                    ? (int)Math.Round(supervisorAssessmentRowValue.Value) : null; // Если значение null, то roundedSupervisorAssessmentRowValue будет null

                var selfAssessmentRowValueInterpretation = string.Empty;
                var supervisorAssessmentRowValueInterpretation = string.Empty;

                // Проверяем и получаем интерпретацию значения самооценки
                if (roundedSelfAssessmentRowValue.HasValue)
                {
                    var selfAssessmentColumn = await _assessmentService.GetMatrixColumnForAssessmentValue(roundedSelfAssessmentRowValue.Value);
                    selfAssessmentRowValueInterpretation = elementsByRow[selfAssessmentColumn].Value;
                }
                else
                {
                    // Обработка случая, когда самооценка null
                    selfAssessmentRowValueInterpretation = "Не указано"; // или другое значение по умолчанию
                }

                // Проверяем и получаем интерпретацию значения оценки супервизора
                if (roundedSupervisorAssessmentRowValue.HasValue)
                {
                    var supervisorAssessmentColumn = await _assessmentService.GetMatrixColumnForAssessmentValue(roundedSupervisorAssessmentRowValue.Value);
                    supervisorAssessmentRowValueInterpretation = elementsByRow[supervisorAssessmentColumn].Value;
                }
                else
                {
                    // Обработка случая, когда оценка супервизора null
                    supervisorAssessmentRowValueInterpretation = "Не указано"; // или другое значение по умолчанию
                }

                var row = table.GetRow(3 + i - 1); // т.к. i начинает отсчет с 1
                AddTextToCellWithFormatting(row.GetCell(0), elementsByRow[0].Value, true);
                AddTextToCellWithFormatting(row.GetCell(1), elementsByRow[1].Value);
                AddTextToCellWithFormatting(row.GetCell(2), selfAssessmentRowValue?.ToString());
                AddTextToCellWithFormatting(row.GetCell(3), selfAssessmentRowValueInterpretation);
                AddTextToCellWithFormatting(row.GetCell(4), supervisorAssessmentRowValue?.ToString());
                AddTextToCellWithFormatting(row.GetCell(5), supervisorAssessmentRowValueInterpretation);
            }

            // Вывод общего результата
            AddTextToCellWithFormatting(table.GetRow(8).GetCell(0), "Общий итог полученных баллов:", true);
            AddTextToCellWithFormatting(table.GetRow(8).GetCell(1), summaryDto.SelfAssessmentResultValues?.Sum(x => x.Value).ToString());
            AddTextToCellWithFormatting(table.GetRow(8).GetCell(3), summaryDto.SupervisorAssessmentResultValues?.Sum(x => x.Value).ToString());
        }

        private void FillQualificationTable(XWPFTable table, User user, GradeExtendedDto gradeDto)
        {
            var qualification = gradeDto.QualificationDto;
            var row = table.GetRow(0);
            var cell3 = row.GetCell(2);

            AddTextToCellWithFormatting(row.GetCell(0), "2.5.", true, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(row.GetCell(1), "Квалификация Руководителя", true, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(cell3, $"Соответствие {user.FullName} квалификационным требованиям:", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, "1. Высшее образование:", removePreviousParagraph: false);
            if (qualification != null && qualification.HigherEducations != null)
            {
                foreach (var higherEducation in qualification.HigherEducations)
                {
                    AddTextToCellWithFormatting(cell3, $"{higherEducation.Education}, специальность {higherEducation.Speciality}, квалификация {higherEducation.QualificationName}, период обучения с {higherEducation.StartDate} по {higherEducation.EndDate};.", removePreviousParagraph: false);
                }
            }
            AddTextToCellWithFormatting(cell3, "", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"2. Стаж работы в банковской системе по состоянию на {qualification?.CurrentStatusDate} – {qualification?.CurrentExperienceYears} лет {qualification?.CurrentExperienceMonths} мес., в т.ч.", removePreviousParagraph: false);
            if (qualification != null && qualification.PreviousJobs != null)
            {
                foreach (var previousJob in qualification.PreviousJobs)
                {
                    AddTextToCellWithFormatting(cell3, $"с {previousJob.StartDate} по {previousJob.EndDate} - {previousJob.OrganizationName} - {previousJob.PositionName};", removePreviousParagraph: false);
                }
            }
            AddTextToCellWithFormatting(cell3, $"ЗАО \"МТБанк\":", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"с {qualification?.CurrentJobStartDate} - по настоящее время - {qualification?.CurrentJobPositionName}", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, "", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"3. {qualification?.EmploymentContarctTerminations} в течение последних двух лет факты расторжения трудового договора (контракта) по", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"инициативе нанимателя в случае совершения лицом виновных действий, являющихся основаниями для", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"утраты доверия к нему со стороны нанимателя", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"4. {qualification?.QualificationResult}", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, gradeDto.QualificationConclusion, removePreviousParagraph: false);
        }

        private void AddTextToCellWithFormatting(XWPFTableCell cell, string? text, bool bold = false, ParagraphAlignment alignment = ParagraphAlignment.LEFT, string? color = null, bool removePreviousParagraph = true, bool underline = false)
        {
            if (removePreviousParagraph)
            {
                cell.RemoveParagraph(0);
            }

            cell.SetVerticalAlignment(XWPFTableCell.XWPFVertAlign.CENTER);

            if (!string.IsNullOrEmpty(color))
            {
                cell.SetColor(color);
            }

            var paragraph = cell.AddParagraph();
            paragraph.SpacingBefore = 30;
            paragraph.SpacingAfter = 30;
            paragraph.IndentationRight = 50;
            paragraph.IndentationLeft = 50;
            paragraph.Alignment = alignment;


            var run = paragraph.CreateRun();
            run.IsBold = bold;
            if (underline)
            {
                run.Underline = UnderlinePatterns.Single;
            }
            run.FontFamily = "Times New Roman";
            run.FontSize = 10;

            if (!string.IsNullOrEmpty(text))
            {
                run.SetText(text);
            }
        }

        private async Task FillConclusionTable(XWPFTable table, AssessmentSummaryDto summaryDto, User user, int gradeId)
        {
            var courseRecommendations = await _recommendationService.GetCourseRecommendationsForGrade(gradeId);
            var seminarRecommendations = await _recommendationService.GetSeminarRecommendationsForGrade(gradeId);
            var competenceRecommendations = await _recommendationService.GetCompetenceRecommendationsForGrade(gradeId);
            var literatureRecommendations = await _recommendationService.GetLiteratureRecommendationsForGrade(gradeId);

            var selfAssessmentSum = summaryDto.SelfAssessmentResultValues.Sum(x => x.Value);
            var supervisorAssessmentSum = summaryDto.SupervisorAssessmentResultValues.Sum(x => x.Value);
            var interpretationLevel = summaryDto.AverageAssessmentInterpretation != null ? summaryDto.AverageAssessmentInterpretation.Level : "Не удалось определить уровень.";
            var interpretationCompetence = summaryDto.AverageAssessmentInterpretation != null ? summaryDto.AverageAssessmentInterpretation.Competence : "Не удалось определить компетенцию";

            // Row 0: Заголовок с выравниванием по центру
            AddTextToCellWithFormatting(table.GetRow(0).GetCell(0), "Общий вывод по оценке Руководителя", true, ParagraphAlignment.CENTER, "#F2F4F0");

            // Row 1: Основной текст с несколькими абзацами
            var row1 = table.GetRow(1);
            AddTextToCellWithFormatting(row1.GetCell(0), "1. Результат оценки управленческих компетенций:", removePreviousParagraph: false, underline: true);
            AddTextToCellWithFormatting(row1.GetCell(0), $"- Самооценка – {selfAssessmentSum} балл;", removePreviousParagraph: false);
            AddTextToCellWithFormatting(row1.GetCell(0), $"- Оценка руководителя – {supervisorAssessmentSum} балл.", removePreviousParagraph: false);
            AddTextToCellWithFormatting(row1.GetCell(0), "Интерпретация результатов:", removePreviousParagraph: false, underline: true);
            AddTextToCellWithFormatting(row1.GetCell(0), $"Уровень управленческих компетенций – {interpretationLevel} {interpretationCompetence}", removePreviousParagraph: false);
            AddTextToCellWithFormatting(row1.GetCell(0), $"По итогам самооценки и оценки руководителя все управленческие компетенции {user.FullName} находятся на ___ уровне развития.", removePreviousParagraph: false);

            // Компетенции, которые стоит поддерживать
            AddTextToCellWithFormatting(row1.GetCell(0), "Вместе с тем, следует поддерживать на должном лидерском уровне следующие компетенции:", removePreviousParagraph: false);

            foreach (var competence in competenceRecommendations)
            {
                AddTextToCellWithFormatting(row1.GetCell(0), "- " + competence.Value, removePreviousParagraph: false);
            }

            AddTextToCellWithFormatting(row1.GetCell(0), string.Empty, removePreviousParagraph: false); // Отступ
            AddTextToCellWithFormatting(row1.GetCell(0), "Для поддержания лидерского уровня развития компетенций рекомендовано на выбор:", removePreviousParagraph: false, underline: true);

            // Рекомендации: бизнес-литература
            AddTextToCellWithFormatting(row1.GetCell(0), "Изучение бизнес-литературы:", true, removePreviousParagraph: false);

            foreach (var literature in literatureRecommendations)
            {
                AddTextToCellWithFormatting(row1.GetCell(0), "- " + literature.Value, removePreviousParagraph: false);
            }

            // Рекомендации: электронный курс
            AddTextToCellWithFormatting(row1.GetCell(0), "Электронный курс на Корпоративном Портале МТSpace:", true, removePreviousParagraph: false);

            foreach (var course in courseRecommendations)
            {
                AddTextToCellWithFormatting(row1.GetCell(0), "- " + course.Value, removePreviousParagraph: false);
            }

            // Рекомендации: семинары и тренинги
            AddTextToCellWithFormatting(row1.GetCell(0), "Семинары, тренинги, курсы, конференции и иное:", true, removePreviousParagraph: false);

            foreach (var seminar in seminarRecommendations)
            {
                AddTextToCellWithFormatting(row1.GetCell(0), "- " + seminar.Value, removePreviousParagraph: false);
            }

            AddTextToCellWithFormatting(row1.GetCell(0), "2. Продление трудовых отношений:", removePreviousParagraph: false, underline: true);
            AddTextToCellWithFormatting(row1.GetCell(0), $"В связи с вышеизложенным, требованием законодательства и предоставленным {user.FullName} заявлением предлагаем продлить трудовые отношения с {user.FullName} в должности {user.Position} сроком на ___ года.", removePreviousParagraph: false);
        }

        private void SetVerticalMerge(XWPFTable table, int startRow, int columnIndex, int rowCount)
        {
            int endRow = startRow + rowCount - 1;
            if (endRow >= table.NumberOfRows) return;

            // Первая ячейка – начало объединения
            var cellStart = table.GetRow(startRow).GetCell(columnIndex);
            var tcPrStart = cellStart.GetCTTc().AddNewTcPr();
            tcPrStart.AddNewVMerge().val = ST_Merge.restart;

            // Остальные ячейки – продолжение объединения
            for (int r = startRow + 1; r <= endRow; r++)
            {
                var cell = table.GetRow(r).GetCell(columnIndex);
                var tcPr = cell.GetCTTc().AddNewTcPr();
                tcPr.AddNewVMerge().val = ST_Merge.@continue;
            }
        }
    }
}

public class KpiPeriod
{
    public string Period { get; set; }
    public List<KpiDto> KpiDtoList { get; set; }
    public KpiPeriod(string period, List<KpiDto> kpiDtoList)
    {
        Period = period;
        KpiDtoList = kpiDtoList;
    }
}