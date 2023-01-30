using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseX;
using FrooxEngine;

namespace GenericTypeType
{
  internal class Program
  {
    public static void Main(string[] args)
    {
      Console.WriteLine($"{typeof(IField<floatQ>).FullName}");
    }
  }
}
