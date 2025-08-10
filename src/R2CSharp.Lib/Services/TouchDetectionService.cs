using Avalonia;
using Avalonia.Input;

namespace R2CSharp.Lib.Services;

public class TouchDetectionService
{
    private const double DragThreshold = 42.0;
    private Point? _startPoint;
    private bool _isTracking;
    
    public event Action<int>? PageChangeRequested;
    
    public void HandlePointerPressed(PointerPressedEventArgs e)
    {
        if (_isTracking) return;
        
        _startPoint = e.GetPosition(null);
        _isTracking = true;
    }
    
    public void HandlePointerReleased(PointerReleasedEventArgs e)
    {
        if (!_isTracking || _startPoint == null) return;
        
        var endPoint = e.GetPosition(null);
        var dragDistance = CalculateDistance(_startPoint.Value, endPoint);
        
        if (dragDistance >= DragThreshold) {
            var deltaX = endPoint.X - _startPoint.Value.X;
            var deltaY = endPoint.Y - _startPoint.Value.Y;
            
            var direction = Math.Abs(deltaX) > Math.Abs(deltaY) 
                ? Math.Sign(deltaX)  // Horizontal drag
                : Math.Sign(deltaY); // Vertical drag
            
            PageChangeRequested?.Invoke(direction);
        }
        
        _startPoint = null;
        _isTracking = false;
    }
    
    private static double CalculateDistance(Point start, Point end)
    {
        var deltaX = end.X - start.X;
        var deltaY = end.Y - start.Y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
} 