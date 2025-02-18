using System.Globalization;
using KOP.BLL.Interfaces;
using KOP.Common.Dtos;
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

        public ReportService(IUnitOfWork unitOfWork, IGradeService gradeService)
        {
            _unitOfWork = unitOfWork;
            _gradeService = gradeService;
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

        public async Task<IBaseResponse<List<UserSummaryDto>>> GetSubordinateUsers(int supervisorId)
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
                    var getSubordinateUsersRes = await GetSubordinateUsers(subdivision);

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

        private async Task<IBaseResponse<List<UserSummaryDto>>> GetSubordinateUsers(Subdivision subdivision)
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
                    var subordinateUsersFromChildSubdivisionRes = await GetSubordinateUsers(childSubdivision);

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
                // Получаем оценку вместе с необходимыми связанными сущностями
                var getGradeDtoRes= await _gradeService.GetGradeDto(gradeId, new List<GradeEntities>
                {
                    GradeEntities.Marks,
                    GradeEntities.Qualification,
                    GradeEntities.StrategicTasks,
                    GradeEntities.Kpis,
                    GradeEntities.TrainingEvents,
                    GradeEntities.Projects,
                    GradeEntities.ValueJudgment,
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

                // Добавляем заголовок пункта 2.1
                AddParagraph(document, "2.1. Результаты деятельности руководителя, достигнутые им при исполнении должностных обязанностей (позадачник)", true, "Cambria", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, "Данные предоставил ФИО сотрудника, начальник Управления ___", false, "Times New Roman", 10, ParagraphAlignment.LEFT);

                // Добавляем таблицу для пункта 2.1 "Подзадачник"
                var strategicProjectsTable = document.CreateTable(2 + gradeDto.StrategicTasks.Count, 7);
                FillStrategicProjectsTable(strategicProjectsTable, gradeDto);

                // Добавляем заголовок пункта 2.2
                AddParagraph(document, "2.2. Результаты выполнения стратегических проектов и задач.", true, "Cambria", 10, ParagraphAlignment.LEFT);

                // Добавляем "Проекты"
                foreach(var project in gradeDto.Projects)
                {
                    AddParagraph(document, $"{project.SupervisorSspName} является заказчиком стратегического проекта \"{project.Name}\". Проект находится на этапе {project.Stage}.", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                    AddParagraph(document, $"Дата открытия проекта - {project.StartDate}", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                    AddParagraph(document, $"Плановая дата завершения проекта - {project.EndDate}", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                    AddParagraph(document, $"По состоянию на {project.CurrentStatusDate} по проекту выполнены {project.FactStages} из {project.PlanStages} этапов, запланированных к реализации до {project.EndDate}.", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                    AddParagraph(document, $"Коэффициент реализации проекта – {project.SPn}%.", false, "Times New Roman", 10, ParagraphAlignment.LEFT);
                }

                // Добавляем заголовок пункта 2.3
                AddParagraph(document, "2.3. Результаты деятельности руководителя, достигнутые им при исполнении должностных обязанностей.", true, "Cambria", 10, ParagraphAlignment.LEFT);
                AddParagraph(document, $"Исполнение ключевых показателей эффективности деятельности (KPI по ТС за {gradeDto.StartDate} по {gradeDto.EndDate}).", false, "Cambria", 10, ParagraphAlignment.LEFT);

                // Добавляем таблицу для пункта 2.3 "KPI"
                var kpiTable = document.CreateTable(2 + gradeDto.Kpis.Count, 7);
                FillKpiTable(kpiTable, gradeDto);

                // Добавляем заголовок пункта 2.4
                AddParagraph(document, "2.4. Управленческие компетенции руководителя.", true, "Cambria", 10, ParagraphAlignment.LEFT);

                // Добавляем таблицу для пункта 2.4 "УК" (9 строк, 6 столбцов)
                var competenciesTable = document.CreateTable(9, 6);
                FillCompetenciesTable(competenciesTable);
                AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

                // Добавляем таблицу для пункта 2.5 "Квалификация руководителя" (1 строка, 3 столбца)
                var qualificationTable = document.CreateTable(1, 3);
                FillQualificationTable(qualificationTable, user, gradeDto);
                AddParagraph(document, string.Empty, false, "Times New Roman", 10, ParagraphAlignment.LEFT); // Отступ

                // Добавляем таблицу «Общий вывод по оценке Руководителя» (2 строки, 1 столбец)
                var conclusionTable = document.CreateTable(2, 1);
                FillConclusionTable(conclusionTable);

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
            table.GetRow(0).GetCell(0).SetText("1.");
            table.GetRow(0).GetCell(0).SetColor("#F5DEB3");
            MakeTextBoldInCell(table.GetRow(0).GetCell(0));
            AlignTextInCell(table.GetRow(0).GetCell(0), ParagraphAlignment.CENTER);

            table.GetRow(0).GetCell(1).SetText("Общие сведения об оцениваемом Руководителе");
            table.GetRow(0).GetCell(1).SetColor("#F5DEB3");
            MakeTextBoldInCell(table.GetRow(0).GetCell(1));
            AlignTextInCell(table.GetRow(0).GetCell(1), ParagraphAlignment.CENTER);

            // Строка 1: Дата
            table.GetRow(1).GetCell(0).SetText("1.1.");
            AlignTextInCell(table.GetRow(1).GetCell(0), ParagraphAlignment.CENTER);
            table.GetRow(1).GetCell(1).SetText("Дата");
            AlignTextInCell(table.GetRow(1).GetCell(1), ParagraphAlignment.CENTER);
            table.GetRow(1).GetCell(2).SetText("");
            AlignTextInCell(table.GetRow(1).GetCell(2), ParagraphAlignment.CENTER);

            // Строка 2: ФИО руководителя
            table.GetRow(2).GetCell(0).SetText("1.2.");
            AlignTextInCell(table.GetRow(2).GetCell(0), ParagraphAlignment.CENTER);
            table.GetRow(2).GetCell(1).SetText("ФИО оцениваемого Руководителя");
            AlignTextInCell(table.GetRow(2).GetCell(1), ParagraphAlignment.CENTER);
            table.GetRow(2).GetCell(2).SetText(user.FullName);
            AlignTextInCell(table.GetRow(2).GetCell(2), ParagraphAlignment.CENTER);

            // Строка 3: Должность
            table.GetRow(3).GetCell(0).SetText("1.3.");
            AlignTextInCell(table.GetRow(3).GetCell(0), ParagraphAlignment.CENTER);
            table.GetRow(3).GetCell(1).SetText("Должность");
            AlignTextInCell(table.GetRow(3).GetCell(1), ParagraphAlignment.CENTER);
            table.GetRow(3).GetCell(2).SetText(user.Position);
            AlignTextInCell(table.GetRow(3).GetCell(2), ParagraphAlignment.CENTER);

            // Строка 4: Оцениваемый период
            table.GetRow(4).GetCell(0).SetText("1.4.");
            AlignTextInCell(table.GetRow(4).GetCell(0), ParagraphAlignment.CENTER);
            table.GetRow(4).GetCell(1).SetText("Оцениваемый период");
            AlignTextInCell(table.GetRow(4).GetCell(1), ParagraphAlignment.CENTER);
            table.GetRow(4).GetCell(2).SetText($"{gradeDto.StartDate} по {gradeDto.EndDate}");
            AlignTextInCell(table.GetRow(4).GetCell(2), ParagraphAlignment.CENTER);

            // Строка 5: Критерии оценки и краткие выводы
            table.GetRow(5).GetCell(0).SetText("2");     
            table.GetRow(5).GetCell(0).SetColor("#F5DEB3");
            MakeTextBoldInCell(table.GetRow(5).GetCell(0));
            AlignTextInCell(table.GetRow(5).GetCell(0), ParagraphAlignment.CENTER);

            table.GetRow(5).GetCell(1).SetText("Критерии Оценки:");
            table.GetRow(5).GetCell(1).SetColor("#F5DEB3");
            MakeTextBoldInCell(table.GetRow(5).GetCell(1));
            AlignTextInCell(table.GetRow(5).GetCell(1), ParagraphAlignment.CENTER);

            table.GetRow(5).GetCell(2).SetText("Краткие выводы:");
            table.GetRow(5).GetCell(2).SetColor("#F5DEB3");
            MakeTextBoldInCell(table.GetRow(5).GetCell(2));
            AlignTextInCell(table.GetRow(5).GetCell(2), ParagraphAlignment.CENTER);

            // Строка 6: Вывод по стратегическим задачам
            table.GetRow(6).GetCell(0).SetText("2.1");
            AlignTextInCell(table.GetRow(6).GetCell(0), ParagraphAlignment.CENTER);
            table.GetRow(6).GetCell(1).SetText("Результаты деятельности руководителя, достигнутые им при исполнении должностных обязанностей. (Таблица 1)");
            AlignTextInCell(table.GetRow(6).GetCell(1), ParagraphAlignment.CENTER);
            table.GetRow(6).GetCell(2).SetText(gradeDto.StrategicTasksConclusion);
            AlignTextInCell(table.GetRow(6).GetCell(2), ParagraphAlignment.CENTER);

            // Строка 7: Стратегические проекты
            table.GetRow(7).GetCell(0).SetText("2.2");
            AlignTextInCell(table.GetRow(7).GetCell(0), ParagraphAlignment.CENTER);
            table.GetRow(7).GetCell(1).SetText("Результаты выполнения стратегических проектов и задач.");
            AlignTextInCell(table.GetRow(7).GetCell(1), ParagraphAlignment.CENTER);
            table.GetRow(7).GetCell(2).SetText("");
            AlignTextInCell(table.GetRow(7).GetCell(2), ParagraphAlignment.CENTER);

            // Строка 8: Вывод по KPI
            table.GetRow(8).GetCell(0).SetText("2.3");
            AlignTextInCell(table.GetRow(8).GetCell(0), ParagraphAlignment.CENTER);
            table.GetRow(8).GetCell(1).SetText("Результаты выполнения ключевых показателей эффективности деятельности. (Таблица 2)");
            AlignTextInCell(table.GetRow(8).GetCell(1), ParagraphAlignment.CENTER);
            table.GetRow(8).GetCell(2).SetText(gradeDto.KPIsConclusion);
            AlignTextInCell(table.GetRow(8).GetCell(2), ParagraphAlignment.CENTER);

            // Строка 9: Управленческие компетенции
            table.GetRow(9).GetCell(0).SetText("2.4");
            AlignTextInCell(table.GetRow(9).GetCell(0), ParagraphAlignment.CENTER);
            table.GetRow(9).GetCell(1).SetText("Оценка управленческих компетенций (Таблица 3)");
            AlignTextInCell(table.GetRow(9).GetCell(1), ParagraphAlignment.CENTER);
            table.GetRow(9).GetCell(2).SetText(gradeDto.ManagmentCompetenciesConclusion);
            AlignTextInCell(table.GetRow(9).GetCell(2), ParagraphAlignment.CENTER);

            // Строка 10: Квалификация руководителя
            table.GetRow(10).GetCell(0).SetText("2.5");
            AlignTextInCell(table.GetRow(5).GetCell(0), ParagraphAlignment.CENTER);
            table.GetRow(10).GetCell(1).SetText("Квалификация Руководителя");
            AlignTextInCell(table.GetRow(5).GetCell(1), ParagraphAlignment.CENTER);
            table.GetRow(10).GetCell(2).SetText(gradeDto.QualificationConclusion);
            AlignTextInCell(table.GetRow(5).GetCell(2), ParagraphAlignment.CENTER);
        }

        private void FillStrategicProjectsTable(XWPFTable table, GradeDto gradeDto)
        {
            // Строка 0: Заголовок таблицы
            var row0 = table.GetRow(0);

            row0.GetCell(0).SetText("Название проекта / стратегической задачи");
            row0.GetCell(0).SetColor("#F5DEB3");
            MakeTextBoldInCell(row0.GetCell(0));

            row0.GetCell(1).SetText("Цель проекта / стратегической задачи");
            row0.GetCell(1).SetColor("#F5DEB3");
            MakeTextBoldInCell(row0.GetCell(1));

            row0.GetCell(2).SetText("Срок");
            row0.GetCell(2).SetColor("#F5DEB3");
            MakeTextBoldInCell(row0.GetCell(2));

            row0.GetCell(3).SetText("");

            row0.GetCell(4).SetText("Результат");
            row0.GetCell(4).SetColor("#F5DEB3");
            MakeTextBoldInCell(row0.GetCell(4));

            row0.GetCell(5).SetText("");

            row0.GetCell(6).SetText("Примечание (в случае несоответствия)");
            row0.GetCell(6).SetColor("#F5DEB3");
            MakeTextBoldInCell(row0.GetCell(6));

            // Горизонтальное объединение ячеек для «Срока» и «Результата»
            row0.MergeCells(2, 3);
            row0.MergeCells(3, 4);

            // Вертикальное объединение для столбцов 0, 1 и 4 на 2 строки
            SetVerticalMerge(table, 0, 0, 2);
            SetVerticalMerge(table, 0, 1, 2);
            SetVerticalMerge(table, 0, 4, 2);

            // Строка 1: Подзаголовки для объединённых ячеек
            var row1 = table.GetRow(1);

            row1.GetCell(2).SetText("План (дата)");
            row1.GetCell(2).SetColor("#F5DEB3");
            MakeTextBoldInCell(row1.GetCell(2));

            row1.GetCell(3).SetText("Факт (дата)");
            row1.GetCell(3).SetColor("#F5DEB3");
            MakeTextBoldInCell(row1.GetCell(3));

            row1.GetCell(4).SetText("План");
            row1.GetCell(4).SetColor("#F5DEB3");
            MakeTextBoldInCell(row1.GetCell(4));

            row1.GetCell(5).SetText("Факт");
            row1.GetCell(5).SetColor("#F5DEB3");
            MakeTextBoldInCell(row1.GetCell(5));

            var rowCounter = 2;
            foreach (var strategicTask in gradeDto.StrategicTasks)
            {
                var row = table.GetRow(rowCounter);
                row.GetCell(0).SetText($"{strategicTask.Name}");
                row.GetCell(1).SetText($"{strategicTask.Purpose}");
                row.GetCell(2).SetText($"{strategicTask.PlanDate}");
                row.GetCell(3).SetText($"{strategicTask.FactDate}");
                row.GetCell(4).SetText($"{strategicTask.PlanResult}");
                row.GetCell(5).SetText($"{strategicTask.FactResult}");
                row.GetCell(6).SetText($"{strategicTask.Remark}");
                rowCounter++;
            }
        }

        private void FillKpiTable(XWPFTable table, GradeDto gradeDto)
        {
            // Строка 0: Заголовок таблицы
            var row0 = table.GetRow(0);

            row0.GetCell(0).SetText("№");
            row0.GetCell(0).SetColor("#F5DEB3");
            MakeTextBoldInCell(row0.GetCell(0));

            row0.GetCell(1).SetText("Показатель KPI");
            row0.GetCell(1).SetColor("#F5DEB3");
            MakeTextBoldInCell(row0.GetCell(1));

            row0.GetCell(2).SetText("Исполнение");
            row0.GetCell(2).SetColor("#F5DEB3");
            MakeTextBoldInCell(row0.GetCell(2));

            row0.GetCell(3).SetText("");
            row0.GetCell(4).SetText("");
            row0.GetCell(5).SetText("");

            // Горизонтальное объединение ячеек для «Исполнения»
            row0.MergeCells(2, 5);

            // Вертикальное объединение для столбцов 0 и 1 на 2 строки
            SetVerticalMerge(table, 0, 0, 2);
            SetVerticalMerge(table, 0, 1, 2);

            // Строка 1: Подзаголовки для объединённых ячеек
            var row1 = table.GetRow(1);

            row1.GetCell(2).SetText("ед. изм.");
            row1.GetCell(2).SetColor("#F5DEB3");
            MakeTextBoldInCell(row1.GetCell(2));

            row1.GetCell(3).SetText("План");
            row1.GetCell(3).SetColor("#F5DEB3");
            MakeTextBoldInCell(row1.GetCell(3));

            row1.GetCell(4).SetText("Факт");
            row1.GetCell(4).SetColor("#F5DEB3");
            MakeTextBoldInCell(row1.GetCell(4));

            row1.GetCell(5).SetText("% выполнения");
            row1.GetCell(5).SetColor("#F5DEB3");
            MakeTextBoldInCell(row1.GetCell(5));

            row1.GetCell(6).SetText("Расчёты показателя");
            row1.GetCell(6).SetColor("#F5DEB3");
            MakeTextBoldInCell(row1.GetCell(6));

            int rowIndex = 2;
            var dates = gradeDto.Kpis
            .GroupBy(x => x.PeriodStartDate.ToString("MMMM yyyy", new CultureInfo("ru-RU")))
            .Select(g => new {  MonthYear = g.Key, Kpis = g.ToList() });

            foreach (var date in dates)
            {
                // Строка с названием месяца и года (объединённая на всю ширину)
                var monthRow = table.GetRow(rowIndex);
                monthRow.GetCell(0).SetText(date.MonthYear);
                MakeTextBoldInCell(monthRow.GetCell(0));
                rowIndex++;

                // Строки с данными по KPI для месяцев
                foreach (var kpi in date.Kpis)
                {
                    var dataRow = table.GetRow(rowIndex);
                    dataRow.GetCell(0).SetText($"{rowIndex - 1}");
                    dataRow.GetCell(1).SetText($"{kpi.Name}");
                    dataRow.GetCell(2).SetText($"");
                    dataRow.GetCell(3).SetText($"");
                    dataRow.GetCell(4).SetText($"");
                    dataRow.GetCell(5).SetText($"{kpi.CompletionPercentage}");
                    dataRow.GetCell(6).SetText($"{kpi.CalculationMethod}");
                    rowIndex++;
                }           
            }
        }

        private void FillCompetenciesTable(XWPFTable table)
        {
            // Row 0: Единая ячейка с заголовком
            var row0 = table.GetRow(0);
            row0.MergeCells(0, 5);
            row0.GetCell(0).SetText("Оценка (от 0 до 13, где 13 – высший балл)");

            // Row 1: Заголовки логических колонок
            var row1 = table.GetRow(1);
            row1.GetCell(0).SetText("Управленческие компетенции");
            row1.GetCell(1).SetText("Описание компетенции");
            row1.GetCell(2).SetText("Самооценка (ФИО сотрудника)");
            row1.GetCell(3).SetText("");
            row1.GetCell(4).SetText("Оценки непосредственного руководителя (ФИО руководителя сотрудника)");
            row1.GetCell(5).SetText("");
            table.GetRow(1).MergeCells(2, 3);
            table.GetRow(1).MergeCells(3, 4);

            // Row 2: Подзаголовки для самооценки и оценки руководителя
            var row2 = table.GetRow(2);
            row2.GetCell(2).SetText("балл");
            row2.GetCell(3).SetText("интерпретация");
            row2.GetCell(4).SetText("балл");
            row2.GetCell(5).SetText("интерпретация");

            // Вертикальное объединение ячеек в столбцах 0 и 1 (Row 1–2)
            SetVerticalMerge(table, 1, 0, 2);
            SetVerticalMerge(table, 1, 1, 2);

            // Пример списка компетенций (5 строк)
            var competencies = new List<(string Title, string Description)>
            {
                ("Постановка целей и декомпозиция", "Способность ставить цель, конкретизировать её и определять путь достижения"),
                ("Руководство и развитие", "Лидерство и поддержка сотрудников в развитии"),
                ("Понимание бизнеса и лояльности", "Понимание влияния деятельности на бизнес"),
                ("Выполнение обязательств", "Ответственность и дисциплина в выполнении обязательств"),
                ("Решение проблем", "Анализ проблемных ситуаций и поиск оптимальных решений")
            };

            for (int i = 0; i < competencies.Count; i++)
            {
                var row = table.GetRow(3 + i);
                row.GetCell(0).SetText(competencies[i].Title);
                row.GetCell(1).SetText(competencies[i].Description);
                row.GetCell(2).SetText("…");
                row.GetCell(3).SetText("…");
                row.GetCell(4).SetText("…");
                row.GetCell(5).SetText("…");
            }

            // Объединяем ячейки последней строки для вывода общего результата
            table.GetRow(8).MergeCells(0, 1);
            table.GetRow(8).GetCell(0).SetText("Общий итог полученных баллов:");
        }

        private void FillQualificationTable(XWPFTable table, User user, GradeDto gradeDto)
        {
            var qualification = gradeDto.Qualification;
            var row = table.GetRow(0);

            row.GetCell(0).SetText("2.5");
            MakeTextBoldInCell(row.GetCell(0));

            row.GetCell(1).SetText("Квалификация Руководителя");
            MakeTextBoldInCell(row.GetCell(1));

            var cell = row.GetCell(2);
            //cell.RemoveParagraph(0);

            AddCellParagraph(cell, $"Соответствие {user.FullName} квалификационным требованиям {qualification.Link}:");
            AddCellParagraph(cell, "1. Образование:");
            AddCellParagraph(cell, $"Высшее образование: {qualification.HigherEducation}, специальность {qualification.Speciality}, квалификация {qualification.QualificationResult}, период обучения с {qualification.StartDate} по {qualification.EndDate}).");
            AddCellParagraph(cell, $"Дополнительное образование (повышение квалификации): {qualification.AdditionalEducation}");
            AddCellParagraph(cell, "");
            AddCellParagraph(cell, $"2. Стаж работы в банковской системе по состоянию на {qualification.CurrentStatusDate} – {qualification.CurrentExperienceYears} лет {qualification.CurrentExperienceMonths} мес., в т.ч.");
            foreach(var previousJob in qualification.PreviousJobs)
            {
                AddCellParagraph(cell, $"с {previousJob.StartDate} по {previousJob.EndDate} - {previousJob.OrganizationName} - {previousJob.PositionName};");
            }          
            AddCellParagraph(cell, $"ЗАО \"МТБанк\":");
            AddCellParagraph(cell, $"с {qualification.CurrentJobStartDate} - по настоящее время - {qualification.CurrentJobPositionName}");
            AddCellParagraph(cell, "");
            AddCellParagraph(cell, $"3. {qualification.EmploymentContarctTerminations} в течение последних двух лет факты расторжения трудового договора (контракта) по");
            AddCellParagraph(cell, $"инициативе нанимателя в случае совершения лицом виновных действий, являющихся основаниями для");
            AddCellParagraph(cell, $"утраты доверия к нему со стороны нанимателя");
            AddCellParagraph(cell, $"");
            AddCellParagraph(cell, $"{user.FullName} соответствует квалификационным требованиям и требованиям к деловой репутации.");
        }

        private void AddCellParagraph(XWPFTableCell cell, string text)
        {
            var paragraph = cell.AddParagraph();
            paragraph.CreateRun().SetText(text);
        }

        private void FillConclusionTable(XWPFTable table)
        {
            // Row 0: Заголовок с выравниванием по центру
            var row0 = table.GetRow(0);
            row0.GetCell(0).RemoveParagraph(0);
            var pTitle = row0.GetCell(0).AddParagraph();
            pTitle.Alignment = ParagraphAlignment.CENTER;
            var runTitle = pTitle.CreateRun();
            runTitle.IsBold = true;
            runTitle.SetText("Общий вывод по оценке Руководителя");

            // Row 1: Основной текст с несколькими абзацами
            var row1 = table.GetRow(1);
            row1.GetCell(0).RemoveParagraph(0);
            AddCellParagraph(row1.GetCell(0), "1. Результат оценки управленческих компетенций:");
            AddCellParagraph(row1.GetCell(0), "- Самооценка – __ балл;\n- Оценка руководителя – __ балл.\nИнтерпретация результатов: Уровень управленческих компетенций – А. Высоко результативный и потенциальный руководитель.");
            AddCellParagraph(row1.GetCell(0), "Вместе с тем, следует поддерживать на должном лидерском уровне следующие компетенции: ...");
            AddCellParagraph(row1.GetCell(0), "Для поддержания лидерского уровня развития компетенций рекомендовано на выбор:");

            // Рекомендации: бизнес-литература
            AddCellParagraph(row1.GetCell(0), "Изучение бизнес-литературы:");
            string[] literature = {
                "Р.Чаран «Исполнение. Система достижения целей» (ссылка ...)",
                "Р.Мауэр «Шаг за шагом к достижению цели» (ссылка ...)",
                "С.Иванова «Развитие потенциала сотрудников...» (ссылка ...)",
                "Д.Лайкер, Д.Майер «Талантливые сотрудники:...» (ссылка ...)"
            };
            foreach (var item in literature)
            {
                AddCellParagraph(row1.GetCell(0), "- " + item);
            }

            // Рекомендации: электронный курс
            AddCellParagraph(row1.GetCell(0), "Электронный курс на Корпоративном Портале МТSpace:");
            string[] courses = {
                "«Как мотивировать сотрудников компании» (ссылка ...)",
                "«Обратная связь как инструмент эффективного руководителя» (ссылка ...)"
            };
            foreach (var course in courses)
            {
                AddCellParagraph(row1.GetCell(0), "- " + course);
            }

            // Рекомендации: семинары и тренинги
            AddCellParagraph(row1.GetCell(0), "Семинары, тренинги, курсы, конференции и иное:");
            string[] events = {
                "Индивидуальная работа с коучем",
                "«Мотивация и вовлеченность персонала» (обучение)",
                "«Современный менеджмент» от Coursera (онлайн-курс)"
            };
            foreach (var ev in events)
            {
                AddCellParagraph(row1.GetCell(0), "- " + ev);
            }

            AddCellParagraph(row1.GetCell(0), "По итогам самооценки и оценки руководителя все управленческие компетенции находятся на высоком лидерском уровне развития...");
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

        private void MakeTextBoldInCell(XWPFTableCell cell)
        {
            // Получаем все параграфы в ячейке
            var paragraphs = cell.Paragraphs;

            // Если в ячейке есть параграфы, обрабатываем первый
            if (paragraphs.Count > 0)
            {
                var paragraph = paragraphs[0];

                // Получаем все текстовые фрагменты (Run) в параграфе
                var runs = paragraph.Runs;

                // Обрабатываем каждый текстовый фрагмент
                foreach (var run in runs)
                {
                    // Делаем текст жирным
                    run.IsBold = true;
                }
            }
        }

        private void AlignTextInCell(XWPFTableCell cell, ParagraphAlignment alignment)
        {
            // Получаем первый параграф в ячейке
            var paragraph = cell.AddParagraph();

            // Устанавливаем выравнивание
            paragraph.Alignment = alignment;
        }
    }
}