// Copyright (c) NatashaPad. All rights reserved.
// Licensed under the Apache license.

namespace NatashaPad.Mvvm.MessageBox;

internal class DefaultErrorMessageBoxService : IErrorMessageBoxService
{
    public void ShowError(string title, string content)
    {
        System.Windows.MessageBox.Show(content, title);
    }
}
