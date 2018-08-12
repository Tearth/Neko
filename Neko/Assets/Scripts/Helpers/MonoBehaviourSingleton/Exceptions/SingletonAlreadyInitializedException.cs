using System;

public class SingletonAlreadyInitializedException : Exception
{
    public SingletonAlreadyInitializedException()
    {

    }

    public SingletonAlreadyInitializedException(string message) : base(message)
    {

    }

    public SingletonAlreadyInitializedException(string message, Exception inner) : base(message, inner)
    {

    }
}
