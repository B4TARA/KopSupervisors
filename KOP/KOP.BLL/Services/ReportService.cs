using System.Globalization;
using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
using KOP.Common.Dtos.AssessmentDtos;
using KOP.Common.Dtos.GradeDtos;
using KOP.Common.Enums;
using KOP.Common.Interfaces;
using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;

namespace KOP.BLL.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGradeService _gradeService;
        private readonly IAssessmentService _assessmentService;

        public ReportService(IUnitOfWork unitOfWork, IGradeService gradeService, IAssessmentService assessmentService)
        {
            _unitOfWork = unitOfWork;
            _gradeService = gradeService;
            _assessmentService = assessmentService;
        }

        public async Task<IBaseResponse<List<GradeSummaryDto>>> GetEmployeeGrades(int employeeId)
        {
            try
            {
                var grades = await _unitOfWork.Grades.GetAllAsync(x => x.UserId == employeeId && x.SystemStatus == SystemStatuses.COMPLETED);
                var gradeSummaryDtoList = new List<GradeSummaryDto>();

                foreach (var grade in grades)
                {
                    gradeSummaryDtoList.Add(new GradeSummaryDto
                    {
                        Id = grade.Id,
                        Number = grade.Number,
                        StartDate = grade.StartDate,
                        EndDate = grade.EndDate,
                        DateOfCreation = grade.DateOfCreation,
                    });
                }

                return new BaseResponse<List<GradeSummaryDto>>()
                {
                    Data = gradeSummaryDtoList,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<GradeSummaryDto>>()
                {
                    Description = $"[ReportService.GetEmployeeGrades] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<List<UserSummaryDto>>> GetSubordinateUsersWithGrade(int supervisorId)
        {
            try
            {
                var supervisor = await _unitOfWork.Users.GetAsync(x => x.Id == supervisorId, includeProperties: new string[]
                {
                    "SubordinateSubdivisions.Users.Grades",
                    "SubordinateSubdivisions.Children.Users.Grades",
                });

                if (supervisor == null)
                {
                    return new BaseResponse<List<UserSummaryDto>>()
                    {
                        Description = $"Пользователь с id = {supervisorId} не найден",
                        StatusCode = StatusCodes.EntityNotFound,
                    };
                }

                var allSubordinateUsers = new List<UserSummaryDto>();

                foreach (var subdivision in supervisor.SubordinateSubdivisions)
                {
                    var getSubordinateUsersRes = await GetSubordinateUsersWithGrade(subdivision);

                    if (!getSubordinateUsersRes.HasData)
                    {
                        return new BaseResponse<List<UserSummaryDto>>()
                        {
                            Description = getSubordinateUsersRes.Description,
                            StatusCode = getSubordinateUsersRes.StatusCode,
                        };
                    }

                    allSubordinateUsers.AddRange(getSubordinateUsersRes.Data);
                }

                return new BaseResponse<List<UserSummaryDto>>()
                {
                    Data = allSubordinateUsers,
                    StatusCode = StatusCodes.OK,
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<UserSummaryDto>>()
                {
                    Description = $"[ReportService.GetSubordinateUsers] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        private async Task<IBaseResponse<List<UserSummaryDto>>> GetSubordinateUsersWithGrade(Subdivision subdivision)
        {
            try
            {
                var subordinateUsers = new List<UserSummaryDto>();

                foreach (var user in subdivision.Users.Where(x => x.SystemRoles.Contains(SystemRoles.Employee) && x.Grades.Any()))
                {
                    subordinateUsers.Add(new UserSummaryDto
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Position = user.Position,
                        SubdivisionFromFile = user.SubdivisionFromFile,
                    });
                }

                foreach (var childSubdivision in subdivision.Children)
                {
                    var subordinateUsersFromChildSubdivisionRes = await GetSubordinateUsersWithGrade(childSubdivision);

                    if (!subordinateUsersFromChildSubdivisionRes.HasData)
                    {
                        return new BaseResponse<List<UserSummaryDto>>()
                        {
                            Description = subordinateUsersFromChildSubdivisionRes.Description,
                            StatusCode = subordinateUsersFromChildSubdivisionRes.StatusCode,
                        };
                    }

                    subordinateUsers.AddRange(subordinateUsersFromChildSubdivisionRes.Data);
                }

                return new BaseResponse<List<UserSummaryDto>>()
                {
                    Data = subordinateUsers,
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<UserSummaryDto>>()
                {
                    Description = $"[ReportService.GetSubordinateUsers] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<byte[]>> GenerateGradeWordDocument(int gradeId)
        {
            try
            {
                var getGradeDtoRes = await _gradeService.GetGradeDto(gradeId, new List<GradeEntities>
                {
                    GradeEntities.Marks,
                    GradeEntities.Qualification,
                    GradeEntities.StrategicTasks,
                    GradeEntities.Kpis,
                    GradeEntities.TrainingEvents,
                    GradeEntities.Projects,
                    GradeEntities.ValueJudgment,
                    GradeEntities.Assessments,
                });
                if (!getGradeDtoRes.HasData)
                {
                    return new BaseResponse<byte[]>
                    {
                        StatusCode = getGradeDtoRes.StatusCode,
                        Description = getGradeDtoRes.Description,
                    };
                }
                var gradeDto = getGradeDtoRes.Data;

                var user = await _unitOfWork.Users.GetAsync(x => x.Id == gradeDto.UserId);
                if (user is null)
                {
                    return new BaseResponse<byte[]>
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                        Description = $"Пользователь с ID = {gradeDto.UserId} не найден",
                    };
                }

                var managmentAssessment = gradeDto.AssessmentDtos.FirstOrDefault(x => x.SystemAssessmentType == SystemAssessmentTypes.ManagementCompetencies);
                if (managmentAssessment is null)
                {
                    return new BaseResponse<byte[]>
                    {
                        StatusCode = StatusCodes.EntityNotFound,
                        Description = $"Оценка управленческих компетенций для пользователя с ID = {gradeDto.UserId} не найдена",
                    };
                }

                var getManagmentAssessmentSummaryRes = await _assessmentService.GetAssessmentSummary(managmentAssessment.Id);
                if (!getManagmentAssessmentSummaryRes.HasData)
                {
                    return new BaseResponse<byte[]>
                    {
                        StatusCode = getManagmentAssessmentSummaryRes.StatusCode,
                        Description = getManagmentAssessmentSummaryRes.Description,
                    };
                }
                var managmentAssessmentSummary = getManagmentAssessmentSummaryRes.Data;

                using var memoryStream = new MemoryStream();
                using var document = new XWPFDocument();

                // Устанавливаем альбомную ориентацию документа
                SetLandscapeOrientation(document);

                // Добавляем заголовок документа
                AddParagraph(document, "Материал для оценки деятельности Руководителя ЗАО «МТБанк»", true, "Cambria", 11, ParagraphAlignment.CENTER);
                AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

                // Добавляем главную таблицу «Общие сведения об оцениваемом Руководителе» (11 строк, 3 столбца)
                var generalInfoTable = document.CreateTable(11, 3);
                FillGeneralInfoTable(generalInfoTable, user, gradeDto);
                AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

                // Добавляем заголовок пункта 2.1
                AddParagraph(document, "2.1. Результаты деятельности руководителя, достигнутые им при исполнении должностных обязанностей (позадачник)", true, "Cambria", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, "Данные предоставил ФИО сотрудника, начальник Управления ___", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, "Таблица 1.", false, "Cambria", 10, ParagraphAlignment.RIGHT);

                // Добавляем таблицу для пункта 2.1 "Подзадачник"
                var strategicTasksTable = document.CreateTable(2 + gradeDto.StrategicTasks.Count, 7);
                FillStrategicTasksTable(strategicTasksTable, gradeDto);
                AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

                // Добавляем заголовок пункта 2.2
                AddParagraph(document, "2.2. Результаты выполнения стратегических проектов и задач.", true, "Cambria", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

                // Добавляем "Проекты"
                foreach (var project in gradeDto.Projects)
                {
                    AddParagraph(document, $"{project.SupervisorSspName} является заказчиком стратегического проекта \"{project.Name}\". Проект находится на этапе {project.Stage}.", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                    AddParagraph(document, $"Дата открытия проекта - {project.StartDate}", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                    AddParagraph(document, $"Плановая дата завершения проекта - {project.EndDate}", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                    AddParagraph(document, $"По состоянию на {project.CurrentStatusDate} по проекту выполнены {project.FactStages} из {project.PlanStages} этапов, запланированных к реализации до {project.EndDate}.", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                    AddParagraph(document, $"Коэффициент реализации проекта – {project.SPn}%.", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                }
                AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

                // Добавляем заголовок пункта 2.3
                AddParagraph(document, "2.3. Результаты деятельности руководителя, достигнутые им при исполнении должностных обязанностей.", true, "Cambria", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, $"Исполнение ключевых показателей эффективности деятельности (KPI по ТС за {gradeDto.StartDate} по {gradeDto.EndDate}).", false, "Cambria", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, "Таблица 2.", false, "Cambria", 10, ParagraphAlignment.RIGHT);

                // Добавляем таблицу для пункта 2.3 "KPI"
                var kpiDates = gradeDto.Kpis
                    .GroupBy(x => x.PeriodStartDate.ToString("MMMM yyyy", new CultureInfo("ru-RU")))
                    .Select(g => new KpiDate { MonthYear = g.Key, Kpis = g.ToList() })
                    .ToList();
                var kpiTable = document.CreateTable(2 + gradeDto.Kpis.Count + kpiDates.Count(), 7);
                FillKpiTable(kpiTable, gradeDto, kpiDates);
                AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

                // Добавляем заголовок пункта 2.4
                AddParagraph(document, "2.4. Управленческие компетенции руководителя.", true, "Cambria", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, "Таблица 3.", false, "Cambria", 10, ParagraphAlignment.RIGHT);

                // Добавляем таблицу для пункта 2.4 "УК" (9 строк, 6 столбцов)
                var competenciesTable = document.CreateTable(9, 6);
                FillCompetenciesTable(competenciesTable, managmentAssessmentSummary);
                AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

                // Добавляем заголовок пункта 2.5
                AddParagraph(document, "Таблица 4.", false, "Cambria", 10, ParagraphAlignment.RIGHT);

                // Добавляем таблицу для пункта 2.5 "Квалификация руководителя" (1 строка, 3 столбца)
                var qualificationTable = document.CreateTable(1, 3);
                FillQualificationTable(qualificationTable, user, gradeDto);
                AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

                // Добавляем таблицу «Общий вывод по оценке Руководителя» (2 строки, 1 столбец)
                var conclusionTable = document.CreateTable(2, 1);
                FillConclusionTable(conclusionTable, managmentAssessmentSummary);

                // Сохраняем документ в MemoryStream
                document.Write(memoryStream);

                return new BaseResponse<byte[]>
                {
                    Data = memoryStream.ToArray(),
                    StatusCode = StatusCodes.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<byte[]>
                {
                    Description = $"[ReportService.GenerateGradeWordDocumentAsync] : {ex.Message}",
                    StatusCode = StatusCodes.InternalServerError,
                };
            }
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

        private void FillGeneralInfoTable(XWPFTable table, User user, GradeDto gradeDto)
        {
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

            // Строка 4: Оцениваемый период
            AddTextToCellWithFormatting(table.GetRow(4).GetCell(0), "1.4.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(4).GetCell(1), "Оцениваемый период", false, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(4).GetCell(2), $"{gradeDto.StartDate} по {gradeDto.EndDate}", false, ParagraphAlignment.LEFT);

            // Строка 5: Критерии оценки и краткие выводы
            AddTextToCellWithFormatting(table.GetRow(5).GetCell(0), "2.", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(table.GetRow(5).GetCell(1), "Критерии Оценки:", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(table.GetRow(5).GetCell(2), "Краткие выводы:", true, ParagraphAlignment.LEFT, "#F2F4F0");

            // Строка 6: Вывод по стратегическим задачам
            AddTextToCellWithFormatting(table.GetRow(6).GetCell(0), "2.1.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(6).GetCell(1), "Результаты деятельности руководителя, достигнутые им при исполнении должностных обязанностей. (Таблица 1)", true, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(6).GetCell(2), gradeDto.StrategicTasksConclusion, false, ParagraphAlignment.LEFT);

            // Строка 7: Стратегические проекты
            AddTextToCellWithFormatting(table.GetRow(7).GetCell(0), "2.2.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(7).GetCell(1), "Результаты выполнения стратегических проектов и задач.", true, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(7).GetCell(2), "", false, ParagraphAlignment.LEFT);

            // Строка 8: Вывод по KPI
            AddTextToCellWithFormatting(table.GetRow(8).GetCell(0), "2.3.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(8).GetCell(1), "Результаты выполнения ключевых показателей эффективности деятельности. (Таблица 2)", true, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(8).GetCell(2), gradeDto.KPIsConclusion, false, ParagraphAlignment.LEFT);

            // Строка 9: Управленческие компетенции
            AddTextToCellWithFormatting(table.GetRow(9).GetCell(0), "2.4.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(9).GetCell(1), "Оценка управленческих компетенций (Таблица 3)", true, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(9).GetCell(2), gradeDto.ManagmentCompetenciesConclusion, false, ParagraphAlignment.LEFT);

            // Строка 10: Квалификация руководителя
            AddTextToCellWithFormatting(table.GetRow(10).GetCell(0), "2.5.", false, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(table.GetRow(10).GetCell(1), "Квалификация Руководителя", true, ParagraphAlignment.LEFT);
            AddTextToCellWithFormatting(table.GetRow(10).GetCell(2), gradeDto.QualificationConclusion, false, ParagraphAlignment.LEFT);
        }

        private void FillStrategicTasksTable(XWPFTable table, GradeDto gradeDto)
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
            foreach (var strategicTask in gradeDto.StrategicTasks)
            {
                var row = table.GetRow(rowCounter);
                AddTextToCellWithFormatting(row.GetCell(0), $"{strategicTask.Name}", false, ParagraphAlignment.LEFT);
                AddTextToCellWithFormatting(row.GetCell(1), $"{strategicTask.Purpose}", false, ParagraphAlignment.LEFT);
                AddTextToCellWithFormatting(row.GetCell(2), $"{strategicTask.PlanDate}", false, ParagraphAlignment.LEFT);
                AddTextToCellWithFormatting(row.GetCell(3), $"{strategicTask.FactDate}", false, ParagraphAlignment.LEFT);
                AddTextToCellWithFormatting(row.GetCell(4), $"{strategicTask.PlanResult}", false, ParagraphAlignment.LEFT);
                AddTextToCellWithFormatting(row.GetCell(5), $"{strategicTask.FactResult}", false, ParagraphAlignment.LEFT);
                AddTextToCellWithFormatting(row.GetCell(6), $"{strategicTask.Remark}", false, ParagraphAlignment.LEFT);
                rowCounter++;
            }
        }

        private void FillKpiTable(XWPFTable table, GradeDto gradeDto, List<KpiDate> kpiDates)
        {
            // Вертикальное объединение для столбцов 0 и 1 на 2 строки
            SetVerticalMerge(table, 0, 0, 2);
            SetVerticalMerge(table, 0, 1, 2);
            SetVerticalMerge(table, 0, 6, 2);

            // Горизонтальное объединение ячеек для «Исполнения»
            table.GetRow(0).MergeCells(2, 5);

            // Строка 0: Заголовок таблицы
            var row0 = table.GetRow(0);
            AddTextToCellWithFormatting(row0.GetCell(0), "№", true, ParagraphAlignment.LEFT, "#F2F4F0");
            AddTextToCellWithFormatting(row0.GetCell(1), "Показатель KPI", true, ParagraphAlignment.LEFT, "#F2F4F0");
            AddTextToCellWithFormatting(row0.GetCell(2), "Исполнение", true, ParagraphAlignment.LEFT, "#F2F4F0");
            AddTextToCellWithFormatting(row0.GetCell(3), "Расчеты показателя", true, ParagraphAlignment.CENTER, "#F2F4F0");

            // Строка 1: Подзаголовки для объединённых ячеек
            var row1 = table.GetRow(1);
            AddTextToCellWithFormatting(row1.GetCell(2), "ед. изм.", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row1.GetCell(3), "План", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row1.GetCell(4), "Факт", true, ParagraphAlignment.CENTER, "#F2F4F0");
            AddTextToCellWithFormatting(row1.GetCell(5), "% выполнения", true, ParagraphAlignment.CENTER, "#F2F4F0");

            var rowIndex = 2;
            var kpiIndex = 1;
            foreach (var date in kpiDates)
            {
                // Строка с названием месяца и года (объединённая на всю ширину)
                var monthRow = table.GetRow(rowIndex);
                table.GetRow(rowIndex).MergeCells(0, 6);
                AddTextToCellWithFormatting(monthRow.GetCell(0), date.MonthYear, true);
                rowIndex++;

                // Строки с данными по KPI для месяцев
                foreach (var kpi in date.Kpis)
                {
                    var dataRow = table.GetRow(rowIndex);
                    AddTextToCellWithFormatting(dataRow.GetCell(0), $"{kpiIndex}");
                    AddTextToCellWithFormatting(dataRow.GetCell(1), $"{kpi.Name}");
                    AddTextToCellWithFormatting(dataRow.GetCell(2), $"-");
                    AddTextToCellWithFormatting(dataRow.GetCell(3), $"-");
                    AddTextToCellWithFormatting(dataRow.GetCell(4), $"-");
                    AddTextToCellWithFormatting(dataRow.GetCell(5), $"{kpi.CompletionPercentage}");
                    AddTextToCellWithFormatting(dataRow.GetCell(6), $"{kpi.CalculationMethod}");
                    rowIndex++;
                    kpiIndex++;
                }
            }
        }

        private void FillCompetenciesTable(XWPFTable table, AssessmentSummaryDto summaryDto)
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
                    ? (int)Math.Round(selfAssessmentRowValue.Value)
                    : (int?)null; // Если значение null, то roundedSelfAssessmentRowValue будет null

                // Округление supervisorAssessmentRowValue до int
                int? roundedSupervisorAssessmentRowValue = supervisorAssessmentRowValue.HasValue
                    ? (int)Math.Round(supervisorAssessmentRowValue.Value)
                    : (int?)null; // Если значение null, то roundedSupervisorAssessmentRowValue будет null
                var selfAssessmentRowValueInterpretation = elementsByRow[_assessmentService.GetInterpretationColumnByAssessmentValue(roundedSelfAssessmentRowValue)].Value;
                var supervisorAssessmentRowValueInterpretation = elementsByRow[_assessmentService.GetInterpretationColumnByAssessmentValue(roundedSupervisorAssessmentRowValue)].Value;

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

        private void FillQualificationTable(XWPFTable table, User user, GradeDto gradeDto)
        {
            var qualification = gradeDto.Qualification;
            var row = table.GetRow(0);
            var cell3 = row.GetCell(2);

            AddTextToCellWithFormatting(row.GetCell(0), "2.5.", true, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(row.GetCell(1), "Квалификация Руководителя", true, ParagraphAlignment.CENTER);
            AddTextToCellWithFormatting(cell3, $"Соответствие {user.FullName} квалификационным требованиям {qualification.Link}:", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, "1. Образование:", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"Высшее образование: {qualification.HigherEducation}, специальность {qualification.Speciality}, квалификация {qualification.QualificationResult}, период обучения с {qualification.StartDate} по {qualification.EndDate}).", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"Дополнительное образование (повышение квалификации): {qualification.AdditionalEducation}", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, "", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"2. Стаж работы в банковской системе по состоянию на {qualification.CurrentStatusDate} – {qualification.CurrentExperienceYears} лет {qualification.CurrentExperienceMonths} мес., в т.ч.", removePreviousParagraph: false);
            foreach (var previousJob in qualification.PreviousJobs)
            {
                AddTextToCellWithFormatting(cell3, $"с {previousJob.StartDate} по {previousJob.EndDate} - {previousJob.OrganizationName} - {previousJob.PositionName};", removePreviousParagraph: false);
            }
            AddTextToCellWithFormatting(cell3, $"ЗАО \"МТБанк\":", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"с {qualification.CurrentJobStartDate} - по настоящее время - {qualification.CurrentJobPositionName}", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, "", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"3. {qualification.EmploymentContarctTerminations} в течение последних двух лет факты расторжения трудового договора (контракта) по", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"инициативе нанимателя в случае совершения лицом виновных действий, являющихся основаниями для", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"утраты доверия к нему со стороны нанимателя", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"", removePreviousParagraph: false);
            AddTextToCellWithFormatting(cell3, $"{user.FullName} соответствует квалификационным требованиям и требованиям к деловой репутации.", removePreviousParagraph: false);
        }

        private void AddTextToCellWithFormatting(XWPFTableCell cell, string? text, bool bold = false, ParagraphAlignment alignment = ParagraphAlignment.LEFT, string? color = null, bool removePreviousParagraph = true)
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
            run.FontFamily = "Times New Roman";
            run.FontSize = 10;

            if (!string.IsNullOrEmpty(text))
            {
                run.SetText(text);
            }
        }

        private void FillConclusionTable(XWPFTable table, AssessmentSummaryDto summaryDto)
        {
            var selfAssessmentSum = summaryDto.SelfAssessmentResultValues.Sum(x => x.Value);
            var supervisorAssessmentSum = summaryDto.SupervisorAssessmentResultValues.Sum(x => x.Value);
            var interpretationLevel = summaryDto.AverageAssessmentInterpretation != null ? summaryDto.AverageAssessmentInterpretation.Level : "";
            var interpretationCompetence = summaryDto.AverageAssessmentInterpretation != null ? summaryDto.AverageAssessmentInterpretation.Competence : "Не удалось определить интерпретацию";

            // Row 0: Заголовок с выравниванием по центру
            AddTextToCellWithFormatting(table.GetRow(0).GetCell(0), "Общий вывод по оценке Руководителя", true, ParagraphAlignment.CENTER, "#F2F4F0");

            // Row 1: Основной текст с несколькими абзацами
            var row1 = table.GetRow(1);
            AddTextToCellWithFormatting(row1.GetCell(0), "1. Результат оценки управленческих компетенций:", removePreviousParagraph: false);
            AddTextToCellWithFormatting(row1.GetCell(0), $"- Самооценка – {selfAssessmentSum} балл;", removePreviousParagraph: false);
            AddTextToCellWithFormatting(row1.GetCell(0), $"- Оценка руководителя – {supervisorAssessmentSum} балл.", removePreviousParagraph: false);
            AddTextToCellWithFormatting(row1.GetCell(0), $"Интерпретация результатов: Уровень управленческих компетенций – {interpretationLevel} {interpretationCompetence}", removePreviousParagraph: false);
            AddTextToCellWithFormatting(row1.GetCell(0), "Вместе с тем, следует поддерживать на должном лидерском уровне следующие компетенции: ...", removePreviousParagraph: false);
            AddTextToCellWithFormatting(row1.GetCell(0), "Для поддержания лидерского уровня развития компетенций рекомендовано на выбор:", removePreviousParagraph: false);

            // Рекомендации: бизнес-литература
            AddTextToCellWithFormatting(row1.GetCell(0), "Изучение бизнес-литературы:", true, removePreviousParagraph: false);
            string[] literature = {
                "Р.Чаран «Исполнение. Система достижения целей» (ссылка ...)",
                "Р.Мауэр «Шаг за шагом к достижению цели» (ссылка ...)",
                "С.Иванова «Развитие потенциала сотрудников...» (ссылка ...)",
                "Д.Лайкер, Д.Майер «Талантливые сотрудники:...» (ссылка ...)"
            };
            foreach (var item in literature)
            {
                AddTextToCellWithFormatting(row1.GetCell(0), "- " + item, removePreviousParagraph: false);
            }

            // Рекомендации: электронный курс
            AddTextToCellWithFormatting(row1.GetCell(0), "Электронный курс на Корпоративном Портале МТSpace:", true, removePreviousParagraph: false);
            string[] courses = {
                "«Как мотивировать сотрудников компании» (ссылка ...)",
                "«Обратная связь как инструмент эффективного руководителя» (ссылка ...)"
            };
            foreach (var course in courses)
            {
                AddTextToCellWithFormatting(row1.GetCell(0), "- " + course, removePreviousParagraph: false);
            }

            // Рекомендации: семинары и тренинги
            AddTextToCellWithFormatting(row1.GetCell(0), "Семинары, тренинги, курсы, конференции и иное:", true, removePreviousParagraph: false);
            string[] events = {
                "Индивидуальная работа с коучем",
                "«Мотивация и вовлеченность персонала» (обучение)",
                "«Современный менеджмент» от Coursera (онлайн-курс)"
            };
            foreach (var ev in events)
            {
                AddTextToCellWithFormatting(row1.GetCell(0), "- " + ev, removePreviousParagraph: false);
            }

            AddTextToCellWithFormatting(row1.GetCell(0), "По итогам самооценки и оценки руководителя все управленческие компетенции находятся на высоком лидерском уровне развития...", removePreviousParagraph: false);
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

public class KpiDate
{
    public string MonthYear { get; set; }
    public List<KpiDto> Kpis { get; set; } = new();
}