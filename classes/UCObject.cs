public class UCObject
{
    public List<GlobalReplaceVar.GlobalReplaceVarInfos> globalVars { get; set; } = [];
    public List<CustomDefine.CustomDefineInfos> customDefines { get; set; } = [];
    public List<Function.FunctionInfos> functions { get; set; } = [];
}