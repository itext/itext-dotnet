/*
This file is part of jsoup, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
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
