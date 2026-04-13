namespace PickC.SharedKernel.Helpers;

public static class DocumentNumberGenerator
{
    public static string PadLeft(string input, int totalWidth, char paddingChar = '0')
    {
        return (input ?? string.Empty).PadLeft(totalWidth, paddingChar);
    }

    public static string GenerateId(string prefix, int sequenceNumber, int padWidth = 6)
    {
        return $"{prefix}{sequenceNumber.ToString().PadLeft(padWidth, '0')}";
    }
}
