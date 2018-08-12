using System;

public class SingletonNotInitializedException : Exception
{
    public SingletonNotInitializedException()
    {

    }

    public SingletonNotInitializedException(string message) : base(message)
    {

    }

    public SingletonNotInitializedException(string message, Exception inner) : base(message, inner)
    {

    }
}
