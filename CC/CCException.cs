using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC
{
  public class CCException : Exception
  {
    public CCException() { }

    public CCException(string message) : base(message) { }
  }
}
