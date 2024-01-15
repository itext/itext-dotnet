/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2024 Apryse Group NV
    Authors: Apryse Software.

    This program is offered under a commercial and under the AGPL license.
    For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

    AGPL licensing:
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.IO;

  namespace iText.StyledXmlParser.Jsoup.Parser
  {
      public class BufferedReader
      {
          private TextReader input;

          private List<char> buffer;

          private int position = 0;

          public BufferedReader(TextReader input, int capacity)
          {
              this.input = input;
              buffer = new List<char>(capacity);
          }

          public int Position
          {
              get => position;
              set => position = value;
          }

          public int Read(char[] buf, int index, int count)
          {
              if (position >= buffer.Count)
              {
                  return ReadFromReader(buf, index, count);
              }

              int charsLeftInBuffer = buffer.Count - position;

              for (int i = 0; i < Math.Min(charsLeftInBuffer, count); ++i)
              {
                  buf[i + index] = buffer[position++];
              }

              if (charsLeftInBuffer < count)
              {
                  return charsLeftInBuffer + ReadFromReader(buf, index + charsLeftInBuffer, count - charsLeftInBuffer);
              }

              return count;
          }

          private int ReadFromReader(char[] buf, int index, int count)
          {
              int charsRead = input.Read(buf, index, count);
              buffer.AddRange(new List<char>(buf).SubList(index, index + charsRead));
              position += charsRead;
              return charsRead;
          }

          public int Read()
          {
              if (position >= buffer.Count)
              {
                  int ch = input.Read();
                  buffer.Add((char) ch);
                  ++position;
                  return ch;
              }

              return buffer[position++];
          }

          public long Skip(long amount)
          {
              return Read(new char[amount], 0, (int) amount);
          }

          public void Close()
          {
              input.Close();
          }
      }
  }
