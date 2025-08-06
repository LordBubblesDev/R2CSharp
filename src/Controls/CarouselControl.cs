using Avalonia;
using Avalonia.Controls;
using R2CSharp.Services;
using System;
using System.Collections.Generic;

namespace R2CSharp.Controls;

public class CarouselControl : Panel
{
    private int _currentIndex;
    private readonly List<Control> _pages = [];
    private Control? _currentPage;
    private Control? _nextPage;
    private bool _isAnimating;
    private bool _allowNavigation;
    
    public static readonly DirectProperty<CarouselControl, int> CurrentIndexProperty =
        AvaloniaProperty.RegisterDirect<CarouselControl, int>(
            nameof(CurrentIndex),
            o => o._currentIndex,
            (o, v) => o.SetCurrentIndex(v));
    
    public static readonly DirectProperty<CarouselControl, bool> AllowNavigationProperty =
        AvaloniaProperty.RegisterDirect<CarouselControl, bool>(
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
    
    public void AddPage(Control page)
    {
        _pages.Add(page);
        
        if (_currentPage == null && _pages.Count > 0)
        {
            SetCurrentIndex(0);
        }
    }
    
    public void Next()
    {
        if (!_allowNavigation) {
            return;
        }
        
        if (_currentIndex < _pages.Count - 1 && !_isAnimating) {
            AnimateToPage(_currentIndex + 1, true);
        }
    }
    
    public void Previous()
    {
        if (!_allowNavigation) {
            return;
        }

        if (_currentIndex <= 0 || _isAnimating) return;
        AnimateToPage(_currentIndex - 1, false);
    }
    
    private void SetCurrentIndex(int index)
    {
        if (index < 0 || index >= _pages.Count || _isAnimating) return;
        _currentIndex = index;
        _currentPage = _pages[index];
        
        this.Children.Clear();
        if (_currentPage != null) {
            this.Children.Add(_currentPage);
        }
            
        System.Diagnostics.Debug.WriteLine($"[CarouselControl] CurrentIndex set to {index}");
    }
    
        private async void AnimateToPage(int targetIndex, bool goingForward)
        {
            try {
                if (_isAnimating || targetIndex < 0 || targetIndex >= _pages.Count) {
                    return;
                }
        
                _isAnimating = true;
                _nextPage = _pages[targetIndex];
        
                try {
                    var containerHeight = Bounds.Height;
                    Children.Add(_nextPage);
            
                    await AnimationService.AnimatePageTransition(_nextPage, goingForward, containerHeight);
            
                    System.Diagnostics.Debug.WriteLine($"[CarouselControl] Animation completed");
            
                    if (_currentPage != null)
                    {
                        Children.Remove(_currentPage);
                    }
            
                    _currentPage = _nextPage;
                    _currentIndex = targetIndex;
                    _nextPage = null;
                    _isAnimating = false;
                }
                catch (Exception e)
                {
                    _isAnimating = false;
                    _nextPage = null;
                }
            }
            catch (Exception e) {
                throw; // TODO handle exception
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