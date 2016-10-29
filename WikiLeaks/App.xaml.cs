using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows;
using WikiLeaks.Properties;

namespace WikiLeaks {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {

        void Application_Exit(object sender, ExitEventArgs e) {
            Settings.Default.Save();
        }

        public static CompositionContainer Container;

        [Import]
        public MainWindow StartWindow
        {
            get { return Current?.MainWindow as MainWindow; }
            set { Current.MainWindow = value; }
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            try {
                ComposeMef();
            }
            catch (ReflectionTypeLoadException ex) {
                foreach (var exception in ex.LoaderExceptions) {
                    MessageBox.Show(exception.Message, ex.GetType().ToString());
                }

                Shutdown();
            }

            StartWindow.Show();
        }

        void ComposeMef() {

            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

            Container = new CompositionContainer(aggregateCatalog);
            Container.ComposeParts(this);
        }
    }

}
