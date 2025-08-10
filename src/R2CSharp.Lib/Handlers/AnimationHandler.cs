using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;

namespace R2CSharp.Lib.Handlers;

public static class AnimationHandler
{
    public static async Task AnimatePageTransition(Control currentPage, Control nextPage, bool goingForward, double containerHeight)
    {
        try {
            // Calculate animation directions
            const double currentPageStartY = 0.0;
            const double nextPageEndY = 0.0;
            var currentPageEndY = goingForward ? -containerHeight : containerHeight;
            var nextPageStartY = goingForward ? containerHeight : -containerHeight;
            
            // Set up transforms
            currentPage.RenderTransform ??= new TranslateTransform();
            nextPage.RenderTransform ??= new TranslateTransform();
            
            var currentTransform = (TranslateTransform)currentPage.RenderTransform;
            var nextTransform = (TranslateTransform)nextPage.RenderTransform;
            
            // Set initial positions
            currentTransform.Y = currentPageStartY;
            nextTransform.Y = nextPageStartY;
            
            // Simple animation loop
            const int steps = 30;
            const int stepDelay = 10;
            
            for (int i = 0; i <= steps; i++) {
                var progress = (double)i / steps;
                var easedProgress = ApplyEasing(progress, new CubicEaseOut());
                
                // Calculate current positions
                var currentY = currentPageStartY + (easedProgress * (currentPageEndY - currentPageStartY));
                var nextY = nextPageStartY + (easedProgress * (nextPageEndY - nextPageStartY));
                
                await Task.Delay(stepDelay);
                
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => {
                    currentTransform.Y = currentY;
                    nextTransform.Y = nextY;
                });
            }
        }
        catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine($"[AnimationHandler] Animation error: {ex.Message}");
        }
    }
    
    private static double ApplyEasing(double progress, IEasing easing) => easing.Ease(progress);
} 