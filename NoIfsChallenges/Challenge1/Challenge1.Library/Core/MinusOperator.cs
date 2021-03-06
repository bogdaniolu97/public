﻿using Challenge1.Library.Contracts;

namespace Challenge1.Library.Core
{
    public class MinusOperator : Operator
    {
        public string Execute(string a, string b) => (int.Parse(a) - int.Parse(b)).ToString();
    }
}