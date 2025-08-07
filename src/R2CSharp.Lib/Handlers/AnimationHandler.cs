using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;

namespace R2CSharp.Lib.Handlers;

public abstract class AnimationHandler
{
    public static async Task AnimatePageTransition(Control currentPage, Control nextPage, bool goingForward, double containerHeight)
    {
        try {
            var tcs = new TaskCompletionSource<bool>();
            var startTime = DateTime.Now;
            var duration = TimeSpan.FromMilliseconds(300);
            var isCompleted = false;
            
            // Calculate animation directions
            const double currentPageStartY = 0.0;
            const double nextPageEndY = 0.0;
            var currentPageEndY = goingForward ? -containerHeight : containerHeight;
            var nextPageStartY = goingForward ? containerHeight : -containerHeight;
            
            // Set up initial transforms
            currentPage.RenderTransform ??= new TranslateTransform();
            nextPage.RenderTransform ??= new TranslateTransform();
            
            var currentTransform = currentPage.RenderTransform as TranslateTransform ?? new TranslateTransform();
            var nextTransform = nextPage.RenderTransform as TranslateTransform ?? new TranslateTransform();
            
            // Set initial positions
            currentTransform.Y = currentPageStartY;
            nextTransform.Y = nextPageStartY;
            
            var timer = new Timer(_ => {
                try {
                    var elapsed = DateTime.Now - startTime;
                    var progress = Math.Min(elapsed.TotalMilliseconds / duration.TotalMilliseconds, 1.0);
                    var easedProgress = ApplyEasing(progress, new CubicEaseOut());
                    
                    // Calculate current positions
                    var currentY = currentPageStartY + (easedProgress * (currentPageEndY - currentPageStartY));
                    var nextY = nextPageStartY + (easedProgress * (nextPageEndY - nextPageStartY));
                    
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                        if (currentPage.RenderTransform is TranslateTransform currentPageTransform) {
                            currentPageTransform.Y = currentY;
                        }
                        if (nextPage.RenderTransform is TranslateTransform nextPageTransform) {
                            nextPageTransform.Y = nextY;
                        }
                        
                        if (!(progress >= 1.0) || isCompleted) return;
                        isCompleted = true;
                        tcs.TrySetResult(true);
                    });
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"[AnimationHandler] Timer error: {ex.Message}");
                    if (!isCompleted) {
                        isCompleted = true;
                        tcs.TrySetException(ex);
                    }
                }
            }, null, 0, 16);
            
            await tcs.Task;
            await timer.DisposeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AnimationHandler] Animation error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[AnimationHandler] Stack trace: {ex.StackTrace}");
            throw;
        }
    }
    
    private static double ApplyEasing(double progress, IEasing easing)
    {
        return easing.Ease(progress);
    }
} 