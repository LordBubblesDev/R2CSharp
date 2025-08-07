using Avalonia.Controls;
using Avalonia.VisualTree;
using R2CSharp.Models;
using R2CSharp.Views;

namespace R2CSharp.Helpers;

public abstract class ButtonHelper
{
    public static void UpdateVisualSelection(PageConfiguration page, RebootOptionPageView currentPageView)
    {
        var buttons = new List<Button>();
        FindButtonsRecursively(currentPageView, buttons);
        
        foreach (var button in buttons) {
            button.Classes.Remove("selected");
        }
        
        if (page.SelectedIndex >= 0 && page.SelectedIndex < buttons.Count) {
            buttons[page.SelectedIndex].Classes.Add("selected");
        }
    }
    
    public static void ClearAllSelections(RebootOptionPageView currentPageView)
    {
        var buttons = new List<Button>();
        FindButtonsRecursively(currentPageView, buttons);
        
        foreach (var button in buttons) {
            button.Classes.Remove("selected");
        }
    }
    
    public static void ApplyPressedState(Button button)
    {
        button.Classes.Add("pressed");
    }
    
    public static void RemovePressedState(Button button)
    {
        button.Classes.Remove("pressed");
    }
    
    public static List<Button> FindButtonsInPage(RebootOptionPageView currentPageView)
    {
        var buttons = new List<Button>();
        FindButtonsRecursively(currentPageView, buttons);
        return buttons;
    }
    
    private static void FindButtonsRecursively(Control control, List<Button> buttons)
    {
        if (control is Button button && button.Name == "OptionButton") {
            buttons.Add(button);
        }
        
        foreach (var child in control.GetVisualChildren()) {
            if (child is Control childControl) {
                FindButtonsRecursively(childControl, buttons);
            }
        }
    }
} 