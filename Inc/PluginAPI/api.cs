using System;
using System.IO;
using System.Reflection;

namespace _cmd
{
    class api
    {
        public enum _cmdReturn
        {

            // No failures or errors, operation completed with success.
            _C_SUCCESS,
            // There was an error with whatever happened, not an actual error.
            // but you failed to complete said task.
            _C_FAILURE,
            // There was an IO error, say you try to read a file & it doesn't exist.
            _C_IOERROR,
            // Access violation, you tried to read something that doesn't exist.
            _C_ACCESSVIOLATION,
            // Unknown error, you should never really throw this.
            _C_UNKNOWN_ERROR,
        }

        public static bool CreateCommand(string _visable, string _internal, Func<_cmdReturn> fmt, string desc)
        {
            var init = AssemblyName.GetAssemblyName("..\\..\\_cmd.exe");
            var assembly = Assembly.Load(init);

            var type = assembly.GetType("_cmd.CommandExecutor");
            var method = type.GetMethod("MakeCommand");

            var _func = Activator.CreateInstance(type);

            object ret = method.Invoke(_func, new object[] { _visable, _internal, fmt, desc });
            return (bool)ret;
        }

        public static void WriteLine(string brand, string info)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}][{brand}]: {info}");
        }
    }
}

namespace TestDll
{
    // this interface must be used.

    interface IPlugin
    {
        string Name { get; }
        string Explaination { get; }
        void _Go();
    }

    // Now this interface is defined, in your class you need to define the name & description.
    // Then override _Go with your (for example) Initialization function.

    // Here is an example.

    class MyGreatPlugin : IPlugin
    {
        public MyGreatPlugin(string _name, string _desc)
        {
            _Name = _name;
            _Desc = _desc;
        }

        private string _Name { get; set; }
        private string _Desc { get; set; }

        string IPlugin.Name => _Name;

        string IPlugin.Explaination => _Desc;

        public void _Go()
        {

        }
    }

    class Init_Class
    {
        public Init_Class()
        {

        }

        ~Init_Class() { }

        public void Initialize()
        {
            // Here you could add a few commands!

            _cmd.api.CreateCommand("suitehelp", "__suite_help", () =>
            {
                // BARE bones example, but literally anything can be done here.
                // Lets just tell them we have no help to provide as we're a 
                // Beta plugin.

                _cmd.api.WriteLine("g_suite", "There is no help to provide right now as we're in beta!");
                return _cmd.api._cmdReturn._C_SUCCESS;

            }, "Provides help with your command suite.");
        }
    }
}
