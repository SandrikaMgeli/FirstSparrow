using FirstSparrow.Application.Services.Abstractions;

namespace FirstSparrow.Application.Services;

public class OtpService : IOtpService
{
    public int Generate(int numDigits)
    {
        if (numDigits <= 0)
            throw new ArgumentException("Number of digits must be positive", nameof(numDigits));

        // Calculate the minimum and maximum values for the given number of digits
        int min = (int)Math.Pow(10, numDigits - 1);
        int max = (int)Math.Pow(10, numDigits) - 1;
        Console.WriteLine(min);
        Console.WriteLine(max);
        // Create a random number generator
        Random random = new Random();

        // Generate a number in the range [min, max]
        return random.Next(min, max + 1);
    }
}