namespace apbd_cw3;

static class CommandsExecutor
{
    private static readonly IDictionary<string, Action<IEnumerable<string>, IDictionary<string, IContainer>, IDictionary<string, IContainerShip>, ContainerTypePreset[]>> commands = new Dictionary<string, Action<IEnumerable<string>, IDictionary<string, IContainer>, IDictionary<string, IContainerShip>, ContainerTypePreset[]>>()
    {
        { "add", CommandsExecutor.EntityAdd },
        { "remove", CommandsExecutor.EntityRemove },
        { "list", CommandsExecutor.EntitiesList },
        { "load", CommandsExecutor.EntityLoad },
        { "unload", CommandsExecutor.EntityUnload },
        { "swap", CommandsExecutor.EntitiesSwap },
        { "move", CommandsExecutor.EntityMove },
        { "exit", CommandsExecutor.ProgramExit },
        { "help", CommandsExecutor.ProgramHelp },
    };

    private static readonly IReadOnlyList<KeyValuePair<string, string>> helpMessages =
    [
        new("help", "help - Displays this help message"),
        new("add-container", "add container [l | g | c>] - creates one or more containers"),
        new("add-ship", "add ship - creates a ship"),
        new("remove-container", "remove container [container_ids] - Removes one or more containers"),
        new("remove-ship", "remove ship [ship_ids] - Removes one or more ships"),
        new("list-containers", "list containers [optional:container_ids] - lists containers (if ids are passed, then only matching containers are listed)"),
        new("list-ships", "list ships [optional:ship_ids] - lists ships (if ids are passed, then only matching ships are listed)"),
        new("load-container", "load container [cargo_preset_name] [quantity] - loads specified cargo into specified container"),
        new("load-ship", "load ship [container_ids] - loads specified containers onto a given ship (the container cannot be onboard any ship)"),
        new("unload-container", "unload container [container_id] - unloads specified container"),
        new("unload-ship", "unload ship [container_ids | all] - unloads specified containers from a given ship"),
        new("swap", "swap [container_id] [container_id] - swaps places of two specified containers"),
        new("move", "move [container_id] [ship_id] - moves a container onto a specified ship (regardless whether the container is onboard a ship or not)"),
        new("exit", "exits the program"),
    ];

    public static bool ExecuteCommand(IEnumerable<string> commands, IDictionary<string, IContainer> looseContainers, IDictionary<string, IContainerShip> ships, ContainerTypePreset[] presets)
    {
        foreach (var command in commands)
        {
            if (!CommandsExecutor.ExecuteCommand(command, looseContainers, ships, presets)) return false;
        }

        return true;
    }

    private static bool ExecuteCommand(string command, IDictionary<string, IContainer> looseContainers, IDictionary<string, IContainerShip> ships, ContainerTypePreset[] presets)
    {
        IEnumerable<string> words = command.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];

        try
        {
            CommandsExecutor.commands[words.First()](words.Skip(1), looseContainers, ships, presets);
        }
        catch (ApplicationException) { return false; }
        catch (KeyNotFoundException) { Console.WriteLine($"[executing: {command}] Command \"{words.First()}\" was not found !"); }
        catch (SyntaxException e) { Console.WriteLine($"[executing: {command}] Syntax error => {e.Message}"); }
        catch (OverfillException e) { Console.WriteLine($"[executing: {command}] {e.Message}"); }
        catch (InvalidOperationException e) { Console.WriteLine($"[executing: {command}] {e.Message}"); }
        catch (Exception e) { Console.Error.WriteLine(e); }

