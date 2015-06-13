using Castle.Windsor;

namespace ClinicalStudy.Site.IoC
{
    public static class IoCContainer
    {
        private static IWindsorContainer _container;

        static public void SetInstance(IWindsorContainer container)
        {
            _container = container;
        }

        public static IWindsorContainer Instance
        {
            get { return _container; }
        }
    }
}
