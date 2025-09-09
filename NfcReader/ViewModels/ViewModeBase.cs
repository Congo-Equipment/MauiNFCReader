using CommunityToolkit.Mvvm.ComponentModel;

namespace NfcReader.ViewModels
{
    public partial class ViewModeBase : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;
    }
}