        return true;
    }

    private static IContainerShip FetchShip(string shipId, IDictionary<string, IContainerShip> ships)
    {
        try { return ships[shipId]; }
        catch (KeyNotFoundException e) { throw new InvalidOperationException($"No ship with id \"{shipId}\" was found !", e); }
    }

    private static KeyValuePair<IContainerShip?, IContainer> FetchContainer(string serialNumber, IDictionary<string, IContainer> looseContainers, IDictionary<string, IContainerShip> ships)
    {
        if (looseContainers.TryGetValue(serialNumber, out var container)) return new(null, container);
        else
        {
            try
            {
                return ships.Values.SelectMany(s => s.Containers.Where(c => c.SerialNumber.Equals(serialNumber)).Select(c => new KeyValuePair<IContainerShip?, IContainer>(s, c))).Single();
            }
            catch { throw new InvalidOperationException($"No container with serial number \"{serialNumber}\" was found !"); }
        }
    }

    private static IContainer FetchLooseContainer(string serialNumber, IDictionary<string, IContainer> looseContainers)
    {
        try { return looseContainers[serialNumber]; }
        catch (KeyNotFoundException e) { throw new InvalidOperationException($"No loose container with serial number \"{serialNumber}\" was found !", e); }
    }

    private static void EntityAdd(IEnumerable<string> commandWords, IDictionary<string, IContainer> looseContainers, IDictionary<string, IContainerShip> ships, ContainerTypePreset[] presets)
    {
        var words = commandWords.GetEnumerator();
        if (!words.MoveNext()) throw new SyntaxException("Either keyword \"container\" or \"ship\" was expected !");

        switch (words.Current)
        {
            case "con":
            case "container":
            {
                if (!words.MoveNext()) throw new SyntaxException("Type of new container was expected !");
                IContainer container;

                do
                {
                    switch (words.Current)
                    {
                        case "l": container = new LiquidContainer() { Height = 4, Depth = 18, TareWeight = Random.Shared.NextInt64(2000, 2500), MaxNetWeight = Random.Shared.NextInt64(20000, 25000) }; break;
                        case "g": container = new GasContainer() { Height = 4, Depth = 18, TareWeight = Random.Shared.NextInt64(2000, 2500), MaxNetWeight = Random.Shared.NextInt64(15000, 20000) }; break;
                        case "c": container = new FreezerContainer() { Height = 4, Depth = 18, TareWeight = Random.Shared.NextInt64(2000, 2500), MaxNetWeight = Random.Shared.NextInt64(22000, 25000), Temperature = Random.Shared.Next(-30, 25) }; break;
                        default: throw new InvalidOperationException($"There is no container of type \"{words.Current}\" !");
                    }
    
                    looseContainers.Add(container.SerialNumber, container);
                }
                while (words.MoveNext());
            }
            break;

            case "ship":
            {
                IContainerShip ship = new ContainerShip() { MaxSpeed = (uint)Random.Shared.Next(20, 40), ContainersCapacity = (uint)Random.Shared.Next(10, 30), MaxNetWeight = (uint)Random.Shared.NextInt64(200, 400), TareWeight = Random.Shared.NextInt64(100, 300) };
                ships.Add(ship.Id, ship);
            }
            break;

            default: throw new SyntaxException($"Unexpected keyword \"{words.Current}\" !");
        }
    }

    private static void EntityRemove(IEnumerable<string> commandWords, IDictionary<string, IContainer> looseContainers, IDictionary<string, IContainerShip> ships, ContainerTypePreset[] presets)
    {
        var words = commandWords.GetEnumerator();
        if (!words.MoveNext()) throw new SyntaxException("Either keyword \"container\" or \"ship\" was expected !");

        switch (words.Current)
        {
            case "con":
            case "container":
            {
                if (!words.MoveNext()) throw new SyntaxException("Serial number of the container to be removed was expected !");
                do
                {
                    if (!looseContainers.Remove(words.Current.ToUpper())) throw new InvalidOperationException($"Loose container with serial number \"{words.Current.ToUpper()}\" was not found !");
                }
                while (words.MoveNext());
            }
            break;

            case "ship":
            {
                if (!words.MoveNext()) throw new SyntaxException("Id of the ship to be removed was expected !");
                do
                {
                    if (!ships.Remove(words.Current.ToUpper())) throw new InvalidOperationException($"Ship with id \"{words.Current.ToUpper()}\" was not found !");
                }
                while (words.MoveNext());
            }
            break;

            default: throw new SyntaxException($"Unexpected keyword \"{words.Current}\" !");
        }
    }

    private static void EntitiesList(IEnumerable<string> commandWords, IDictionary<string, IContainer> looseContainers, IDictionary<string, IContainerShip> ships, ContainerTypePreset[] presets)
    {
        var words = commandWords.GetEnumerator();
        if (!words.MoveNext()) throw new SyntaxException("Either \"containers\", \"cargo\" or \"ship\" was expected !");
        var builder = new System.Text.StringBuilder();
        IEnumerable<string> lines;

        switch (words.Current)
        {
            case "cons":
            case "containers":
            {
                if (!words.MoveNext())
                {
                    builder.Append("List of loose containers ↓");
                    lines = looseContainers.Values.OrderBy(p => p.SerialNumber).Select(c => c.ToString() ?? string.Empty);
                }
                else
                {
                    lines = [];

                    do
                    {
                        KeyValuePair<IContainerShip?, IContainer> pair = CommandsExecutor.FetchContainer(words.Current.ToUpper(), looseContainers, ships);
                        builder.AppendLine();
                        if (pair.Key is not null) builder.Append("[Ship ").Append(pair.Key.Id).Append("] ");
                        builder.Append(pair.Value.ToString());
                    }
                    while (words.MoveNext());
                }
            }
            break;

            case "cargo":
            {
                builder.Append("List of cargo presets ↓");
                lines = presets.Select(p => '\n' + p.ToString());
            }
            break;

            case "ships":
            {
                if (!words.MoveNext())
                {
                    builder.Append("List of ships ↓");
                    lines = ships.Select(s => s.Value.ToString() ?? string.Empty);
                }
                else
                {
                    lines = [];
                    do lines = lines.Append(ships.Values.Where(s => s.Id.Equals(words.Current.ToUpper())).Select(s => s.ToString() ?? string.Empty).Single());
                    while (words.MoveNext());
                }
            }
            break;

            default: throw new SyntaxException($"Unexpected keyword \"{words.Current}\" !");
        }

        foreach (var line in lines) builder.Append('\n').Append(line);
        Console.WriteLine(builder);
    }

    private static void EntityLoad(IEnumerable<string> commandWords, IDictionary<string, IContainer> looseContainers, IDictionary<string, IContainerShip> ships, ContainerTypePreset[] presets)
    {
        var words = commandWords.GetEnumerator();
        if (!words.MoveNext()) throw new SyntaxException("Either keyword \"container\" or \"ship\" was expected !");

        switch (words.Current)
        {
            case "con":
            case "container":
            {
                if (!words.MoveNext()) throw new SyntaxException("Serial number of a container to have cargo loaded was expected !");

                var container = CommandsExecutor.FetchLooseContainer(words.Current.ToUpper(), looseContainers);
                if (!words.MoveNext()) throw new SyntaxException("Cargo type to be loaded was expected !");

                KeyValuePair<string, CargoPreset> preset;
                try { preset = presets.SelectMany(p => p.Presets.Select(c => new KeyValuePair<string, CargoPreset>(p.ContainerType, c))).Where(p => p.Value.Name.Equals(words.Current)).First(); }
                catch (KeyNotFoundException e) { throw new InvalidOperationException($"Cargo preset \"{words.Current}\" was not found !", e); }

                if (!words.MoveNext() || !long.TryParse(words.Current, out var quantity)) throw new SyntaxException("Cargo quantity (in kg) was expected !");

                AbstractFreight freight;
                switch (preset.Key.ToLower())
                {
                    case "l": freight = new LiquidFreight() { Name = preset.Value.Name, IsHazardous = preset.Value.IsHazardous, Weight = quantity, MinTemperature = preset.Value.MinTemperature }; break;
                    case "g": freight = new GasFreight() { Name = preset.Value.Name, IsHazardous = preset.Value.IsHazardous, Weight = quantity, MinTemperature = preset.Value.MinTemperature }; break;
                    case "c": freight = new FreezerFreight() { Name = preset.Value.Name, IsHazardous = preset.Value.IsHazardous, Weight = quantity, MinTemperature = preset.Value.MinTemperature }; break;
                    default: return;
                }

                container.Load(freight);
            }
            break;

            case "ship":
            {
                if (!words.MoveNext()) throw new SyntaxException("Id of a ship to have a container loaded was expected !");

                IContainerShip ship = CommandsExecutor.FetchShip(words.Current.ToUpper(), ships);

                if (!words.MoveNext()) throw new SyntaxException("Serial number of a container to be loaded onto a ship was expected !");

                do
                {
                    var container = CommandsExecutor.FetchLooseContainer(words.Current.ToUpper(), looseContainers);

                    ship.Load(container);
                    looseContainers.Remove(container.SerialNumber);
                }
                while (words.MoveNext());
            }
            break;

            default: throw new SyntaxException($"Unexpected keyword \"{words.Current}\" !");
        }
    }

    private static void EntityUnload(IEnumerable<string> commandWords, IDictionary<string, IContainer> looseContainers, IDictionary<string, IContainerShip> ships, ContainerTypePreset[] presets)
    {
        var words = commandWords.GetEnumerator();
        if (!words.MoveNext()) throw new SyntaxException("Either keywords \"container\" or \"ship\" was expected !");

        switch (words.Current)
        {
            case "con":
            case "container":
            {
                if (!words.MoveNext()) throw new SyntaxException("Serial number of a container to have its contents unloaded was expected !");
                CommandsExecutor.FetchLooseContainer(words.Current.ToUpper(), looseContainers).Unload();
            }
            break;

            case "ship":
            {
                if (!words.MoveNext()) throw new SyntaxException("Id of a ship to have a container unloaded was expected !");

                IContainerShip ship = FetchShip(words.Current.ToUpper(), ships);

                if (!words.MoveNext()) throw new SyntaxException("Serial number of a container to be unloaded onto a ship was expected !");

                do
                {
                    if (words.Current.Equals("all"))
                    {
                        foreach (var container in ship.Containers)
                        {
                            ship.Unload(container.SerialNumber);
                            looseContainers.Add(container.SerialNumber, container);
                        }
                        break;
                    }
                    else
                    {
                        IContainer container = ship[words.Current.ToUpper()];
                        ship.Unload(container.SerialNumber);
                        looseContainers.Add(container.SerialNumber, container);
                    }
                }
                while (words.MoveNext());
            }
            break;

            default: throw new SyntaxException($"Unexpected keyword \"{words.Current}\" !");
        }
    }

    private static void EntitiesSwap(IEnumerable<string> commandWords, IDictionary<string, IContainer> looseContainers, IDictionary<string, IContainerShip> ships, ContainerTypePreset[] presets)
    {
        void performSwapStep(KeyValuePair<IContainerShip?, IContainer> firstPair, KeyValuePair<IContainerShip?, IContainer> secondPair)
        {
            if (firstPair.Key is null)
            {
                looseContainers.Remove(firstPair.Value.SerialNumber);
                looseContainers.Add(secondPair.Value.SerialNumber, secondPair.Value);
            }
            else
            {
                firstPair.Key.Unload(firstPair.Value.SerialNumber);
                firstPair.Key.Load(secondPair.Value);
            }
        }

        var words = commandWords.GetEnumerator();

        if (!words.MoveNext()) throw new SyntaxException("Serial number of a container to swapped was expected !");

        var firstPair = FetchContainer(words.Current.ToUpper(), looseContainers, ships);

        if (!words.MoveNext()) throw new SyntaxException($"Serial number of a second container was expected !");

        var secondPair = FetchContainer(words.Current.ToUpper(), looseContainers, ships);

        performSwapStep(firstPair, secondPair);
        performSwapStep(secondPair, firstPair);
    }

    private static void EntityMove(IEnumerable<string> commandWords, IDictionary<string, IContainer> looseContainers, IDictionary<string, IContainerShip> ships, ContainerTypePreset[] presets)
    {
        var words = commandWords.GetEnumerator();

        if (!words.MoveNext()) throw new SyntaxException("Serial number of a container to move was expected !");

        var pair = FetchContainer(words.Current.ToUpper(), looseContainers, ships);

        if (!words.MoveNext()) throw new SyntaxException($"Id of a ship to have container \"{pair.Value.SerialNumber}\" moved to was expected !");

        IContainerShip ship = CommandsExecutor.FetchShip(words.Current.ToUpper(), ships);

        if (pair.Key is not null) pair.Key.Unload(pair.Value.SerialNumber);
        else looseContainers.Remove(pair.Value.SerialNumber);

        ship.Load(pair.Value);
    }

    private static void ProgramExit(IEnumerable<string> commandWords, IDictionary<string, IContainer> looseContainers, IDictionary<string, IContainerShip> ships, ContainerTypePreset[] presets) => throw new ApplicationException("Exit called");

    private static void ProgramHelp(IEnumerable<string> commandWords, IDictionary<string, IContainer> looseContainers, IDictionary<string, IContainerShip> ships, ContainerTypePreset[] presets)
    {
        System.Text.StringBuilder builder = new("List of commands ↓");
        foreach (var pair in CommandsExecutor.helpMessages) builder.Append("\n · ").Append(pair.Key).Append(" => ").Append(pair.Value);
        builder.Append("Commands can be terminated with \";\" and thus combined into one prompt");
        System.Console.WriteLine(builder.ToString());
    }
}
