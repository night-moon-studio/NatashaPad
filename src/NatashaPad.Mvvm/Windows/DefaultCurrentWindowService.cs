// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

using System.Windows;

namespace NatashaPad.Mvvm.Windows;

internal class DefaultCurrentWindowService : ICurrentWindowService
{
    private readonly Window window;

    public DefaultCurrentWindowService(Window window)
    {
        this.window = window;
    }

    public void Close()
    {
        window.Close();
    }

    public void Hide()
    {
        window.Hide();
    }

    public void Show()
    {
        window.Show();
    }
}
