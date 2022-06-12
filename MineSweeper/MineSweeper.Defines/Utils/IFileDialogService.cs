using MineSweeper.Defines.Enums;

namespace MineSweeper.Defines.Utils;

public interface IFileDialogService
{
    string? GetFile();

    string[]? GetFiles(Platform? platform);
}
