using Avalonia.Controls;
using Avalonia.Animation;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace R2CSharp.Controls;

public class ControlledCarousel : Carousel
{
    private bool _allowNavigation = false;
    
    public ControlledCarousel()
    {
        // Ensure proper initialization
        System.Diagnostics.Debug.WriteLine("[ControlledCarousel] Constructor called");
        
        // Subscribe to property changes to debug
        this.PropertyChanged += (sender, e) =>
        {
            if (e.Property == ItemsSourceProperty)
            {
                System.Diagnostics.Debug.WriteLine($"[ControlledCarousel] ItemsSource changed: {ItemsSource}");
            }
            else if (e.Property == SelectedIndexProperty)
            {
                System.Diagnostics.Debug.WriteLine($"[ControlledCarousel] SelectedIndex changed: {SelectedIndex}");
            }
            else if (e.Property == DataContextProperty)
            {
                System.Diagnostics.Debug.WriteLine($"[ControlledCarousel] DataContext changed: {DataContext}");
            }
        };
        
        // Add static constructor debug
        System.Diagnostics.Debug.WriteLine("[ControlledCarousel] Static constructor check");
    }
    
    static ControlledCarousel()
    {
        System.Diagnostics.Debug.WriteLine("[ControlledCarousel] Static constructor called");
    }
    
    /// <summary>
    /// Allows navigation to be controlled externally
    /// </summary>
    public void SetNavigationAllowed(bool allowed)
    {
        _allowNavigation = allowed;
        System.Diagnostics.Debug.WriteLine($"[ControlledCarousel] Navigation allowed: {allowed}");
    }
    
    /// <summary>
    /// Override Next() to only allow when navigation is explicitly allowed
    /// </summary>
    public new void Next()
    {
        if (_allowNavigation)
        {
            System.Diagnostics.Debug.WriteLine("[ControlledCarousel] Next() allowed - calling base");
            base.Next();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("[ControlledCarousel] Next() blocked - navigation not allowed");
        }
    }
    
    /// <summary>
    /// Override Previous() to only allow when navigation is explicitly allowed
    /// </summary>
    public new void Previous()
    {
        if (_allowNavigation)
        {
            System.Diagnostics.Debug.WriteLine("[ControlledCarousel] Previous() allowed - calling base");
            base.Previous();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("[ControlledCarousel] Previous() blocked - navigation not allowed");
        }
    }
    
    /// <summary>
    /// Override the SelectedIndex property to prevent unauthorized changes
    /// </summary>
    public new int SelectedIndex
    {
        get => base.SelectedIndex;
        set
        {
            // Allow initial selection (when SelectedIndex is -1 and we're setting to 0)
            if (_allowNavigation || (base.SelectedIndex == -1 && value == 0))
            {
                System.Diagnostics.Debug.WriteLine($"[ControlledCarousel] SelectedIndex change allowed: {value}");
                base.SelectedIndex = value;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[ControlledCarousel] SelectedIndex change blocked: {value}");
            }
        }
    }
    
    /// <summary>
    /// Override the SelectedItem property to prevent unauthorized changes
    /// </summary>
    public new object? SelectedItem
    {
        get => base.SelectedItem;
        set
        {
            // Allow initial selection (when SelectedItem is null and we're setting to first item)
            if (_allowNavigation || (base.SelectedItem == null && value != null))
            {
                System.Diagnostics.Debug.WriteLine($"[ControlledCarousel] SelectedItem change allowed");
                base.SelectedItem = value;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[ControlledCarousel] SelectedItem change blocked");
            }
        }
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        System.Diagnostics.Debug.WriteLine("[ControlledCarousel] Template applied");
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        System.Diagnostics.Debug.WriteLine($"[ControlledCarousel] Property changed: {change.Property.Name}");
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        System.Diagnostics.Debug.WriteLine($"[ControlledCarousel] OnLoaded - Children count: {this.GetVisualChildren().Count()}");
        foreach (var child in this.GetVisualChildren())
        {
            System.Diagnostics.Debug.WriteLine($"[ControlledCarousel] Child: {child.GetType().Name}");
        }
    }
} 