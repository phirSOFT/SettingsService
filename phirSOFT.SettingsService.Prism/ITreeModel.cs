using System.Collections.Generic;

namespace phirSOFT.SettingsService.Prism
{
    public interface ITreeModel<T> where T : ITreeModel<T>
    {
        ITreeNode Node { get; }
        ISettingPage SettingPage { get; set; }
        ICollection<T> Children { get; }
    }
}