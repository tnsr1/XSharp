﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XSharp.Tokens {
  public class Register08 : Register {
    public static readonly string[] Names = "AH,AL,BH,BL,CH,CL,DH,DL".Split(',');

    protected override object IsMatch(object aValue) {
      if (aValue is string && Names.Contains((string)aValue)) {
        return aValue;
      }
      return null;
    }
  }
}
