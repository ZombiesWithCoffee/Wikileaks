using System.Windows.Input;

namespace WikiLeaks {
    public interface IMainWindowViewModel {
        ICommand UpdateCommand { get; }
    }
}