using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace phirSOFT.SettingsService.Prism
{
    public static class TreeHelper
    {
        public static ICollection<T> BuildTree<T>(this IEnumerable<ISettingPage> settingPages,
            Func<ITreeNode, T> modelFactory)
            where T : class, ITreeModel<T>
        {
            var rootNodes = new Collection<T>();
            rootNodes.ExtendTree(settingPages, modelFactory);
            return rootNodes;
        }

        public static void ExtendTree<T>(this ICollection<T> tree, IEnumerable<ISettingPage> settingPages,
            Func<ITreeNode, T> modelFactory) where T : class, ITreeModel<T>
        {
            foreach (var settingPage in settingPages)
                using (var enumerator = settingPage.Path.GetEnumerator())
                {
                    if (enumerator.MoveNext()) tree.Insert(enumerator, settingPage, modelFactory);
                }
        }

        public static void Insert<T>(this ICollection<T> tree, IEnumerator<ITreeNode> settingPages, ISettingPage page,
            Func<ITreeNode, T> modelFactory) where T : class, ITreeModel<T>
        {
            while (true)
            {
                var currentNode = settingPages.Current;
                var child = tree.FirstOrDefault(model => model.Node.Equals(currentNode));

                if (child == null)
                {
                    child = modelFactory(currentNode);
                    tree.Add(child);
                }

                if (settingPages.MoveNext())
                {
                    tree = child.Children;
                    continue;
                }

                child.SettingPage = page;
                break;
            }
        }
    }
}