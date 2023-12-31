﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPD.Utilities
{
    public class Utils
    {
        public static string SwapChar(string refStr, int loc, char pokeChar)
        {
            if (loc >= refStr.Length)
                return refStr + pokeChar;
            return refStr.Substring(0, loc) + pokeChar + refStr.Substring(loc + 1);
            // conceptially we're replacing the char in this string at the 'loc' character
            // for loc = 12 => take the first 12 (0-11), then this char, then start at 13th char (13++)
        }
        public static string InsertChar(string refStr, int loc, char pokeChar)
        {
            // for loc 12 (13th char):
            // take the 12 to the left, insert this char, then start with orig 13th char (loc)
            if (loc >= refStr.Length)
                return refStr + pokeChar;
            return refStr.Substring(0, loc) + pokeChar + refStr.Substring(loc);
            // conceptially we're replacing the char in this string at the 'loc' character
            // for loc = 12 => take the first 12 (0-11), then this char, then start at 12th char (12++)
        }
        public static string PokeString(string refStr, int loc, string pokeStr)
        {
            // conceptially same as char, except insert a string
            if (loc >= refStr.Length)
                return refStr + pokeStr;
            return refStr.Substring(0, loc) + pokeStr + refStr.Substring(loc);
        }
        public static int GetNthIndex(string s, char t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == t)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}
