using System;
using System.IO;

namespace iText.Test {
    public static class TestUtil {
        public static String GetParentProjectDirectory(String testDirectory) {
            DirectoryInfo projectDirectoryInfo = new DirectoryInfo(testDirectory);
            while (!projectDirectoryInfo.Name.Equals("bin", StringComparison.OrdinalIgnoreCase)) {
                projectDirectoryInfo = projectDirectoryInfo.Parent;
            }
            return projectDirectoryInfo.Parent.FullName;
        }
    }
}