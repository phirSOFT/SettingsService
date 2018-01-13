using System;

namespace phirSOFT.SettingsService.Prism
{
    public interface ITreeNode : IEquatable<ITreeNode>
    {
        string DisplayText { get; }
    }
}