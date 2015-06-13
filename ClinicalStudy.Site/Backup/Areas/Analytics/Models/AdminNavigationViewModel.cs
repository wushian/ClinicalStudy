using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace ClinicalStudy.Site.Areas.Analytics.Models {
    public class AdminNavigationViewModel {
        public ItemsData NavigationCategories { get; set; }
    }

    public class ItemsData : IHierarchicalEnumerable, IEnumerable {
        public ItemsData(IEnumerable data) {
            Data = data;
        }

        public IEnumerable Data { get; set; }

        public IEnumerator GetEnumerator() {
            return Data.GetEnumerator();
        }

        public IHierarchyData GetHierarchyData(object enumeratedItem) {
            return (IHierarchyData) enumeratedItem;
        }

        public ItemData GetItem(string categoryName) {
            return (Data as List<ItemData>).Where(x => x.Name == categoryName).FirstOrDefault();
        }
    }

    public class ItemData : IHierarchyData {
        public ItemData() {
            HasChildren = false;
            Name = Text = ActiveImageName = InactiveImageName = String.Empty;
        }

        public ItemData(string name, string text, string activeImageName, string inactiveImageName, bool hasChildren) {
            Name = name;
            Text = text;
            HasChildren = hasChildren;
            ActiveImageName = activeImageName;
            InactiveImageName = inactiveImageName;
            if (hasChildren)
                Children = new ItemsData(new List<ItemData> {new ItemData()});
        }

        public string Name { get; set; }
        public string Text { get; set; }
        public string ActiveImageName { get; set; }
        public string InactiveImageName { get; set; }

        public ItemsData Children { get; set; }

        public bool HasChildren { get; set; }

        public string NavigateUrl { get; set; }

        // IHierarchyData
        bool IHierarchyData.HasChildren {
            get { return HasChildren; }
        }

        object IHierarchyData.Item {
            get { return this; }
        }

        string IHierarchyData.Path {
            get { return String.Empty; }
        }

        string IHierarchyData.Type {
            get { return GetType().ToString(); }
        }

        IHierarchicalEnumerable IHierarchyData.GetChildren() {
            return Children;
        }

        IHierarchyData IHierarchyData.GetParent() {
            return null;
        }
    }

    public static class ItemDataFactory {
        public static ItemData GetParentItem(string name, string text, string activeImageName, string inactiveImageName) {
            return new ItemData(name, text, activeImageName, inactiveImageName, true);
        }

        public static ItemData GetChildItem(string name, string text, string navigateUrl) {
            return new ItemData(name, text, String.Empty, String.Empty, false) { NavigateUrl = navigateUrl };
        }
    }
}
