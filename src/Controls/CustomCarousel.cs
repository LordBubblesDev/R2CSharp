using Avalonia;
using Avalonia.Controls;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using System.Collections.Generic;
using System.Linq;

namespace R2CSharp.Controls;

public class CustomCarousel : Panel
{
    private int _currentIndex = 0;
    private List<Control> _pages = new();
    private Control? _currentPage;
    private Control? _nextPage;
    private bool _isAnimating = false;
    private bool _allowNavigation = false;
    
    public static readonly DirectProperty<CustomCarousel, int> CurrentIndexProperty =
        AvaloniaProperty.RegisterDirect<CustomCarousel, int>(
            nameof(CurrentIndex),
            o => o._currentIndex,
            (o, v) => o.SetCurrentIndex(v));
    
    public static readonly DirectProperty<CustomCarousel, bool> AllowNavigationProperty =
        AvaloniaProperty.RegisterDirect<CustomCarousel, bool>(
            nameof(AllowNavigation),
            o => o._allowNavigation,
            (o, v) => o._allowNavigation = v);
    
    public int CurrentIndex
    {
        get => _currentIndex;
        set => SetCurrentIndex(value);
    }
    
    public bool AllowNavigation
    {
        get => _allowNavigation;
        set => _allowNavigation = value;
    }
    
    public CustomCarousel()
    {
        System.Diagnostics.Debug.WriteLine("[CustomCarousel] Constructor called");
    }
    
    public void AddPage(Control page)
    {
        _pages.Add(page);
        System.Diagnostics.Debug.WriteLine($"[CustomCarousel] Page added, total pages: {_pages.Count}");
        
        if (_currentPage == null && _pages.Count > 0)
        {
            SetCurrentIndex(0);
        }
    }
    
    public void Next()
    {
        if (!_allowNavigation)
        {
            System.Diagnostics.Debug.WriteLine("[CustomCarousel] Next() blocked - navigation not allowed");
            return;
        }
        
        if (_currentIndex < _pages.Count - 1 && !_isAnimating)
        {
            System.Diagnostics.Debug.WriteLine($"[CustomCarousel] Next() allowed - moving from {_currentIndex} to {_currentIndex + 1}");
            AnimateToPage(_currentIndex + 1, true);
        }
    }
    
    public void Previous()
    {
        if (!_allowNavigation)
        {
            System.Diagnostics.Debug.WriteLine("[CustomCarousel] Previous() blocked - navigation not allowed");
            return;
        }
        
        if (_currentIndex > 0 && !_isAnimating)
        {
            System.Diagnostics.Debug.WriteLine($"[CustomCarousel] Previous() allowed - moving from {_currentIndex} to {_currentIndex - 1}");
            AnimateToPage(_currentIndex - 1, false);
        }
    }
    
    private void SetCurrentIndex(int index)
    {
        if (index >= 0 && index < _pages.Count && !_isAnimating)
        {
            var oldIndex = _currentIndex;
            _currentIndex = index;
            _currentPage = _pages[index];
            
            // Clear existing children and add current page
            this.Children.Clear();
            if (_currentPage != null)
            {
                this.Children.Add(_currentPage);
            }
            
            System.Diagnostics.Debug.WriteLine($"[CustomCarousel] CurrentIndex set to {index}");
        }
    }
    
        private async void AnimateToPage(int targetIndex, bool goingForward)
    {
        if (_isAnimating || targetIndex < 0 || targetIndex >= _pages.Count)
            return;
        
        _isAnimating = true;
        _nextPage = _pages[targetIndex];
        
        System.Diagnostics.Debug.WriteLine($"[CustomCarousel] Starting animation to page {targetIndex}");
        
        try
        {
            // For now, just switch pages without animation to get it working
            // TODO: Implement proper slide animation later
            
            // Clean up current page
            if (_currentPage != null)
            {
                this.Children.Remove(_currentPage);
            }
            
            // Set new page
            _currentPage = _nextPage;
            _currentIndex = targetIndex;
            this.Children.Add(_currentPage);
            
            _nextPage = null;
            _isAnimating = false;
            
            System.Diagnostics.Debug.WriteLine($"[CustomCarousel] Page switched to {_currentIndex}");
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine($"[CustomCarousel] Animation error: {e.Message}");
            _isAnimating = false;
            _nextPage = null;
        }
    }
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (var child in this.Children)
        {
            child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
        }
        return finalSize;
    }
    
    protected override Size MeasureOverride(Size availableSize)
    {
        var maxSize = new Size();
        foreach (var child in this.Children)
        {
            child.Measure(availableSize);
            maxSize = new Size(
                Math.Max(maxSize.Width, child.DesiredSize.Width),
                Math.Max(maxSize.Height, child.DesiredSize.Height));
        }
        return maxSize;
    }
} 