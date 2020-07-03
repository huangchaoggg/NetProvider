namespace NetProvider.Channels
{
    public class Parameters
    {
        public Parameters(string interfaceName,
            string methodName,
            object[] obj)
        {
            this.InterfaceName = interfaceName;
            this.MethodName = methodName;
            this.ParametersInfo = obj;
        }
        public string InterfaceName { get; set; }
        public string MethodName { get; set; }
        public object[] ParametersInfo { get; set; }
    }
}
