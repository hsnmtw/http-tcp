using System;

public static class Test
{
    public static void Main(string[]args)
    {
        Console.Write("<h1>{0}</h1>", "executed from server: @Ayah");
        for(int i=0;i<100;i++){
            System.Console.WriteLine("<p>{0}</p>",i);
        }
        Environment.Exit(0);
    }
}