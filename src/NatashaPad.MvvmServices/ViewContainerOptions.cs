using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NatashaPad.MvvmServices
{
    public class ViewContainerOptions : Collection<Tuple<Type, Type>>
    {
        public ViewContainerOptions()
        {
        }

        public void Register<TView, TViewModel>()
        {
            Add(Tuple.Create(typeof(TView), typeof(TViewModel)));
        }
    }
}
