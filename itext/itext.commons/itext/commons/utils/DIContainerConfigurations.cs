using System;
using System.Collections.Generic;
using System.Reflection;

namespace iText.Commons.Utils {
    public class DIContainerConfigurations {

        private static readonly Dictionary<String, String> DEFAULT_CONFIGURATIONS = new Dictionary<String, String>() {
            { "iText.Forms", "iText.Forms.Util.RegisterDefaultDiContainer" }
        };

        public static void LoadDefaultConfigurations() {
            foreach (var defaultConfigurationClass in DEFAULT_CONFIGURATIONS) {
                try {
                    Activator.CreateInstance(Assembly.Load(defaultConfigurationClass.Key).GetType(defaultConfigurationClass.Value));
                }
                catch (Exception) {
                    //ignore
                }
            }
        }
    }
}