using System;

namespace ExampleRazorTemplatesLibrary.Services
{
    public class ExampleService
    {
        public string GetSomeValue()
        {
            return $"Some Random Value - {new Random().Next()}";
        }
    }
}
