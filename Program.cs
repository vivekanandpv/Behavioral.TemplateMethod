using System;

namespace Behavioral.TemplateMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            var primePersonalLoanService = new PrimePersonalLoanService();
            var loan = primePersonalLoanService.Process();
        }
    }
}
