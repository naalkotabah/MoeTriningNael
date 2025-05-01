namespace Moe.Core.Services;

public class PhoneNumberNormalizer
{
    public string Normalize(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return null;

        return phoneNumber.TrimStart('0');
    }

    public string NormalizeCountryCode(string phoneCountryCode)
    {
        if (string.IsNullOrEmpty(phoneCountryCode))
        {
            return null;
        }
    
        string normalizedCountryCode = phoneCountryCode.StartsWith("+") ? phoneCountryCode : "+" + phoneCountryCode;
    
        return normalizedCountryCode;
    }
}