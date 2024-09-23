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