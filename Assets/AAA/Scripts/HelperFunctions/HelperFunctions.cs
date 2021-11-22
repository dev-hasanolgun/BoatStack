public static class HelperFunctions
{
    public static int ReverseClampToInt(int value, int min, int max) // Clamp value to the min value if it is bigger than max value or smaller than min value
    {
        if (value < min) value = max;
        else if (value > max) value = min;
        
        return value;
    }
}