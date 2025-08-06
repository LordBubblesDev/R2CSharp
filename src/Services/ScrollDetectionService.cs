using Avalonia.Input;
using System;

namespace R2CSharp.Services;

public class ScrollDetectionService
{
    private const double SCROLL_THRESHOLD = 4.0; // Minimum scroll distance to trigger page change
    private double _accumulatedScrollX = 0.0;
    private bool _isTracking = false;
    private bool _isProcessing = false; // Prevent double calls
    
    public event Action<int>? PageChangeRequested; // -1 for previous, +1 for next
    
    public void HandlePointerWheelChanged(PointerWheelEventArgs e)
    {
        // Handle both horizontal and vertical scroll
        if (Math.Abs(e.Delta.X) > 0 || Math.Abs(e.Delta.Y) > 0)
        {
            e.Handled = true;
            
            if (!_isTracking)
            {
                _isTracking = true;
                _accumulatedScrollX = 0.0;
                Console.WriteLine($"[ScrollDetection] Started tracking scroll gesture");
            }
            
            // Accumulate both horizontal and vertical scroll
            _accumulatedScrollX += e.Delta.X + e.Delta.Y;
            
            Console.WriteLine($"[ScrollDetection] Delta: X={e.Delta.X:F2}, Y={e.Delta.Y:F2}, Accumulated={_accumulatedScrollX:F2}, Threshold={SCROLL_THRESHOLD}");
            
            // Check if threshold is reached
            if (Math.Abs(_accumulatedScrollX) >= SCROLL_THRESHOLD && !_isProcessing)
            {
                _isProcessing = true;
                
                // Up/Left (negative) = previous page, Down/Right (positive) = next page
                int direction = _accumulatedScrollX > 0 ? 1 : -1;
                Console.WriteLine($"[ScrollDetection] Threshold reached! Direction={direction}, Accumulated={_accumulatedScrollX:F2}");
                PageChangeRequested?.Invoke(direction);
                
                // Reset for next gesture
                _accumulatedScrollX = 0.0;
                _isTracking = false;
                _isProcessing = false;
                Console.WriteLine($"[ScrollDetection] Reset tracking");
            }
        }
        else
        {
            Console.WriteLine($"[ScrollDetection] Ignored scroll - Delta: X={e.Delta.X:F2}, Y={e.Delta.Y:F2}");
        }
    }
    
    public void Reset()
    {
        _accumulatedScrollX = 0.0;
        _isTracking = false;
        _isProcessing = false;
        Console.WriteLine($"[ScrollDetection] Manual reset");
    }
} 