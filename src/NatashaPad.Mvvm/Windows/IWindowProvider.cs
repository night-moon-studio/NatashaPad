// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using System.Windows;

namespace NatashaPad.Mvvm.Windows;

public interface IWindowProvider
{
    Window Create(object view, object viewModel);
}
