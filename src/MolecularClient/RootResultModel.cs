namespace MolecularClient
{
    public class RootResultModel<T>
    {

        public bool Valid { get; set; }

        public string Message { get; set; }

        public T Datas { get; set; }

    }

}
