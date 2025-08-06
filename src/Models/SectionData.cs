using System.Collections.ObjectModel;

namespace R2CSharp.Models;

public class SectionData
{
    public string Title { get; set; } = string.Empty;
    public string EmptyMessage { get; set; } = string.Empty;
    public ObservableCollection<RebootOption> Items { get; set; } = [];
} 