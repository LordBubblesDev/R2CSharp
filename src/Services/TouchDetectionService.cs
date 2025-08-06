using Avalonia.Input;
using Avalonia;

namespace R2CSharp.Services;

public class TouchDetectionService
{
    private const double DragThreshold = 50.0; // Minimum drag distance to trigger page change
    private Point? _startPoint;
    private bool _isTracking;
    private bool _isProcessing;
    
    public event Action<int>? PageChangeRequested;
    
    public void HandlePointerPressed(PointerPressedEventArgs e)
    {
        if (_isTracking || _isProcessing) return;
        
        _startPoint = e.GetPosition(null);
        _isTracking = true;
    }
    
    public void HandlePointerReleased(PointerReleasedEventArgs e)
    {
        if (!_isTracking || _isProcessing) return;
        
        var endPoint = e.GetPosition(null);
        var dragDistance = _startPoint.HasValue ? CalculateDistance(_startPoint.Value, endPoint) : 0.0;
        
        if (dragDistance >= DragThreshold)
        {
            _isProcessing = true;
            
            // Calculate drag direction
            var deltaX = endPoint.X - (_startPoint?.X ?? 0);
            var deltaY = endPoint.Y - (_startPoint?.Y ?? 0);
            
            // Determine primary direction (horizontal or vertical)
            var direction = Math.Abs(deltaX) > Math.Abs(deltaY) 
                ? (deltaX > 0 ? 1 : -1)  // Horizontal drag
                : (deltaY > 0 ? 1 : -1); // Vertical drag
            
            PageChangeRequested?.Invoke(direction);
        }
        
        _startPoint = null;
        _isTracking = false;
        _isProcessing = false;
    }
    
    public void HandlePointerMoved(PointerEventArgs e)
    {
        if (!_isTracking || _isProcessing) return;
        
        var currentPoint = e.GetPosition(null);
        var dragDistance = _startPoint.HasValue ? CalculateDistance(_startPoint.Value, currentPoint) : 0.0;
    }
    
    public void Reset()
    {
        _startPoint = null;
        _isTracking = false;
        _isProcessing = false;
    }
    
    private static double CalculateDistance(Point start, Point end)
    {
        var deltaX = end.X - start.X;
        var deltaY = end.Y - start.Y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
} 