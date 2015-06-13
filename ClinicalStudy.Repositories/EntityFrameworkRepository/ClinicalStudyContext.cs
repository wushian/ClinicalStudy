using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using ClinicalStudy.DomainModel;
using ClinicalStudy.DomainModel.FormData;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalStudy.Repositories.EntityFrameworkRepository {
	public class ClinicalStudyContext : DbContext {

		public ClinicalStudyContext(string connectionString) : base(connectionString) {
		}


		public DbSet<User> Users { get; set; }
		public DbSet<Clinic> Clinics { get; set; }
		public DbSet<Patient> Patients { get; set; }
		public DbSet<Visit> Visits { get; set; }
		public DbSet<Form> Forms { get; set; }

		public DbSet<DemographicFormData> DemographicFormDatas { get; set; }
		public DbSet<HappinessFormData> HappinessFormDatas { get; set; }
		public DbSet<InventoryFormData> InventoryFormDatas { get; set; }
		public DbSet<VitalsFormData> VitalsFormDatas { get; set; }
		public DbSet<ElectrocardiogramFormData> ElectrocardiogramFormDatas { get; set; }
		public DbSet<AdverseEventFormData> AdverseEventFormDatas { get; set; }

		public DbSet<Question> Questions { get; set; }
		public DbSet<Query> Queries { get; set; }
		public DbSet<Attachment> Attachments { get; set; }

		public DbSet<ChangeNote> ChangeNotes { get; set; }

		public DbSet<RepeatableInventoryData> RepeatableInventoryDatas { get; set; }

		public DbSet<Timepoint> Timepoints { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Entity<User>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<Clinic>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<Patient>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<Visit>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<Form>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

			modelBuilder.Entity<DemographicFormData>().Property(e => e.Id).HasDatabaseGeneratedOption(
				DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<HappinessFormData>().Property(e => e.Id).HasDatabaseGeneratedOption(
				DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<InventoryFormData>().Property(e => e.Id).HasDatabaseGeneratedOption(
				DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<VitalsFormData>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<ElectrocardiogramFormData>().Property(e => e.Id).HasDatabaseGeneratedOption(
				DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<AdverseEventFormData>().Property(e => e.Id).HasDatabaseGeneratedOption(
				DatabaseGeneratedOption.Identity);

			modelBuilder.Entity<Question>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			modelBuilder.Entity<Attachment>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

			modelBuilder.Entity<RepeatableInventoryData>().Property(e => e.Id).HasDatabaseGeneratedOption(
				DatabaseGeneratedOption.Identity);


			modelBuilder.Entity<ChangeNote>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

			modelBuilder.Entity<Timepoint>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			base.OnModelCreating(modelBuilder);
		}
	}
}
