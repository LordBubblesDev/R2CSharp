using Avalonia.Input;

namespace R2CSharp.Lib.Services;

public class ScrollDetectionService
{
    private const double ScrollThreshold = 2.0;
    private const int TimeThresholdMs = 500;
    private double _accumulatedScroll;
    private DateTime _trackingStartTime = DateTime.MinValue;
    
    public event Action<int>? PageChangeRequested;
    
    public void HandlePointerWheelChanged(PointerWheelEventArgs e)
    {
        var delta = e.Delta.X + e.Delta.Y;
        if (Math.Abs(delta) <= 0) return;
        
        e.Handled = true;
        
        var now = DateTime.Now;
        if ((now - _trackingStartTime).TotalMilliseconds > TimeThresholdMs) {
            _accumulatedScroll = 0;
            _trackingStartTime = now;
        }
        
        _accumulatedScroll += delta;
        
        if (Math.Abs(_accumulatedScroll) >= ScrollThreshold) {
            var direction = _accumulatedScroll > 0 ? 1 : -1;
            PageChangeRequested?.Invoke(direction);
            
            _accumulatedScroll = 0;
            _trackingStartTime = DateTime.MinValue;
        }
    }
}