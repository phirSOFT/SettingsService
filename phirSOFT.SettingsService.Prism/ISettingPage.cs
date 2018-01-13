using System;
using System.Collections.Generic;

namespace phirSOFT.SettingsService.Prism
{
    public interface ISettingPage
    {
        Uri NavigationUri { get; }

        IEnumerable<ITreeNode> Path { get; }
    }
}