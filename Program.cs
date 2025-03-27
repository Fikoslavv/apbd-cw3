using System.Text.Json;

namespace apbd_cw3;

class Program
{
    private static IDictionary<string, IContainer> looseContainers = new Dictionary<string, IContainer>();
    private static IDictionary<string, IContainerShip> ships = new Dictionary<string, IContainerShip>();
    private static ContainerTypePreset[] presets = [];

    static void Main(string[] args)
    {
        var config = new ProgramConfig(args);
        Program.presets = Program.LoadConfig(config);

        Program.RunConsoleUI();
    }

    public static ContainerTypePreset[] LoadConfig(ProgramConfig config)
    {
        try
        {
            using var reader = new FileStream(config.ConfigPath, FileMode.Open);

            var json = System.Text.Json.Nodes.JsonObject.Parse(reader)?.AsObject();

            return json?.AsObject()["cargo-presets"].Deserialize<ContainerTypePreset[]>() ?? [];
        }
        catch (System.Exception e) { Console.Error.WriteLine(e); }

        return [];
    }

    private static void RunConsoleUI()
    {
        do Console.Write("\nCommand >> ");
        while (CommandsExecutor.ExecuteCommand((Console.ReadLine() ?? string.Empty).Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), Program.looseContainers, Program.ships, Program.presets));

        Program.looseContainers.Clear();
        Program.ships.Clear();
    }
}
