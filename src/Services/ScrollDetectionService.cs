using Avalonia.Input;

namespace R2CSharp.Services;

public class ScrollDetectionService
{
    private const double ScrollThreshold = 2.0; // 2px scroll threshold
    private const int TimeThresholdMs = 500; // 500ms time window
    private double _accumulatedScrollX;
    private bool _isTracking;
    private bool _isProcessing;
    private DateTime _trackingStartTime;
    
    public event Action<int>? PageChangeRequested;
    
    public void HandlePointerWheelChanged(PointerWheelEventArgs e)
    {
        if (!(Math.Abs(e.Delta.X) > 0) && !(Math.Abs(e.Delta.Y) > 0)) return;
        e.Handled = true;
            
        if (!_isTracking) {
            _isTracking = true;
            _accumulatedScrollX = 0.0;
            _trackingStartTime = DateTime.Now;
        }
            
        _accumulatedScrollX += e.Delta.X + e.Delta.Y;

        var elapsedTime = (DateTime.Now - _trackingStartTime).TotalMilliseconds;
        
        if (elapsedTime > TimeThresholdMs)  {
            _accumulatedScrollX = 0.0;
            _isTracking = false;
            return;
        }

        if (!(Math.Abs(_accumulatedScrollX) >= ScrollThreshold) || _isProcessing) return;
        _isProcessing = true;
                
        var direction = _accumulatedScrollX > 0 ? 1 : -1;
        PageChangeRequested?.Invoke(direction);
                
        _accumulatedScrollX = 0.0;
        _isTracking = false;
        _isProcessing = false;
    }
}