namespace ParleyDevTools.Application.Core.Display;

internal static class LogoPrinter
{
    public static void Print()
    {
        string path = $"{Directory.GetCurrentDirectory()}/Resources/Logos/ParleyLogo.txt";

        if (!File.Exists(path))
            return;

        foreach (var line in File.ReadAllLines(path))
            Console.WriteLine(line);

        Console.WriteLine();
    }
}