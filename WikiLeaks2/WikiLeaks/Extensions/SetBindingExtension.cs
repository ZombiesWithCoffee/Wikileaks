using System.Windows.Data;
using WikiLeaks.Properties;

namespace Wikileaks.Extensions {
    public class SettingBindingExtension : Binding {

        public SettingBindingExtension() {
            Initialize();
        }

        public SettingBindingExtension(string path) : base(path) {
            Initialize();
        }

        void Initialize() {
            Source = Settings.Default;
            Mode = BindingMode.TwoWay;
        }
    }
}
