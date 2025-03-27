namespace apbd_cw3;

struct ProgramConfig
{
    public bool DisplayHelpMessage { get; init; }
    public bool VerboseMode { get; init; }

    public string ConfigPath { get; init; } = string.Empty;

    public ProgramConfig(string[] args)
    {
        foreach (var arg in args)
        {
            if (arg.StartsWith("--"))
            {
                var argBase = arg.Split("=", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).First();

                switch (argBase)
                {
                    case "--help":
                        this.DisplayHelpMessage = true;
                    break;

                    case "--verbose":
                        this.VerboseMode = true;
                    break;

                    case "--config":
                        this.ConfigPath = arg.Substring(argBase.Length + 1);
                    break;
                }
            }
            else
            {
                if (arg.StartsWith("-"))
                {
                    foreach (var letter in arg.Substring(1))
                    {
                        switch (letter)
                        {
                            case 'h':
                                this.DisplayHelpMessage = true;
                            break;

                            case 'v':
                                this.VerboseMode = true;
                            break;
                        }
                    }
                }
            }
        }

        if (this.ConfigPath is null) this.DisplayHelpMessage = true;
    }
}
