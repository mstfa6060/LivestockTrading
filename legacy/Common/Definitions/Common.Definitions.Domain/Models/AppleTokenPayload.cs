using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Definitions.Domain.Models;
public class AppleTokenPayload
{
    public string AccessToken { get; set; }
    public string IdToken { get; set; }

    [JsonIgnore]
    public IdTokenContent IdTokenPayload => DecodeJwt(IdToken);

    private IdTokenContent DecodeJwt(string jwt)
    {
        if (string.IsNullOrEmpty(jwt)) return null;

        var payload = jwt.Split('.')[1];
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(PadBase64(payload)));
        return JsonSerializer.Deserialize<IdTokenContent>(json);
    }

    private string PadBase64(string input)
    {
        switch (input.Length % 4)
        {
            case 2: return input + "==";
            case 3: return input + "=";
            default: return input;
        }
    }
}

public class IdTokenContent
{
    public string Sub { get; set; }
}
