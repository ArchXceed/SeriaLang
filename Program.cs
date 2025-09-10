public class Program
{
    public static string[] argsGlobal = [];
    public static void Main(string[] args)
    {
        argsGlobal = args;
        Typer typer = new Typer();
        typer.TyperMain(argsGlobal);
        typer.Serialize(argsGlobal[1]);
    }
}