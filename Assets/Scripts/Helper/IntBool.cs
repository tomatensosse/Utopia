// You just make me cringe. 
//Why would you do this?
//Why would you make a class that has an int and a bool?
//Why would you name it IntBool?
//Why would you make it public?
//Why would you make it a class? 
//Why would you make it a script?
//Why would you make it a helper?
//hy would you make it a file?
//Why would you make it a thing?
//Why would you make it?
//Why would you?

public class IntBool
{
    public int sInt;
    public bool sBool;

    // returns the lower int and true if a is lower than b
    public static IntBool LowerInt(int a, int b)
    {
        IntBool intBool = new IntBool();
        if (a < b) 
        {
            intBool.sInt = a;
            intBool.sBool = true;
        }
        else
        {
            intBool.sInt = b;
            intBool.sBool = false;
        }
        return intBool;
    }
}