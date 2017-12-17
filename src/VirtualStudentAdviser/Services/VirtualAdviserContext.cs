﻿using System;
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtualStudentAdviser.Models;

namespace VirtualStudentAdviser.Services
{
    /// <summary>
    /// DbContext for reading and writing to the vsaDev datavase
    /// </summary>
    public  class VirtualAdviserContext : DbContext
    {

        //public VirtualAdviserContext(DbContextOptions<VirtualAdviserContext> options)
        //    : base(options)
        //{
        //}

        public virtual DbSet<AcademicYear> AcademicYear { get; set; }
        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<CourseTime> CourseTime { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<JobType> JobType { get; set; }
        public virtual DbSet<Major> Major { get; set; }
        public virtual DbSet<School> School { get; set; }
        public virtual DbSet<Quarter> Quarter { get; set; }
        public virtual DbSet<Student> Student { get; set; } 
        public virtual DbSet<TimeSlot> TimeSlot { get; set; }
        public virtual DbSet<WeekDay> WeekDay { get; set; } 
        public virtual DbSet<Prerequisite> Prerequisite { get; set; }
        public virtual DbSet<AdmissionRequiredCourses> AdmissionRequiredCourses { get; set; }

        public virtual DbSet<GeneratedPlan> GeneratedPlan { get; set; }
        public virtual DbSet<StudyPlan> StudyPlan { get; set; }
        public virtual DbSet<StudentStudyPlan> StudentStudyPlan { get; set; }
        public virtual DbSet<ReviewedStudyPlan> ReviewedStudyPlan { get; set; } 
        public virtual DbSet<ParameterSet> ParameterSet { get; set; } 
        public virtual DbSet<Budget> Budget { get; set; }
        public virtual DbSet<TimePreference> TimePreference { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["VirtualAdvisorDatabase"].ConnectionString);
          // string conn = @"Server=fcmb-cdl-ba004; Database=VirtualAdvisor;User Id =sa; password=system";
          string conn = @"Server=sql11.ezhostingserver.com; Database=vsaDev;User Id =rc53kxCYoM; password=ThesisDefense17";
          optionsBuilder.UseSqlServer(conn);

        }
        
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddDbConstext<VirtualAdvisorContext>(options =>
        //        options.UseSqlServer(Configuration.GetConnectionString("VirtualAdvisorDatabase")));
        //    services.AddMvc();
        //    services.AddScoped<IVirtualAdviserRepository, VirtualAdviserRepository>();
        //}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AcademicYear>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.AbbreviatedTitle).HasColumnType("varchar(50)");

                entity.Property(e => e.CoRequisites).HasColumnType("varchar(250)");

                entity.Property(e => e.CourseNumber).HasColumnType("nchar(10)");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Description).HasColumnType("varchar(1000)");

                entity.Property(e => e.PreRequisites).HasColumnType("varchar(250)");

                entity.Property(e => e.Title).HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<CourseTime>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.CourseNumber).HasColumnType("nchar(10)");

                entity.Property(e => e.DayId).HasColumnName("DayID");

                entity.Property(e => e.EndTimeId).HasColumnName("EndTimeID");

                entity.Property(e => e.QuarterId).HasColumnName("QuarterID");

