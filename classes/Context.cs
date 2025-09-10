public class Context
{
    public string file = "<eval>";
    public int line = 0;
    public string function = "<global>";
    public List<string> insideBlockCode = new List<string>();
    public string lineCode = "";
}