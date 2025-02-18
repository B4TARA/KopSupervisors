﻿// <auto-generated />
using System;
using KOP.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KOP.DAL.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250218104557_AddAssessmentGradeConnection")]
    partial class AddAssessmentGradeConnection
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.Assessment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AssessmentTypeId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<int>("GradeId")
                        .HasColumnType("integer");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<int>("SystemStatus")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("GradeId");

                    b.HasIndex("UserId");

                    b.HasIndex("AssessmentTypeId", "GradeId")
                        .IsUnique();

                    b.ToTable("Assessments");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentInterpretation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AssessmentTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("Competence")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<string>("HtmlClassName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MaxValue")
                        .HasColumnType("integer");

                    b.Property<int>("MinValue")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AssessmentTypeId");

                    b.ToTable("AssessmentInterpretations");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentMatrix", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<int>("MaxAssessmentMatrixResultValue")
                        .HasColumnType("integer");

                    b.Property<int>("MinAssessmentMatrixResultValue")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("AssessmentMatrices");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentMatrixElement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Column")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<string>("HtmlClassName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MatrixId")
                        .HasColumnType("integer");

                    b.Property<int>("Row")
                        .HasColumnType("integer");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("MatrixId");

                    b.ToTable("AssessmentMatrixElements");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AssessmentId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<int>("JudgeId")
                        .HasColumnType("integer");

                    b.Property<DateOnly?>("ResultDate")
                        .HasColumnType("date");

                    b.Property<int>("SystemStatus")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AssessmentId");

                    b.HasIndex("JudgeId");

                    b.ToTable("AssessmentResults");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentResultValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AssessmentMatrixRow")
                        .HasColumnType("integer");

                    b.Property<int>("AssessmentResultId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<int>("Value")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AssessmentResultId");

                    b.ToTable("AssessmentResultValues");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AssessmentMatrixId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SystemAssessmentType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AssessmentMatrixId");

                    b.ToTable("AssessmentTypes");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.Grade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CorporateCompetenciesConclusion")
                        .HasColumnType("text");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<bool>("IsKpisFinalized")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsMarksFinalized")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsProjectsFinalized")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsQualificationFinalized")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsStrategicTasksFinalized")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsValueJudgmentFinalized")
                        .HasColumnType("boolean");

                    b.Property<string>("KPIsConclusion")
                        .HasColumnType("text");

                    b.Property<string>("ManagmentCompetenciesConclusion")
                        .HasColumnType("text");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<string>("QualificationConclusion")
                        .HasColumnType("text");

                    b.Property<int>("QualificationId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<string>("StrategicTasksConclusion")
                        .HasColumnType("text");

                    b.Property<int>("SystemStatus")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("ValueJudgmentId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("QualificationId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.HasIndex("ValueJudgmentId")
                        .IsUnique();

                    b.ToTable("Grades");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.Kpi", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CalculationMethod")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CompletionPercentage")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<int>("GradeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("PeriodEndDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("PeriodStartDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("GradeId");

                    b.ToTable("Kpis");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.Mark", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<int>("GradeId")
                        .HasColumnType("integer");

                    b.Property<int>("MarkTypeId")
                        .HasColumnType("integer");

                    b.Property<int>("PercentageValue")
                        .HasColumnType("integer");

                    b.Property<string>("Period")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GradeId");

                    b.HasIndex("MarkTypeId");

                    b.ToTable("Marks");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.MarkType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("MarkTypes");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.PreviousJob", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("OrganizationName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PositionName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("QualificationId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.HasIndex("QualificationId");

                    b.ToTable("PreviousJobs");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("CurrentStatusDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<int>("FactStages")
                        .HasColumnType("integer");

                    b.Property<int>("GradeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PlanStages")
                        .HasColumnType("integer");

                    b.Property<int>("SPn")
                        .HasColumnType("integer");

                    b.Property<string>("Stage")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<string>("SupervisorSspName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GradeId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.Qualification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AdditionalEducation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CurrentExperienceMonths")
                        .HasColumnType("integer");

                    b.Property<int>("CurrentExperienceYears")
                        .HasColumnType("integer");

                    b.Property<string>("CurrentJobPositionName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("CurrentJobStartDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("CurrentStatusDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<string>("EmploymentContarctTerminations")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<int>("GradeId")
                        .HasColumnType("integer");

                    b.Property<string>("HigherEducation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("QualificationResult")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Speciality")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<string>("SupervisorSspName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Qualifications");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.StrategicTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<DateOnly>("FactDate")
                        .HasColumnType("date");

                    b.Property<string>("FactResult")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("GradeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("PlanDate")
                        .HasColumnType("date");

                    b.Property<string>("PlanResult")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Purpose")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Remark")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GradeId");

                    b.ToTable("StrategicTasks");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.TrainingEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Competence")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<int>("GradeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GradeId");

                    b.ToTable("TrainingEvents");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.ValueJudgment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BehaviorToCorrect")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<int>("GradeId")
                        .HasColumnType("integer");

                    b.Property<string>("RecommendationsForDevelopment")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Strengths")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ValueJudgments");
                });

            modelBuilder.Entity("KOP.DAL.Entities.Subdivision", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NestingLevel")
                        .HasColumnType("integer");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Subdivisions");
                });

            modelBuilder.Entity("KOP.DAL.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("ContractEndDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("ContractStartDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("DateOfCreation")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GradeGroup")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateOnly>("HireDate")
                        .HasColumnType("date");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsSuspended")
                        .HasColumnType("boolean");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ParentSubdivisionId")
                        .HasColumnType("integer");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ServiceNumber")
                        .HasColumnType("integer");

                    b.Property<string>("SubdivisionFromFile")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int[]>("SystemRoles")
                        .IsRequired()
                        .HasColumnType("integer[]");

                    b.HasKey("Id");

                    b.HasIndex("ParentSubdivisionId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SubdivisionUser", b =>
                {
                    b.Property<int>("SubordinateSubdivisionsId")
                        .HasColumnType("integer");

                    b.Property<int>("SupervisingUsersId")
                        .HasColumnType("integer");

                    b.HasKey("SubordinateSubdivisionsId", "SupervisingUsersId");

                    b.HasIndex("SupervisingUsersId");

                    b.ToTable("SubdivisionUser");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.Assessment", b =>
                {
                    b.HasOne("KOP.DAL.Entities.AssessmentEntities.AssessmentType", "AssessmentType")
                        .WithMany("Assessments")
                        .HasForeignKey("AssessmentTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KOP.DAL.Entities.GradeEntities.Grade", "Grade")
                        .WithMany("Assessments")
                        .HasForeignKey("GradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KOP.DAL.Entities.User", "User")
                        .WithMany("Assessments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssessmentType");

                    b.Navigation("Grade");

                    b.Navigation("User");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentInterpretation", b =>
                {
                    b.HasOne("KOP.DAL.Entities.AssessmentEntities.AssessmentType", "AssessmentType")
                        .WithMany("AssessmentInterpretations")
                        .HasForeignKey("AssessmentTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssessmentType");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentMatrixElement", b =>
                {
                    b.HasOne("KOP.DAL.Entities.AssessmentEntities.AssessmentMatrix", "Matrix")
                        .WithMany("Elements")
                        .HasForeignKey("MatrixId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Matrix");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentResult", b =>
                {
                    b.HasOne("KOP.DAL.Entities.AssessmentEntities.Assessment", "Assessment")
                        .WithMany("AssessmentResults")
                        .HasForeignKey("AssessmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KOP.DAL.Entities.User", "Judge")
                        .WithMany()
                        .HasForeignKey("JudgeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Assessment");

                    b.Navigation("Judge");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentResultValue", b =>
                {
                    b.HasOne("KOP.DAL.Entities.AssessmentEntities.AssessmentResult", "AssessmentResult")
                        .WithMany("AssessmentResultValues")
                        .HasForeignKey("AssessmentResultId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssessmentResult");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentType", b =>
                {
                    b.HasOne("KOP.DAL.Entities.AssessmentEntities.AssessmentMatrix", "AssessmentMatrix")
                        .WithMany("AssessmentTypes")
                        .HasForeignKey("AssessmentMatrixId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssessmentMatrix");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.Grade", b =>
                {
                    b.HasOne("KOP.DAL.Entities.GradeEntities.Qualification", "Qualification")
                        .WithOne("Grade")
                        .HasForeignKey("KOP.DAL.Entities.GradeEntities.Grade", "QualificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KOP.DAL.Entities.User", "User")
                        .WithMany("Grades")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KOP.DAL.Entities.GradeEntities.ValueJudgment", "ValueJudgment")
                        .WithOne("Grade")
                        .HasForeignKey("KOP.DAL.Entities.GradeEntities.Grade", "ValueJudgmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Qualification");

                    b.Navigation("User");

                    b.Navigation("ValueJudgment");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.Kpi", b =>
                {
                    b.HasOne("KOP.DAL.Entities.GradeEntities.Grade", "Grade")
                        .WithMany("Kpis")
                        .HasForeignKey("GradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Grade");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.Mark", b =>
                {
                    b.HasOne("KOP.DAL.Entities.GradeEntities.Grade", "Grade")
                        .WithMany("Marks")
                        .HasForeignKey("GradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KOP.DAL.Entities.GradeEntities.MarkType", "MarkType")
                        .WithMany("Marks")
                        .HasForeignKey("MarkTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Grade");

                    b.Navigation("MarkType");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.PreviousJob", b =>
                {
                    b.HasOne("KOP.DAL.Entities.GradeEntities.Qualification", "Qualification")
                        .WithMany("PreviousJobs")
                        .HasForeignKey("QualificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Qualification");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.Project", b =>
                {
                    b.HasOne("KOP.DAL.Entities.GradeEntities.Grade", "Grade")
                        .WithMany("Projects")
                        .HasForeignKey("GradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Grade");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.StrategicTask", b =>
                {
                    b.HasOne("KOP.DAL.Entities.GradeEntities.Grade", "Grade")
                        .WithMany("StrategicTasks")
                        .HasForeignKey("GradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Grade");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.TrainingEvent", b =>
                {
                    b.HasOne("KOP.DAL.Entities.GradeEntities.Grade", "Grade")
                        .WithMany("TrainingEvents")
                        .HasForeignKey("GradeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Grade");
                });

            modelBuilder.Entity("KOP.DAL.Entities.Subdivision", b =>
                {
                    b.HasOne("KOP.DAL.Entities.Subdivision", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("KOP.DAL.Entities.User", b =>
                {
                    b.HasOne("KOP.DAL.Entities.Subdivision", "ParentSubdivision")
                        .WithMany("Users")
                        .HasForeignKey("ParentSubdivisionId");

                    b.Navigation("ParentSubdivision");
                });

            modelBuilder.Entity("SubdivisionUser", b =>
                {
                    b.HasOne("KOP.DAL.Entities.Subdivision", null)
                        .WithMany()
                        .HasForeignKey("SubordinateSubdivisionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KOP.DAL.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("SupervisingUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.Assessment", b =>
                {
                    b.Navigation("AssessmentResults");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentMatrix", b =>
                {
                    b.Navigation("AssessmentTypes");

                    b.Navigation("Elements");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentResult", b =>
                {
                    b.Navigation("AssessmentResultValues");
                });

            modelBuilder.Entity("KOP.DAL.Entities.AssessmentEntities.AssessmentType", b =>
                {
                    b.Navigation("AssessmentInterpretations");

                    b.Navigation("Assessments");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.Grade", b =>
                {
                    b.Navigation("Assessments");

                    b.Navigation("Kpis");

                    b.Navigation("Marks");

                    b.Navigation("Projects");

                    b.Navigation("StrategicTasks");

                    b.Navigation("TrainingEvents");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.MarkType", b =>
                {
                    b.Navigation("Marks");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.Qualification", b =>
                {
                    b.Navigation("Grade")
                        .IsRequired();

                    b.Navigation("PreviousJobs");
                });

            modelBuilder.Entity("KOP.DAL.Entities.GradeEntities.ValueJudgment", b =>
                {
                    b.Navigation("Grade")
                        .IsRequired();
                });

            modelBuilder.Entity("KOP.DAL.Entities.Subdivision", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("KOP.DAL.Entities.User", b =>
                {
                    b.Navigation("Assessments");

                    b.Navigation("Grades");
                });
#pragma warning restore 612, 618
        }
    }
}
