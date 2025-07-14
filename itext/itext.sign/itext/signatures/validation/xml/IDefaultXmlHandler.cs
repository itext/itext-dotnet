using System;
using System.Collections.Generic;

namespace iText.Signatures.Validation.Xml {
    public interface IDefaultXmlHandler {
        void StartElement(String uri, String localName, String qName, Dictionary<String, String> attributes);

        void EndElement(String uri, String localName, String qName);

        void Characters(char[] ch, int start, int length);
    }
}
