/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Validate;

namespace iText.StyledXmlParser.Css.Validate.Impl.Datatype {
    /// <summary>
    /// <see cref="iText.StyledXmlParser.Css.Validate.ICssDataTypeValidator"/>
    /// implementation for css transform property .
    /// </summary>
    public class CssTransformValidator : ICssDataTypeValidator {
        /* (non-Javadoc)
        * @see com.itextpdf.styledxmlparser.css.validate.ICssDataTypeValidator#isValid(java.lang.String)
        */
        public virtual bool IsValid(String objectString) {
            if (CommonCssConstants.NONE.Equals(objectString)) {
                return true;
            }
            String[] components = iText.Commons.Utils.StringUtil.Split(objectString, "\\)");
            foreach (String component in components) {
                if (!IsValidComponent(component)) {
                    return false;
                }
            }
            return true;
        }

        private bool IsValidComponent(String objectString) {
            String function;
            String args;
            if (!CommonCssConstants.NONE.Equals(objectString) && objectString.IndexOf('(') > 0) {
                function = objectString.JSubstring(0, objectString.IndexOf('(')).Trim();
                args = objectString.Substring(objectString.IndexOf('(') + 1);
            }
            else {
                return false;
            }
            if (CommonCssConstants.MATRIX.Equals(function) || CommonCssConstants.SCALE.Equals(function) || CommonCssConstants
                .SCALE_X.Equals(function) || CommonCssConstants.SCALE_Y.Equals(function)) {
                String[] arg = iText.Commons.Utils.StringUtil.Split(args, ",");
                if (arg.Length == 6 && CommonCssConstants.MATRIX.Equals(function) || (arg.Length == 1 || arg.Length == 2) 
                    && CommonCssConstants.SCALE.Equals(function) || arg.Length == 1 && (CommonCssConstants.SCALE_X.Equals(
                    function) || CommonCssConstants.SCALE_Y.Equals(function))) {
                    int i = 0;
                    for (; i < arg.Length; i++) {
                        try {
                            float.Parse(arg[i].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (FormatException) {
                            return false;
                        }
                    }
                    if (i == arg.Length) {
                        return true;
                    }
                }
                return false;
            }
            else {
                if (CommonCssConstants.TRANSLATE.Equals(function) || CommonCssConstants.TRANSLATE_X.Equals(function) || CommonCssConstants
                    .TRANSLATE_Y.Equals(function)) {
                    String[] arg = iText.Commons.Utils.StringUtil.Split(args, ",");
                    if ((arg.Length == 1 || arg.Length == 2 && CommonCssConstants.TRANSLATE.Equals(function))) {
                        foreach (String a in arg) {
                            if (!IsValidForTranslate(a)) {
                                return false;
                            }
                        }
                        return true;
                    }
                    return false;
                }
                else {
                    if (CommonCssConstants.ROTATE.Equals(function)) {
                        try {
                            float value = float.Parse(args, System.Globalization.CultureInfo.InvariantCulture);
                            if (value == 0.0f) {
                                return true;
                            }
                        }
                        catch (FormatException) {
                        }
                        int deg = args.IndexOf('d');
                        int rad = args.IndexOf('r');
                        if (deg > 0 && args.Substring(deg).Equals("deg") || rad > 0 && args.Substring(rad).Equals("rad")) {
                            try {
                                Double.Parse(args.JSubstring(0, deg > 0 ? deg : rad), System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch (FormatException) {
                                return false;
                            }
                            return true;
                        }
                        return false;
                    }
                    else {
                        if (CommonCssConstants.SKEW.Equals(function) || CommonCssConstants.SKEW_X.Equals(function) || CommonCssConstants
                            .SKEW_Y.Equals(function)) {
                            String[] arg = iText.Commons.Utils.StringUtil.Split(args, ",");
                            if ((arg.Length == 1) || (arg.Length == 2 && CommonCssConstants.SKEW.Equals(function))) {
                                for (int k = 0; k < arg.Length; k++) {
                                    try {
                                        float value = float.Parse(arg[k], System.Globalization.CultureInfo.InvariantCulture);
                                        if (value != 0.0f) {
                                            return false;
                                        }
                                    }
                                    catch (FormatException) {
                                    }
                                    int deg = arg[k].IndexOf('d');
                                    int rad = arg[k].IndexOf('r');
                                    if (deg < 0 && rad < 0) {
                                        return false;
                                    }
                                    if (deg > 0 && !arg[k].Substring(deg).Equals("deg") && rad < 0 || (rad > 0 && !arg[k].Substring(rad).Equals
                                        ("rad"))) {
                                        return false;
                                    }
                                    try {
                                        float.Parse(arg[k].Trim().JSubstring(0, rad > 0 ? rad : deg), System.Globalization.CultureInfo.InvariantCulture
                                            );
                                    }
                                    catch (FormatException) {
                                        return false;
                                    }
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static bool IsValidForTranslate(String @string) {
            if (@string == null) {
                return false;
            }
            int pos = 0;
            while (pos < @string.Length) {
                if (@string[pos] == '+' || @string[pos] == '-' || @string[pos] == '.' || @string[pos] >= '0' && @string[pos
                    ] <= '9') {
                    pos++;
                }
                else {
                    break;
                }
            }
            if (pos > 0) {
                try {
                    float.Parse(@string.JSubstring(0, pos), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (FormatException) {
                    return false;
                }
                return (float.Parse(@string.JSubstring(0, pos), System.Globalization.CultureInfo.InvariantCulture) == 0.0f
                     || @string.Substring(pos).Equals(CommonCssConstants.PT) || @string.Substring(pos).Equals(CommonCssConstants
                    .IN) || @string.Substring(pos).Equals(CommonCssConstants.CM) || @string.Substring(pos).Equals(CommonCssConstants
                    .Q) || @string.Substring(pos).Equals(CommonCssConstants.MM) || @string.Substring(pos).Equals(CommonCssConstants
                    .PC) || @string.Substring(pos).Equals(CommonCssConstants.PX) || @string.Substring(pos).Equals(CommonCssConstants
                    .PERCENTAGE));
            }
            return false;
        }
    }
}
