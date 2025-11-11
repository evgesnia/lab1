using ImGuiNET;
using System.Text;

partial class Program
{
    public string inputMessage = "";
    public string outputMessage = "";
    public string name;
    public int key = 0;

    private readonly Encryptor encryptor = new Encryptor();
    private readonly Decryptor decryptor = new Decryptor();
    private readonly Hacker hacker = new Hacker();

    protected override void Render()
    {
        ImGui.Begin("Шифр Цезаря");

        if (ImGui.Button("Закрыть")) Close();
        ImGui.SameLine();
        if (ImGui.Button("Зашифровать"))
        {
            outputMessage = encryptor.Execute(inputMessage, key);
            name = "Зашифрованное сообщение:";
        }
        ImGui.SameLine();
        if (ImGui.Button("Расшифровать"))
        {
            outputMessage = decryptor.Execute(inputMessage, key);
            name = "Расшифрованное сообщение:";
        }
        ImGui.SameLine();
        if (ImGui.Button("Взломать"))
        {
            outputMessage = hacker.Execute(inputMessage);
            name = "Взломанное сообщение:";
        }

        ImGui.InputInt("Введите ключ", ref key);
        ImGui.PushItemWidth(-1);
        ImGui.InputTextMultiline("Введите сообщение", ref inputMessage, 8192, new System.Numerics.Vector2(-1, 150),
            ImGuiInputTextFlags.AllowTabInput);
        ImGui.PopItemWidth();
        ImGui.Text($"{name}");

        ImGui.PushItemWidth(-1);
        var flags = ImGuiInputTextFlags.ReadOnly;
        ImGui.InputTextMultiline("##output", ref outputMessage, 16384, new System.Numerics.Vector2(-1, 150), flags);
        ImGui.PopItemWidth();

        if (ImGui.Button("Копировать результат"))
        {
            ImGui.SetClipboardText(outputMessage ?? "");
        }
        ImGui.End();
    }
}

public class Encryptor
{
    private readonly string alfavit = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

    public string Execute(string message, int offsetStep)
    {
        var result = new StringBuilder(message.Length);
        foreach (char ch in message)
        {
            int i = alfavit.IndexOf(char.ToLowerInvariant(ch));
            if (i >= 0)
            {
                int newIndex = (i + offsetStep) % alfavit.Length;
                if (newIndex < 0) newIndex += alfavit.Length;
                char encChar = alfavit[newIndex];
                result.Append(char.IsUpper(ch) ? char.ToUpperInvariant(encChar) : encChar);
            }
            else
            {
                result.Append(ch);
            }
        }
        return result.ToString();
    }
}

public class Decryptor
{
    private readonly string alfavit = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
    public string Execute(string message, int offsetStep)
    {
        var result = new StringBuilder(message.Length);
        foreach (char ch in message)
        {
            char lower = char.ToLowerInvariant(ch);
            int i = alfavit.IndexOf(lower);
            if (i >= 0)
            {
                int newIndex = (i - offsetStep) % alfavit.Length;
                if (newIndex < 0) newIndex += alfavit.Length;
                char decChar = alfavit[newIndex];
                result.Append(char.IsUpper(ch) ? char.ToUpperInvariant(decChar) : decChar);
            }
            else
            {
                result.Append(ch);
            }
        }
        return result.ToString();
    }
}
public class Hacker
{
    private readonly string alfavit = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
    private readonly Dictionary<char, double> ruFreq = new Dictionary<char, double>
    {
        ['а'] = 8.01, ['б'] = 1.59, ['в'] = 4.54, ['г'] = 1.7,  ['д'] = 2.98,
        ['е'] = 8.45, ['ё'] = 0.04, ['ж'] = 0.94, ['з'] = 1.65, ['и'] = 7.35,
        ['й'] = 1.21, ['к'] = 3.49, ['л'] = 4.4,  ['м'] = 3.21, ['н'] = 6.7,
        ['о'] = 10.97, ['п'] = 2.81, ['р'] = 4.73, ['с'] = 5.47, ['т'] = 6.26,
        ['у'] = 2.62, ['ф'] = 0.26, ['х'] = 0.97, ['ц'] = 0.48, ['ч'] = 1.44,
        ['ш'] = 0.73, ['щ'] = 0.36, ['ъ'] = 0.04, ['ы'] = 1.9,  ['ь'] = 1.74,
        ['э'] = 0.32, ['ю'] = 0.64, ['я'] = 2.01
    };

    public string Execute(string message)
    {
        var result = new StringBuilder(message.Length);

        int totalLetters = 0;
        foreach (char c in message)
        {
            if (alfavit.IndexOf(char.ToLowerInvariant(c)) >= 0)
                totalLetters++;
        }
        var counts = new Dictionary<char, int>();
        foreach (char a in alfavit)
            counts[a] = 0;

        foreach (char c in message)
        {
            char lower = char.ToLowerInvariant(c);
            if (counts.ContainsKey(lower))
                counts[lower]++;
        }

        const double matchThreshold = 0.2;
        foreach (char ch in message)
        {
            int i = alfavit.IndexOf(char.ToLowerInvariant(ch));
            if (i >= 0)
            {
                char lowerCh = char.ToLowerInvariant(ch);
                char substitution = ch; 

                if (totalLetters > 0)
                {
                    double freqOfCh = counts.ContainsKey(lowerCh) ? (double)counts[lowerCh] / totalLetters * 100.0 : 0.0;
                    double bestDiff = double.MaxValue;
                    char bestRuChar = lowerCh;
                    foreach (var kv in ruFreq)
                    {
                        double ruCharFreq = kv.Value;
                        double diff = Math.Abs(freqOfCh - ruCharFreq);
                        if (diff < bestDiff)
                        {
                            bestDiff = diff;
                            bestRuChar = kv.Key;
                        }
                    }
                    if (bestDiff <= matchThreshold)
                    {
                        substitution = char.IsUpper(ch) ? char.ToUpperInvariant(bestRuChar) : bestRuChar;
                    }
                }
                result.Append(substitution);
            }
            else
            {
                result.Append(ch);
            }
        }
        return result.ToString();
    }
}
