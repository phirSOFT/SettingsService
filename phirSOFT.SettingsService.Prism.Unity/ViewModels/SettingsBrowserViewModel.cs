using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommonServiceLocator;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace phirSOFT.SettingsService.Prism.Unity.ViewModels
{
    internal class SettingsBrowserViewModel : BindableBase
    {
        private string _filter;

        public SettingsBrowserViewModel()
        {
            RootNodes = ServiceLocator.Current.GetAllInstances<ISettingPage>().BuildTree(SettingsNodeViewModel.Create);
        }

        public ICollection<SettingsNodeViewModel> RootNodes { get; }

        public string Filter
        {
            get => _filter;
            set
            {
                SetProperty(ref _filter, value, () =>
                {
                    foreach (var node in RootNodes)
                        node.Filter = _filter;
                });
            }
        }

    }
}