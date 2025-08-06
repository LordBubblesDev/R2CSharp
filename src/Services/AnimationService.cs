using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;

namespace R2CSharp.Services;

public abstract class AnimationService
{
    public static async Task AnimatePageTransition(Control nextPage, bool goingForward, double containerHeight)
    {
        try {
            var startOffset = goingForward ? containerHeight : -containerHeight;
            
            nextPage.RenderTransform ??= new TranslateTransform();
            
            var transform = nextPage.RenderTransform as TranslateTransform ?? new TranslateTransform();
            transform.X = 0;
            transform.Y = startOffset;
            nextPage.RenderTransform = transform;
            
            var tcs = new TaskCompletionSource<bool>();
            var startTime = DateTime.Now;
            var duration = TimeSpan.FromMilliseconds(300);
            
            var isCompleted = false;
            var timer = new Timer(_ => {
                try {
                    var elapsed = DateTime.Now - startTime;
                    var progress = Math.Min(elapsed.TotalMilliseconds / duration.TotalMilliseconds, 1.0);
                    var easedProgress = ApplyEasing(progress, new CubicEaseOut());
                    var currentY = startOffset + (easedProgress * -startOffset);
                    
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                        if (nextPage.RenderTransform is TranslateTransform currentTransform) {
                            currentTransform.Y = currentY;
                        }
                        
                        if (!(progress >= 1.0) || isCompleted) return;
                        isCompleted = true;
                        tcs.TrySetResult(true);
                    });
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"[AnimationService] Timer error: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"[AnimationService] Animation error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[AnimationService] Stack trace: {ex.StackTrace}");
            throw;
        }
    }
    
    private static double ApplyEasing(double progress, IEasing easing)
    {
        return easing.Ease(progress);
    }
} 