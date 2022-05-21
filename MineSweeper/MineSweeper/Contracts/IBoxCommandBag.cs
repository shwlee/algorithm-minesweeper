using MineSweeper.Models;
using System.Windows.Input;

namespace MineSweeper.Contracts;

public interface IBoxCommandBag
{
    public ICommand OpenCommand { get; }

    public void OpenBox(Box box);
}
