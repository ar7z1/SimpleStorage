using StructureMap;

namespace SimpleStorage.IoC
{
    public static class IoCFactory
    {
        public static Container NewContainer()
        {
            return new Container(new SimpleStorageRegistry());
        }
    }
}