                entity.Property(e => e.StartTimeId).HasColumnName("StartTimeID");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CourseTime)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("FK_CourseTime_Course");

                entity.HasOne(d => d.Day)
                    .WithMany(p => p.CourseTime)
                    .HasForeignKey(d => d.DayId)
                    .HasConstraintName("FK_ClassTime_WeekDay");

                entity.HasOne(d => d.EndTime)
                    .WithMany(p => p.CourseTimeEndTime)
                    .HasForeignKey(d => d.EndTimeId)
                    .HasConstraintName("FK_ClassTime_TimeSlot1");

                entity.HasOne(d => d.Quarter)
                    .WithMany(p => p.CourseTime)
                    .HasForeignKey(d => d.QuarterId)
                    .HasConstraintName("FK_ClassTime_Quarter");

                entity.HasOne(d => d.StartTime)
                    .WithMany(p => p.CourseTimeStartTime)
                    .HasForeignKey(d => d.StartTimeId)
                    .HasConstraintName("FK_ClassTime_TimeSlot");
            });

            modelBuilder.Entity<Prerequisite>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.PrerequisiteId).HasColumnName("PrerequisiteID");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Prerequisite)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("FK_Prerequisite_Course");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Prerequisite)
                    .HasForeignKey(d => d.PrerequisiteId)
                    .HasConstraintName("FK_CPrerequisite_Course");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Code).HasColumnType("varchar(50)");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("varchar(300)");

                entity.Property(e => e.LastDateModified).HasColumnType("datetime");

                entity.Property(e => e.Name).HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<JobType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.JobType1)
                    .HasColumnName("JobType")
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Major>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DegreeType).HasColumnType("varchar(50)");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.Description).HasColumnType("varchar(250)");

                entity.Property(e => e.Name).HasColumnType("varchar(250)");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Major)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Major_Department");
            });

            modelBuilder.Entity<Quarter>(entity =>
            {
                entity.Property(e => e.QuarterId).HasColumnName("QuarterID");

                entity.Property(e => e.QuarterName)
                    .HasColumnName("Quarter")
                    .HasColumnType("nchar(10)");
                entity.Property(e => e.Status).HasColumnName("Status");

            });

            modelBuilder.Entity<School>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasColumnName("Name");
            });


        modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");

                entity.Property(e => e.JobTypeId).HasColumnName("JobTypeID");

                entity.Property(e => e.MajorId).HasColumnName("MajorID");

                entity.HasOne(d => d.JobType)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.JobTypeId)
                    .HasConstraintName("FK_Student_JobType");

                entity.HasOne(d => d.Major)
                    .WithMany(p => p.Student)
                    .HasForeignKey(d => d.MajorId)
                    .HasConstraintName("FK_Student_Major");
            });
             

            modelBuilder.Entity<TimeSlot>(entity =>
            {
                entity.HasKey(e => e.TimeId)
                    .HasName("PK_TimeSlot");

                entity.Property(e => e.TimeId).HasColumnName("TimeID");

                entity.Property(e => e.Time).HasColumnType("time(0)");
            });

            modelBuilder.Entity<WeekDay>(entity =>
            {
                entity.HasKey(e => e.DayId)
                    .HasName("PK_WeekDay");

                entity.Property(e => e.DayId).HasColumnName("DayID");

                entity.Property(e => e.WeekDay1)
                    .HasColumnName("WeekDay")
                    .HasColumnType("nchar(10)");
            });

            modelBuilder.Entity<GeneratedPlan>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasColumnType("varchar(50)");

                entity.Property(e => e.ParameterSetId).HasColumnName("ParameterSetID");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.LastDateModified).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnName("Status");

            });

            modelBuilder.Entity<ParameterSet>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MajorId).HasColumnName("MajorID");

                entity.Property(e => e.SchoolId).HasColumnName("SchoolID");

                entity.Property(e => e.JobTypeId).HasColumnName("JobTypeID");

                entity.Property(e => e.BudgetId).HasColumnName("BudgetID");

                entity.Property(e => e.TimePreferenceId).HasColumnName("TimePreferenceID");

                entity.Property(e => e.QuarterPreferenceId).HasColumnName("QuarterPreferenceID"); 

                entity.Property(e => e.CompletedCourses).HasColumnType("varchar(250)");

                entity.Property(e => e.PlacementCourses).HasColumnType("varchar(250)");
                 

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.LastDateModified).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnName("Status");

            });  

            modelBuilder.Entity<StudyPlan>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.Property(e => e.QuarterId).HasColumnName("QuarterID");

                entity.Property(e => e.YearId).HasColumnName("YearID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.LastDateModified).HasColumnType("datetime");

            });

            modelBuilder.Entity<StudentStudyPlan>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.LastDateModified).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnName("Status");
                entity.Property(e => e.PlanName).HasColumnName("PlanName");
            });

            modelBuilder.Entity<TimePreference>(entity =>
            {
                //    public int Id { get; set; }
                //  public string TimePeriod { get; set; }

                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.TimePeriod).HasColumnName("TimePeriod");
            });
        modelBuilder.Entity<ReviewedStudyPlan>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.Property(e => e.PlanName).HasColumnType("varchar(50)");

                entity.Property(e => e.AdvisorId).HasColumnName("AdvisorID");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.ReviewDate).HasColumnType("datetime");

                entity.Property(e => e.LastDateModified).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnName("Status");

            });

            modelBuilder.Entity<Budget>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.MaxCredit).HasColumnName("MaxCredit");
                entity.Property(e => e.Name).HasColumnName("Budget");
                entity.Property(e => e.ResidentStatusId).HasColumnName("ResidentStatusID");
            });
        }
        
    }
}