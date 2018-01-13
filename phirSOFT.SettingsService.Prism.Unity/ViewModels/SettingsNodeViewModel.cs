using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommonServiceLocator;
using Prism;
using Prism.Regions;

namespace phirSOFT.SettingsService.Prism.Unity.ViewModels
{
    internal class SettingsNodeViewModel : ITreeModel<SettingsNodeViewModel>, IActiveAware
    {
        private readonly IRegionManager _regionManager;
        private string _filter;
        private bool _isActive;

        public SettingsNodeViewModel(ITreeNode node)
        {
            Node = node;
            _regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            Children = new Collection<SettingsNodeViewModel>();
        }

        public string Filter
        {
            get => _filter;
            set
            {
                if (value.StartsWith(_filter) && !IsVisible)
                {
                    _filter = value;
                    return;
                }

                _filter = value;
                UpdateFilter();
            }
        }

        public bool IsVisible { get; set; }

        public string DisplayLabel => Node.DisplayText;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnIsActiveChanged();
                if (value)
                    _regionManager.RequestNavigate("SettingsPane", SettingPage.NavigationUri);
            }
        }

        public event EventHandler IsActiveChanged;

        public ITreeNode Node { get; }

        public ISettingPage SettingPage { get; set; }

        public ICollection<SettingsNodeViewModel> Children { get; }

        private void UpdateFilter()
        {
            foreach (var child in Children)
                child.Filter = _filter;

            IsVisible = Node.DisplayText.Contains(Filter) || Children.Any(c => c.IsVisible);
        }

        protected virtual void OnIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        public static SettingsNodeViewModel Create(ITreeNode node)
        {
            return new SettingsNodeViewModel(node);
        }
    }
}