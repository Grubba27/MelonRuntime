﻿using Jint;
using MelonJs.Static.Tools.Scripting;
using MelonJs.Static.Tools.Output;
using MelonJs.Static.Tools.Web;
using MelonJs.JavaScript.Containers;
using MelonJs.Models.Web;
using MelonJs.WebApps;
using MelonJs.Models.FileSystem;
using MelonJs.Static;
using MelonJs.Models.Project;
using MelonJs.Static.Tools.FileSystem;
using MelonJs.Static.Tools.EngineManagement;
using MelonJs.Models.BuiltIn;

namespace MelonJs.JavaScript.Extensions
{
    public static class EngineExtensions
    {
        public static void SetupAll(this Engine engine, App currentApp, JintContainer container)
        {
            foreach(var i in Enum.GetNames(typeof(BuiltInJsModule)))
            {
                engine.SetupFor(Enum.Parse<BuiltInJsModule>(i), currentApp, container);
            }
        }

        public static void SetupFor(this Engine engine, BuiltInJsModule module, App currentApp, JintContainer container)
        {
            switch(module)
            {
                case BuiltInJsModule.LibrariesAndPolyfills:
                    engine.Execute(BindingManager.Get("Libraries/esprima"));
                    engine.Execute(BindingManager.Get("Libraries/escodegen"));
                    engine.Execute(BindingManager.Get("Polyfills/String.prototype.replaceAll"));
                    engine.Execute(BindingManager.Get("Polyfills/Function.prototype.asString"));
                    break;

                case BuiltInJsModule.Application:
                    engine.SetValue("__basedir__", Environment.CurrentDirectory.Replace("\\", "/"));
                    engine.SetValue("__application__", currentApp);
                    engine.SetValue("__cache__", MelonCache.Internal);
                    engine.Execute(BindingManager.Get("Tools/application"));
                    break;

                case BuiltInJsModule.Environment:
                    engine.SetValue("__environment__", typeof(MelonEnvironment));
                    engine.SetValue("__environment_vars__", MelonCache.Environment);
                    engine.Execute(BindingManager.Get("Tools/environment"));
                    break;

                case BuiltInJsModule.InputOutput:
                    /* console */
                    engine.SetValue("__console_log__", new Action<object, int>(MelonConsole.Write));
                    engine.SetValue("__console_clear__", new Action(Console.Clear));
                    engine.SetValue("__console_readLine__", new Func<string?>(Console.ReadLine));
                    engine.Execute(BindingManager.Get("Tools/console"));
                    /* fs/files */
                    engine.SetValue("__fs_read__", new Func<string, string>(MelonFileManager.ReadText));
                    engine.SetValue("__fs_write__", new Action<string, string>(MelonFileManager.WriteText));
                    engine.SetValue("__save_file__", new Action<string, byte[]>(MelonFileManager.WriteBytes));
                    engine.SetValue("__delete_file__", new Action<string>(MelonFileManager.Delete));
                    engine.SetValue("__copy_file__", new Action<string, string>(MelonFileManager.Copy));
                    engine.SetValue("__move_file__", new Action<string, string>(MelonFileManager.Move));
                    engine.SetValue("__file__", typeof(MelonFile));
                    engine.SetValue("__create_folder__", new Func<string, DirectoryInfo>(Directory.CreateDirectory));
                    engine.SetValue("__folder__", typeof(MelonFolder));
                    engine.Execute(BindingManager.Get("Constructors/FileSystem/File"));
                    engine.Execute(BindingManager.Get("Constructors/FileSystem/Folder"));
                    engine.Execute(BindingManager.Get("Tools/fs"));
                    break;

                case BuiltInJsModule.UnsafeScripting:
                    engine.SetValue("__script_injector__", new Action<string>(EngineWrapper.ExecuteDirectly));
                    break;

                case BuiltInJsModule.DataManagement:
                    engine.SetValue("__converter__", typeof(MelonConvert));
                    break;

                case BuiltInJsModule.Engine:
                    engine.SetValue("__reset_current_execution__", new Action<Engine?>(EngineManager.ResetEngine));
                    engine.Execute(BindingManager.Get("Constructors/Empty"));
                    engine.Execute(BindingManager.Get("Constructors/Async/AsyncTask"));
                    engine.Execute(BindingManager.Get("Constructors/ConstructorAssembler"));
                    engine.Execute(BindingManager.Get("Constructors/Errors/FileErrorConstants"));
                    engine.Execute(BindingManager.Get("Constructors/Set"));
                    engine.Execute(BindingManager.Get("Constructors/Map"));
                    engine.Execute(BindingManager.Get("Constructors/Queue"));
                    engine.Execute(BindingManager.Get("Constructors/IndexedArray"));
                    engine.Execute(BindingManager.Get("Constructors/Enumerable"));
                    engine.Execute(BindingManager.Get("Constructors/Numbers/BigFloat"));
                    engine.Execute(BindingManager.Get("Constructors/Numbers/NumberPeriod"));
                    break;

                case BuiltInJsModule.Debug:
                    engine.SetValue("__debug_set_stack_tracing__",
                        new Action<bool>((bool status) => container.EnableStackTracing = status));
                    engine.Execute(BindingManager.Get("Tools/debug"));
                    break;

                case BuiltInJsModule.HttpOperations:
                    engine.SetValue("__fetch_request__",
                        new Func<string, string, string, string, MelonHttpResponse>(MelonHttp.Request));
                    engine.SetValue("__ping_request__",
                        new Func<string, uint, MelonPingReply>(MelonHttp.Ping));
                    engine.Execute(BindingManager.Get("Tools/http"));
                    engine.Execute(BindingManager.Get("Constructors/Response"));
                    engine.Execute(BindingManager.Get("Constructors/PingResponse"));

                    engine.SetValue("__http_application_run__",
                        new Action<string, int, string, string, bool>
                        (WebApplicationManager.ExecuteWebApplication));
                    engine.Execute(BindingManager.Get("Constructors/HttpRoute"));
                    engine.Execute(BindingManager.Get("Constructors/HttpApplication"));
                    break;
            }
        }
    }
}
