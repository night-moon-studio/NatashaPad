using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace NewMvvm
{
    public class ViewContainerOptions : Collection<RegisterInfo>
    {
        public ViewContainerOptions()
        {
        }

        public void Register<TView, TViewModel>()
        {
            Add(new RegisterInfo(typeof(TView), typeof(TViewModel)));
        }

        public void Register<TView, TViewModel>(Action<ViewInfo> action)
        {
            var reg = new RegisterInfo(typeof(TView), typeof(TViewModel));

            var viewInfo = new ViewInfo();
            action?.Invoke(viewInfo);
            reg.ViewInfo = viewInfo;

            Add(reg);
        }
    }

    public sealed class RegisterInfo
    {
        public RegisterInfo(Type viewType, Type viewModelType, string name)
        {
            ViewType = viewType;
            ViewModelType = viewModelType;
            Name = name;
        }

        public RegisterInfo(Type viewType, Type viewModelType) : this(viewType, viewModelType, string.Empty)
        {
        }

        public string Name { get; }

        public Type ViewType { get; }
        public Type ViewModelType { get; }

        public ViewInfo ViewInfo { get; set; }
    }

    public class ViewInfo
    {
        public int? Width { get; set; }
        public int? Height { get; set; }

        public string Title { get; set; }

        public WindowStartupLocation WindowStartupLocation { get; set; } = WindowStartupLocation.Manual;

        public SizeToContent? SizeToContent { get; set; }
    }
}
