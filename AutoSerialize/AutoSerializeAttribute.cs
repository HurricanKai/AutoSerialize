namespace AutoSerialize
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class AutoSerializeAttribute : System.Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        private readonly int _order;

        // This is a positional argument
        public AutoSerializeAttribute(int order)
        {
            this._order = order;
        }

        public int Order
        {
            get { return _order; }
        }
    }
}