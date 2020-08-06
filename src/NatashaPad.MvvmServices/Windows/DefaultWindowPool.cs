using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NatashaPad.MvvmServices.Windows
{
    internal class DefaultWindowPool : IWindowPool
    {
        private readonly LinkedList<Element> list;

        public DefaultWindowPool()
        {
            list = new LinkedList<Element>();
        }

        //TODO: 不允许重复的vm加入
        public void Add<TView, TViewModel>(Window window, TView view, TViewModel viewModel) where TViewModel : class
        {
            list.AddFirst(new Element(view, viewModel, window));
        }

        public bool TryGetWindow<TViewModel>(TViewModel viewModel, out Window window) where TViewModel : class
        {
            var ele = list.FirstOrDefault(x => x.ViewModel is TViewModel vm && vm == viewModel);
            window = ele?.Window;
            return ele != default;
        }

        public void Remove(Window window)
        {
            var ele = list.FirstOrDefault(x => x.Window == window);
            if (ele != default)
                list.Remove(ele);
        }

        public class Element
        {
            public Element(object view, object viewModel, Window window)
            {
                View = view;
                ViewModel = viewModel;
                Window = window;
            }

            public object View { get; }
            public object ViewModel { get; }
            public Window Window { get; }
        }
    }
}
