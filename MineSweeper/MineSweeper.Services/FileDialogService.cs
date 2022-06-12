using Microsoft.Win32;
using MineSweeper.Defines.Enums;
using MineSweeper.Defines.Utils;

namespace MineSweeper.Services;

public class FileDialogService : IFileDialogService
{
    private const string AllFilter = @"C#, C++|*.dll|javascript|*.js|python|*.py";

    private readonly Dictionary<Platform, string> _platformFilters = new()
    {
        { Platform.CS, "C#|*.dll" },
        { Platform.CPP, "C++|*.dll" },
        { Platform.Javascript, "javscript|*.js" },
        { Platform.Python, "Python|*.py" },
    };

    public string? GetFile()
    {
        return OpenDialog<string>(AllFilter);
    }

    public string[]? GetFiles(Platform? platform)
    {
        var filter = platform is null ? AllFilter : _platformFilters[platform.Value];
        return OpenDialog<string[]>(filter, true);
    }

    private T? OpenDialog<T>(string filter, bool isMultiSelect = false)
        where T : class
    {
        var dialog = new OpenFileDialog();
        dialog.DefaultExt = ".*"; // Default file extension        
        dialog.Filter = filter;
        dialog.Multiselect = isMultiSelect;

        // Show open file dialog box
        var result = dialog.ShowDialog();
        return result is null ? null : (isMultiSelect) ? dialog.FileNames as T : dialog.FileName as T;
    }
}
