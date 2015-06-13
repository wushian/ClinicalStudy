using System;
using System.Configuration;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using DevExpress.Web;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ClinicalStudy.DemoData;
using ClinicalStudy.DomainModel.ModelBuilders;
using ClinicalStudy.ModelBuilders;
using ClinicalStudy.Repositories.Interface;
using ClinicalStudy.Repositories.Interface.FormData;
using ClinicalStudy.Repositories.MemoryRepositories.DataStorage;
using ClinicalStudy.Site.IoC;
using DbRepositories = ClinicalStudy.Repositories.EntityFrameworkRepository;
using MemoryRepositories = ClinicalStudy.Repositories.MemoryRepositories;
using DevExpress.DemoData.Helpers;

namespace ClinicalStudy.Site {
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication {
		public bool UseDbStorage {
			get {
				var siteModeString = ConfigurationManager.AppSettings["SiteMode"];
				var siteMode = true;
				bool.TryParse(siteModeString, out siteMode);
				return !siteMode;
			}
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute(){View = "Error"});
		}

		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");


			routes.MapRoute(
				"RefreshSession",
				"RefreshSession",
				new {controller = "Account", action = "RefreshSession"}
			);

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Account", action = "Logon", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		protected void Session_Start() {
			if (!UseDbStorage) {
				var initializator = IoCContainer.Instance.Resolve<InMemoryPerSessionInitialisator>();
				initializator.InitialisePerSessionData();
			}
			else {
				var initializator = IoCContainer.Instance.Resolve<FirstStartInitializer>();
				initializator.ReinitializeDateIfRequired();
			}
		}

		protected void Application_Start() {
			//register our IoC here
			RegisterCastleWindsorDependencyResolver();
			ClinicalStudyThemeAssembly.ThemesProviderEx.Register();

            Regex regex = new Regex("loginPhoto\\w+_");
            BinaryStorageConfigurator.RegisterStorageStrategy(new LoginPhotoImageStorageStrategy(),
                delegate(ASPxWebControlBase control) {
                    return control.ID != null && regex.IsMatch(control.ID);
                }
            );

			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
            DevExpress.Web.Internal.DemoUtils.RegisterDemo("ClinicalStudy");
            DevExpress.Web.ASPxWebControl.CallbackError += new EventHandler(CallbackError);
        }

        void CallbackError(object sender, EventArgs e) {
            // Logging exceptions occur on callback events of DevExpress ASP.NET MVC controls. 
            // To learn more, see http://www.devexpress.com/Support/Center/Example/Details/E4588
        }

		private void RegisterCastleWindsorDependencyResolver() {
			var container = new WindsorContainer();
			container.AddFacility<TypedFactoryFacility>();
			//register every controller in this assebmly
			container.Register(Classes
				.FromThisAssembly()
				.BasedOn<IController>()
				.LifestyleTransient());
			//Register clinical study design factory, auto-implemented by Windsor
			container.Register(Component.For<IClinicalStudyDesignFactory>().AsFactory());
			//register Clinical Study Design object
			container.Register(Component
				.For<IClinicalStudyDesign>()
				.ImplementedBy<ClinicalStudyDesign>());
			container.Register(Component
				.For<IChangeNoteBuilder>()
				.ImplementedBy<ChangeNoteBuilder>());


			IoCContainer.SetInstance(container);

			if (UseDbStorage) {
				RegisterDatabaseRepositories(container);
			}
			else {
				RegisterInMemoryRepositories(container);
			}

			var controllerFactory = new WindsorControllerFactory(container.Kernel);
			ControllerBuilder.Current.SetControllerFactory(controllerFactory);
		}

		private static void RegisterInMemoryRepositories(IWindsorContainer container) {
			//register per-session data storage
			container.Register(Component
				.For<IDataStorage>()
				.ImplementedBy<PerSessionDataStorage>())
				//register in-memory repositories
				.Register(Component
					.For<IUserRepository>()
					.ImplementedBy<MemoryRepositories.UserRepository>())
				.Register(Component
					.For<IClinicRepository>()
					.ImplementedBy<MemoryRepositories.ClinicRepository>())
				.Register(Component
					.For<IPatientRepository>()
					.ImplementedBy<MemoryRepositories.PatientRepository>())
				.Register(Component
					.For<IVisitRepository>()
					.ImplementedBy<MemoryRepositories.VisitRepository>())
				.Register(Component
					.For<IFormRepository>()
					.ImplementedBy<MemoryRepositories.FormRepository>())
				.Register(Component
					.For<IQuestionRepository>()
					.ImplementedBy<MemoryRepositories.QuestionRepository>())
				.Register(Component
					.For<IQueryRepository>()
					.ImplementedBy<MemoryRepositories.QueryRepository>())
				.Register(Component
					.For<IAttachmentRepository>()
					.ImplementedBy<MemoryRepositories.AttachmentRepository>())
				.Register(Component
					.For<IDemographicFormDataRepository>()
					.ImplementedBy<MemoryRepositories.FormData.DemographicFormDataRepository>())
				.Register(Component
					.For<IVitalsFormDataRepository>()
					.ImplementedBy<MemoryRepositories.FormData.VitalsFormDataRepository>())
				.Register(Component
					.For<IHappinessFormDataRepository>()
					.ImplementedBy<MemoryRepositories.FormData.HappinessFormDataRepository>())
				.Register(Component
					.For<IAdverseEventFormDataRepository>()
					.ImplementedBy<MemoryRepositories.FormData.AdverseEventFormDataRepository>())
				.Register(Component
					.For<IElectrocardiogramFormDataRepository>()
					.ImplementedBy<MemoryRepositories.FormData.ElectrocardiogramFormDataRepository>())
				.Register(Component
					.For<IInventoryFormDataRepository>()
					.ImplementedBy<MemoryRepositories.FormData.InventoryFormDataRepository>())
				.Register(Component
					.For<IChangeNoteRepository>()
					.ImplementedBy<MemoryRepositories.ChangeNoteRepository>());
			container.Register(
				Component
					.For<DbRepositories.ClinicalStudyContext>()
					.LifestyleTransient()
					.DependsOn(new { connectionString = DatabaseConnectionString}));
			container.Register(Component.For<InMemoryPerSessionInitialisator>().LifestyleTransient());
		}

		private static void RegisterDatabaseRepositories(IWindsorContainer container) {


			var connectionString = DatabaseConnectionString;
			//register per-session data storage
			container
				.Register(Component
							.For<DbRepositories.ClinicalStudyContext>()
							.LifestylePerWebRequest()
							.DependsOn(new { connectionString }))
				.Register(Component
							.For<DbRepositories.IClinicalStudyContextFactory>()
							.AsFactory())
				.Register(Component
							.For<FirstStartInitializer>()
							.LifestyleTransient())
				.Register(Component
							.For<IUserRepository>()
							.ImplementedBy<DbRepositories.UserRepository>())
				.Register(Component
							.For<IClinicRepository>()
							.ImplementedBy<DbRepositories.ClinicRepository>())
				.Register(Component
							.For<IPatientRepository>()
							.ImplementedBy<DbRepositories.PatientRepository>())
				.Register(Component
							.For<IVisitRepository>()
							.ImplementedBy<DbRepositories.VisitRepository>())
				.Register(Component
							.For<IFormRepository>()
							.ImplementedBy<DbRepositories.FormRepository>())
				.Register(Component
							.For<IQuestionRepository>()
							.ImplementedBy<DbRepositories.QuestionRepository>())
				.Register(Component
							.For<IQueryRepository>()
							.ImplementedBy<DbRepositories.QueryRepository>())
				.Register(Component
							.For<IAttachmentRepository>()
							.ImplementedBy<DbRepositories.AttachmentRepository>())
				.Register(Component
							.For<IDemographicFormDataRepository>()
							.ImplementedBy<DbRepositories.FormData.DemographicFormDataRepository>())
				.Register(Component
							.For<IVitalsFormDataRepository>()
							.ImplementedBy<DbRepositories.FormData.VitalsFormDataRepository>())
				.Register(Component
							.For<IHappinessFormDataRepository>()
							.ImplementedBy<DbRepositories.FormData.HappinessFormDataRepository>())
				.Register(Component
							.For<IAdverseEventFormDataRepository>()
							.ImplementedBy<DbRepositories.FormData.AdverseEventFormDataRepository>())
				.Register(Component
							.For<IElectrocardiogramFormDataRepository>()
							.ImplementedBy<DbRepositories.FormData.ElectrocardiogramFormDataRepository>())
				.Register(Component
							.For<IInventoryFormDataRepository>()
							.ImplementedBy<DbRepositories.FormData.InventoryFormDataRepository>())
				.Register(Component
							.For<IChangeNoteRepository>()
							.ImplementedBy<DbRepositories.ChangeNoteRepository>());


			Database.SetInitializer(new DemoDataInitializer(IoCContainer.Instance.Resolve<IClinicalStudyDesign>()));

		}

        private static string DatabaseConnectionString {
            get {
                string sqlExpressString = ConfigurationManager.ConnectionStrings["ClinicalStudyContext"].ConnectionString;
                return DbEngineDetector.PatchConnectionString(sqlExpressString);
            }
        }
    }
}
