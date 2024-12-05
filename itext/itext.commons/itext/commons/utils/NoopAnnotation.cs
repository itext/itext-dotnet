using System;

namespace iText.Commons.Utils {

    /**
     * An annotation that does nothing. It is used as a transpilation workaround for Java annotations that are
     * unneeded in C#.
     * 
     */
    public class NoopAnnotation : Attribute {
    }
}
