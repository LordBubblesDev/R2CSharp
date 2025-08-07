using Avalonia.Controls;
using Avalonia.VisualTree;
using R2CSharp.Models;
using R2CSharp.Views;

namespace R2CSharp.Helpers;

public class ButtonHelper
{
    public void UpdateVisualSelection(PageConfiguration page, StandardPageView currentPageView)
    {
        var buttons = new List<Button>();
        FindButtonsRecursively(currentPageView, buttons);
        
        foreach (var button in buttons)
        {
            button.Classes.Remove("selected");
        }
        
        if (page.SelectedIndex >= 0 && page.SelectedIndex < buttons.Count)
        {
            buttons[page.SelectedIndex].Classes.Add("selected");
        }
    }
    
    public void ClearAllSelections(StandardPageView currentPageView)
    {
        var buttons = new List<Button>();
        FindButtonsRecursively(currentPageView, buttons);
        
        foreach (var button in buttons)
        {
            button.Classes.Remove("selected");
        }
    }
    
    public void ApplyPressedState(Button button)
    {
        button.Classes.Add("pressed");
    }
    
    public void RemovePressedState(Button button)
    {
        button.Classes.Remove("pressed");
    }
    
    public List<Button> FindButtonsInPage(StandardPageView currentPageView)
    {
        var buttons = new List<Button>();
        FindButtonsRecursively(currentPageView, buttons);
        return buttons;
    }
    
    private static void FindButtonsRecursively(Control control, List<Button> buttons)
    {
        if (control is Button button && button.Name == "OptionButton")
        {
            buttons.Add(button);
        }
        
        foreach (var child in control.GetVisualChildren())
        {
            if (child is Control childControl)
            {
                FindButtonsRecursively(childControl, buttons);
            }
        }
    }
} 