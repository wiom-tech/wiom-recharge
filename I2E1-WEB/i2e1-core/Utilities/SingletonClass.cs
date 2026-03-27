namespace i2e1_core.Utilities
{
    public abstract class SingletonClass<T> where T : SingletonClass<T>, new ()
    {
        protected SingletonClass()
        {

        }

        protected static T instance;

        protected abstract void init(params object[] arguments);

        public static T CreateInstance(params object[] arguments)
        {
            if (instance == null)
            {
                instance = new T();
                instance.init(arguments);
            }
            return instance;
        }

        public static T GetInstance()
        {
            return instance;
        }
    }
}
