using Avalonia.Controls;
using Avalonia.VisualTree;
using R2CSharp.Lib.Models;
using R2CSharp.Lib.Views;

namespace R2CSharp.Lib.Helpers;

public static class ButtonHelper
{
    private static List<Button> FindButtonsInPage(RebootOptionPageView currentPageView)
    {
        var buttons = new List<Button>();
        FindButtonsRecursively(currentPageView, buttons);
        return buttons;
    }
    
    private static void FindButtonsRecursively(Control control, List<Button> buttons)
    {
        if (control is Button button && button.Name == "OptionButton") buttons.Add(button);
        foreach (var child in control.GetVisualChildren()) {
            if (child is Control childControl) FindButtonsRecursively(childControl, buttons);
        }
    }
    
    public static void UpdateVisualSelection(PageConfiguration page, RebootOptionPageView currentPageView)
    {
        var buttons = FindButtonsInPage(currentPageView);
        foreach (var button in buttons) button.Classes.Remove("selected");
        if (page.SelectedIndex >= 0 && page.SelectedIndex < buttons.Count) {
            buttons[page.SelectedIndex].Classes.Add("selected");
        }
    }
    
    public static void ClearAllSelections(RebootOptionPageView currentPageView)
    {
        var buttons = FindButtonsInPage(currentPageView);
        foreach (var button in buttons) button.Classes.Remove("selected");
    }
    
    public static void HandleButtonPress(PageConfiguration currentPage, RebootOptionPageView currentPageView, int buttonIndex)
    {
        var buttons = FindButtonsInPage(currentPageView);
        if (buttonIndex >= buttons.Count) return;
        
        var selectedButton = buttons[buttonIndex];
        var selectedOption = currentPage.Options[buttonIndex];
        
        selectedButton.Classes.Add("pressed");
        if (selectedOption.Command?.CanExecute(selectedOption) == true) {
            selectedOption.Command.Execute(selectedOption);
        }
        
        Task.Delay(150).ContinueWith(_ => {
            Avalonia.Threading.Dispatcher.UIThread.Post(() => selectedButton.Classes.Remove("pressed"));
        });
    }
}
