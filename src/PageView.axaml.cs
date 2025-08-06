using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.VisualTree;
using R2CSharp.Services;

namespace R2CSharp;

public partial class PageView : UserControl
{
    private readonly ScrollDetectionService _scrollService;
    
    public PageView()
    {
        InitializeComponent();
        Console.WriteLine($"[PageView] InitializeComponent completed");
        
        DataContext = new PageViewModel();
        
        // Initialize scroll detection service
        _scrollService = new ScrollDetectionService();
        _scrollService.PageChangeRequested += OnPageChangeRequested;
        
        // Handle scroll wheel events at the window level
        this.AttachedToVisualTree += OnAttachedToVisualTree;
        
        Console.WriteLine($"[PageView] PageView initialized, event handlers attached");
    }
    
    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        // Get the window and attach scroll handler
        if (this.GetVisualRoot() is Window window)
        {
            window.PointerWheelChanged += OnPointerWheelChanged;
            Console.WriteLine($"[PageView] Attached scroll handler to window");
        }
        
        // Debug: Check if pages are loaded
        if (DataContext is PageViewModel viewModel)
        {
            Console.WriteLine($"[PageView] Pages count: {viewModel.Pages.Count}");
            for (int i = 0; i < viewModel.Pages.Count; i++)
            {
                Console.WriteLine($"[PageView] Page {i}: {viewModel.Pages[i].SectionName} with {viewModel.Pages[i].Options.Count} options");
            }
            
            // Subscribe to property changes to detect when pages are loaded
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        
        // Debug: Check carousel state
        if (MainCarousel != null)
        {
            Console.WriteLine($"[PageView] MainCarousel found: Type={MainCarousel.GetType().Name}");
        }
        else
        {
            Console.WriteLine($"[PageView] MainCarousel is null!");
        }
    }
    
    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PageViewModel.Pages) && sender is PageViewModel r2cModel)
        {
            Console.WriteLine($"[PageView] Pages property changed! New count: {r2cModel.Pages.Count}");
            for (int i = 0; i < r2cModel.Pages.Count; i++)
            {
                Console.WriteLine($"[PageView] Page {i}: {r2cModel.Pages[i].SectionName} with {r2cModel.Pages[i].Options.Count} options");
            }
            
            // Add pages to custom carousel
            if (MainCarousel != null)
            {
                for (int i = 0; i < r2cModel.Pages.Count; i++)
                {
                    var pageView = new Views.StandardPageView { DataContext = r2cModel.Pages[i] };
                    MainCarousel.AddPage(pageView);
                }
                Console.WriteLine($"[PageView] Added {r2cModel.Pages.Count} pages to CustomCarousel");
            }
        }
        else if (e.PropertyName == nameof(PageViewModel.IsLoading) && sender is PageViewModel viewModel)
        {
            Console.WriteLine($"[PageView] IsLoading changed: {viewModel.IsLoading}");
        }
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        Console.WriteLine($"[PageView] PointerWheelChanged received: Delta X={e.Delta.X:F2}, Y={e.Delta.Y:F2}");
        _scrollService.HandlePointerWheelChanged(e);
    }
    
    private void OnPageChangeRequested(int direction)
    {
        if (MainCarousel != null)
        {
            Console.WriteLine($"[PageView] Page change requested: Direction={direction}");
            
            // Enable navigation temporarily
            MainCarousel.AllowNavigation = true;
            
            if (direction > 0) // Positive scroll (down/right) = next page
            {
                Console.WriteLine($"[PageView] Calling MainCarousel.Next()");
                MainCarousel.Next();
            }
            else // Negative scroll (up/left) = previous page
            {
                Console.WriteLine($"[PageView] Calling MainCarousel.Previous()");
                MainCarousel.Previous();
            }
            
            // Disable navigation after use
            MainCarousel.AllowNavigation = false;
        }
        else
        {
            Console.WriteLine($"[PageView] MainCarousel is null!");
        }
    }
    
    private void OnPreviousButtonClick(object? sender, RoutedEventArgs e)
    {
        if (MainCarousel != null)
        {
            MainCarousel.AllowNavigation = true;
            MainCarousel.Previous();
            MainCarousel.AllowNavigation = false;
        }
    }
    
    private void OnNextButtonClick(object? sender, RoutedEventArgs e)
    {
        if (MainCarousel != null)
        {
            MainCarousel.AllowNavigation = true;
            MainCarousel.Next();
            MainCarousel.AllowNavigation = false;
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (DataContext is PageViewModel viewModel) 
        {
            viewModel.Cleanup();
            viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        
        // Clean up event handlers
        if (this.GetVisualRoot() is Window window)
        {
            window.PointerWheelChanged -= OnPointerWheelChanged;
        }
        _scrollService.PageChangeRequested -= OnPageChangeRequested;
    }
}