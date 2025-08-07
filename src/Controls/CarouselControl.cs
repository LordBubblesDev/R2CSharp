using Avalonia;
using Avalonia.Controls;
using R2CSharp.Handlers;

namespace R2CSharp.Controls;

public class CarouselControl : Panel
{
    private int _currentIndex;
    private readonly List<Control> _pages = [];
    private Control? _currentPage;
    private Control? _nextPage;
    private bool _isAnimating;

    public static readonly DirectProperty<CarouselControl, int> CurrentIndexProperty =
        AvaloniaProperty.RegisterDirect<CarouselControl, int>(
            nameof(CurrentIndex),
            o => o._currentIndex,
            (o, v) => o.SetCurrentIndex(v));
    
    public int CurrentIndex
    {
        get => _currentIndex;
        set => SetCurrentIndex(value);
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
        if (_currentIndex < _pages.Count - 1 && !_isAnimating) {
            AnimateToPage(_currentIndex + 1, true);
        }
    }
    
    public void Previous()
    {
        if (_currentIndex > 0 && !_isAnimating) {
            AnimateToPage(_currentIndex - 1, false);
        }
    }
    
    private void SetCurrentIndex(int index)
    {
        if (index < 0 || index >= _pages.Count || _isAnimating) return;
        var oldIndex = _currentIndex;
        _currentIndex = index;
        _currentPage = _pages[index];
        
        Children.Clear();
        if (_currentPage != null) {
            Children.Add(_currentPage);
        }
        
        if (oldIndex != _currentIndex) {
            SetAndRaise(CurrentIndexProperty, ref oldIndex, _currentIndex);
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

                    if (_currentPage == null) return;
                    
                    await AnimationHandler.AnimatePageTransition(_currentPage, _nextPage, goingForward,
                        containerHeight);

                    System.Diagnostics.Debug.WriteLine($"[CarouselControl] Animation completed");

                    if (_currentPage != null) {
                        Children.Remove(_currentPage);
                    }

                    var oldIndex = _currentIndex;
                    _currentPage = _nextPage;
                    _currentIndex = targetIndex;
                    _nextPage = null;
                    _isAnimating = false;

                    if (oldIndex != _currentIndex) {
                        SetAndRaise(CurrentIndexProperty, ref oldIndex, _currentIndex);
                    }
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