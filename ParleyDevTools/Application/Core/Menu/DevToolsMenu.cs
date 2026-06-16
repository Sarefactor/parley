using Microsoft.Extensions.DependencyInjection;
using ParleyDevTools.Application.Core.Display;
using ParleyDevTools.Application.Services;

namespace ParleyDevTools.Application.Core.Menu;

internal class DevToolsMenu
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IList<MenuOption> _menuOptions;

    public DevToolsMenu(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _menuOptions = GetMenuOptions();
    }

    public async Task Start()
    {
        var runApplicationLoop = true;

        while (runApplicationLoop)
        {
            PrintMenuOptions();

            var optionSelected = SelectMenuOption();

            switch (optionSelected)
            {
                case 1:
                    var workflowService = _serviceProvider.GetRequiredService<IWorkflowService>();
                    await workflowService.RunParleyWorkflowsAsync();
                    break;
                case 10:
                    runApplicationLoop = false;
                    break;
            }
        }

        Console.WriteLine("Hit any key to exit...");
    }

    private static IList<MenuOption> GetMenuOptions()
    {
        List<MenuOption> menuOptions = new()
        {
            new MenuOption { Id = 1, Description = "Workflow Service" },
            new MenuOption { Id = 10, Description = "Exit Application" }
        };

        return menuOptions;
    }

    private void PrintMenuOptions()
    {
        Console.WriteLine();
        LogoPrinter.Print();
        Console.WriteLine("Waterworks Options Menu: \n");

        foreach (MenuOption menuOption in _menuOptions)
        {
            Console.WriteLine($"{menuOption.Id} - {menuOption.Description}");
        }

        Console.WriteLine();
    }

    private int SelectMenuOption()
    {
        int userOption;

        do
        {
            Console.Write("Select an Option: ");
            int.TryParse(Console.ReadLine(), out userOption);

        } while (!_menuOptions.Select(mo => mo.Id).ToList().Contains(userOption));

        return userOption;
    }
}